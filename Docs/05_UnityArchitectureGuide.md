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
| Manual action | 탭 부스트 없음. 플레이어 입력은 해금/업그레이드/확장 클릭 |
| Main character role | 주문 접수, 생산, 전달을 수행하는 기본 자동 에이전트 |
| Staff role | 하단 스테이지 업그레이드의 `직원 +1`로 추가되는 현재 스테이지 범용 자동 에이전트 |
| Stage scope | 놀이동산 1~3스테이지 검증, 테마 전체 목표는 8스테이지 |
| Unlock flow | 주문 후 해금칸 점멸, 정보창, 매대 해금, 생산 테이블 무료 활성화 |
| Production | 생산 위치 이동 후 원형 진행 게이지 표시 |
| Delivery | 생산 완료 후 손님에게 자동 전달 |
| Movement | 손님, 메인 캐릭터, 직원 모두 waypoint 기반 자동 이동 |
| Equipment | 장기 핵심 성장축. MVP에서는 데이터 자리만 고려 |
| Save | 로컬 JSON 파일 |
| Runtime state save | 진행 중 주문, 진행 중 생산, 손님 위치는 저장하지 않음 |

## 3. Recommended Folder Structure

```text
Assets/
  Art/
    Characters/
    Stage/
    UI/
  Audio/
  Data/
    Characters/
    Customers/
    Equipment/
    Economy/
    MainCharacter/
    Products/
    Staff/
    Stages/
    Themes/
  Prefabs/
    Customers/
    Staff/
    Stage/
    UI/
  Scenes/
    MainStage.unity
  Scripts/
    Characters/
    Core/
    Customers/
    Equipment/
    Economy/
    Firebase/
    MainCharacter/
    Products/
    Save/
    Staff/
    Stage/
    UI/
  Settings/
```

## 4. Main Systems

| System | Responsibility |
|---|---|
| GameBootstrap | 게임 시작, 데이터 로드, 시스템 초기화 |
| GameState | 현재 Gold, 스테이지, 저장 상태 보관 |
| EconomyService | Gold 획득/소비, 구매 가능 여부 |
| ActionAvailabilityService | Gold 기반 해금/업그레이드 가능 여부 계산 |
| NoticeService | 유저가 확인한 전역/기능성 알림 관리 |
| MainCharacterAgent | 메인 캐릭터 자동 행동 상태 머신 |
| WorkerAgent | 직원 자동 행동 상태 머신 |
| AgentStatsService | 메인 캐릭터, 직원, 캐릭터 패시브, 장비 보너스 합산 |
| UnlockService | 상품 매대 해금과 생산 테이블 무료 활성화 처리 |
| StageProgressionService | 현재 테마/스테이지와 다음 스테이지 조건 처리 |
| StageUpgradeService | 하단 업그레이드 리스트 구매와 효과 적용 |
| CustomerSpawner | 손님 퇴장 후 다음 손님 등장 지연과 최대 수 관리 |
| CustomerAgent | 손님 상태 머신과 이동 처리 |
| ProductStand | 상품 매대 해금과 시각 상태 처리 |
| ProductionTable | 상품 생산 가능 여부와 생산 진행 처리 |
| ProductionSlot | 에이전트 1명이 점유하는 개별 생산 위치 |
| ProductStarService | 상품 레벨 기준 별 개수, 보상 배율, 슬롯 표시 조건 계산 |
| ProductionSlotActivationService | 별 조건을 만족한 생산 슬롯의 무료 터치 활성화 처리 |
| OrderRequestService | 첫 주문/해금 상품 제한, 균등 랜덤 주문, 상품 1개 단위 작업 생성 관리 |
| StaffManager | 직원 생성, 레벨 상태, 작업 배정 |
| EquipmentService | 장비 착용, 옵션 계산, 추후 강화 처리 |
| EquipmentCraftService | 장비 합성/제작, MVP 이후 |
| EquipmentDismantleService | 장비 분해와 Star Dust 지급, MVP 이후 |
| CharacterRosterService | 보유 캐릭터, 선택 캐릭터, 캐릭터 패시브 계산, MVP 이후 |
| ShopService | 상점 아이템 구매와 보상 연결, MVP 이후 |
| TimedBoostService | 활성 시간제 부스터와 수익 배율 계산, MVP 이후 |
| GlobalUpgradeService | 금고/전역 업그레이드 효과 계산, MVP 이후 |
| UpgradeService | 상품 레벨 업그레이드와 공통 업그레이드 비용 계산 |
| NumberFormatService | Gold 축약 표기 처리 |
| SaveService | 저장/로드 |
| OfflineRewardService | 오프라인 보상 계산 |
| FirebaseBootstrap | Firebase SDK 초기화, MVP 이후 또는 버전 관리 패치 |
| RemoteConfigService | Firebase Remote Config 값 로드와 기본값 관리 |
| VersionGateService | 최소 지원 버전, 강제 업데이트, 점검 모드 판단 |
| UIController | HUD와 패널 갱신 |

