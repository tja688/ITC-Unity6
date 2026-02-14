#!/usr/bin/env python3
"""
Unity WebGL self-check scanner.

Outputs structured findings to support planning and review decisions.
No CI gate behavior is enforced; normal execution returns exit code 0.
"""

from __future__ import annotations

import argparse
import datetime as dt
import fnmatch
import json
import re
import sys
from pathlib import Path, PurePosixPath
from typing import Any

TOOL_VERSION = "0.1.0"

DEFAULT_INCLUDES = [
    "ProjectSettings/ProjectSettings.asset",
    "ProjectSettings/ProjectVersion.txt",
    "Assets/**/*.cs",
    "Assets/**/*.js",
    "Assets/**/*.jslib",
    "Assets/**/*.jspre",
]

DEFAULT_EXCLUDES = [
    "Assets/Plugins/**",
    "Library/**",
    "Temp/**",
    "Logs/**",
]

SEVERITY_ORDER = {"BLOCKER": 0, "WARNING": 1, "INFO": 2}

TEXT_RULES = [
    {
        "rule_id": "UWG-THR-001",
        "severity": "BLOCKER",
        "category": "Threading",
        "title": "Managed threading API usage detected",
        "globs": ["**/*.cs"],
        "pattern": re.compile(r"\bSystem\.Threading\b|\bSystem\.Timers\b|\bThreadPool\b|\bTask\.Run\s*\(|\bnew\s+Thread\b"),
        "recommendation": "Replace with coroutine/state-machine/frame-slicing or server-side execution.",
    },
    {
        "rule_id": "UWG-NET-001",
        "severity": "BLOCKER",
        "category": "Network",
        "title": "System.Net usage detected",
        "globs": ["**/*.cs"],
        "pattern": re.compile(r"\bSystem\.Net\b"),
        "recommendation": "Use UnityWebRequest or explicit JS bridge compatible with browser networking.",
    },
    {
        "rule_id": "UWG-NET-002",
        "severity": "BLOCKER",
        "category": "Network",
        "title": "UnityEngine.Ping usage detected",
        "globs": ["**/*.cs"],
        "pattern": re.compile(r"\bUnityEngine\.Ping\b"),
        "recommendation": "Remove ICMP-based logic and use supported WebGL connectivity checks.",
    },
    {
        "rule_id": "UWG-NET-003",
        "severity": "BLOCKER",
        "category": "Network",
        "title": "Blocking wait loop on isDone detected",
        "globs": ["**/*.cs"],
        "pattern": re.compile(r"while\s*\(\s*!\s*[^)]*\.isDone\s*\)"),
        "recommendation": "Use asynchronous coroutine flow (`yield return`) instead of blocking loops.",
    },
    {
        "rule_id": "UWG-AUD-001",
        "severity": "BLOCKER",
        "category": "Audio",
        "title": "Microphone API usage detected",
        "globs": ["**/*.cs"],
        "pattern": re.compile(r"\bMicrophone\b"),
        "recommendation": "Replace microphone-dependent WebGL logic with a supported browser strategy.",
    },
    {
        "rule_id": "UWG-JS-001",
        "severity": "BLOCKER",
        "category": "JS Interop",
        "title": "ES6 syntax detected in Unity JS plugin file",
        "globs": ["**/*.jslib", "**/*.jspre"],
        "pattern": re.compile(r"\blet\b|\bconst\b|=>|\bclass\s+\w+"),
        "recommendation": "Use ES5-compatible syntax in `.jslib` and `.jspre` plugin files.",
    },
]

SETTING_RULES = [
    {
        "rule_id": "UWG-SET-001",
        "severity": "WARNING",
        "category": "Project Settings",
        "title": "webGLThreadsSupport enabled",
        "key": "webGLThreadsSupport",
        "check": lambda value: value == 1,
        "recommendation": "Document and validate full COOP/COEP/CORP deployment requirements.",
    },
    {
        "rule_id": "UWG-SET-002",
        "severity": "WARNING",
        "category": "Project Settings",
        "title": "webGLDecompressionFallback enabled",
        "key": "webGLDecompressionFallback",
        "check": lambda value: value == 1,
        "recommendation": "Confirm tradeoff and hosting strategy because wasm streaming compatibility changes.",
    },
    {
        "rule_id": "UWG-SET-003",
        "severity": "WARNING",
        "category": "Project Settings",
        "title": "webGLDataCaching disabled",
        "key": "webGLDataCaching",
        "check": lambda value: value == 0,
        "recommendation": "Validate cache-off decision against expected repeat-load performance.",
    },
    {
        "rule_id": "UWG-SET-004",
        "severity": "WARNING",
        "category": "Project Settings",
        "title": "webGLExceptionSupport is not minimal",
        "key": "webGLExceptionSupport",
        "check": lambda value: value != 0,
        "recommendation": "Use minimal release exception settings unless active debugging is required.",
    },
    {
        "rule_id": "UWG-SET-005",
        "severity": "BLOCKER",
        "category": "Project Settings",
        "title": "webGLMaximumMemorySize exceeds 2048",
        "key": "webGLMaximumMemorySize",
        "check": lambda value: isinstance(value, int) and value > 2048,
        "recommendation": "Set maximum memory to 2048 or lower.",
    },
    {
        "rule_id": "UWG-SET-006",
        "severity": "WARNING",
        "category": "Project Settings",
        "title": "webGLUseEmbeddedResources enabled",
        "key": "webGLUseEmbeddedResources",
        "check": lambda value: value == 1,
        "recommendation": "Confirm binary-size tradeoff and justify embedded resources usage.",
    },
]


