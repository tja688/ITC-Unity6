"""
ITC 对话JSON → 剧本格式 转换脚本
将原始的对话系统JSON清洗为更易读的剧本格式
"""

import json
from collections import defaultdict
from pathlib import Path

def load_json(filepath):
    """加载JSON文件"""
    with open(filepath, 'r', encoding='utf-8-sig') as f:
        return json.load(f)

def get_field_value(fields, field_name):
    """从fields数组中获取指定字段的值"""
    for field in fields:
        if field.get('title') == field_name:
            return field.get('value', '')
    return ''

def build_actor_map(data):
    """构建演员ID到名称的映射"""
    actor_map = {}
    for actor in data.get('actors', []):
        actor_id = actor.get('id')
        name = get_field_value(actor['fields'], 'Name')
        actor_map[str(actor_id)] = name
    return actor_map

def build_variable_info(data):
    """整理变量信息"""
    variables = []
    for var in data.get('variables', []):
        name = get_field_value(var['fields'], 'Name')
        initial_value = get_field_value(var['fields'], 'Initial Value')
        description = get_field_value(var['fields'], 'Description')
        variables.append({
            'name': name,
            'initial_value': initial_value,
            'description': description
        })
    return variables

def get_actor_name(actor_id, actor_map):
    """获取演员名称，处理特殊类型"""
    name = actor_map.get(str(actor_id), f'Unknown({actor_id})')
    return name

def build_dialogue_graph(conversation):
    """构建对话图结构"""
    entries = conversation.get('dialogueEntries', [])
    entry_map = {e['id']: e for e in entries}
    
    # 构建邻接表
    adjacency = defaultdict(list)
    incoming = defaultdict(list)
    
    for entry in entries:
        entry_id = entry['id']
        for link in entry.get('outgoingLinks', []):
            dest_id = link.get('destinationDialogueID')
            dest_conv = link.get('destinationConversationID')
            adjacency[entry_id].append({
                'dest_id': dest_id,
                'dest_conv': dest_conv,
                'is_connector': link.get('isConnector', False)
            })
            incoming[dest_id].append(entry_id)
    
    return entry_map, adjacency, incoming

def categorize_actor(actor_name):
    """分类演员类型"""
    system_actors = ['旁白', '动画', '过场', '人物介绍面板', '黑屏字幕', '书面信息', '操作环节']
    if actor_name in system_actors:
        return 'SYSTEM'
    elif actor_name == 'Player' or actor_name == 'Barks':
        return 'PLAYER'
    else:
        return 'NPC'

def format_dialogue_line(entry, actor_map, show_description=True):
    """格式化单条对话"""
    actor_id = get_field_value(entry['fields'], 'Actor')
    actor_name = get_actor_name(actor_id, actor_map)
    dialogue_text = get_field_value(entry['fields'], 'Dialogue Text')
    description = get_field_value(entry['fields'], 'Description')
    menu_text = get_field_value(entry['fields'], 'Menu Text')
    condition = entry.get('conditionsString', '')
    user_script = entry.get('userScript', '')
    
    lines = []
    
    # 添加描述(舞台指示)
    if show_description and description:
        lines.append(f"    【舞台指示】{description}")
    
    # 添加条件
    if condition:
        lines.append(f"    【条件】{condition}")
    
    # 添加脚本
    if user_script:
        lines.append(f"    【脚本】{user_script}")
    
    # 格式化角色和对话
    actor_type = categorize_actor(actor_name)
    
    if dialogue_text:
        if actor_type == 'SYSTEM':
            lines.append(f"    ◆ [{actor_name}] {dialogue_text}")
        elif actor_type == 'PLAYER':
            lines.append(f"    ★ [Barks] {dialogue_text}")
        else:
            lines.append(f"    ● [{actor_name}] {dialogue_text}")
    elif menu_text:
        lines.append(f"    → [选项] {menu_text}")
    
    return lines

def find_scene_breaks(entries, adjacency, incoming):
    """识别场景分割点"""
    scene_breaks = set()
    
    for entry_id, entry in entries.items():
        # 多个入边 = 汇聚点
        if len(incoming[entry_id]) > 1:
            scene_breaks.add(entry_id)
        
        # 多个出边 = 分支点
        if len(adjacency[entry_id]) > 1:
            scene_breaks.add(entry_id)
        
        # 有条件的节点
        if entry.get('conditionsString'):
            scene_breaks.add(entry_id)
        
        # 描述中包含特定关键词(CG镜头等)
        desc = get_field_value(entry['fields'], 'Description')
        if any(kw in desc for kw in ['CG镜头', '背景画面', '章节', 'BGM']):
            scene_breaks.add(entry_id)
    
    return scene_breaks

def trace_linear_path(start_id, entry_map, adjacency):
    """追踪线性对话路径"""
    path = []
    current = start_id
    visited = set()
    
    while current is not None and current not in visited:
        visited.add(current)
        if current not in entry_map:
            break
        path.append(current)
        
        outgoing = adjacency.get(current, [])
        if len(outgoing) == 1 and not outgoing[0].get('is_connector'):
            current = outgoing[0]['dest_id']
        else:
            break
    
    return path

