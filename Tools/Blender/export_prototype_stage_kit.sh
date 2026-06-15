#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
BLENDER_BIN="${BLENDER_BIN:-/Applications/Blender.app/Contents/MacOS/Blender}"
SCRIPT_PATH="$ROOT_DIR/Tools/Blender/scripts/generate_prototype_stage_kit.py"
FBX_OUT="${1:-$ROOT_DIR/Assets/Malltopia/Art/Models/Prototype/PrototypeStageKit.fbx}"
BLEND_OUT="${2:-$ROOT_DIR/Tools/Blender/output/PrototypeStageKit.blend}"

mkdir -p "$(dirname "$FBX_OUT")" "$(dirname "$BLEND_OUT")"

"$BLENDER_BIN" \
  --background \
  --python "$SCRIPT_PATH" \
  -- \
  --fbx "$FBX_OUT" \
  --blend "$BLEND_OUT"