def normalize_value(raw: str) -> Any:
    if re.fullmatch(r"-?\d+", raw):
        return int(raw)
    if re.fullmatch(r"-?\d+\.\d+", raw):
        return float(raw)
    return raw


def load_unity_version(repo_root: Path) -> str:
    version_file = repo_root / "ProjectSettings/ProjectVersion.txt"
    if not version_file.exists():
        return "unknown"
    for line in version_file.read_text(encoding="utf-8", errors="ignore").splitlines():
        if line.startswith("m_EditorVersion:"):
            return line.split(":", 1)[1].strip()
    return "unknown"


def load_webgl_settings(repo_root: Path) -> dict[str, Any]:
    settings_file = repo_root / "ProjectSettings/ProjectSettings.asset"
    parsed: dict[str, Any] = {}
    if not settings_file.exists():
        return parsed

    for line in settings_file.read_text(encoding="utf-8", errors="ignore").splitlines():
        match = re.match(r"^\s*(webGL[A-Za-z0-9_]+):\s*(.*?)\s*$", line)
        if not match:
            continue
        key = match.group(1)
        raw_value = match.group(2)
        parsed[key] = normalize_value(raw_value)

    return dict(sorted(parsed.items()))


def matches_any(path_str: str, patterns: list[str]) -> bool:
    posix_path = PurePosixPath(path_str)
    return any(posix_path.match(pattern) or fnmatch.fnmatch(path_str, pattern) for pattern in patterns)


def collect_files(repo_root: Path, includes: list[str], excludes: list[str]) -> list[Path]:
    files: dict[str, Path] = {}
    for pattern in includes:
        for path in repo_root.glob(pattern):
            if not path.is_file():
                continue
            rel = path.relative_to(repo_root).as_posix()
            if matches_any(rel, excludes):
                continue
            files[rel] = path
    return [files[rel] for rel in sorted(files.keys())]


def make_finding(
    rule_id: str,
    severity: str,
    category: str,
    title: str,
    path: str,
    line: int | None,
    evidence: str,
    recommendation: str,
) -> dict[str, Any]:
    return {
        "rule_id": rule_id,
        "severity": severity,
        "category": category,
        "title": title,
        "path": path,
        "line": line,
        "evidence": evidence,
        "recommendation": recommendation,
    }


def run_text_rules(repo_root: Path, files: list[Path]) -> list[dict[str, Any]]:
    findings: list[dict[str, Any]] = []
    for file_path in files:
        rel = file_path.relative_to(repo_root).as_posix()
        text = file_path.read_text(encoding="utf-8", errors="ignore")
        lines = text.splitlines()
        for rule in TEXT_RULES:
            if not matches_any(rel, rule["globs"]):
                continue
            for index, line in enumerate(lines, start=1):
                if not rule["pattern"].search(line):
                    continue
                findings.append(
                    make_finding(
                        rule_id=rule["rule_id"],
                        severity=rule["severity"],
                        category=rule["category"],
                        title=rule["title"],
                        path=rel,
                        line=index,
                        evidence=line.strip(),
                        recommendation=rule["recommendation"],
                    )
                )
    return findings


def run_setting_rules(settings: dict[str, Any]) -> list[dict[str, Any]]:
    findings: list[dict[str, Any]] = []
    for rule in SETTING_RULES:
        key = rule["key"]
        if key not in settings:
            continue
        value = settings[key]
        if not rule["check"](value):
            continue
        findings.append(
            make_finding(
                rule_id=rule["rule_id"],
                severity=rule["severity"],
                category=rule["category"],
                title=rule["title"],
                path="ProjectSettings/ProjectSettings.asset",
                line=None,
                evidence=f"{key}: {value}",
                recommendation=rule["recommendation"],
            )
        )
    return findings