### Agent Stat Outputs

`AgentStatsService`는 메인 캐릭터와 직원의 최종 능력치를 계산한다.

필수 출력값:

| Stat | Applies To | Description |
|---|---|---|
| incomeMultiplier | MainCharacter, Staff | 판매 수익 배율 |
| moveSpeedMultiplier | MainCharacter, Staff | 이동속도 배율 |
| workSpeedMultiplier | MainCharacter, Staff | 주문/생산/전달 작업속도 배율 |
| doubleChancePct | MainCharacter, Staff | 더블 판매 확률 |
| sGradeChancePct | MainCharacter, Staff | S상품 발생 확률 |
| sGradeRewardMultiplier | MainCharacter, Staff | S상품 보상 배율 |
| instantProductionChancePct | MainCharacter, Staff | 생산 시간을 건너뛰고 즉시 생산할 확률 |
| customerMoveSpeedMultiplier | Customer | 손님 입장/퇴장 이동속도 배율 |
| customerOrderSpeedMultiplier | Customer | 손님 주문 대기 시간 감소 배율 |
| equipmentDismantleStarDustBonusPct | Account | 장비 분해 Star Dust 획득량 증가 |
| globalSkillEffectBonusPct | Account | 금고/공용 스킬 효과 소폭 증가 |

판매 수익 계산은 장비 기본 수익, 장비 옵션, 상점 시간제 부스터, 더블, S상품을 모두 곱한다. 캐릭터는 기본적으로 큰 판매 수익 배율에 참여하지 않고, 낮은 이동/작업/유틸 패시브만 제공한다.

```text
finalSaleGold =
  baseSaleGold
  * equipmentBaseIncomeMultiplier
  * incomeOptionMultiplier
  * activeShopBoostMultiplier
  * doubleMultiplier
  * sGradeMultiplier
```

서로 다른 상점 수익 부스터가 동시에 활성화되어 있으면 `activeShopBoostMultiplier`는 각 부스터 배율을 곱한 값이다. 더블과 S상품도 동시에 발동할 수 있다.

`Staff` 대상 옵션은 해금/고용된 직원에게만 적용하며, 메인 캐릭터에는 적용하지 않는다. 메인 캐릭터와 직원을 모두 포함하는 옵션은 `AllWorkers` 대상으로 정의한다.

즉시생산은 생산 시작 시점에 판정한다. 발동하면 생산 시간만 건너뛰고 이동과 전달은 그대로 수행한다.

## 5. Data Classes

ScriptableObject 예시:

- `StageData`
- `StageLayoutPointData`
- `ThemeData`
- `MainCharacterConfigData`
- `CharacterData`
- `CharacterPassiveOptionData`
- `ProductData`
- `ProductStarMilestoneData`
- `ProductProductionSlotData`
- `StaffData`
- `GlobalUpgradeData`
- `ShopItemData`
- `TimedBoostConfigData`
- `EquipmentItemData`
- `EquipmentBlueprintData`
- `EquipmentCraftRecipeData`
- `EquipmentDismantleConfigData`
- `EquipmentOptionData`
- `EquipmentOptionPoolData`
- `StageUpgradeData`
- `CustomerTypeData`
- `CustomerOrderConfigData`
- `NumberFormatConfigData`
- `EconomyConfigData`
- `OfflineRewardConfigData`

