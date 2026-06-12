# Malltopia Unity Architecture Guide

Version: 0.1  
Status: MVP implementation guide  

## 1. Goal

이 문서는 Unity MVP를 만들 때 사용할 프로젝트 구조와 주요 시스템 경계를 정의한다.

목표:

- MVP 기능을 빠르게 구현한다.
- 기획/밸런스/테이블 문서와 코드가 같은 기준을 보게 한다.
- Stage 2 이후 확장이 가능한 최소 구조를 만든다.

## 2. Unity Direction

권장 구현 방향:

- 2.5D 또는 탑다운 카메라
- 단일 메인 씬에서 MVP 구현
- ScriptableObject 기반 마스터 데이터
- 로컬 JSON 저장
- waypoint 기반 이동으로 시작

MVP에서는 복잡한 NavMesh보다 waypoint 이동이 더 빠르고 통제하기 쉽다.

### Fixed MVP Decisions

다음 결정은 코드 구현 시 그대로 따른다.

| Topic | Decision |
|---|---|
| Manual action | 탭 부스트, 수동 계산, 수동 재입고 없음 |
| Checkout | 계산대는 직원 없이도 기본 속도로 자동 결제 |
| Cashier | 고용 시 결제 처리 시간 감소 |
| Restock | 진열대는 직원 없이도 기본 속도로 자동 재입고 |
| Restocker | 고용 시 재고 부족 상품을 더 빠르게 보충 |
| Movement | waypoint 기반 이동 |
| Save | 로컬 JSON 파일 |
| Runtime state save | 재고, 손님 위치, 대기열은 저장하지 않음 |

## 3. Recommended Folder Structure

```text
Assets/
  Art/
    Characters/
    Store/
    UI/
  Audio/
  Data/
    Customers/
    Economy/
    Products/
    Staff/
    Stages/
    Store/
  Prefabs/
    Customers/
    Staff/
    Store/
    UI/
  Scenes/
    MainStore.unity
  Scripts/
    Core/
    Customers/
    Economy/
    Products/
    Save/
    Staff/
    Store/
    UI/
  Settings/
```

## 4. Main Systems

| System | Responsibility |
|---|---|
| GameBootstrap | 게임 시작, 데이터 로드, 시스템 초기화 |
| GameState | 현재 Gold, 스테이지, 저장 상태 보관 |
| EconomyService | Gold 획득/소비, 구매 가능 여부 |
| CustomerSpawner | 손님 생성 주기와 최대 수 관리 |
| CustomerAgent | 손님 상태 머신과 이동 처리 |
| ProductShelf | 상품 재고, 해금 상태, 상품 집기 처리 |
| CheckoutCounter | 계산 대기열과 결제 처리 |
| StaffManager | 직원 고용, 업그레이드, 작업 배정 |
| StaffAgent | 직원 이동과 작업 수행 |
| UpgradeService | 상품/직원/매장 업그레이드 |
| SaveService | 저장/로드 |
| OfflineRewardService | 오프라인 보상 계산 |
| UIController | HUD와 패널 갱신 |

## 5. Data Classes

ScriptableObject 예시:

- `StageData`
- `ProductData`
- `StaffData`
- `StoreUpgradeData`
- `CustomerTypeData`
- `EconomyConfigData`
- `OfflineRewardConfigData`

Runtime save model 예시:

- `GameSaveData`
- `ProductSaveData`
- `StaffSaveData`

## 6. Runtime Flow

게임 시작:

1. `GameBootstrap` 실행
2. 마스터 데이터 로드
3. 저장 데이터 로드
4. 저장 데이터가 없으면 신규 데이터 생성
5. 오프라인 보상 계산
6. 씬 오브젝트 상태 적용
7. 손님 생성 루프 시작
8. 자동 저장 루프 시작

손님 구매:

1. `CustomerSpawner`가 손님 생성
2. `CustomerAgent`가 구매 가능한 `ProductShelf` 탐색
3. 진열대로 이동
4. 상품 재고 1 감소
5. 계산대로 이동
6. `CheckoutCounter` 대기열에 등록
7. 계산 완료
8. `EconomyService.AddGold()` 호출
9. 손님 퇴장

