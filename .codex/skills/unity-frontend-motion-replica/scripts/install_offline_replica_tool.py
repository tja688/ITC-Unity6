#!/usr/bin/env python3
from __future__ import annotations

import argparse
from pathlib import Path


def find_project_root(start: Path) -> Path:
    if (start / "Assets").is_dir() and (start / "ProjectSettings").is_dir():
        return start
    raise SystemExit(f"Unity project root not found: {start}")


def install(project_root: Path, skill_root: Path) -> Path:
    src = skill_root / "assets" / "ReplicaOfflineDeployTool.cs.txt"
    if not src.is_file():
        raise SystemExit(f"Template missing: {src}")

    dst_dir = project_root / "Assets" / "Editor" / "Replica"
    dst_dir.mkdir(parents=True, exist_ok=True)
    dst = dst_dir / "ReplicaOfflineDeployTool.cs"
    dst.write_text(src.read_text(encoding="utf-8"), encoding="utf-8")
    return dst


def main() -> None:
    parser = argparse.ArgumentParser(
        description="Install ReplicaOfflineDeployTool.cs into a Unity project."
    )
    parser.add_argument(
        "--project-root",
        default=".",
        help="Path to Unity project root (contains Assets/ and ProjectSettings/).",
    )
    args = parser.parse_args()

    skill_root = Path(__file__).resolve().parent.parent
    project_root = find_project_root(Path(args.project_root).resolve())
    dst = install(project_root, skill_root)
    print(f"Installed: {dst}")


if __name__ == "__main__":
    main()
