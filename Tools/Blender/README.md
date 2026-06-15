# Malltopia Blender Pipeline

이 폴더는 Blender로 Malltopia Unity용 3D 에셋을 생성하는 작업 공간이다.

## Folder Rules

- `sources`: 사람이 Blender에서 직접 수정하는 원본 `.blend`
- `scripts`: 자동 생성/export용 Python 스크립트
- `output`: 미리보기, 임시 `.blend` 등 다시 만들어도 되는 생성물
- `Assets/Malltopia/Art/Models`: Unity가 읽는 최종 FBX

## Current Flow

1. Blender에서 `Tools/Blender/sources/Customers/CustomerChibi.blend`를 수정한다.
2. `Tools/Blender/export_customer_chibi.sh`로 FBX를 export한다.
3. 결과 FBX는 `Assets/Malltopia/Art/Models/Customers/CustomerChibi.fbx`에 저장된다.
4. Unity가 열린 상태라면 파일 변경을 감지해서 자동 import한다.

## Run

```bash
Tools/Blender/export_customer_chibi.sh
Tools/Blender/export_prototype_stage_kit.sh
```

기본 출력:

- Editable source: `Tools/Blender/sources/Customers/CustomerChibi.blend`
- Unity asset: `Assets/Malltopia/Art/Models/Customers/CustomerChibi.fbx`
- Unity asset: `Assets/Malltopia/Art/Models/Prototype/PrototypeStageKit.fbx`
- Preview: `Tools/Blender/output/CustomerChibi_Preview.png`

## Rebuild From Code

`CustomerChibi.blend`를 완전히 코드 생성본으로 다시 만들 때만 아래 명령을 쓴다.

```bash
Tools/Blender/rebuild_customer_chibi_from_script.sh
```

이 명령은 `Tools/Blender/sources/Customers/CustomerChibi.blend`를 덮어쓴다.

`Tools/Blender/output`은 생성 작업용 폴더라 git에 올리지 않는다.

`CustomerChibi.fbx`는 프로토타입 씬의 `StagePrototypeBootstrap`에서 손님 에셋으로 자동 로드한다.

## Notes

- Blender 실행 파일 기본 위치는 `/Applications/Blender.app/Contents/MacOS/Blender`다.
- Codex 샌드박스 안에서는 Blender 5.1.2의 Metal 초기화가 크래시날 수 있으므로, Blender 실행은 샌드박스 밖 권한으로 돌린다.
- 현재 Unity 프로젝트에는 glTF/GLB import 패키지가 없어서 FBX를 기본 포맷으로 사용한다.