def generate_scene_script(conversation, actor_map, conv_id):
    """生成场景剧本"""
    title = get_field_value(conversation['fields'], 'Title')
    description = get_field_value(conversation['fields'], 'Description')
    
    entry_map, adjacency, incoming = build_dialogue_graph(conversation)
    scene_breaks = find_scene_breaks(entry_map, adjacency, incoming)
    
    output = []
    output.append(f"\n{'='*80}")
    output.append(f"# 章节 {conv_id}: {title}")
    output.append(f"{'='*80}")
    if description:
        output.append(f"【章节描述】{description}")
    output.append("")
    
    # 找到START节点
    start_entry = None
    for entry in conversation.get('dialogueEntries', []):
        entry_title = get_field_value(entry['fields'], 'Title')
        if entry_title == 'START':
            start_entry = entry
            break
    
    if not start_entry:
        output.append("  [警告: 未找到START节点]")
        return '\n'.join(output)
    
    # BFS遍历所有节点，按场景组织
    visited = set()
    scene_counter = 1
    queue = [(start_entry['id'], None)]  # (node_id, parent_id)
    
    current_scene_lines = []
    current_scene_desc = "开场"
    
    while queue:
        current_id, parent_id = queue.pop(0)
        
        if current_id in visited:
            continue
        visited.add(current_id)
        
        if current_id not in entry_map:
            continue
        
        entry = entry_map[current_id]
        
        # 检查是否需要开启新场景
        is_new_scene = current_id in scene_breaks and current_scene_lines
        
        if is_new_scene:
            # 输出当前场景
            output.append(f"\n## 场景 {conv_id}.{scene_counter}: {current_scene_desc}")
            output.append("-" * 40)
            output.extend(current_scene_lines)
            current_scene_lines = []
            scene_counter += 1
            
            # 获取新场景描述
            desc = get_field_value(entry['fields'], 'Description')
            if desc:
                current_scene_desc = desc[:30] + "..." if len(desc) > 30 else desc
            else:
                current_scene_desc = f"继续"
        
        # 格式化当前节点
        lines = format_dialogue_line(entry, actor_map)
        if lines:
            current_scene_lines.extend(lines)
        
        # 处理分支
        outgoing = adjacency.get(current_id, [])
        
        if len(outgoing) > 1:
            # 分支点
            current_scene_lines.append("")
            current_scene_lines.append(f"    ╔══ 分支点 (ID={current_id}) ══╗")
            for i, link in enumerate(outgoing):
                dest_id = link['dest_id']
                dest_conv = link['dest_conv']
                
                if dest_conv != conversation['id']:
                    current_scene_lines.append(f"    ║ → 分支 {i+1}: 跳转到其他章节 (Conv {dest_conv})")
                else:
                    dest_entry = entry_map.get(dest_id)
                    if dest_entry:
                        dest_cond = dest_entry.get('conditionsString', '')
                        dest_menu = get_field_value(dest_entry['fields'], 'Menu Text')
                        dest_desc = get_field_value(dest_entry['fields'], 'Description')
                        
                        branch_label = dest_menu or dest_cond or dest_desc or f"(ID={dest_id})"
                        current_scene_lines.append(f"    ║ → 分支 {i+1}: {branch_label[:50]}")
                        
                        queue.append((dest_id, current_id))
            current_scene_lines.append(f"    ╚{'═'*30}╝")
            current_scene_lines.append("")
        elif len(outgoing) == 1:
            dest = outgoing[0]
            if dest.get('is_connector') or dest['dest_conv'] != conversation['id']:
                current_scene_lines.append(f"    → [跳转到章节 {dest['dest_conv']}]")
            else:
                queue.append((dest['dest_id'], current_id))
    
    # 输出最后一个场景
    if current_scene_lines:
        output.append(f"\n## 场景 {conv_id}.{scene_counter}: {current_scene_desc}")
        output.append("-" * 40)
        output.extend(current_scene_lines)
    
    return '\n'.join(output)

