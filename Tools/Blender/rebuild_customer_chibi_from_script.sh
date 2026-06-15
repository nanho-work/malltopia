#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
BLENDER_BIN="${BLENDER_BIN:-/Applications/Blender.app/Contents/MacOS/Blender}"
SCRIPT_PATH="$ROOT_DIR/Tools/Blender/scripts/generate_customer_chibi.py"
FBX_OUT="${1:-$ROOT_DIR/Assets/Malltopia/Art/Models/Customers/CustomerChibi.fbx}"
SOURCE_BLEND_OUT="${2:-$ROOT_DIR/Tools/Blender/sources/Customers/CustomerChibi.blend}"
PREVIEW_OUT="${3:-$ROOT_DIR/Tools/Blender/output/CustomerChibi_Preview.png}"

mkdir -p "$(dirname "$FBX_OUT")" "$(dirname "$SOURCE_BLEND_OUT")" "$(dirname "$PREVIEW_OUT")"

"$BLENDER_BIN" \
  --background \
  --python "$SCRIPT_PATH" \
  -- \
  --fbx "$FBX_OUT" \
  --blend "$SOURCE_BLEND_OUT" \
  --preview "$PREVIEW_OUT"
