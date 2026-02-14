
import re
import json
import os

SOURCE_FILE = r'c:\Users\jinji\Documents\GitHub\ITC-Unity6\ITC-Unity6\Assets\Doc\ITC Doc\dialogue\ITC_剧本_精编版_导演稿.md'
OUTPUT_DIR = r'c:\Users\jinji\Documents\GitHub\ITC-Unity6\ITC-Unity6\.gemini\scripts\dialogue_data'

def parse_dialogue_file(file_path):
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # Split by lines starting with "## Line"
    # But carefully since there might be other H2 headers.
    # We look for "## Line \d+" specifically.
    
    # Use re.split but keep the delimiters or reconstruct.
    # Or just iterate matches.
    
    pattern = re.compile(r'## Line (\d+)[^\n]*（(.*?) → (.*?)）\n(.*?)(?=## Line \d+|## 质量自检|## 参考资料|$)', re.DOTALL)
    
    matches = pattern.finditer(content)
    
    dialogues = []
    
    for match in matches:
        line_id = match.group(1)
        speaker = match.group(2).strip()
        addressee = match.group(3).strip()
        body = match.group(4)
        
        # Extract Act/Scene/Original Text
        act_match = re.search(r'-\s*幕：(.*)', body)
        scene_match = re.search(r'-\s*场景：(.*)', body)
        original_text_match = re.search(r'\*\*原句\*\*：(.*)', body)
        
        act = act_match.group(1).strip() if act_match else ""
        scene = scene_match.group(1).strip() if scene_match else ""
        original_text = original_text_match.group(1).strip() if original_text_match else ""
        
        # Sometimes original text might be multiline or contain markdown.
        # But based on the sample file, it seems to be one paragraph starting after **原句**：
        # Let's rely on the simple regex for now. If it's missing, log it.
        
        if not original_text:
            print(f"Warning: No original text found for Line {line_id}")
            continue

        dialogues.append({
            "line_id": line_id,
            "speaker": speaker,
            "addressee": addressee,
            "act": act,
            "scene": scene,
            "original_text": original_text
        })
        
    return dialogues

def main():
    if not os.path.exists(OUTPUT_DIR):
        os.makedirs(OUTPUT_DIR)
        
    dialogues = parse_dialogue_file(SOURCE_FILE)
    print(f"Parsed {len(dialogues)} dialogue lines.")
    
    # Save full JSON for reference
    with open(os.path.join(OUTPUT_DIR, 'full_dialogue.json'), 'w', encoding='utf-8') as f:
        json.dump(dialogues, f, ensure_ascii=False, indent=2)

    # Split into chunks of 20
    chunk_size = 20
    for i in range(0, len(dialogues), chunk_size):
        chunk = dialogues[i:i + chunk_size]
        chunk_file = os.path.join(OUTPUT_DIR, f'chunk_{i//chunk_size}.json')
        with open(chunk_file, 'w', encoding='utf-8') as f:
            json.dump(chunk, f, ensure_ascii=False, indent=2)
        print(f"Saved {chunk_file} with {len(chunk)} items.")

if __name__ == "__main__":
    main()