Runtime save model 예시:

- `GameSaveData`
- `MainCharacterSaveData`
- `ActiveTimedBoostSaveData`
- `ProductSaveData`
- `StaffSaveData`
- `StageUpgradeSaveData`

Firebase Remote Config model 예시:

- `FirebaseRemoteConfigDefaults`
- `VersionGateConfig`

## 6. Runtime Flow

게임 시작:

1. `GameBootstrap` 실행
2. 마스터 데이터 로드
3. 저장 데이터 로드
4. 저장 데이터가 없으면 신규 데이터 생성
5. 오프라인 보상 계산
6. 씬 오브젝트 상태 적용
7. 메인 캐릭터 자동 에이전트 생성 또는 활성화
8. 손님 생성 루프 시작
9. 자동 저장 루프 시작

주문 처리:

1. `CustomerSpawner`가 손님 생성
2. `MainCharacterAgent`가 손님에게 이동
3. `OrderRequestService`가 주문 생성. 첫 손님은 첫 상품, 이후에는 해금되고 생산 슬롯이 활성화된 상품만 대상
4. 주문 상품 종류 수, 상품 선택, 상품별 1~3개 수량은 균등 랜덤으로 결정
5. 손님 주문 말풍선 표시
6. 해금 가능한 상품칸 또는 생산 테이블이 점멸
7. 플레이어가 해금칸을 클릭해 정보창 확인
8. Gold를 지불해 매대를 해금
9. 표시된 생산 테이블 활성화칸을 터치해 무료 활성화
10. 주문 접수 후 `OrderRequestService`가 상품 1개 단위 작업으로 분해
11. 빈 작업자가 작업 1개를 선택하고 생산 위치로 이동
12. `ProductionTable`이 생산 진행 게이지 표시
13. 생산 완료 후 에이전트가 해당 주문 손님에게 전달
14. 상품 1개 전달 시 `EconomyService.AddGold()` 호출
15. 주문 상품을 모두 받은 손님 퇴장

상품 레벨 업그레이드:

1. UI에서 업그레이드 버튼 클릭
2. `UpgradeService`가 비용 확인
3. `EconomyService.TrySpendGold()` 호출
4. 상품 레벨 증가
5. `ProductStarService`가 업그레이드 전후 별 개수 차이를 계산
6. 새로 도달한 별 마일스톤이 있으면 보상 배율과 생산 슬롯 표시 조건 갱신
7. 새로 도달한 별 마일스톤의 Diamond 보상 지급
8. UI 레벨, 판매 보상, 업그레이드 비용, 별 진행도 즉시 갱신
9. `ProductFeedbackToast`로 보너스 수익과 Diamond 지급 안내
10. 새 피드백 토스트가 발생하면 기존 토스트를 즉시 교체
11. UI와 상품 매대 상태 갱신
12. 저장 예약

생산 슬롯 활성화:

1. 상품 레벨이 별 마일스톤에 도달
2. 연결된 `ProductionSlot` 활성화칸이 씬에 표시됨
3. 플레이어가 활성화칸 클릭
4. `ProductionSlotActivationService`가 슬롯 활성화 상태 저장
5. 에이전트 배정 후보에 해당 슬롯 추가

스테이지 업그레이드:

1. 하단 메뉴 아이콘 클릭
2. `StageUpgradePopup`이 현재 스테이지 업그레이드 리스트 표시
3. 플레이어가 업그레이드 항목 클릭
4. `StageUpgradeService`가 비용과 구매 가능 횟수 확인
5. `EconomyService.TrySpendGold()` 호출
6. 생산속도, 보상, 손님 수, 직원 수 같은 현재 스테이지 효과 적용
7. UI와 저장 상태 갱신

## 7. Movement

MVP 이동은 waypoint 기반으로 구현한다.