def generate_full_script(data):
    """生成完整剧本"""
    actor_map = build_actor_map(data)
    variables = build_variable_info(data)
    
    output = []
    
    # 标题页
    output.append("╔══════════════════════════════════════════════════════════════════════════════╗")
    output.append("║                                                                              ║")
    output.append("║                    INFERNAL TRANSFER COMPANY                                 ║")
    output.append("║                          地狱转运公司                                        ║")
    output.append("║                                                                              ║")
    output.append("║                        ——完整剧本文档——                                      ║")
    output.append("║                                                                              ║")
    output.append("╚══════════════════════════════════════════════════════════════════════════════╝")
    output.append("")
    
    # 元信息
    output.append("# 剧本元信息")
    output.append("=" * 80)
    output.append("")
    
    # 角色列表
    output.append("## 角色列表 (Actors)")
    output.append("-" * 40)
    for actor_id, name in actor_map.items():
        actor_type = categorize_actor(name)
        type_label = {'SYSTEM': '系统/特效', 'PLAYER': '玩家', 'NPC': 'NPC'}[actor_type]
        output.append(f"  • [{actor_id}] {name} ({type_label})")
    output.append("")
    
    # NPC介绍
    output.append("### 主要NPC")
    output.append("-" * 30)
    npc_intros = {
        'Henet': 'ITC公司老板，恶魔。既美艳又可怖，掌控一切。',
        'OLD TOM': 'Henet的贴身侍从、管家，数千年来最得力的助手。',
        'Receptionist': 'ITC前台文员，长年蝉联优秀员工。严肃冷漠的工作狂。',
        '骷髅门童': '地狱旅馆门童，穿着门童服装的骷髅。',
        'ITC门卫老骷髅': 'ITC公司门卫。',
        '13号窗口老员工': 'ITC公司老员工，在13号窗口工作多年。',
        'Veer': '神秘角色。',
    }
    for name, intro in npc_intros.items():
        if name in actor_map.values():
            output.append(f"  ● {name}: {intro}")
    output.append("")
    
    # 变量列表
    output.append("## 游戏变量 (Variables)")
    output.append("-" * 40)
    for var in variables:
        output.append(f"  • {var['name']} = {var['initial_value']}")
        if var['description']:
            output.append(f"    说明: {var['description']}")
    output.append("")
    
    # 章节概览
    output.append("## 章节概览")
    output.append("-" * 40)
    for i, conv in enumerate(data.get('conversations', []), 1):
        title = get_field_value(conv['fields'], 'Title')
        desc = get_field_value(conv['fields'], 'Description')
        entry_count = len(conv.get('dialogueEntries', []))
        output.append(f"  {i}. {title} ({entry_count}个对话节点)")
        if desc:
            output.append(f"     └─ {desc}")
    output.append("")
    
    # 整体剧情流程图
    output.append("## 整体剧情流程")
    output.append("-" * 40)
    output.append("""
    ┌─────────────────────────────────────────────────────────────┐
    │                        ITC 剧情流程                          │
    └─────────────────────────────────────────────────────────────┘
    
    [游戏序章] ─────────────────────────────────────────────────────┐
    │  • Barks在地狱旅馆醒来                                        │
    │  • 骷髅门童介绍情况                                           │
    │  • 获得钥匙和信封                                             │
    │  • 穿越门户来到圣纽约市                                       │
    │  • 到达ITC公司大楼                                            │
    └──────────────────────────────────────────────────────────────┘
                              ↓
    [上班前剧情] ───────────────────────────────────────────────────┐
    │  • 进入ITC大厅                                                │
    │  • 遇见Receptionist                                          │
    │  • 乘电梯到6楼                                                │
    │  • 遇见OLD TOM                                                │
    │  • 进入Henet办公室，签订试工合同                              │
    │  • 被分配到13号窗口                                           │
    └──────────────────────────────────────────────────────────────┘
                              ↓
    [签约主线] ─────────────────────────────────────────────────────┐
    │  • 核心玩法循环: 接待客户 → 审核文件 → 盖章 → 收取灵魂        │
    │  • 多个客户故事线                                             │
    │  • 根据表现积累 satisfaction / Sign mistake 等变量             │
    │  • 失误次数达标触发结局                                       │
    └──────────────────────────────────────────────────────────────┘
                              ↓
    [结局] ─────────────────────────────────────────────────────────┐
    │  • 根据变量状态触发不同结局                                   │
    └──────────────────────────────────────────────────────────────┘
    
    [下班剧情] ─ 每日工作结束后的过渡剧情
    """)
    output.append("")
    
    # 图例说明
    output.append("## 剧本格式图例")
    output.append("-" * 40)
    output.append("  【舞台指示】CG画面、音效、动画等制作说明")
    output.append("  【条件】触发此对话需要满足的条件")
    output.append("  【脚本】对话执行时运行的脚本(变量修改等)")
    output.append("  ◆ [系统类型] 旁白、过场、黑屏字幕等系统显示")
    output.append("  ★ [Barks] 玩家角色的台词或内心独白")
    output.append("  ● [角色名] NPC的台词")
    output.append("  → [选项] 玩家选择项")
    output.append("  ╔══ 分支点 ══╗ 标记对话分支")
    output.append("")
    
    # 生成各章节详细剧本
    output.append("")
    output.append("╔══════════════════════════════════════════════════════════════════════════════╗")
    output.append("║                             详 细 剧 本                                      ║")
    output.append("╚══════════════════════════════════════════════════════════════════════════════╝")
    
    for i, conv in enumerate(data.get('conversations', []), 1):
        script = generate_scene_script(conv, actor_map, i)
        output.append(script)
    
    return '\n'.join(output)

def main():
    # 加载数据
    script_dir = Path(__file__).parent
    json_path = script_dir / 'ITC对话Json.json'
    output_path = script_dir / 'ITC_完整剧本_清洗版.md'
    
    print(f"正在读取: {json_path}")
    data = load_json(json_path)
    
    print(f"正在生成剧本...")
    script = generate_full_script(data)
    
    print(f"正在保存: {output_path}")
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(script)
    
    print(f"完成! 剧本已保存到: {output_path}")
    print(f"共 {len(script.split(chr(10)))} 行")

if __name__ == '__main__':
    main()
