import argparse
import math
import sys
from pathlib import Path

import bpy
from mathutils import Vector


def parse_args():
    argv = []
    if "--" in sys.argv:
        argv = sys.argv[sys.argv.index("--") + 1 :]

    parser = argparse.ArgumentParser(
        description="Generate a cute 2-head Malltopia customer character for Unity."
    )
    parser.add_argument(
        "--fbx",
        default="Assets/Malltopia/Art/Models/Customers/CustomerChibi.fbx",
        help="FBX output path for Unity.",
    )
    parser.add_argument(
        "--blend",
        default="Tools/Blender/sources/Customers/CustomerChibi.blend",
        help="Optional .blend source output path.",
    )
    parser.add_argument(
        "--preview",
        default="Tools/Blender/output/CustomerChibi_Preview.png",
        help="Optional PNG preview output path.",
    )
    return parser.parse_args(argv)


def clear_scene():
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete()


def make_material(name, color, roughness=0.68):
    material = bpy.data.materials.new(name)
    material.diffuse_color = color
    material.use_nodes = True
    principled = material.node_tree.nodes.get("Principled BSDF")
    if principled:
        principled.inputs["Base Color"].default_value = color
        principled.inputs["Roughness"].default_value = roughness
    return material


def apply_material(obj, material):
    obj.data.materials.clear()
    obj.data.materials.append(material)


def shade_smooth(obj):
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)
    bpy.ops.object.shade_smooth()
    obj.select_set(False)
    obj.modifiers.new(f"{obj.name}_WeightedNormals", "WEIGHTED_NORMAL")


def add_uv_sphere(name, location, scale, material, segments=32, rings=16):
    bpy.ops.mesh.primitive_uv_sphere_add(
        segments=segments,
        ring_count=rings,
        radius=1.0,
        location=location,
    )
    obj = bpy.context.object
    obj.name = name
    obj.scale = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    apply_material(obj, material)
    shade_smooth(obj)
    return obj


def add_cylinder_between(name, start, end, radius, material, vertices=24):
    start_v = Vector(start)
    end_v = Vector(end)
    direction = end_v - start_v
    midpoint = start_v + direction * 0.5
    bpy.ops.mesh.primitive_cylinder_add(
        vertices=vertices,
        radius=radius,
        depth=direction.length,
        location=midpoint,
    )
    obj = bpy.context.object
    obj.name = name
    obj.rotation_euler = direction.to_track_quat("Z", "Y").to_euler()
    apply_material(obj, material)
    shade_smooth(obj)
    return obj


def add_cube(name, location, scale, material, bevel=0.0):
    bpy.ops.mesh.primitive_cube_add(size=1, location=location)
    obj = bpy.context.object
    obj.name = name
    obj.dimensions = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    apply_material(obj, material)

    if bevel > 0:
        bevel_modifier = obj.modifiers.new(f"{name}_SoftEdges", "BEVEL")
        bevel_modifier.width = bevel
        bevel_modifier.segments = 3
        bevel_modifier.affect = "EDGES"
        obj.modifiers.new(f"{name}_WeightedNormals", "WEIGHTED_NORMAL")

    return obj


def add_smile(name, material):
    curve = bpy.data.curves.new(name, "CURVE")
    curve.dimensions = "3D"
    curve.resolution_u = 16
    curve.bevel_depth = 0.012
    curve.bevel_resolution = 3

    spline = curve.splines.new("BEZIER")
    spline.bezier_points.add(2)
    points = [
        (-0.14, -0.515, 1.42),
        (0.0, -0.56, 1.38),
        (0.14, -0.515, 1.42),
    ]
    for point, co in zip(spline.bezier_points, points):
        point.co = co
        point.handle_left_type = "AUTO"
        point.handle_right_type = "AUTO"

    obj = bpy.data.objects.new(name, curve)
    bpy.context.collection.objects.link(obj)
    apply_material(obj, material)

    bpy.ops.object.select_all(action="DESELECT")
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)
    bpy.ops.object.convert(target="MESH")
    bpy.context.object.name = name
    return bpy.context.object