권장 포인트:

- EntrancePoint
- ExitPoint
- MainCharacterStartPoint
- OrderPoint
- ProductionPoint per Product
- ProductStandPoint per Product
- DeliveryPoint
- StaffWaitingPoint
- StaffWorkPoint per Product or Role

MVP 이후 매장이 커지면 NavMesh 전환을 검토한다.

## 8. Save Strategy

MVP 저장 방식:

- JSON serialize
- 로컬 파일 저장
- 자동 저장 주기: 10초
- 주요 구매/업그레이드 직후 저장 예약
- 앱 종료 또는 pause 시 저장

저장 파일에는 마스터 데이터 전체를 저장하지 않는다. ID와 진행 상태만 저장한다.

Firebase Remote Config는 저장 데이터가 아니라 운영 설정으로 취급한다. 앱 시작 시 기본값을 먼저 사용하고, 네트워크로 Remote Config를 가져오면 버전 게이트와 운영 플래그를 갱신한다.

Remote Config 판단 순서:

1. 로컬 기본 Remote Config 값 로드
2. Firebase 초기화
3. Remote Config fetch and activate
4. `maintenance_mode` 확인
5. `min_supported_version` 확인
6. `latest_version` 기준 권장 업데이트 표시 여부 확인
7. 통과 시 로컬 SaveData 로드

저장하지 않는 항목:

- 진행 중 주문
- 진행 중 생산
- 현재 손님 목록
- 손님 위치
- 직원 작업 중 상태

## 9. Offline Reward Strategy

오프라인 보상은 정확한 시뮬레이션이 아니라 추정값으로 계산한다.

입력:

- 마지막 저장 시각
- 현재 상품 판매가
- 상품 해금 상태
- 손님 퇴장 후 다음 손님 등장 지연
- 주문/전달 처리 시간
- 오프라인 효율

출력:

- 지급 Gold
- 오프라인 시간

## 10. UI Architecture

MVP UI는 다음 패널로 나눈다.

| UI | Responsibility |
|---|---|
| HudView | Gold, Stage 표시 |
| FooterMenuView | 하단 메뉴 버튼과 다음 스테이지 버튼 표시 |
| OrderBubbleView | 손님 주문 상품 표시 |
| UnlockSlotView | 해금/활성화 가능한 네모칸 점멸과 클릭 처리 |
| ActionAvailableIndicatorView | 구매 가능 원형 화살표 표시 |
| UnlockInfoPopup | 상품명, 해금 필요 Gold 표시 |
| AgentProgressView | 생산 중 원형 진행 게이지 표시 |
| ProductStationInfoPopup | 상품 레벨, 별, 다음 별 진행도, 보상, 생산 시간, 업그레이드 비용 표시 |
| ProductFeedbackToast | 레벨업, 별 보너스, Diamond 지급 안내. 최신 토스트가 기존 토스트를 즉시 교체 |
| ProductUpgradeView | 상품 해금/업그레이드 |
| DiamondHudView | 현재 Diamond 표시 |
| StageUpgradePopup | 생산속도, 보상, 손님 +1, 직원 +1 업그레이드 |
| NextStageButtonView | 조건 충족 시 점멸하고 이동 비용 표시 |
| CharacterRosterView | 캐릭터 보유, 선택, 패시브 표시, MVP 이후 |
| EquipmentView | 장비 착용과 옵션 표시, MVP 이후 |
| OfflineRewardPopup | 오프라인 보상 |
| SettingsPopup | 저장 초기화, 옵션 |

UI는 직접 데이터를 변경하지 않고 서비스에 요청한다.

예시:

```text
ProductUpgradeButton -> UpgradeService.TryUpgradeProduct(productId)
```

### Indicator Rules

- Gold 기반 스테이지 내부 액션은 `ActionAvailabilityService`가 구매 가능 여부를 계산한다.
- 구매 가능한 Gold 액션에는 원형 화살표 표시를 계속 노출한다.
- Gold 액션 표시 상태는 저장하지 않는다.
- Diamond, 캐릭터, 장비, 금고, 상자, 신규 기능 알림은 `NoticeService`가 `seenNoticeIds`를 기준으로 표시 여부를 결정한다.
- 전역/기능성 알림은 유저가 해당 화면을 확인하면 숨긴다.

