#!/usr/bin/env python3
import argparse
import re
import sys
from pathlib import Path

TITLE_RE = re.compile(r"^title:\s*(.*?)\s*$")
BRACE_RE = re.compile(r"\{([^{}]+)\}")
BRACKET_TAG_RE = re.compile(r"\[[^\]\n]*[A-Za-z][^\]\n]*\]")
ANGLE_TAG_RE = re.compile(r"<\s*\/?\s*\??\s*[A-Za-z]")


def collect_files(inputs):
    files = []
    missing = []
    for raw in inputs:
        path = Path(raw)
        if path.is_dir():
            files.extend(sorted(path.rglob("*.yarn")))
        elif path.is_file():
            files.append(path)
        else:
            missing.append(raw)
    return files, missing


def scan_file(path, mode):
    errors = []
    warnings = []

    try:
        text = path.read_text(encoding="utf-8", errors="replace")
    except OSError as exc:
        errors.append((0, f"Failed to read file: {exc}"))
        return errors, warnings

    lines = text.splitlines()
    titles = {}
    current_node = None

    for line_no, line in enumerate(lines, start=1):
        stripped = line.strip()
        is_comment = stripped.startswith("//")

        if not is_comment:
            title_match = TITLE_RE.match(line)
            if title_match:
                title = (title_match.group(1) or "").strip()
                if not title:
                    errors.append((line_no, "Empty title."))
                if title in titles:
                    errors.append(
                        (line_no, f'Duplicate title "{title}" (first at line {titles[title]}).')
                    )
                if current_node is not None:
                    errors.append((line_no, "New title found before previous node ended."))
                current_node = {"title": title, "start": line_no, "header": None}
                titles[title] = line_no
                continue

            if stripped == "---":
                if current_node is None:
                    errors.append((line_no, "Found '---' before a title."))
                elif current_node["header"] is not None:
                    errors.append((line_no, "Duplicate '---' in node header."))
                else:
                    current_node["header"] = line_no
                continue

            if stripped == "===":
                if current_node is None:
                    errors.append((line_no, "Found '===' before a title."))
                elif current_node["header"] is None:
                    errors.append((line_no, "Found '===' before '---' in node header."))
                else:
                    current_node = None
                continue

        if "\\n" in line:
            warnings.append((line_no, "Literal \\n found. Split lines instead."))

        if not is_comment:
            for match in BRACE_RE.finditer(line):
                content = match.group(1).strip()
                if not content:
                    errors.append((line_no, "Empty {} block is not allowed."))
                    continue
                if content.isdigit():
                    continue
                if content.startswith("$"):
                    continue
                errors.append((line_no, f"Invalid {{}} usage: {{{content}}}"))

            if mode == "default":
                if BRACKET_TAG_RE.search(line):
                    errors.append((line_no, "Yarn markup [] tags are not allowed in default mode."))
            elif mode == "converter":
                if ANGLE_TAG_RE.search(line):
                    errors.append((line_no, "Angle bracket tags <> are not allowed in converter mode."))

    if current_node is not None:
        if current_node["header"] is None:
            errors.append((current_node["start"], "Node is missing '---' header separator."))
        else:
            errors.append((current_node["start"], "Node is missing '===' end marker."))

    if not titles:
        errors.append((0, "No nodes found (missing title:)."))

    return errors, warnings


def main():
    parser = argparse.ArgumentParser(description="Lint ITC .yarn files for project rules.")
    parser.add_argument(
        "paths",
        nargs="+",
        help="File or directory paths to scan (.yarn files in directories).",
    )
    parser.add_argument(
        "--mode",
        choices=["default", "converter"],
        default="default",
        help="Markup mode: default (<> allowed, [] banned) or converter ([] allowed, <> banned).",
    )
    parser.add_argument(
        "--strict",
        action="store_true",
        help="Treat warnings as errors.",
    )
    args = parser.parse_args()

    files, missing = collect_files(args.paths)
    issues = []

    for missing_path in missing:
        issues.append(("ERROR", missing_path, 0, "Path not found."))

    if not files and not missing:
        issues.append(("ERROR", "", 0, "No .yarn files found."))

    for file_path in files:
        errors, warnings = scan_file(file_path, args.mode)
        for line_no, message in errors:
            issues.append(("ERROR", str(file_path), line_no, message))
        for line_no, message in warnings:
            issues.append(("WARN", str(file_path), line_no, message))

    for level, file_path, line_no, message in issues:
        location = file_path
        if line_no:
            location = f"{file_path}:{line_no}"
        print(f"{level}: {location} - {message}")

    has_errors = any(level == "ERROR" for level, _, _, _ in issues)
    has_warnings = any(level == "WARN" for level, _, _, _ in issues)

    if has_errors or (args.strict and has_warnings):
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
