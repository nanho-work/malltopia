import argparse
import math
import sys
from pathlib import Path

import bpy


def parse_args():
    argv = []
    if "--" in sys.argv:
        argv = sys.argv[sys.argv.index("--") + 1 :]

    parser = argparse.ArgumentParser(
        description="Export the currently open Blender asset to Unity FBX."
    )
    parser.add_argument("--fbx", required=True, help="FBX output path for Unity.")
    parser.add_argument("--preview", help="Optional PNG preview output path.")
    parser.add_argument("--ortho-scale", type=float, default=2.45)
    parser.add_argument("--camera-y", type=float, default=-5.2)
    parser.add_argument("--camera-z", type=float, default=1.08)
    return parser.parse_args(argv)


def ensure_camera(args):
    camera = bpy.data.objects.get("AssetPreview_Camera")
    if camera == None:
        bpy.ops.object.camera_add()
        camera = bpy.context.object
        camera.name = "AssetPreview_Camera"

    camera.location = (0, args.camera_y, args.camera_z)
    camera.rotation_euler = (math.radians(90), 0, 0)
    camera.data.type = "ORTHO"
    camera.data.ortho_scale = args.ortho_scale
    bpy.context.scene.camera = camera


def export_fbx(path):
    for obj in bpy.context.scene.objects:
        obj.select_set(obj.type == "MESH")

    bpy.ops.export_scene.fbx(
        filepath=str(path),
        use_selection=True,
        object_types={"MESH"},
        apply_unit_scale=True,
        axis_forward="-Z",
        axis_up="Y",
        bake_space_transform=False,
    )


def render_preview(path, args):
    ensure_camera(args)
    bpy.context.scene.render.engine = "BLENDER_WORKBENCH"
    bpy.context.scene.display.shading.light = "STUDIO"
    bpy.context.scene.display.shading.color_type = "MATERIAL"
    bpy.context.scene.render.resolution_x = 1200
    bpy.context.scene.render.resolution_y = 1200
    bpy.context.scene.render.film_transparent = False
    bpy.context.scene.world.color = (0.93, 0.95, 0.96)
    bpy.context.scene.render.filepath = str(path)
    bpy.ops.render.render(write_still=True)


def main():
    args = parse_args()
    fbx_path = Path(args.fbx).expanduser().resolve()
    fbx_path.parent.mkdir(parents=True, exist_ok=True)
    export_fbx(fbx_path)
    print(f"Exported Unity FBX: {fbx_path}")

    if args.preview:
        preview_path = Path(args.preview).expanduser().resolve()
        preview_path.parent.mkdir(parents=True, exist_ok=True)
        render_preview(preview_path, args)
        print(f"Exported preview PNG: {preview_path}")


if __name__ == "__main__":
    main()
