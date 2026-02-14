游戏核心视觉特征：

1. **美术风格 (Art Style)**：典型的 2D 矢量平涂风（Vector Art / Cel-shading），带有美式独立游戏或图文冒险游戏的特征，线条硬朗。
2. **色彩与光影 (Colors & Lighting)**：极度偏好**暖色调/琥珀色/复古橙色**作为环境光（黄昏或夕阳设定）。阴影部分不使用渐变，而是使用**大面积的纯黑色块 (Solid black shadows)** 来切割画面，对比极其强烈。
3. **题材与元素 (Theme)**：蒸汽朋克 (Steampunk) 与早期工业革命混合风格。充斥着黄铜管道、齿轮、砖石拱门、烟囱、木箱和机械装置。
4. **构图 (Composition)**：大量使用**对称构图**或**画框式构图**（如窗户、走廊、货架将画面向中心引导），为前景可能出现的角色或UI留出空间。


```json
{
  "style_template": {
    "template_name": "Retro Steampunk 2D Game Background",
    "description": "用于生成具有复古工业风、矢量平涂、暖色调、强黑白对比的2D游戏背景原画。",
    
    "fixed_parameters": {
      "base_art_style": "2D vector art, flat colors, cel-shading, indie game background asset, visual novel background, comic book style",
      "lighting_and_colors": "amber and orange color palette, sunset lighting, monochromatic warm tones, golden hour, high contrast, solid pitch-black shadows, no gradients",
      "lineart_and_texture": "thick outlines, hard edges, crisp shapes, stylized geometric forms, simplified textures",
      "world_building_vibe": "steampunk, industrial revolution aesthetic, retro-futurism, mechanical vibe"
    },

    "variable_parameters": {
      "scene_description": {
        "description_cn": "【请在此处填写你想要生成的具体场景，建议包含主体和视角】",
        "example_1": "inside a steampunk library, symmetrical bookshelves, a glowing brass reading lamp on a wooden desk",
        "example_2": "an industrial blacksmith workshop, giant gears on the wall, a glowing furnace in the center",
        "user_input": "[请在此处替换你的场景描述 / REPLACE_WITH_YOUR_SCENE]"
      },
      "specific_elements": {
        "description_cn": "【请在此处填写场景中需要出现的具体物件，增强细节】",
        "example_1": "books, brass pipes, dusty glass windows",
        "example_2": "anvils, metal scrap, hanging tools",
        "user_input": "[请在此处替换具体物件 / REPLACE_WITH_SPECIFIC_ELEMENTS]"
      },
      "composition": {
        "description_cn": "【画面构图方式】",
        "options": [
          "perfectly symmetrical composition (绝对对称构图 - 如杂货铺/走廊)",
          "looking through a large arched window (透过拱形窗户向外看 - 如办公室/餐厅)",
          "side scrolling view (横版过关视角 - 如车站)",
          "framed composition, empty space in the center (画框构图，中心留白 - 适合放NPC/对话框)"
        ],
        "user_input": "[请选择或输入构图方式 / REPLACE_WITH_COMPOSITION]"
      }
    },
    
    "negative_prompt": {
      "description_cn": "【用于排除不符合该画风的元素，特别是防止AI生成过度写实或3D的图像】",
      "default_value": "3d render, realistic, photorealistic, gradient shading, soft shadows, watercolor, oil painting, messy lines, modern architecture, neon lights, cold colors"
    },

    "prompt_assembly_formula": "{base_art_style}, {scene_description}, {specific_elements}, {world_building_vibe}, {composition}, {lighting_and_colors}, {lineart_and_texture} --no {negative_prompt}"
  }
}

```
