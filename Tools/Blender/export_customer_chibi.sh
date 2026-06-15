#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
BLENDER_BIN="${BLENDER_BIN:-/Applications/Blender.app/Contents/MacOS/Blender}"
SCRIPT_PATH="$ROOT_DIR/Tools/Blender/scripts/export_blend_asset.py"
SOURCE_BLEND="${1:-$ROOT_DIR/Tools/Blender/sources/Customers/CustomerChibi.blend}"
FBX_OUT="${2:-$ROOT_DIR/Assets/Malltopia/Art/Models/Customers/CustomerChibi.fbx}"
PREVIEW_OUT="${3:-$ROOT_DIR/Tools/Blender/output/CustomerChibi_Preview.png}"

mkdir -p "$(dirname "$FBX_OUT")" "$(dirname "$PREVIEW_OUT")"

"$BLENDER_BIN" \
  --background \
  "$SOURCE_BLEND" \
  --python "$SCRIPT_PATH" \
  -- \
  --fbx "$FBX_OUT" \
  --preview "$PREVIEW_OUT"