## 11. Implementation Milestones

### Milestone 1: Data and Economy

- MainCharacterConfigData 작성
- ThemeData 작성
- StageData 작성
- ProductData 작성
- ProductStarMilestoneData 작성
- ProductProductionSlotData 작성
- StaffData 작성
- GlobalUpgradeData 자리 작성
- StageUpgradeData 작성
- NumberFormatConfigData 작성
- EquipmentItemData 자리 작성
- EquipmentBlueprintData 자리 작성
- EquipmentCraftRecipeData 자리 작성
- EquipmentDismantleConfigData 자리 작성
- EquipmentOptionData 자리 작성
- EquipmentOptionPoolData 자리 작성
- EconomyService 구현
- Gold 획득/소비 테스트

### Milestone 2: Order and Unlock Loop

- MainCharacterAgent 구현
- ProductStand 구현
- ProductionTable 구현
- UnlockService 구현
- CustomerSpawner 구현
- CustomerAgent 상태 머신 구현
- OrderBubbleView 구현
- Gold 획득 루프 완성

### Milestone 3: Upgrades

- 상품 해금
- 상품 업그레이드
- 별 마일스톤 처리
- 별 달성 시 Diamond +1 지급
- 상품 보너스 안내 토스트
- 길게 눌러 레벨업 중 토스트 최신 이벤트 교체
- 생산 슬롯 무료 활성화 처리
- 하단 스테이지 업그레이드 팝업
- 손님 +1 업그레이드
- 직원 +1 업그레이드
- 다음 스테이지 버튼 점멸 조건

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
- Gold 계열 값은 MVP에서 `double` 기반 `CurrencyAmount` 규칙으로 처리한다.
- UI는 서비스 계층을 통해서만 상태를 변경한다.
- 저장 데이터에는 ID와 레벨만 저장한다.
- Gold 기반 가능 표시 상태는 저장하지 않고 런타임 계산으로 처리한다.
- 확인 후 숨겨야 하는 전역/기능성 알림만 `seenNoticeIds`로 저장한다.
- 메인 캐릭터와 직원의 런타임 위치, 진행 중 작업 상태는 저장하지 않는다.
- MVP에서는 과한 추상화를 피한다.
- 이벤트는 필요한 곳에만 사용한다.
- 튜닝할 숫자는 반드시 데이터로 뺀다.

## 13. Test Checklist

기본 검증:

- 새 게임 시작 가능
- 메인 캐릭터 자동 이동 가능
- 주문 말풍선 표시 가능
- 상품 매대 해금과 생산 테이블 무료 활성화 가능
- Gold 기반 구매 가능 원형 화살표 표시 가능
- 전역/기능성 신규 알림 확인 후 숨김 가능
- 상품 별 마일스톤 계산 가능
- 별 조건 후 추가 생산 슬롯 무료 활성화 가능
- 생산 진행 게이지 표시 가능
- 손님 생성 가능
- 손님이 상품을 구매함
- 상품 전달 후 Gold 증가
- 상품 해금 가능
- 상품 업그레이드 가능
- 스테이지 업그레이드 리스트에서 손님 +1 구매 가능
- 스테이지 업그레이드 리스트에서 직원 +1 구매 가능
- 다음 스테이지 버튼 점멸 가능
- 저장 후 재실행 시 상태 복원
- 오프라인 보상 지급

밸런스 검증:

- 상품 순번별 생산 시간이 5초, 9초, 13초 기준으로 적용되는지 확인
- 매대 1칸 이동 시간이 1.0초 기준으로 적용되는지 확인
- 첫 판매, 첫 레벨업, 첫 별, 첫 직원 +1 시점이 자연스럽게 느껴지는지 확인
- 10분 플레이 후 매장 혼잡도 증가 확인
