# Malltopia Unity Folder Guide

이 폴더는 Malltopia Unity 구현 전용 영역이다.

## Main Folders

- `Scripts`: C# 코드
- `ScriptableObjects`: 밸런스/마스터 데이터 에셋
- `Prefabs`: 재사용 가능한 씬/UI 오브젝트
- `Art`: 이미지, 아이콘, 캐릭터, 상품, UI 리소스
- `Audio`: BGM과 SFX
- `Animations`: UI와 에이전트 애니메이션
- `Materials`: 머티리얼
- `Fonts`: 폰트

## Prototype Scene

임시 도형으로 MVP 동작을 확인하려면 Unity 상단 메뉴에서
`Malltopia > Prototype > Create Game Scene`을 누른 뒤 `Play`를 실행한다.

- `S1`, `S2`, `S3`: Stage 1~3 전환
- `+G`: 테스트 골드 지급
- `Reset`: 현재 스테이지 초기화
- 빨간 `↑`: 해금, 생산칸 활성화, 업그레이드 가능 표시
- 키보드 `1`, `2`, `3`: Stage 1~3 전환

## Script Rules

- `Data` 계열 파일은 Unity 인스펙터에서 편집할 ScriptableObject다.
- `Service` 계열 파일은 화면 없이 계산 가능한 순수 로직이다.
- `Save` 계열 파일은 로컬 JSON 저장용 모델이다.
- UI와 씬 동작은 이후 패치에서 `Prefabs`와 `Scripts/UI` 아래에 추가한다.
