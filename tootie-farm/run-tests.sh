#!/usr/bin/env bash
set -euo pipefail

# Optional path argument, defaults to all tests under res://test/
TARGET="${1:-res://test/}"

# 1. Build C# project first so Godot runs against the latest binaries
dotnet build

# 2. Run gdUnit4 test runner headlessly
godot-mono --headless -s addons/gdUnit4/bin/GdUnitCmdTool.gd --ignoreHeadlessMode -a "$TARGET"