def build_customer():
    mats = {
        "skin": make_material("MT_Customer_Skin_Peach", (1.0, 0.78, 0.62, 1.0)),
        "blush": make_material("MT_Customer_Blush_Rose", (1.0, 0.43, 0.5, 1.0)),
        "hair": make_material("MT_Customer_Hair_Brown", (0.22, 0.12, 0.07, 1.0)),
        "shirt": make_material("MT_Customer_Shirt_Mint", (0.24, 0.72, 0.62, 1.0)),
        "collar": make_material("MT_Customer_Collar_Cream", (1.0, 0.94, 0.78, 1.0)),
        "detail": make_material("MT_Customer_Detail_Dark", (0.04, 0.05, 0.07, 1.0)),
        "bag": make_material("MT_Customer_Bag_Yellow", (1.0, 0.77, 0.25, 1.0)),
    }

    # Body is intentionally small and rounded for a 2-head chibi feel.
    add_uv_sphere(
        "CustomerChibi_Body_RoundedTorso",
        (0, 0, 0.66),
        (0.43, 0.34, 0.58),
        mats["shirt"],
        32,
        16,
    )
    add_cube("CustomerChibi_Collar_Left", (-0.12, -0.31, 1.03), (0.18, 0.04, 0.12), mats["collar"], 0.02)
    add_cube("CustomerChibi_Collar_Right", (0.12, -0.31, 1.03), (0.18, 0.04, 0.12), mats["collar"], 0.02)
    add_cube("CustomerChibi_Bag", (0.36, -0.28, 0.61), (0.22, 0.11, 0.28), mats["bag"], 0.025)
    add_cylinder_between("CustomerChibi_Bag_Strap", (0.18, -0.34, 0.98), (0.44, -0.34, 0.45), 0.018, mats["detail"], 16)

    # Head and soft facial features.
    add_uv_sphere("CustomerChibi_Head", (0, 0, 1.42), (0.55, 0.5, 0.55), mats["skin"], 48, 24)
    add_uv_sphere("CustomerChibi_Ear_Left", (-0.52, -0.01, 1.41), (0.1, 0.06, 0.13), mats["skin"], 24, 12)
    add_uv_sphere("CustomerChibi_Ear_Right", (0.52, -0.01, 1.41), (0.1, 0.06, 0.13), mats["skin"], 24, 12)

    # Hair cap and a tiny front tuft.
    add_uv_sphere("CustomerChibi_Hair_Cap", (0, 0.03, 1.67), (0.57, 0.47, 0.28), mats["hair"], 48, 16)
    add_uv_sphere("CustomerChibi_Hair_Tuft_Center", (0.0, -0.38, 1.79), (0.16, 0.08, 0.2), mats["hair"], 24, 12)
    add_uv_sphere("CustomerChibi_Hair_Tuft_Left", (-0.16, -0.34, 1.73), (0.13, 0.07, 0.16), mats["hair"], 24, 12)
    add_uv_sphere("CustomerChibi_Hair_Tuft_Right", (0.16, -0.34, 1.73), (0.13, 0.07, 0.16), mats["hair"], 24, 12)

    # Face points toward -Y so it reads well in Unity's default scene view.
    add_uv_sphere("CustomerChibi_Eye_Left", (-0.18, -0.48, 1.49), (0.045, 0.018, 0.07), mats["detail"], 20, 10)
    add_uv_sphere("CustomerChibi_Eye_Right", (0.18, -0.48, 1.49), (0.045, 0.018, 0.07), mats["detail"], 20, 10)
    add_uv_sphere("CustomerChibi_Blush_Left", (-0.32, -0.49, 1.36), (0.07, 0.014, 0.035), mats["blush"], 16, 8)
    add_uv_sphere("CustomerChibi_Blush_Right", (0.32, -0.49, 1.36), (0.07, 0.014, 0.035), mats["blush"], 16, 8)
    add_smile("CustomerChibi_Smile", mats["detail"])

    # Simple arms and mitten-like hands. Feet are intentionally omitted.
    add_cylinder_between("CustomerChibi_Arm_Left", (-0.36, -0.02, 0.93), (-0.61, -0.08, 0.58), 0.07, mats["shirt"], 24)
    add_cylinder_between("CustomerChibi_Arm_Right", (0.36, -0.02, 0.93), (0.61, -0.08, 0.58), 0.07, mats["shirt"], 24)
    add_uv_sphere("CustomerChibi_Hand_Left", (-0.64, -0.08, 0.54), (0.11, 0.1, 0.11), mats["skin"], 24, 12)
    add_uv_sphere("CustomerChibi_Hand_Right", (0.64, -0.08, 0.54), (0.11, 0.1, 0.11), mats["skin"], 24, 12)


def add_scene_helpers():
    bpy.ops.object.light_add(type="AREA", location=(0, -3.2, 3.8))
    key_light = bpy.context.object
    key_light.name = "CustomerChibi_Source_KeyLight"
    key_light.data.energy = 430
    key_light.data.size = 3.0

    bpy.ops.object.light_add(type="POINT", location=(-2.2, 1.4, 2.3))
    fill_light = bpy.context.object
    fill_light.name = "CustomerChibi_Source_FillLight"
    fill_light.data.energy = 80

    bpy.ops.object.camera_add(location=(0, -5.2, 1.08), rotation=(math.radians(90), 0, 0))
    camera = bpy.context.object
    camera.name = "CustomerChibi_Source_Camera"
    camera.data.type = "ORTHO"
    camera.data.ortho_scale = 2.45
    bpy.context.scene.camera = camera


def export_outputs(args):
    fbx_path = Path(args.fbx).expanduser().resolve()
    blend_path = Path(args.blend).expanduser().resolve()
    preview_path = Path(args.preview).expanduser().resolve()
    fbx_path.parent.mkdir(parents=True, exist_ok=True)
    blend_path.parent.mkdir(parents=True, exist_ok=True)
    preview_path.parent.mkdir(parents=True, exist_ok=True)

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

    bpy.context.scene.render.engine = "BLENDER_WORKBENCH"
    bpy.context.scene.display.shading.light = "STUDIO"
    bpy.context.scene.display.shading.color_type = "MATERIAL"
    bpy.context.scene.render.resolution_x = 1200
    bpy.context.scene.render.resolution_y = 1200
    bpy.context.scene.render.film_transparent = False
    bpy.context.scene.world.color = (0.93, 0.95, 0.96)
    bpy.context.scene.render.filepath = str(preview_path)
    bpy.ops.render.render(write_still=True)

    print(f"Exported Blender source: {blend_path}")
    print(f"Exported Unity FBX: {fbx_path}")
    print(f"Exported preview PNG: {preview_path}")


def main():
    args = parse_args()
    clear_scene()
    bpy.context.scene.unit_settings.system = "METRIC"
    bpy.context.scene.unit_settings.scale_length = 1.0
    build_customer()
    add_scene_helpers()
    export_outputs(args)


if __name__ == "__main__":
    main()
