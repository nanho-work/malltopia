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
        description="Generate a small Malltopia prototype stage kit for Unity."
    )
    parser.add_argument(
        "--fbx",
        default="Assets/Malltopia/Art/Models/Prototype/PrototypeStageKit.fbx",
        help="FBX output path for Unity.",
    )
    parser.add_argument(
        "--blend",
        default="Tools/Blender/output/PrototypeStageKit.blend",
        help="Optional .blend source output path.",
    )
    return parser.parse_args(argv)


def clear_scene():
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete()


def make_material(name, color):
    material = bpy.data.materials.new(name)
    material.diffuse_color = color
    return material


def apply_material(obj, material):
    obj.data.materials.clear()
    obj.data.materials.append(material)


def add_cube(name, location, scale, material, bevel=0.0):
    bpy.ops.mesh.primitive_cube_add(size=1, location=location)
    obj = bpy.context.object
    obj.name = name
    obj.dimensions = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    apply_material(obj, material)

    if bevel > 0:
        modifier = obj.modifiers.new(f"{name}_SoftEdges", "BEVEL")
        modifier.width = bevel
        modifier.segments = 2
        modifier.affect = "EDGES"
        obj.modifiers.new(f"{name}_WeightedNormals", "WEIGHTED_NORMAL")

    return obj


def add_cylinder(name, location, radius, depth, material, vertices=24, rotation=(0, 0, 0)):
    bpy.ops.mesh.primitive_cylinder_add(
        vertices=vertices,
        radius=radius,
        depth=depth,
        location=location,
        rotation=rotation,
    )
    obj = bpy.context.object
    obj.name = name
    apply_material(obj, material)
    obj.modifiers.new(f"{name}_WeightedNormals", "WEIGHTED_NORMAL")
    return obj


def add_uv_sphere(name, location, radius, material, segments=24, rings=12):
    bpy.ops.mesh.primitive_uv_sphere_add(
        segments=segments,
        ring_count=rings,
        radius=radius,
        location=location,
    )
    obj = bpy.context.object
    obj.name = name
    apply_material(obj, material)
    obj.modifiers.new(f"{name}_WeightedNormals", "WEIGHTED_NORMAL")
    return obj


def add_text(name, text, location, size, material, rotation=(math.radians(90), 0, 0)):
    bpy.ops.object.text_add(location=location, rotation=rotation)
    obj = bpy.context.object
    obj.name = name
    obj.data.body = text
    obj.data.align_x = "CENTER"
    obj.data.align_y = "CENTER"
    obj.data.size = size
    obj.data.extrude = 0.025
    apply_material(obj, material)

    bpy.ops.object.convert(target="MESH")
    bpy.context.object.name = name
    return bpy.context.object