def sort_findings(findings: list[dict[str, Any]]) -> list[dict[str, Any]]:
    return sorted(
        findings,
        key=lambda item: (
            SEVERITY_ORDER.get(item["severity"], 99),
            item["path"] or "",
            item["line"] or 0,
            item["rule_id"],
        ),
    )


def summarize(findings: list[dict[str, Any]]) -> dict[str, int]:
    counts = {"blocker_count": 0, "warning_count": 0, "info_count": 0}
    for finding in findings:
        severity = finding["severity"]
        if severity == "BLOCKER":
            counts["blocker_count"] += 1
        elif severity == "WARNING":
            counts["warning_count"] += 1
        elif severity == "INFO":
            counts["info_count"] += 1
    return counts


def build_markdown_report(
    meta: dict[str, Any],
    project_settings: dict[str, Any],
    findings: list[dict[str, Any]],
    summary: dict[str, int],
) -> str:
    lines: list[str] = []
    lines.append("# Unity WebGL Self-Check Report")
    lines.append("")
    lines.append("## Meta")
    lines.append(f"- tool_version: {meta['tool_version']}")
    lines.append(f"- scanned_at: {meta['scanned_at']}")
    lines.append(f"- repo_root: {meta['repo_root']}")
    lines.append(f"- unity_version: {meta['unity_version']}")
    lines.append("")
    lines.append("## Summary")
    lines.append(f"- BLOCKER: {summary['blocker_count']}")
    lines.append(f"- WARNING: {summary['warning_count']}")
    lines.append(f"- INFO: {summary['info_count']}")
    lines.append("")
    lines.append("## Project Settings Snapshot")
    if project_settings:
        lines.append("| Key | Value |")
        lines.append("|---|---|")
        for key, value in project_settings.items():
            lines.append(f"| {key} | {value} |")
    else:
        lines.append("- No WebGL settings found.")
    lines.append("")

    if not findings:
        lines.append("## Findings")
        lines.append("- No findings.")
        return "\n".join(lines) + "\n"

    grouped: dict[str, list[dict[str, Any]]] = {"BLOCKER": [], "WARNING": [], "INFO": []}
    for finding in findings:
        grouped.setdefault(finding["severity"], []).append(finding)

    lines.append("## Findings")
    for severity in ["BLOCKER", "WARNING", "INFO"]:
        items = grouped.get(severity, [])
        if not items:
            continue
        lines.append(f"### {severity}")
        for item in items:
            location = item["path"] if item["line"] is None else f"{item['path']}:{item['line']}"
            lines.append(
                f"- `{item['rule_id']}` {item['title']} at `{location}`. "
                f"Evidence: `{item['evidence']}`. Recommendation: {item['recommendation']}"
            )
        lines.append("")

    return "\n".join(lines) + "\n"


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Unity WebGL planning/review self-check scanner.")
    parser.add_argument("--repo-root", default=".", help="Repository root path. Default: current directory.")
    parser.add_argument("--json-out", default=None, help="Optional JSON output path.")
    parser.add_argument("--md-out", default=None, help="Optional Markdown output path.")
    parser.add_argument("--include", action="append", default=None, help="Include glob (repeatable).")
    parser.add_argument("--exclude", action="append", default=None, help="Exclude glob (repeatable).")
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    repo_root = Path(args.repo_root).resolve()
    includes = args.include if args.include else DEFAULT_INCLUDES
    excludes = args.exclude if args.exclude else DEFAULT_EXCLUDES

    unity_version = load_unity_version(repo_root)
    project_settings = load_webgl_settings(repo_root)
    files = collect_files(repo_root, includes, excludes)

    findings = run_text_rules(repo_root, files)
    findings.extend(run_setting_rules(project_settings))
    findings = sort_findings(findings)
    summary = summarize(findings)

    meta = {
        "tool_version": TOOL_VERSION,
        "scanned_at": dt.datetime.now(dt.timezone.utc).isoformat(),
        "repo_root": repo_root.as_posix(),
        "unity_version": unity_version,
    }

    result = {
        "meta": meta,
        "project_settings": project_settings,
        "findings": findings,
        "summary": summary,
    }

    if args.json_out:
        json_path = Path(args.json_out)
        json_path.parent.mkdir(parents=True, exist_ok=True)
        json_path.write_text(json.dumps(result, indent=2), encoding="utf-8")

    markdown_report = build_markdown_report(meta, project_settings, findings, summary)
    if args.md_out:
        md_path = Path(args.md_out)
        md_path.parent.mkdir(parents=True, exist_ok=True)
        md_path.write_text(markdown_report, encoding="utf-8")
    else:
        sys.stdout.write(markdown_report)

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