상품 업그레이드:

1. UI에서 업그레이드 버튼 클릭
2. `UpgradeService`가 비용 확인
3. `EconomyService.TrySpendGold()` 호출
4. 상품 레벨 증가
5. UI와 진열대 상태 갱신
6. 저장 예약

## 7. Movement

MVP 이동은 waypoint 기반으로 구현한다.

권장 포인트:

- EntrancePoint
- ExitPoint
- ShelfPoint per ProductShelf
- CheckoutQueuePoints
- CheckoutPoint
- StaffWaitingPoint
- RestockPoint per ProductShelf

MVP 이후 매장이 커지면 NavMesh 전환을 검토한다.

## 8. Save Strategy

MVP 저장 방식:

- JSON serialize
- 로컬 파일 저장
- 자동 저장 주기: 10초
- 주요 구매/업그레이드 직후 저장 예약
- 앱 종료 또는 pause 시 저장

저장 파일에는 마스터 데이터 전체를 저장하지 않는다. ID와 진행 상태만 저장한다.

저장하지 않는 항목:

- 현재 진열대 재고
- 현재 손님 목록
- 손님 위치
- 계산 대기열
- 직원 작업 중 상태

## 9. Offline Reward Strategy

오프라인 보상은 정확한 시뮬레이션이 아니라 추정값으로 계산한다.

입력:

- 마지막 저장 시각
- 현재 상품 판매가
- 상품 해금 상태
- 손님 생성 주기
- 계산 처리 시간
- 오프라인 효율

출력:

- 지급 Gold
- 오프라인 시간

## 10. UI Architecture

MVP UI는 다음 패널로 나눈다.

| UI | Responsibility |
|---|---|
| HudView | Gold, Stage 표시 |
| ProductUpgradeView | 상품 해금/업그레이드 |
| StaffUpgradeView | 직원 고용/업그레이드 |
| StoreUpgradeView | 매장 업그레이드 |
| OfflineRewardPopup | 오프라인 보상 |
| SettingsPopup | 저장 초기화, 옵션 |

UI는 직접 데이터를 변경하지 않고 서비스에 요청한다.

예시:

```text
ProductUpgradeButton -> UpgradeService.TryUpgradeProduct(productId)
```

## 11. Implementation Milestones

### Milestone 1: Data and Economy

- ProductData 작성
- StaffData 작성
- EconomyService 구현
- Gold 획득/소비 테스트

### Milestone 2: Store Loop

- ProductShelf 구현
- CustomerSpawner 구현
- CustomerAgent 상태 머신 구현
- CheckoutCounter 구현
- Gold 획득 루프 완성

### Milestone 3: Upgrades

- 상품 해금
- 상품 업그레이드
- 직원 고용
- 직원 업그레이드
- 매장 업그레이드

### Milestone 4: Save and Offline

- SaveService 구현
- 자동 저장
- 앱 재실행 로드
- 오프라인 보상 팝업

### Milestone 5: MVP Polish

- UI 피드백
- 간단한 애니메이션
- 골드 획득 연출
- 밸런스 튜닝
- 10분 플레이 검증

## 12. Coding Guidelines

- 마스터 데이터 값은 코드에 직접 박지 않는다.
- UI는 서비스 계층을 통해서만 상태를 변경한다.
- 저장 데이터에는 ID와 레벨만 저장한다.
- MVP에서는 과한 추상화를 피한다.
- 이벤트는 필요한 곳에만 사용한다.
- 튜닝할 숫자는 반드시 데이터로 뺀다.

## 13. Test Checklist

기본 검증:

- 새 게임 시작 가능
- 손님 생성 가능
- 손님이 상품을 구매함
- 계산 후 Gold 증가
- 상품 해금 가능
- 상품 업그레이드 가능
- 직원 고용 가능
- 직원 업그레이드 가능
- 매장 업그레이드 가능
- 저장 후 재실행 시 상태 복원
- 오프라인 보상 지급

밸런스 검증:

- 첫 업그레이드까지 1분 이내
- 음료 해금까지 3분 전후
- 첫 직원 고용까지 7분 전후
- 10분 플레이 후 매장 혼잡도 증가 확인