def build_stage_kit():
    mats = {
        "floor": make_material("MT_Floor_Mint", (0.42, 0.77, 0.63, 1.0)),
        "walkway": make_material("MT_Walkway_Cream", (0.94, 0.88, 0.68, 1.0)),
        "counter": make_material("MT_Counter_Coral", (0.96, 0.39, 0.34, 1.0)),
        "counter_top": make_material("MT_CounterTop_WarmWhite", (1.0, 0.96, 0.88, 1.0)),
        "stand": make_material("MT_Stand_Teal", (0.12, 0.56, 0.62, 1.0)),
        "shelf": make_material("MT_Shelf_Yellow", (1.0, 0.74, 0.26, 1.0)),
        "product_a": make_material("MT_Product_Pink", (1.0, 0.45, 0.65, 1.0)),
        "product_b": make_material("MT_Product_Blue", (0.22, 0.5, 0.95, 1.0)),
        "product_c": make_material("MT_Product_Green", (0.34, 0.8, 0.42, 1.0)),
        "trim": make_material("MT_Trim_Navy", (0.09, 0.14, 0.25, 1.0)),
        "sign": make_material("MT_Sign_Gold", (1.0, 0.83, 0.27, 1.0)),
        "text": make_material("MT_Text_Dark", (0.08, 0.08, 0.09, 1.0)),
        "light": make_material("MT_Light_Warm", (1.0, 0.93, 0.62, 1.0)),
    }

    # Ground pieces
    add_cube("StageKit_Floor_Base_8x5m", (0, 0, -0.05), (8.0, 5.0, 0.1), mats["floor"])
    add_cube("StageKit_Walkway_Main", (0, -1.25, 0.02), (7.4, 1.05, 0.06), mats["walkway"])
    add_cube("StageKit_Walkway_Center", (0, 0.5, 0.025), (1.4, 3.0, 0.06), mats["walkway"])

    # Counter
    add_cube("StageKit_Checkout_Counter_Base", (0, 1.85, 0.45), (2.6, 0.7, 0.9), mats["counter"], 0.035)
    add_cube("StageKit_Checkout_Counter_Top", (0, 1.85, 0.94), (2.85, 0.85, 0.16), mats["counter_top"], 0.04)
    add_cube("StageKit_Checkout_Register", (-0.78, 1.72, 1.12), (0.45, 0.32, 0.28), mats["trim"], 0.03)
    add_cube("StageKit_Checkout_Display", (-0.78, 1.55, 1.34), (0.55, 0.08, 0.34), mats["text"], 0.02)

    # Product stands
    stand_x = [-2.65, 2.65]
    for index, x in enumerate(stand_x, start=1):
        add_cube(f"StageKit_ProductStand_{index}_Base", (x, 0.35, 0.35), (1.35, 1.1, 0.7), mats["stand"], 0.035)
        add_cube(f"StageKit_ProductStand_{index}_TopShelf", (x, 0.35, 0.78), (1.55, 1.2, 0.12), mats["shelf"], 0.03)
        add_cube(f"StageKit_ProductStand_{index}_BackBoard", (x, 0.93, 1.15), (1.55, 0.14, 0.95), mats["stand"], 0.025)
        for slot in range(3):
            px = x - 0.45 + slot * 0.45
            add_cube(
                f"StageKit_ProductStand_{index}_Box_{slot + 1}",
                (px, 0.0, 1.02),
                (0.28, 0.34, 0.32),
                mats[["product_a", "product_b", "product_c"][slot]],
                0.025,
            )

    # Amusement-park style arch and bulbs
    add_cylinder("StageKit_Arch_LeftPost", (-3.45, 1.45, 1.15), 0.08, 2.3, mats["trim"])
    add_cylinder("StageKit_Arch_RightPost", (3.45, 1.45, 1.15), 0.08, 2.3, mats["trim"])
    add_cube("StageKit_Arch_Header", (0, 1.45, 2.28), (7.1, 0.18, 0.16), mats["trim"], 0.025)
    for i in range(9):
        x = -3.0 + i * 0.75
        add_uv_sphere(f"StageKit_Arch_Bulb_{i + 1}", (x, 1.32, 2.45), 0.11, mats["light"], 16, 8)

    # Signboard
    add_cube("StageKit_Sign_Backplate", (0, 1.25, 2.75), (2.8, 0.16, 0.72), mats["sign"], 0.04)
    add_text("StageKit_Sign_Text", "MALLTOPIA", (0, 1.14, 2.78), 0.32, mats["text"])

    # Simple placement markers that can be used as visual references in Unity.
    for i, x in enumerate([-1.8, 0.0, 1.8], start=1):
        add_cylinder(
            f"StageKit_CustomerQueueMarker_{i}",
            (x, -1.55, 0.045),
            0.18,
            0.035,
            mats["counter"],
            vertices=32,
        )

    # Camera and light are useful when opening the .blend source.
    bpy.ops.object.light_add(type="AREA", location=(0, -4.5, 6.0))
    light = bpy.context.object
    light.name = "StageKit_Source_AreaLight"
    light.data.energy = 550
    light.data.size = 5.0

    bpy.ops.object.camera_add(location=(0, -6.6, 4.6), rotation=(math.radians(58), 0, 0))
    bpy.context.scene.camera = bpy.context.object


def export_outputs(args):
    fbx_path = Path(args.fbx).expanduser().resolve()
    blend_path = Path(args.blend).expanduser().resolve()
    fbx_path.parent.mkdir(parents=True, exist_ok=True)
    blend_path.parent.mkdir(parents=True, exist_ok=True)

    bpy.ops.wm.save_as_mainfile(filepath=str(blend_path))

    for obj in bpy.context.scene.objects:
        obj.select_set(obj.type == "MESH")

    bpy.ops.export_scene.fbx(
        filepath=str(fbx_path),
        use_selection=True,
        object_types={"MESH"},
        apply_unit_scale=True,
        axis_forward="-Z",
        axis_up="Y",
        bake_space_transform=False,
    )

    print(f"Exported Blender source: {blend_path}")
    print(f"Exported Unity FBX: {fbx_path}")


def main():
    args = parse_args()
    clear_scene()
    bpy.context.scene.unit_settings.system = "METRIC"
    bpy.context.scene.unit_settings.scale_length = 1.0
    build_stage_kit()
    export_outputs(args)


if __name__ == "__main__":
    main()
