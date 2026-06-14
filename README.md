# Malltopia

쇼핑 제국을 건설하는 방치형 경영 시뮬레이션.

## Documents

- [Game Design Document](Docs/01_GameDesignDocument.md)
- [MVP Specification](Docs/02_MVP_Specification.md)
- [Balance Guide](Docs/03_BalanceGuide.md)
- [Data Table Specification](Docs/04_DataTableSpec.md)
- [Unity Architecture Guide](Docs/05_UnityArchitectureGuide.md)
- [Next Patch Plan](Docs/06_NextPatchPlan.md)

## Unity Prototype Controls

- `S1`, `S2`, `S3`: debug stage switch
- Footer `plane`: stage clear condition and move cost check
- Footer `safe`: vault placeholder
- Footer `booster`: debug Gold grant
- Footer `person`: character placeholder
- Footer `up arrow`: stage upgrade list open/close
- Keyboard `R`: reload current stage

## Prototype UI Assets

- Footer button and icon PNGs are managed in `Assets/Malltopia/Art/UI/Icons`.
- Current footer background files: `button_footer_base.png`, `button_footer_base_pressed.png`.
- Current sample icon file: `footer_character.png`.

## Prototype Theme Skin

- The current test theme is `AMUSE PARK`.
- The prototype theme is assembled from floor colors, counter colors, stand colors, and simple decoration objects inside the Unity scene.
- Full-background PNGs are avoided for now so stage layouts can change without repainting the whole map.
