# Malltopia Asset and Scene Separation

Version: 0.1  
Status: Working convention

## Goal

Unity에서 보이는 게임 에셋과 Blender 작업 원본을 안전하게 분리한다.

원칙:

- 사람이 수정하는 원본과 자동 생성물을 섞지 않는다.
- Unity가 읽는 최종 에셋 경로를 명확히 둔다.
- 현재 2D 프로토타입과 향후 3D 매장판 전환 작업을 같은 파일에서 섞지 않는다.

## Blender Files

```text
Tools/Blender/
  sources/
    Customers/
      CustomerChibi.blend      # 사람이 Blender에서 수정하는 원본
  scripts/
    export_blend_asset.py      # 열린 .blend를 Unity FBX로 export
    generate_customer_chibi.py # 코드로 손님 원본을 재생성
  output/                      # preview, 임시 파일, git 제외
```

`sources` 아래 파일은 수동 편집 대상이다.  
`output` 아래 파일은 언제든 다시 만들 수 있는 생성물이다.

## Unity Model Files

```text
Assets/Malltopia/Art/Models/
  Customers/
    CustomerChibi.fbx          # Unity가 사용하는 손님 최종 모델
  Prototype/
    PrototypeStageKit.fbx      # 프로토타입 확인용 모델
```

Unity에서는 FBX와 `.meta`를 함께 관리한다. 파일을 옮길 때는 `.meta`도 같이 옮긴다.

## Customer Size

프로토타입 손님 크기는 FBX 원본 scale이 아니라 `StagePrototypeBootstrap`의 칸 기준 값으로 맞춘다.

- `customerVisualHeightCells`: 손님이 화면에서 몇 칸 높이로 보일지
- `customerPrefabScale`: 위 값을 기준으로 한 추가 배율

예시:

```text
customerVisualHeightCells = 2.0   # 기본 손님 키가 2칸
customerVisualHeightCells = 2.3   # 더 크게 보임
```

`0.80`은 80배가 아니라 80% 배율이다.

크기를 바꿨는데 기존 손님이 바로 안 바뀌면, 생성된 손님에 이전 값이 남아 있는 상황일 수 있다. 현재는 Play 중 Inspector 값을 바꾸면 생성된 손님도 다시 맞추도록 처리한다.

손님을 아무리 키워도 화면에서 작게 느껴질 수 있는 주된 이유는 카메라가 너무 많은 행을 한 화면에 넣기 때문이다. `cameraVisibleRows`는 Game view에 세로로 몇 칸을 보여줄지 정한다. 값이 작을수록 줌인되어 손님과 매대가 크게 보인다.

## Current Prototype Scene

현재 `StagePrototypeBootstrap`은 빠른 기능 검증용 2D 프로토타입이다.

현재 좌표 의미:

```text
X = 화면 좌우
Y = 화면 위아래
Z = 표시 레이어
```

향후 3D 매장판에서는 아래 구조로 분리한다.

```text
X = 월드 좌우
Y = 높이
Z = 월드 앞뒤
```

UI 헤더/푸터는 Canvas에 남기고, 매장판/손님/매대는 3D 월드 오브젝트로 분리한다.

## Safe Editing Flow

손님 모델 수정:

1. `Tools/Blender/sources/Customers/CustomerChibi.blend`를 Blender에서 연다.
2. 머리, 몸통, 팔, 가방 등을 수정한다.
3. `Tools/Blender/export_customer_chibi.sh`를 실행한다.
4. Unity에서 `Assets/Malltopia/Art/Models/Customers/CustomerChibi.fbx`가 갱신되는지 확인한다.

코드 생성본으로 되돌릴 때만:

```bash
Tools/Blender/rebuild_customer_chibi_from_script.sh
```

이 명령은 수동 편집 원본을 덮어쓴다.
