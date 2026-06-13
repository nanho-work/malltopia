# Malltopia Data Table Specification

Version: 0.1  
Status: MVP data contract  

## 1. Purpose

Malltopia MVP는 유저 진행용 서버 DB를 사용하지 않는다. 이 문서는 Unity 클라이언트에서 사용할 마스터 데이터와 저장 데이터의 구조를 정의한다. 단, 앱 버전 관리와 운영 설정에는 Firebase Remote Config를 사용할 수 있다.

권장 방식:

- 마스터 데이터: ScriptableObject 또는 CSV/JSON
- 저장 데이터: 로컬 JSON
- 런타임 상태: C# class instance
- 운영 설정: Firebase Remote Config

초기 개발은 ScriptableObject를 기준으로 한다. 밸런스 조정이 잦아지면 CSV/Google Sheets 기반으로 옮길 수 있다.

## 2. Naming Rules

ID 규칙:

- 소문자 영문
- 단어 구분은 underscore
- 표시 이름은 별도 필드 사용

예시:

- `theme_amusement`
- `amusement_001`
- `cotton_candy`
- `bead_icecream`
- `chicken_skewer`
- `generalist_staff`

Gold 값 규칙:

- Gold, 비용, 판매가, 보상처럼 커질 수 있는 값은 `CurrencyAmount`로 취급한다.
- MVP 코드 구현 타입은 `double`을 기본으로 한다.
- 표시 문자열은 저장하지 않고 `NumberFormatConfig`와 `NumberFormatService`로 변환한다.
- `aa`, `ab`, `ac` 이후의 초대형 수치에서 정밀도가 문제가 되면 별도 BigNumber 구조로 교체한다.

## 3. Table List

| Table | Purpose | MVP Required |
|---|---|---|
| ThemeTable | 테마 기본 정보 | Yes |
| StageTable | 스테이지 기본 정보 | Yes |
| StageClearRewardTable | 스테이지 클리어 보상 정보 | Later |
| StageLayoutPointTable | 스테이지 배치/웨이포인트 정보 | Yes |
| MainCharacterConfig | 메인 캐릭터 자동 에이전트 기본 능력치 | Yes |
| ProductTable | 상품 기본 정보 | Yes |
| ProductLevelCurveTable | 상품 레벨별 판매가/업그레이드 비용 곡선 | Yes |
| ProductStarMilestoneTable | 상품 별 마일스톤 정보 | Yes |
| ProductProductionSlotTable | 상품 생산 슬롯 표시/활성화 정보 | Yes |
| StaffTable | 직원 기본 정보 | Yes |
| StageUpgradeTable | 하단 메뉴 업그레이드 리스트 | Yes |
| CustomerTypeTable | 손님 타입 정보 | Yes |
| CustomerOrderConfig | 손님 주문 생성 규칙 | Yes |
| NumberFormatConfig | Gold 축약 표기 설정 | Yes |
| GlobalUpgradeTable | 금고/전역 업그레이드 정보 | Later |
| ChestTable | Diamond 상자 정보 | Later |
| ChestDropRateTable | 상자 등급별 드랍 확률 | Later |
| ChestPityConfig | 상자 천장/보장 설정 | Later |
| ShopItemTable | 상점 판매 아이템 정보 | Later |
| TimedBoostConfig | 시간제 부스터 효과 정보 | Later |
| CharacterTable | 구매/선택 가능한 캐릭터 정보 | Later |
| CharacterPassiveOptionTable | 캐릭터 패시브 옵션 정보 | Later |
| EquipmentItemTable | 장비 아이템 정보 | Later |
| EquipmentBlueprintTable | 장비 제작 문서 정보 | Later |
| EquipmentCraftRecipeTable | 장비 합성/제작 레시피 | Later |
| EquipmentDismantleConfig | 장비 분해 보상 설정 | Later |
| EquipmentOptionTable | 장비 옵션 정보 | Later |
| EquipmentOptionPoolTable | 등급/부위별 옵션 등장 풀 | Later |
| EconomyConfig | 경제 공식 기준값 | Yes |
| OfflineRewardConfig | 오프라인 보상 기준값 | Yes |
| AdRewardConfig | 광고 보상 기준값 | Stub |
| FirebaseRemoteConfigKeys | Firebase Remote Config 키 명세 | Stub |
| SaveData | 유저 저장 데이터 | Yes |
| SeenNoticeSaveData | 유저가 확인한 전역/기능성 알림 | Yes |

## 4. ThemeTable

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| themeId | string | Yes | theme_amusement | 테마 ID |
| displayName | string | Yes | 놀이동산 | 표시 이름 |
| order | int | Yes | 1 | 테마 순서 |
| stageIds | string[] | Yes | amusement_001 | 포함 스테이지 |
| targetStageCount | int | No | 8 | 목표 스테이지 수 |

## 5. StageTable

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| stageId | string | Yes | amusement_001 | 스테이지 ID |
| themeId | string | Yes | theme_amusement | 소속 테마 |
| displayName | string | Yes | 놀이동산 1 | 표시 이름 |
| stageNumber | int | Yes | 1 | 테마 내 스테이지 번호 |
| gridWidth | int | Yes | 11 | 스테이지 화면 그리드 가로 칸 수 |
| gridHeight | int | Yes | 20 | 스테이지 화면 그리드 세로 칸 수 |
| scrollMode | enum | Yes | FixedNoScroll | 스테이지 카메라/스크롤 방식 |
| productIds | string[] | Yes | cotton_candy | 포함 상품 |
| startingGold | double | Yes | 5 | 스테이지 시작 Gold |
| startingCustomerCount | int | Yes | 1 | 스테이지 시작 손님 수 |
| startingStaffCount | int | Yes | 0 | 스테이지 시작 직원 수 |
| nextStageCostRule | enum | Yes | MaxFinalUpgradeCostMultiplier | 다음 스테이지 이동 비용 산출 규칙 |
| nextStageCostMultiplier | float | Yes | 1.5 | 가장 비싼 최종 레벨업 비용에 곱할 배율 |
| nextStageCostOverride | double | No | 0 | 0이면 공식 사용, 특수 스테이지에서만 직접 비용 지정 |
| clearConditionType | enum | Yes | AllProductsMaxLevel | 클리어 조건 타입 |
| requiredMaxLevelProductIds | string[] | Yes | cotton_candy | 다음 스테이지 조건 상품 |
| maxProductStandCount | int | No | 1 | 해당 스테이지 최대 매대 수 |
| clearRewardGroupId | string | No | amusement_001_clear_reward | 클리어 보상 그룹 ID |

### StageClearConditionType Enum

| Value | Description |
|---|---|
| AllProductsMaxLevel | 현재 스테이지의 모든 상품을 해금하고 최대 레벨까지 업그레이드 |

### StageScrollMode Enum

| Value | Description |
|---|---|
| FixedNoScroll | 스테이지 전체가 한 화면에 들어오며 맵 스크롤이 없음 |

### NextStageCostRule Enum

| Value | Description |
|---|---|
| MaxFinalUpgradeCostMultiplier | 현재 스테이지 상품 중 가장 비싼 최종 레벨업 비용 x `nextStageCostMultiplier` |

## 5.1 StageClearRewardTable

스테이지 클리어 보상 정보다. 참고 게임에서는 Stage 1 클리어 후 Common Chest x1 같은 보상이 관측됐다. MVP에서는 실제 상자 오픈을 구현하지 않더라도, 스테이지 보상 데이터 자리는 둘 수 있다.

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| rewardId | string | Yes | amusement_001_clear_common_chest | 보상 ID |
| rewardGroupId | string | Yes | amusement_001_clear_reward | StageTable에서 참조하는 보상 그룹 ID |
| stageId | string | Yes | amusement_001 | 대상 스테이지 ID |
| rewardType | enum | Yes | Chest | 보상 타입 |
| rewardTargetId | string | No | chest_common | 보상 대상 ID |
| amount | int | Yes | 1 | 지급 수량 |
| enabledInMvp | bool | Yes | false | MVP에서 실제 지급/사용 여부 |

### StageClearRewardType Enum

| Value | Description |
|---|---|
| Chest | 상자 지급 |
| Diamond | Diamond 지급 |
| Gold | Gold 지급 |
| Item | 특정 아이템 지급 |

## 5.2 StageLayoutPointTable

스테이지 안의 주요 배치 지점과 손님 주문 위치를 정의한다. MVP는 고정 좌표로 시작해도 되지만, 손님 +1 이후 카운터 앞/좌우/뒤쪽 fallback 배치를 위해 데이터 구조를 둔다.

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| pointId | string | Yes | amusement_001_counter_01 | 지점 ID |
| stageId | string | Yes | amusement_001 | 소속 스테이지 |
| pointType | enum | Yes | CustomerOrder | 지점 타입 |
| productId | string | No | cotton_candy | 특정 상품과 연결되는 경우 |
| gridX | int | Yes | 1 | 스테이지 그리드 X |
| gridY | int | Yes | 0 | 스테이지 그리드 Y |
| priority | int | Yes | 1 | 손님 배치 우선순위. 낮을수록 먼저 사용 |
| capacity | int | Yes | 1 | 해당 지점 수용 인원 |

### StageLayoutPointType Enum

| Value | Description |
|---|---|
| Entrance | 손님 입장 위치 |
| Exit | 손님 퇴장 위치 |
| CustomerPath | 손님 이동길 |
| CustomerOrder | 카운터 앞 기본 주문 위치 |
| CustomerOrderSide | 카운터 좌우 확장 주문 위치 |
| CustomerOrderBack | 카운터 뒤쪽 fallback 주문 위치 |
| Counter | 카운터 타일 |
| ProductStand | 상품 매대 위치 |
| ProductionSlot | 생산 슬롯 위치 |
| MainCharacterStart | 메인 캐릭터 시작 위치 |
| StaffWaiting | 직원 대기 위치 |
| DecorationArea | 장식/여백 영역 기준점 |

## 6. MainCharacterConfig

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| mainCharacterId | string | Yes | default_main_character | 메인 캐릭터 설정 ID |
| moveTimePerTileSec | float | Yes | 1.0 | 매대 1칸 기준 이동 시간 |
| carryCapacity | int | Yes | 1 | 한 번에 운반 가능한 상품 수 |
| maxCarryCapacity | int | Yes | 2 | 자원/장비 확장 후 허용할 최대 운반 수 |
| orderTakeTimeSec | float | Yes | 0.4 | 주문 접수 시간 |
| workSpeedMultiplier | float | Yes | 1.0 | 상품 생산 시간에 곱하는 기본 작업속도 배율 |
| deliveryHandoffTimeSec | float | Yes | 0.5 | 상품 전달 처리 시간 |
| baseIncomeBonusPct | float | Yes | 0 | 기본 판매 수익 보너스 |
| sGradeChancePct | float | Yes | 0 | 기본 S급 상품 확률 |

## 7. ProductTable

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| productId | string | Yes | cotton_candy | 고유 ID |
| displayName | string | Yes | 솜사탕 | 표시 이름 |
| themeId | string | Yes | theme_amusement | 소속 테마 |
| stageId | string | Yes | amusement_001 | 소속 스테이지 |
| standOrder | int | Yes | 1 | 스테이지 내 매대 순서 |
| isStartingStand | bool | Yes | true | 스테이지 시작 매대 여부 |
| unlockCost | double | Yes | 5 | 해금 비용 |
| basePrice | double | Yes | 2 | 레벨 1 판매가 |
| baseProductionAmount | int | Yes | 1 | 1회 생산 시 기본 생산 수량 |
| baseProductionTimeSec | float | Yes | 5.0 | 기본 생산 시간 |
| maxLevel | int | Yes | 100 | 최대 레벨 |
| maxStarCount | int | Yes | 1 | 최대 별 개수 |
| baseProductionSlotCount | int | Yes | 1 | 기본 생산 슬롯 수 |
| maxProductionSlotCount | int | Yes | 2 | 최대 생산 슬롯 수 |
| upgradeBaseCost | double | Yes | 3 | 첫 업그레이드 비용 |
| levelCurveId | string | No | curve_mvp_starter_25 | 판매가/업그레이드 비용 구간 곡선 ID |
| priceGrowth | float | No | 1.10 | levelCurveId가 없을 때 사용하는 판매가 증가율 fallback |
| costGrowth | float | No | 1.12 | levelCurveId가 없을 때 사용하는 비용 증가율 fallback |
| standPrefabKey | string | No | stand_cotton_candy | 매대 프리팹 참조 키 |
| iconKey | string | No | icon_cotton_candy | UI 아이콘 키 |

MVP 상품 생산 시간 규칙:

```text
baseProductionTimeSec = 5 + ((standOrder - 1) * 4)
```

모든 스테이지의 첫 상품은 5초, 두 번째 상품은 9초, 세 번째 상품은 13초로 시작한다.

## 7.1 ProductLevelCurveTable

MVP 초반 밸런스는 구간 곡선을 우선 사용한다. 별 배율은 이 테이블에 포함하지 않고, `ProductStarMilestoneTable`의 별 개수를 기준으로 별도 곱한다. 초반 튜토리얼 구간은 `Add` 성장을 사용할 수 있고, 25레벨 이후 관측값 기준 구간은 `Multiply` 성장을 사용한다.

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| curveId | string | Yes | curve_mvp_starter_75 | 곡선 ID |
| levelStart | int | Yes | 1 | 구간 시작 레벨 |
| levelEnd | int | Yes | 9 | 구간 종료 레벨 |
| levelBaseSaleGoldAtStart | double | Yes | 2 | 구간 시작 레벨의 별 제외 기본 판매가 |
| saleGrowthType | enum | Yes | Add | 판매가 성장 타입 |
| saleGrowthValue | double | Yes | 1 | Add면 증가량, Multiply면 배율 |
| upgradeCostGoldAtStart | double | Yes | 3 | 구간 시작 레벨에서 다음 레벨로 올리는 비용 |
| upgradeCostGrowthType | enum | Yes | Add | 업그레이드 비용 성장 타입 |
| upgradeCostGrowthValue | double | Yes | 1 | Add면 증가량, Multiply면 배율 |
| note | string | No | no star | 밸런스 메모 |

### ProductLevelCurveGrowthType Enum

| Value | Description |
|---|---|
| Add | 시작값에 `levelOffset * growthValue`를 더함 |
| Multiply | 시작값에 `growthValue ^ levelOffset`을 곱함 |
| Fixed | 구간 내 값을 시작값으로 고정 |

구간 내 계산:

```text
levelOffset = level - levelStart

if growthType == Add:
  value = startValue + (levelOffset * growthValue)

if growthType == Multiply:
  value = startValue * growthValue ^ levelOffset

if growthType == Fixed:
  value = startValue
```

별 마일스톤 레벨은 별 배율로 큰 성장을 보여주는 지점이므로, `levelBaseSaleGold` 자체는 직전 구간 끝값에 가깝게 둔다. 최종 판매가는 `levelBaseSaleGold * starMultiplier * otherMultipliers`로 계산한다.

## 8. ProductStarMilestoneTable

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| milestoneId | string | Yes | cotton_candy_star_01 | 마일스톤 ID |
| productId | string | Yes | cotton_candy | 대상 상품 ID |
| starIndex | int | Yes | 1 | 별 순서 |
| requiredLevel | int | Yes | 10 | 별 획득 필요 레벨 |
| rewardMultiplier | float | Yes | 2.0 | 별 획득 시 보상 배율 |
| diamondReward | int | Yes | 1 | 별 획득 시 지급 Diamond |
| showProductionSlotIds | string[] | No | chicken_skewer_slot_02 | 조건 충족 후 표시할 생산 슬롯 |

별 개수는 저장하지 않고 상품 레벨과 이 테이블을 기준으로 계산한다. Diamond 보상은 로드 시 재계산해 지급하지 않고, 상품 레벨업 처리 중 새로 도달한 마일스톤에 대해서만 1회 지급한다.

## 9. ProductProductionSlotTable

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| slotId | string | Yes | chicken_skewer_slot_02 | 생산 슬롯 ID |
| productId | string | Yes | cotton_candy | 대상 상품 ID |
| slotIndex | int | Yes | 2 | 상품 내 슬롯 순서 |
| requiredStarCount | int | Yes | 1 | 활성화칸 표시 필요 별 개수 |
| activationMode | enum | Yes | FreeTap | 활성화칸 표시 후 활성화 방식 |
| isDefaultActivated | bool | Yes | false | 새 게임 또는 상품 해금 시 기본 활성화 여부 |
| pointKey | string | No | production_bead_icecream_02 | 씬 배치 포인트 키 |

### ProductionSlotActivationMode Enum

| Value | Description |
|---|---|
| FreeTap | 조건 충족 후 표시된 칸을 한 번 터치해 무료 활성화 |

## 10. StaffTable

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| staffId | string | Yes | generalist_staff | 고유 ID |
| displayName | string | Yes | 범용 직원 | 표시 이름 |
| role | enum | Yes | Generalist | 직원 역할 |
| addSource | enum | Yes | StageUpgrade | 추가 방식 |
| moveTimePerTileSec | float | Yes | 1.0 | 매대 1칸 기준 이동 시간 |
| carryCapacity | int | Yes | 1 | 한 번에 운반 가능한 상품 수 |
| maxCarryCapacity | int | Yes | 2 | 자원/장비 확장 후 허용할 최대 운반 수 |
| workSpeedMultiplier | float | Yes | 1.0 | 상품 생산 시간에 곱하는 기본 작업속도 배율 |
| minProductionTimeSec | float | Yes | 0.6 | 생산 시간 최소값 |
| maxLevel | int | Yes | 50 | 최대 레벨 |
| upgradeBaseCost | double | Yes | 100 | 첫 업그레이드 비용 |
| workTimeMultiplierPerLevel | float | Yes | 0.97 | 레벨당 작업 시간 배율 |
| costGrowth | float | Yes | 1.15 | 업그레이드 비용 증가율 |
| prefabKey | string | No | staff_service | 프리팹 참조 키 |
| iconKey | string | No | icon_service | UI 아이콘 키 |

### StaffRole Enum

| Value | Description |
|---|---|
| Generalist | 주문/생산/전달을 모두 수행하는 MVP 기본 직원 |
| Service | 주문/전달 보조 |
| Production | 생산 보조 |

## 11. StageUpgradeTable

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| upgradeId | string | Yes | cotton_candy_speed_001 | 업그레이드 ID |
| themeId | string | Yes | theme_amusement | 소속 테마 |
| stageId | string | Yes | amusement_001 | 소속 스테이지 |
| displayName | string | Yes | 솜사탕 생산속도 증가 | 표시 이름 |
| targetType | enum | Yes | Product | 대상 타입 |
| targetId | string | No | cotton_candy | 대상 ID |
| effectType | enum | Yes | ProductionSpeedPct | 효과 타입 |
| effectValue | float | Yes | 50 | 효과 수치 |
| cost | double | Yes | 50 | 비용 |
| maxPurchaseCount | int | Yes | 1 | 구매 가능 횟수 |
| prerequisiteUpgradeIds | string[] | No |  | 선행 업그레이드 |
| sortOrder | int | Yes | 1 | 팝업 표시 순서 |

### StageUpgradeTargetType Enum

| Value | Description |
|---|---|
| Product | 특정 상품 |
| Customer | 현재 스테이지 손님 수 |
| Staff | 현재 스테이지 직원 수 |
| Stage | 현재 스테이지 |

### StageUpgradeEffectType Enum

| Value | Description |
|---|---|
| ProductionSpeedPct | 생산속도 증가 |
| RewardMultiplier | 보상 배율 증가 |
| CustomerCountAdd | 손님 수 증가 |
| StaffCountAdd | 직원 수 증가 |

## 12. CustomerTypeTable

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| customerTypeId | string | Yes | normal | 고유 ID |
| displayName | string | Yes | 일반 손님 | 표시 이름 |
| orderQuantityMultiplier | float | Yes | 1.0 | 주문 수량 배율. 기본 주문 수량은 CustomerOrderConfig가 결정 |
| expensiveProductWeight | float | No | 1.0 | 고가 상품 선호도 |
| moveSpeed | float | Yes | 1.5 | 이동 속도 |
| spawnWeight | int | Yes | 100 | 등장 가중치 |
| prefabKey | string | No | customer_normal | 프리팹 참조 키 |

MVP에서는 `normal` 하나만 사용한다.

## 12.1 CustomerOrderConfig

손님 주문 생성 규칙이다.

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| configId | string | Yes | mvp_default_order | 주문 규칙 ID |
| firstOrderMode | enum | Yes | StartingStandOnly | 스테이지 첫 손님 주문 규칙 |
| eligibleProductRule | enum | Yes | UnlockedAndProductionSlotActivated | 일반 주문 가능 상품 규칙 |
| productSelectionRule | enum | Yes | UniformEligibleProducts | 주문 가능 상품 선택 확률 규칙 |
| distinctProductCountRule | enum | Yes | UniformRange | 한 주문에 포함할 상품 종류 수 확률 규칙 |
| maxDistinctProductTypes | int | Yes | 2 | 한 주문에 포함 가능한 최대 상품 종류 수 |
| quantityRule | enum | Yes | UniformIntRange | 상품별 주문 수량 확률 규칙 |
| minQuantityPerProduct | int | Yes | 1 | 상품별 최소 주문 수량 |
| maxQuantityPerProduct | int | Yes | 3 | 상품별 최대 주문 수량 |
| workAssignmentRule | enum | Yes | OrderTakerThenProductUnitTasks | 작업자 배정 규칙 |
| goldRewardTiming | enum | Yes | PerProductDelivered | Gold 지급 시점 |

### FirstOrderMode Enum

| Value | Description |
|---|---|
| StartingStandOnly | 첫 손님은 첫 상품만 주문한다. 첫 상품은 해금 전이어도 주문 가능 |

### EligibleProductRule Enum

| Value | Description |
|---|---|
| UnlockedAndProductionSlotActivated | 매대가 해금되고 생산 슬롯이 하나 이상 활성화된 상품만 주문 가능 |

### ProductSelectionRule Enum

| Value | Description |
|---|---|
| UniformEligibleProducts | 현재 주문 가능한 상품 풀에서 균등 확률로 선택 |

### DistinctProductCountRule Enum

| Value | Description |
|---|---|
| UniformRange | 1부터 `min(maxDistinctProductTypes, eligibleProductCount)`까지 균등 확률로 선택 |

### QuantityRule Enum

| Value | Description |
|---|---|
| UniformIntRange | `minQuantityPerProduct`부터 `maxQuantityPerProduct`까지 정수 균등 확률로 선택 |

### WorkAssignmentRule Enum

| Value | Description |
|---|---|
| OrderTakerThenProductUnitTasks | 작업자 1명이 전체 주문을 접수하고, 이후 상품 1개 단위 작업으로 분해해 빈 작업자가 처리 |

### GoldRewardTiming Enum

| Value | Description |
|---|---|
| PerProductDelivered | 주문 전체 완료가 아니라 상품 1개 전달 완료 시마다 Gold 지급 |

## 13. NumberFormatConfig

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| suffixes | string[] | Yes | k,m,b,t,aa,ab | 축약 단위 |
| decimals | int | Yes | 1 | 소수점 자리 |

## 14. GlobalUpgradeTable

금고 또는 전역 업그레이드는 MVP 이후 구현 대상이지만, Diamond 사용처의 핵심 후보이므로 테이블 자리를 미리 정의한다.

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| upgradeId | string | Yes | ad_boost_duration | 업그레이드 ID |
| displayName | string | Yes | 광고 부스트 시간 증가 | 표시 이름 |
| effectType | enum | Yes | AdBoostDurationSecAdd | 효과 타입 |
| baseValue | float | Yes | 300 | 레벨 0 또는 기본값 |
| valuePerLevel | float | Yes | 60 | 레벨당 증가값 |
| maxLevel | int | Yes | 20 | 최대 레벨 |
| baseCostDiamond | int | Yes | 5 | 첫 업그레이드 Diamond 비용 |
| costGrowth | float | Yes | 1.35 | 비용 증가율 |
| iconKey | string | No | icon_vault_ad_time | 아이콘 키 |

### GlobalUpgradeEffectType Enum

| Value | Description |
|---|---|
| AdBoostDurationSecAdd | 광고 부스트 지속시간 증가 |
| AdBoostMultiplierAdd | 광고 Gold 배율 증가 |
| OfflineEfficiencyPct | 오프라인 보상 효율 증가 |
| StartingGoldBonus | 새 스테이지 시작 Gold 보너스 |
| CustomerMoveSpeedPct | 손님 이동속도 증가 |

## 15. ChestTable

상자는 MVP 이후 Diamond 사용처 후보이며, 테이블 자리를 미리 정의한다.

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| chestId | string | Yes | chest_rare | 상자 ID |
| displayName | string | Yes | 레어 상자 | 표시 이름 |
| costDiamond | int | Yes | 80 | 구매 비용 |
| itemCount | int | Yes | 6 | 상자 1개당 지급 아이템 수 |
| dropRateSetId | string | Yes | drop_rare | 드랍 확률 세트 ID |
| iconKey | string | No | icon_chest_rare | 아이콘 키 |

## 16. ChestDropRateTable

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| dropRateSetId | string | Yes | drop_rare | 드랍 확률 세트 ID |
| rewardType | enum | Yes | EquipmentItem | 보상 타입 |
| grade | enum | Yes | Rare | 장비 등급 |
| weightPct | float | Yes | 20 | 해당 등급 등장 확률 |

초기 상자 후보:

| Chest | Diamond Cost | Item Count | Normal | Rare | Unique | Legendary Blueprint | Mythic Blueprint | Note |
|---|---:|---:|---:|---:|---:|---:|---:|---|
| Normal | 30 | 6 | 100 | 0 | 0 | 0 | 0 | 확정 일반 |
| Rare | 80 | 6 | 80 | 20 | 0 | 0 | 0 |  |
| Unique | 150 | 6 | 50 | 42 | 8 | 0 | 0 |  |
| Legendary | 300 | 6 | 40 | 40 | 18 | 2 | 0 | 2%는 전설 제작 문서 |
| Event | TBD | 6 | TBD | TBD | TBD | TBD | TBD | Mythic 제작 문서 가능 |

### ChestRewardType Enum

| Value | Description |
|---|---|
| EquipmentItem | 완제품 장비 |
| EquipmentBlueprint | 장비 제작 문서 |

### ChestPityConfig

상자 천장 시스템 설정이다. MVP에서는 구현하지 않지만, Legendary Chest에는 50회 Hard Pity를 고정 규칙으로 둔다.

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| pityId | string | Yes | pity_legendary_blueprint | 천장 설정 ID |
| chestId | string | Yes | chest_legendary | 대상 상자 |
| targetRewardType | enum | Yes | EquipmentBlueprint | 보장 대상 보상 타입 |
| targetGrade | enum | Yes | Legendary | 보장 대상 등급 |
| hardPityCount | int | Yes | 50 | 해당 횟수 도달 시 보장 |
| softPityStartCount | int | No | 0 | 첫 구현에서는 소프트 피티 없음 |
| rewardSelectionType | enum | Yes | RandomInGrade | 보장 보상 선택 방식 |
| resetOnTargetReward | bool | Yes | true | 목표 보상 획득 시 카운터 초기화 여부 |

### PityRewardSelectionType Enum

| Value | Description |
|---|---|
| RandomInGrade | 대상 등급의 제작 문서 중 랜덤 1개 지급 |

## 17. ShopItemTable

상점은 상자, 시간제 부스터, 캐릭터를 함께 판매할 수 있다. MVP에서는 실제 구매를 구현하지 않는다.

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| shopItemId | string | Yes | shop_income_2x_15m | 상점 아이템 ID |
| displayName | string | Yes | 15분 수익 2배 | 표시 이름 |
| shopItemType | enum | Yes | TimedBoost | 상점 아이템 타입 |
| linkedContentId | string | Yes | income_2x_15m | Chest ID, Boost ID, Character ID |
| costType | enum | Yes | Diamond | 비용 타입 |
| costAmount | double | Yes | 50 | 비용 수량 |
| sortOrder | int | Yes | 1 | 상점 표시 순서 |
| iconKey | string | No | icon_boost_2x | 아이콘 키 |

### ShopItemType Enum

| Value | Description |
|---|---|
| Chest | 상자 구매 |
| TimedBoost | 시간제 부스터 구매 |
| Character | 캐릭터 구매 |

### ShopCostType Enum

| Value | Description |
|---|---|
| Diamond | Diamond 사용 |
| Iap | 실제 결제 상품 |
| AdReward | 광고 보상 |

## 18. TimedBoostConfig

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| boostId | string | Yes | income_2x_15m | 부스터 ID |
| displayName | string | Yes | 15분 수익 2배 | 표시 이름 |
| effectType | enum | Yes | IncomeMultiplier | 효과 타입 |
| multiplier | float | Yes | 2.0 | 효과 배율 |
| durationSec | int | Yes | 900 | 지속 시간 |
| stackingGroup | string | Yes | income_multiplier | 중첩 그룹 |
| sameBoostStackRule | enum | Yes | ExtendDuration | 같은 부스터 재구매 규칙 |

### TimedBoostEffectType Enum

| Value | Description |
|---|---|
| IncomeMultiplier | 상품 판매 Gold 배율 증가 |

### SameTimedBoostStackRule Enum

| Value | Description |
|---|---|
| ExtendDuration | 같은 부스터가 이미 활성화되어 있으면 남은 시간을 연장 |

중첩 규칙:

- 서로 다른 `boostId`의 수익 부스터가 동시에 활성화되면 배율을 곱한다.
- 같은 `boostId`를 다시 구매하면 배율을 또 곱하지 않고 지속시간을 연장한다.

예시:

```text
10 Gold * income_2x_15m * income_5x_5m
= 10 * 2 * 5
= 100 Gold
```

## 19. CharacterTable

캐릭터는 MVP 이후 상점에서 Diamond로 구매하고, 보유 캐릭터 중 1명을 선택하는 수집/편의 성장 요소다. 캐릭터 효과는 장비보다 낮은 수치의 패시브로 제한한다.

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| characterId | string | Yes | char_rare_runner | 캐릭터 ID |
| displayName | string | Yes | 빠른 점원 | 표시 이름 |
| grade | enum | Yes | Rare | 캐릭터 등급 |
| costDiamond | int | Yes | 500 | 상점 구매 비용 |
| passiveOptionIds | string[] | No | char_move_rare_001 | 캐릭터 패시브 옵션 ID 목록 |
| iconKey | string | No | icon_char_runner | UI 아이콘 키 |
| modelKey | string | No | model_char_runner | 캐릭터 모델/프리팹 키 |
| unlockSource | enum | Yes | Shop | 획득처 |

### CharacterGrade Enum

| Value | Description |
|---|---|
| Normal | 기본 캐릭터 |
| Rare | 낮은 이동 편의 |
| Unique | 낮은 작업 편의 |
| Epic | 이동/작업 혼합 |
| Legendary | 낮은 유틸 패시브 포함 |
| Mythic | 이벤트/특수 캐릭터 후보 |

### CharacterUnlockSource Enum

| Value | Description |
|---|---|
| Default | 새 게임 시작 시 기본 지급 |
| Shop | Diamond 상점 구매 |
| Event | 이벤트 보상 또는 특수 상점 |

## 20. CharacterPassiveOptionTable

캐릭터 패시브는 장비 옵션보다 약한 값으로 관리한다. 수익 배율, 더블 확률, S상품 확률 같은 강한 효과는 기본 캐릭터 옵션으로 사용하지 않는다.

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| passiveOptionId | string | Yes | char_move_rare_001 | 패시브 옵션 ID |
| displayName | string | Yes | 주인공 이동속도 증가 | 표시 이름 |
| effectType | enum | Yes | MainCharacterMoveSpeedPct | 효과 타입 |
| value | float | Yes | 5 | 효과 값 |
| valueType | enum | Yes | Percent | 값 타입 |
| applyScope | enum | Yes | EquippedCharacterOnly | 적용 범위 |

### CharacterPassiveEffectType Enum

| Value | Description |
|---|---|
| MainCharacterMoveSpeedPct | 주인공 이동속도 증가 |
| MainCharacterWorkSpeedPct | 주인공 주문/생산/전달 작업속도 증가 |
| EquipmentDismantleStarDustBonusPct | 장비 분해 시 Star Dust 획득량 증가 |
| GlobalSkillEffectBonusPct | 금고/공용 스킬 효과 소폭 증가 |

### CharacterPassiveValueType Enum

| Value | Description |
|---|---|
| Percent | 퍼센트 증가 |

### CharacterPassiveApplyScope Enum

| Value | Description |
|---|---|
| EquippedCharacterOnly | 현재 선택한 캐릭터의 패시브만 적용 |

## 21. EquipmentItemTable

장비는 MVP 이후 구현 대상이지만, 메인 캐릭터 성장 컨셉의 핵심이므로 테이블 자리를 미리 정의한다.

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| equipmentId | string | Yes | cap_normal_001 | 장비 ID |
| displayName | string | Yes | 낡은 점원 모자 | 표시 이름 |
| equipmentFamilyId | string | Yes | cap_cashier | 동일 장비 계열 ID |
| slot | enum | Yes | Hat | 착용 부위 |
| grade | enum | Yes | Normal | 장비 등급 |
| baseIncomeMultiplier | float | Yes | 1.2 | 장비 기본 수익 배율 |
| optionIds | string[] | No | move_speed_001 | 보유 옵션 ID |
| maxEnhanceLevel | int | No | 30 | 최대 강화 레벨 |
| canDropAsCompleteItem | bool | Yes | true | 상자에서 완제품으로 등장 가능 여부 |
| canUseAsEnhanceMaterial | bool | Yes | true | 강화 재료로 사용 가능 여부 |

### EquipmentGrade Enum

| Value | Description |
|---|---|
| Normal | 기본 등급 |
| Rare | 희귀 등급 |
| Unique | 고유 옵션 가능 |
| Epic | 높은 옵션 수치 |
| Legendary | 고유 효과와 강한 보너스 |
| Mythic | 이벤트/특수 제작 중심의 최상위 등급 |

### EquipmentSlot Enum

| Value | Description |
|---|---|
| Hat | 모자 또는 머리 장식 |
| Top | 상의 또는 유니폼 |
| Bottom | 하의 |
| Tool | 먼지털이개, 바코드 스캐너, 카드 단말기 같은 도구 |
| Extra1 | 이후 확장 슬롯 1 |
| Extra2 | 이후 확장 슬롯 2 |
| Extra3 | 이후 확장 슬롯 3 |
| Extra4 | 이후 확장 슬롯 4 |

## 22. EquipmentBlueprintTable

전설 이상 장비는 완제품이 아니라 제작 문서를 통해 제작하는 구조를 기본으로 한다.

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| blueprintId | string | Yes | bp_robot_head_legendary | 제작 문서 ID |
| displayName | string | Yes | 로봇 머리 제작 문서 | 표시 이름 |
| targetEquipmentId | string | Yes | robot_head_legendary | 제작 결과 장비 ID |
| targetGrade | enum | Yes | Legendary | 제작 결과 등급 |
| sourceType | enum | Yes | Chest | 획득처 |
| iconKey | string | No | icon_bp_robot_head | 아이콘 키 |

### BlueprintSourceType Enum

| Value | Description |
|---|---|
| Chest | 일반 또는 등급 상자 |
| EventChest | 이벤트 상자 |
| EventReward | 이벤트 보상 |
| Achievement | 업적 보상 |

## 23. EquipmentCraftRecipeTable

장비 등급 상승과 제작 레시피를 정의한다.

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| recipeId | string | Yes | craft_robot_head_legendary | 레시피 ID |
| resultEquipmentId | string | Yes | robot_head_legendary | 결과 장비 ID |
| recipeType | enum | Yes | BlueprintCraft | 레시피 타입 |
| requiredBlueprintId | string | No | bp_robot_head_legendary | 필요한 제작 문서 |
| requiredEquipmentFamilyId | string | No | robot_head | 동일 장비 계열 조건 |
| requiredSlot | enum | No | Hat | 동일 부위 조건 |
| requiredMaterialMaxGrade | enum | No | Epic | 사용할 수 있는 재료 최대 등급 |
| requiredMaterialCount | int | Yes | 5 | 필요한 재료 수 |
| requiredDuplicateCount | int | No | 2 | 동일 장비 합성에 필요한 개수 |
| requiredEquipmentLevel | int | No | 30 | 합성 대상 필요 레벨 |

### EquipmentRecipeType Enum

| Value | Description |
|---|---|
| DuplicateGradeUp | 30레벨 동일 부위, 동일 장비 2개 합성 |
| BlueprintCraft | 제작 문서와 하위 등급 재료로 제작 |

기본 규칙:

- Normal -> Rare -> Unique -> Epic 구간은 `DuplicateGradeUp`을 사용한다.
- Legendary는 `BlueprintCraft`를 사용하며, 제작 문서 1개와 하위 등급 재료 아이템 5개를 요구한다.
- Mythic은 이후 이벤트 제작 문서와 이벤트 재료를 통해 확장한다.

## 24. EquipmentDismantleConfig

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| grade | enum | Yes | Rare | 분해 대상 등급 |
| baseStarDustReward | int | Yes | 5 | 기본 Star Dust 지급량 |
| levelBonusMultiplier | float | Yes | 0.1 | 장비 레벨에 따른 추가 보상 계수 |

`Star Dust`는 캐릭터 상태창에서 메인 캐릭터 레벨업에 사용하는 재화다.

## 25. EquipmentOptionTable

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| optionId | string | Yes | move_speed_001 | 옵션 ID |
| optionType | enum | Yes | MoveSpeedPct | 옵션 타입 |
| target | enum | Yes | MainCharacter | 적용 대상 |
| scope | enum | Yes | Global | 적용 범위 |
| stageId | string | No | amusement_001 | 스테이지 한정 옵션일 때 대상 스테이지 |
| productCategoryId | string | No | snack | 상품군 한정 옵션일 때 대상 상품군 |
| value | float | Yes | 5.0 | 옵션 수치 |
| valueFormat | enum | Yes | Percent | 옵션 수치 표기 방식 |
| description | string | Yes | 이동속도 +5% | 표시 설명 |

### EquipmentOptionType Enum

| Value | Description |
|---|---|
| IncomeMultiplier | 판매 수익 배율 |
| MoveSpeedPct | 이동속도 증가 |
| WorkSpeedPct | 주문/생산/전달 작업속도 증가 |
| DoubleChancePct | 더블 판매 발동 확률 |
| SGradeChancePct | S상품 등장 확률 증가 |
| SGradeRewardMultiplier | S상품 보상 배율 증가 |
| InstantProductionChancePct | 생산 시간을 건너뛰고 즉시 생산할 확률 |
| CustomerMoveSpeedPct | 손님 입장/퇴장 이동속도 증가 |
| CustomerOrderSpeedPct | 손님 주문 대기 시간 감소 |
| ProductCategoryIncomeMultiplier | 특정 상품군 수익 배율 |
| StageIncomeMultiplier | 특정 스테이지 수익 배율 |

### EquipmentOptionTarget Enum

| Value | Description |
|---|---|
| MainCharacter | 메인 캐릭터 |
| Staff | 해금/고용된 직원 전체, 메인 캐릭터 제외 |
| AllWorkers | 메인 캐릭터와 직원 전체 |
| Customer | 손님 |
| Product | 상품 |
| ProductCategory | 상품군 |
| Stage | 스테이지 |

### EquipmentOptionScope Enum

| Value | Description |
|---|---|
| Global | 전체 적용 |
| Stage | 특정 스테이지에만 적용 |
| Role | 특정 직원 역할에만 적용 |
| ProductCategory | 특정 상품군에만 적용 |

### EquipmentOptionValueFormat Enum

| Value | Description |
|---|---|
| Percent | +5%처럼 표시 |
| Multiplier | x2처럼 표시 |
| ChancePercent | 확률 +5%처럼 표시 |

## 26. EquipmentOptionPoolTable

등급과 부위에 따라 어떤 옵션이 등장할 수 있는지 정의한다.

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| poolId | string | Yes | pool_tool_epic | 옵션 풀 ID |
| slot | enum | Yes | Tool | 장비 부위 |
| grade | enum | Yes | Epic | 장비 등급 |
| optionType | enum | Yes | DoubleChancePct | 등장 가능한 옵션 타입 |
| target | enum | Yes | MainCharacter | 적용 대상 |
| minValue | float | Yes | 3 | 최소 수치 |
| maxValue | float | Yes | 6 | 최대 수치 |
| weight | int | Yes | 100 | 등장 가중치 |

기본 규칙:

- Normal과 Rare는 서브 옵션이 없으므로 옵션 풀을 사용하지 않는다.
- Unique는 이동속도와 작업속도 중심 옵션 1개를 사용한다.
- Epic은 수익, 더블, S상품 확률 같은 판매 연결 옵션 1개를 사용할 수 있다.
- Legendary는 옵션 2개를 사용한다.
- Mythic은 옵션 3개를 사용한다.

초기 옵션 풀 방향:

| Slot | Unique | Epic | Legendary | Mythic |
|---|---|---|---|---|
| Tool | MainCharacter Move/Work | MainCharacter Income/Double/SGrade | MainCharacter mixed sale options | All or special sale options |
| Hat | Staff Move/Work | Staff Income/Double/SGrade/InstantProduction | Staff mixed sale options | Staff or special sale options |
| Top | Main/Staff Income | AllWorkers Work/Income, CustomerOrderSpeed | Main + Staff mixed options | AllWorkers Income + special options |
| Bottom | Main/Staff Move | AllWorkers Move/Work, CustomerMoveSpeed | Move + sale utility | AllWorkers Move + special options |

## 27. EconomyConfig

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| startingGold | double | Yes | 0 | 새 게임 시작 Gold |
| startingDiamond | int | Yes | 0 | 새 게임 시작 Diamond |
| startingStarDust | int | Yes | 0 | 새 게임 시작 Star Dust |
| nextCustomerSpawnDelaySec | float | Yes | 0.2 | 기본 maxCustomers=1에서 손님 퇴장 후 다음 손님 등장 전 지연 |
| customerPickProductDurationSec | float | Yes | 0.6 | 손님이 주문 위치에 도착한 뒤 주문을 확정하기까지의 시간 |
| leaveDelaySec | float | Yes | 0.2 | 상품 수령 후 퇴장 전 지연 |
| goldRoundingUnit | int | Yes | 1 | Gold 반올림 단위 |
| autosaveIntervalSec | float | Yes | 10.0 | 자동 저장 주기 |

## 28. OfflineRewardConfig

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| minOfflineSec | int | Yes | 60 | 보상 표시 최소 오프라인 시간 |
| maxOfflineSec | int | Yes | 10800 | 최대 인정 시간 |
| efficiency | float | Yes | 0.25 | 오프라인 효율 |

## 29. AdRewardConfig

MVP에서는 실제 광고 SDK를 붙이지 않는다. 다만 이후 확장을 위해 설정 구조만 둘 수 있다.

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| rewardId | string | Yes | gold_boost_5m | 보상 ID |
| rewardType | enum | Yes | GoldBoost | 보상 타입 |
| durationSec | int | No | 300 | 지속 시간 |
| multiplier | float | No | 2.0 | 배율 |
| amount | double | No | 0 | 지급량 |
| dailyLimit | int | No | 0 | 일일 수령 제한, 0이면 제한 없음 |
| affectedByGlobalUpgrade | bool | Yes | true | 금고/전역 업그레이드 영향 여부 |
| enabledInMvp | bool | Yes | false | MVP 활성 여부 |

### AdRewardType Enum

| Value | Description |
|---|---|
| GoldBoost | 일정 시간 Gold 획득 배율 증가 |
| Diamond | Diamond 즉시 지급 |

## 30. FirebaseRemoteConfigKeys

Firebase Remote Config는 앱 버전 관리와 운영 설정에 사용한다. MVP 유저 진행 데이터의 원본 저장소로 사용하지 않는다.

| Key | Type | Required | Example | Description |
|---|---|---|---|---|
| min_supported_version | string | Yes | 0.1.0 | 이 버전 미만이면 강제 업데이트 |
| latest_version | string | Yes | 0.1.2 | 권장 업데이트 표시 기준 |
| force_update_required | bool | Yes | false | 강제 업데이트 여부 |
| maintenance_mode | bool | Yes | false | 점검 모드 여부 |
| maintenance_message | string | No | 점검 중입니다 | 점검 안내 문구 |
| master_data_version | int | Yes | 1 | 로컬 마스터 데이터 버전 |
| balance_version | int | Yes | 1 | 밸런스 설정 버전 |
| event_config_version | int | Yes | 0 | 이벤트 설정 버전, 이벤트 미운영 시 0 |
| android_store_url | string | No | TBD | Android 업데이트 URL |
| ios_store_url | string | No | TBD | iOS 업데이트 URL |

운영 규칙:

- `min_supported_version`보다 앱 버전이 낮으면 강제 업데이트 팝업을 표시한다.
- `latest_version`보다 앱 버전이 낮고 강제 업데이트 대상이 아니면 권장 업데이트를 표시할 수 있다.
- `maintenance_mode`가 true면 게임 진입 전 점검 팝업을 표시한다.
- `master_data_version`과 `balance_version`은 로컬 데이터 버전 표시와 디버그 확인용으로 사용한다.
- 클라우드 저장, 랭킹, 보상 검증은 이 테이블이 아니라 Firebase Auth, Firestore, Cloud Functions 또는 별도 서버 설계에서 다룬다.

## 31. SaveData

저장 데이터는 마스터 데이터와 분리한다.

진행 중 주문, 진행 중 생산, 손님 위치 같은 런타임 상태는 저장하지 않는다. 재접속 시에는 마스터 데이터와 진행 상태를 기준으로 매장 루프를 새로 시작한다.

```json
{
  "version": 1,
  "gold": 0,
  "diamond": 0,
  "starDust": 0,
  "currentThemeId": "theme_amusement",
  "currentStageId": "amusement_001",
  "currentCustomerCount": 1,
  "currentStaffCount": 0,
  "nextStageReady": false,
  "mainCharacter": {
    "selectedCharacterId": "char_normal_default",
    "ownedCharacterIds": [
      "char_normal_default"
    ],
    "equippedEquipmentIds": []
  },
  "equipmentInventory": [],
  "ownedBlueprints": [],
  "chestPityCounters": [],
  "activeTimedBoosts": [],
  "products": [
    {
      "productId": "cotton_candy",
      "unlocked": true,
      "standUnlocked": true,
      "productionTableActivated": true,
      "level": 1,
      "activatedProductionSlotIds": [
        "cotton_candy_slot_01"
      ]
    }
  ],
  "staff": [
    {
      "staffId": "generalist_staff",
      "count": 0,
      "level": 1
    }
  ],
  "stageUpgrades": [
    {
      "upgradeId": "cotton_candy_speed_001",
      "purchaseCount": 0
    }
  ],
  "seenNoticeIds": [],
  "lastSavedAtUtc": "2026-06-13T00:00:00Z",
  "totalPlayTimeSec": 0
}
```

### SaveData Fields

| Field | Type | Required | Description |
|---|---|---|---|
| version | int | Yes | 저장 데이터 버전 |
| gold | double | Yes | 현재 Gold |
| diamond | int | Yes | 현재 Diamond |
| starDust | int | Yes | 현재 Star Dust |
| currentThemeId | string | Yes | 현재 테마 |
| currentStageId | string | Yes | 현재 스테이지 |
| currentCustomerCount | int | Yes | 현재 스테이지 손님 수 |
| currentStaffCount | int | Yes | 현재 스테이지 직원 수 |
| nextStageReady | bool | Yes | 다음 스테이지 이동 조건 충족 여부 |
| mainCharacter | MainCharacterSaveData | Yes | 메인 캐릭터 진행 상태 |
| equipmentInventory | EquipmentInventorySaveData[] | Yes | 장비 인벤토리, MVP 미구현 시 빈 배열 |
| ownedBlueprints | EquipmentBlueprintSaveData[] | Yes | 보유 제작 문서, MVP 미구현 시 빈 배열 |
| chestPityCounters | ChestPitySaveData[] | Yes | 상자 천장 카운터, MVP 미구현 시 빈 배열 |
| activeTimedBoosts | ActiveTimedBoostSaveData[] | Yes | 활성 시간제 부스터, MVP 미구현 시 빈 배열 |
| products | ProductSaveData[] | Yes | 상품 진행 상태 |
| staff | StaffSaveData[] | Yes | 직원 진행 상태 |
| stageUpgrades | StageUpgradeSaveData[] | Yes | 스테이지 업그레이드 구매 상태 |
| seenNoticeIds | string[] | Yes | 확인 완료한 전역/기능성 알림 ID |
| lastSavedAtUtc | string | Yes | 마지막 저장 UTC 시각 |
| totalPlayTimeSec | long | Yes | 누적 플레이 시간 |

Gold 기반 스테이지 내부 알림은 저장하지 않는다. 현재 Gold, 비용, 해금 상태를 기준으로 매 프레임 또는 UI 갱신 시 계산한다. `seenNoticeIds`는 Diamond, 캐릭터, 장비, 금고, 상자, 신규 기능처럼 유저가 한 번 확인하면 숨겨야 하는 알림에만 사용한다.

## 32. MainCharacterSaveData

| Field | Type | Required | Description |
|---|---|---|---|
| selectedCharacterId | string | Yes | 현재 선택한 캐릭터 ID |
| ownedCharacterIds | string[] | Yes | 보유 캐릭터 ID 목록 |
| equippedEquipmentIds | string[] | Yes | 착용 중인 장비 ID 목록 |

MVP에서 캐릭터 구매와 장비를 구현하지 않는 경우 기본 캐릭터 1개와 빈 장비 배열로 저장한다.

## 33. EquipmentInventorySaveData

장비는 중복 습득이 가능하므로 인스턴스 단위로 저장한다.

| Field | Type | Required | Description |
|---|---|---|---|
| equipmentInstanceId | string | Yes | 보유 장비 인스턴스 ID |
| equipmentId | string | Yes | 마스터 장비 ID |
| level | int | Yes | 현재 장비 레벨, 최대 30 |
| locked | bool | Yes | 분해/재료 사용 방지 잠금 여부 |

## 34. EquipmentBlueprintSaveData

| Field | Type | Required | Description |
|---|---|---|---|
| blueprintId | string | Yes | 제작 문서 ID |
| count | int | Yes | 보유 수량 |

## 35. ChestPitySaveData

| Field | Type | Required | Description |
|---|---|---|---|
| pityId | string | Yes | 천장 설정 ID |
| currentCount | int | Yes | 목표 보상 미획득 누적 횟수. Legendary Chest는 0~49까지 표시되고 50회째 보장 후 0으로 초기화 |

## 36. ActiveTimedBoostSaveData

| Field | Type | Required | Description |
|---|---|---|---|
| boostId | string | Yes | 활성화된 부스터 ID |
| endsAtUtc | string | Yes | 부스터 종료 UTC 시각 |

같은 `boostId`를 다시 구매하면 새 항목을 추가하지 않고 `endsAtUtc`를 연장한다.

## 37. ProductSaveData

| Field | Type | Required | Description |
|---|---|---|---|
| productId | string | Yes | 상품 ID |
| unlocked | bool | Yes | 해금 여부 |
| standUnlocked | bool | Yes | 매대 해금 여부 |
| productionTableActivated | bool | Yes | 생산 테이블 활성화 여부 |
| level | int | Yes | 현재 레벨 |
| activatedProductionSlotIds | string[] | Yes | 활성화된 생산 슬롯 ID 목록 |

## 38. StaffSaveData

| Field | Type | Required | Description |
|---|---|---|---|
| staffId | string | Yes | 직원 ID |
| count | int | Yes | 현재 스테이지에 추가된 직원 수 |
| level | int | Yes | 현재 레벨 |

## 39. StageUpgradeSaveData

| Field | Type | Required | Description |
|---|---|---|---|
| upgradeId | string | Yes | 업그레이드 ID |
| purchaseCount | int | Yes | 구매 횟수 |

## 40. Initial Data

### ThemeTable

| themeId | displayName | order | stageIds | targetStageCount |
|---|---|---:|---|---:|
| theme_amusement | 놀이동산 | 1 | amusement_001,amusement_002,amusement_003 | 8 |

### StageTable

| stageId | themeId | displayName | stageNumber | gridWidth | gridHeight | scrollMode | productIds | startingGold | startingCustomerCount | startingStaffCount | nextStageCostRule | nextStageCostMultiplier | nextStageCostOverride | clearConditionType | requiredMaxLevelProductIds | maxProductStandCount | clearRewardGroupId |
|---|---|---|---:|---:|---:|---|---|---:|---:|---:|---|---:|---:|---|---|---:|---|
| amusement_001 | theme_amusement | 놀이동산 1 | 1 | 11 | 20 | FixedNoScroll | cotton_candy | 5 | 1 | 0 | MaxFinalUpgradeCostMultiplier | 1.5 | 0 | AllProductsMaxLevel | cotton_candy | 1 | amusement_001_clear_reward |
| amusement_002 | theme_amusement | 놀이동산 2 | 2 | 11 | 20 | FixedNoScroll | bead_icecream,chicken_skewer | 5 | 1 | 0 | MaxFinalUpgradeCostMultiplier | 1.5 | 0 | AllProductsMaxLevel | bead_icecream,chicken_skewer | 2 |  |
| amusement_003 | theme_amusement | 놀이동산 3 | 3 | 11 | 20 | FixedNoScroll | popcorn,hotdog,churros | 5 | 1 | 0 | MaxFinalUpgradeCostMultiplier | 1.5 | 0 | AllProductsMaxLevel | popcorn,hotdog,churros | 3 |  |

### StageClearRewardTable

| rewardId | rewardGroupId | stageId | rewardType | rewardTargetId | amount | enabledInMvp |
|---|---|---|---|---|---:|---|
| amusement_001_clear_common_chest | amusement_001_clear_reward | amusement_001 | Chest | chest_common | 1 | false |

### StageLayoutPointTable

| pointId | stageId | pointType | productId | gridX | gridY | priority | capacity |
|---|---|---|---|---:|---:|---:|---:|
| amusement_001_entrance | amusement_001 | Entrance |  | 5 | 0 | 1 | 1 |
| amusement_001_exit | amusement_001 | Exit |  | 6 | 0 | 1 | 1 |
| amusement_001_path_01 | amusement_001 | CustomerPath |  | 5 | 1 | 1 | 1 |
| amusement_001_path_02 | amusement_001 | CustomerPath |  | 5 | 2 | 2 | 1 |
| amusement_001_path_03 | amusement_001 | CustomerPath |  | 5 | 3 | 3 | 1 |
| amusement_001_order_01 | amusement_001 | CustomerOrder |  | 5 | 4 | 1 | 1 |
| amusement_001_order_side_l | amusement_001 | CustomerOrderSide |  | 4 | 4 | 2 | 1 |
| amusement_001_order_side_r | amusement_001 | CustomerOrderSide |  | 6 | 4 | 3 | 1 |
| amusement_001_order_back_01 | amusement_001 | CustomerOrderBack |  | 5 | 3 | 4 | 1 |
| amusement_001_order_back_l | amusement_001 | CustomerOrderBack |  | 4 | 3 | 5 | 1 |
| amusement_001_order_back_r | amusement_001 | CustomerOrderBack |  | 6 | 3 | 6 | 1 |
| amusement_001_counter_l | amusement_001 | Counter |  | 4 | 5 | 1 | 1 |
| amusement_001_counter_c | amusement_001 | Counter |  | 5 | 5 | 2 | 1 |
| amusement_001_counter_r | amusement_001 | Counter |  | 6 | 5 | 3 | 1 |
| amusement_001_cotton_stand | amusement_001 | ProductStand | cotton_candy | 4 | 6 | 1 | 1 |
| amusement_001_cotton_slot_01 | amusement_001 | ProductionSlot | cotton_candy | 5 | 6 | 1 | 1 |
| amusement_001_main_start | amusement_001 | MainCharacterStart |  | 6 | 7 | 1 | 1 |
| amusement_001_staff_wait_01 | amusement_001 | StaffWaiting |  | 5 | 7 | 1 | 1 |
| amusement_001_decoration_base | amusement_001 | DecorationArea |  | 5 | 13 | 1 | 1 |
| amusement_002_entrance | amusement_002 | Entrance |  | 5 | 0 | 1 | 1 |
| amusement_002_exit | amusement_002 | Exit |  | 6 | 0 | 1 | 1 |
| amusement_002_path_01 | amusement_002 | CustomerPath |  | 5 | 1 | 1 | 1 |
| amusement_002_path_02 | amusement_002 | CustomerPath |  | 5 | 2 | 2 | 1 |
| amusement_002_path_03 | amusement_002 | CustomerPath |  | 5 | 3 | 3 | 1 |
| amusement_002_order_01 | amusement_002 | CustomerOrder |  | 5 | 4 | 1 | 1 |
| amusement_002_order_02 | amusement_002 | CustomerOrder |  | 4 | 4 | 2 | 1 |
| amusement_002_order_03 | amusement_002 | CustomerOrder |  | 6 | 4 | 3 | 1 |
| amusement_002_order_04 | amusement_002 | CustomerOrder |  | 3 | 4 | 4 | 1 |
| amusement_002_order_05 | amusement_002 | CustomerOrder |  | 7 | 4 | 5 | 1 |
| amusement_002_order_back_01 | amusement_002 | CustomerOrderBack |  | 5 | 3 | 6 | 1 |
| amusement_002_order_back_02 | amusement_002 | CustomerOrderBack |  | 4 | 3 | 7 | 1 |
| amusement_002_order_back_03 | amusement_002 | CustomerOrderBack |  | 6 | 3 | 8 | 1 |
| amusement_002_order_back_04 | amusement_002 | CustomerOrderBack |  | 3 | 3 | 9 | 1 |
| amusement_002_order_back_05 | amusement_002 | CustomerOrderBack |  | 7 | 3 | 10 | 1 |
| amusement_002_counter_01 | amusement_002 | Counter |  | 3 | 5 | 1 | 1 |
| amusement_002_counter_02 | amusement_002 | Counter |  | 4 | 5 | 2 | 1 |
| amusement_002_counter_03 | amusement_002 | Counter |  | 5 | 5 | 3 | 1 |
| amusement_002_counter_04 | amusement_002 | Counter |  | 6 | 5 | 4 | 1 |
| amusement_002_counter_05 | amusement_002 | Counter |  | 7 | 5 | 5 | 1 |
| amusement_002_bead_stand | amusement_002 | ProductStand | bead_icecream | 3 | 6 | 1 | 1 |
| amusement_002_bead_slot_01 | amusement_002 | ProductionSlot | bead_icecream | 4 | 6 | 1 | 1 |
| amusement_002_chicken_stand | amusement_002 | ProductStand | chicken_skewer | 6 | 6 | 1 | 1 |
| amusement_002_chicken_slot_01 | amusement_002 | ProductionSlot | chicken_skewer | 7 | 6 | 1 | 1 |
| amusement_002_chicken_slot_02 | amusement_002 | ProductionSlot | chicken_skewer | 7 | 7 | 2 | 1 |
| amusement_002_main_start | amusement_002 | MainCharacterStart |  | 5 | 8 | 1 | 1 |
| amusement_002_staff_wait_01 | amusement_002 | StaffWaiting |  | 4 | 8 | 1 | 1 |
| amusement_002_staff_wait_02 | amusement_002 | StaffWaiting |  | 6 | 8 | 2 | 1 |
| amusement_002_decoration_base | amusement_002 | DecorationArea |  | 5 | 13 | 1 | 1 |
| amusement_003_entrance | amusement_003 | Entrance |  | 5 | 0 | 1 | 1 |
| amusement_003_exit | amusement_003 | Exit |  | 6 | 0 | 1 | 1 |
| amusement_003_path_01 | amusement_003 | CustomerPath |  | 5 | 1 | 1 | 1 |
| amusement_003_path_02 | amusement_003 | CustomerPath |  | 5 | 2 | 2 | 1 |
| amusement_003_path_03 | amusement_003 | CustomerPath |  | 5 | 3 | 3 | 1 |
| amusement_003_order_01 | amusement_003 | CustomerOrder |  | 5 | 4 | 1 | 1 |
| amusement_003_order_02 | amusement_003 | CustomerOrder |  | 4 | 4 | 2 | 1 |
| amusement_003_order_03 | amusement_003 | CustomerOrder |  | 6 | 4 | 3 | 1 |
| amusement_003_order_04 | amusement_003 | CustomerOrder |  | 3 | 4 | 4 | 1 |
| amusement_003_order_05 | amusement_003 | CustomerOrder |  | 7 | 4 | 5 | 1 |
| amusement_003_order_06 | amusement_003 | CustomerOrder |  | 2 | 4 | 6 | 1 |
| amusement_003_order_07 | amusement_003 | CustomerOrder |  | 8 | 4 | 7 | 1 |
| amusement_003_order_back_01 | amusement_003 | CustomerOrderBack |  | 5 | 3 | 8 | 1 |
| amusement_003_order_back_02 | amusement_003 | CustomerOrderBack |  | 4 | 3 | 9 | 1 |
| amusement_003_order_back_03 | amusement_003 | CustomerOrderBack |  | 6 | 3 | 10 | 1 |
| amusement_003_order_back_04 | amusement_003 | CustomerOrderBack |  | 3 | 3 | 11 | 1 |
| amusement_003_order_back_05 | amusement_003 | CustomerOrderBack |  | 7 | 3 | 12 | 1 |
| amusement_003_order_back_06 | amusement_003 | CustomerOrderBack |  | 2 | 3 | 13 | 1 |
| amusement_003_order_back_07 | amusement_003 | CustomerOrderBack |  | 8 | 3 | 14 | 1 |
| amusement_003_counter_01 | amusement_003 | Counter |  | 2 | 5 | 1 | 1 |
| amusement_003_counter_02 | amusement_003 | Counter |  | 3 | 5 | 2 | 1 |
| amusement_003_counter_03 | amusement_003 | Counter |  | 4 | 5 | 3 | 1 |
| amusement_003_counter_04 | amusement_003 | Counter |  | 5 | 5 | 4 | 1 |
| amusement_003_counter_05 | amusement_003 | Counter |  | 6 | 5 | 5 | 1 |
| amusement_003_counter_06 | amusement_003 | Counter |  | 7 | 5 | 6 | 1 |
| amusement_003_counter_07 | amusement_003 | Counter |  | 8 | 5 | 7 | 1 |
| amusement_003_popcorn_stand | amusement_003 | ProductStand | popcorn | 2 | 6 | 1 | 1 |
| amusement_003_popcorn_slot_01 | amusement_003 | ProductionSlot | popcorn | 3 | 6 | 1 | 1 |
| amusement_003_hotdog_stand | amusement_003 | ProductStand | hotdog | 5 | 6 | 1 | 1 |
| amusement_003_hotdog_slot_01 | amusement_003 | ProductionSlot | hotdog | 6 | 6 | 1 | 1 |
| amusement_003_hotdog_slot_02 | amusement_003 | ProductionSlot | hotdog | 6 | 7 | 2 | 1 |
| amusement_003_churros_stand | amusement_003 | ProductStand | churros | 8 | 6 | 1 | 1 |
| amusement_003_churros_slot_01 | amusement_003 | ProductionSlot | churros | 9 | 6 | 1 | 1 |
| amusement_003_churros_slot_02 | amusement_003 | ProductionSlot | churros | 9 | 7 | 2 | 1 |
| amusement_003_main_start | amusement_003 | MainCharacterStart |  | 5 | 8 | 1 | 1 |
| amusement_003_staff_wait_01 | amusement_003 | StaffWaiting |  | 4 | 8 | 1 | 1 |
| amusement_003_staff_wait_02 | amusement_003 | StaffWaiting |  | 6 | 8 | 2 | 1 |
| amusement_003_staff_wait_03 | amusement_003 | StaffWaiting |  | 5 | 9 | 3 | 1 |
| amusement_003_decoration_base | amusement_003 | DecorationArea |  | 5 | 13 | 1 | 1 |

### MainCharacterConfig

| mainCharacterId | moveTimePerTileSec | carryCapacity | maxCarryCapacity | orderTakeTimeSec | workSpeedMultiplier | deliveryHandoffTimeSec | baseIncomeBonusPct | sGradeChancePct |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| default_main_character | 1.0 | 1 | 2 | 0.4 | 1.0 | 0.5 | 0 | 0 |

### ProductTable

| productId | displayName | themeId | stageId | standOrder | isStartingStand | unlockCost | basePrice | baseProductionAmount | baseProductionTimeSec | maxLevel | maxStarCount | baseProductionSlotCount | maxProductionSlotCount | upgradeBaseCost | levelCurveId | priceGrowth | costGrowth |
|---|---|---|---|---:|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---|---:|---:|
| cotton_candy | 솜사탕 | theme_amusement | amusement_001 | 1 | true | 5 | 2 | 1 | 5.0 | 25 | 2 | 1 | 1 | 3 | curve_mvp_starter_25 | 1.00 | 1.00 |
| bead_icecream | 구슬아이스크림 | theme_amusement | amusement_002 | 1 | true | 5 | 2 | 1 | 5.0 | 75 | 4 | 1 | 1 | 3 | curve_mvp_starter_75 | 1.00 | 1.00 |
| chicken_skewer | 닭꼬치 | theme_amusement | amusement_002 | 2 | false | 250 | 320 | 1 | 9.0 | 75 | 4 | 1 | 2 | 150 | curve_mvp_second_75 | 1.00 | 1.00 |
| popcorn | 팝콘 | theme_amusement | amusement_003 | 1 | true | 5 | 2 | 1 | 5.0 | 75 | 4 | 1 | 1 | 3 | curve_mvp_starter_75 | 1.00 | 1.00 |
| hotdog | 핫도그 | theme_amusement | amusement_003 | 2 | false | 250 | 320 | 1 | 9.0 | 75 | 4 | 1 | 2 | 150 | curve_mvp_second_75 | 1.00 | 1.00 |
| churros | 츄러스 | theme_amusement | amusement_003 | 3 | false | 12500 | 24 | 1 | 13.0 | 75 | 4 | 1 | 2 | 36 | curve_mvp_third_75 | 1.00 | 1.00 |

### ProductLevelCurveTable

| curveId | levelStart | levelEnd | levelBaseSaleGoldAtStart | saleGrowthType | saleGrowthValue | upgradeCostGoldAtStart | upgradeCostGrowthType | upgradeCostGrowthValue | note |
|---|---:|---:|---:|---|---:|---:|---|---:|---|
| curve_mvp_starter_25 | 1 | 9 | 2 | Add | 1 | 3 | Add | 1 | no star |
| curve_mvp_starter_25 | 10 | 10 | 10 | Fixed | 0 | 16 | Fixed | 0 | star 1 |
| curve_mvp_starter_25 | 11 | 24 | 24 | Multiply | 1.08 | 22 | Multiply | 1.20 | after star 1 |
| curve_mvp_starter_25 | 25 | 25 | 76 | Fixed | 0 | 0 | Fixed | 0 | star 2 max |
| curve_mvp_starter_75 | 1 | 9 | 2 | Add | 1 | 3 | Add | 1 | no star |
| curve_mvp_starter_75 | 10 | 10 | 10 | Fixed | 0 | 16 | Fixed | 0 | star 1 |
| curve_mvp_starter_75 | 11 | 24 | 24 | Multiply | 1.08 | 22 | Multiply | 1.20 | after star 1 |
| curve_mvp_starter_75 | 25 | 25 | 76 | Fixed | 0 | 240 | Fixed | 0 | star 2 |
| curve_mvp_starter_75 | 26 | 49 | 82 | Multiply | 1.08 | 288 | Multiply | 1.20 | after star 2, Lv35~41 calibration |
| curve_mvp_starter_75 | 50 | 50 | 520 | Fixed | 0 | 22900 | Fixed | 0 | star 3, temporary extrapolation |
| curve_mvp_starter_75 | 51 | 74 | 562 | Multiply | 1.08 | 27500 | Multiply | 1.20 | after star 3, temporary extrapolation |
| curve_mvp_starter_75 | 75 | 75 | 3570 | Fixed | 0 | 0 | Fixed | 0 | star 4 max |
| curve_mvp_second_75 | 1 | 74 | 320 | Multiply | 1.08 | 150 | Multiply | 1.20 | observed through level 25, extrapolated after |
| curve_mvp_second_75 | 75 | 75 | 95172 | Fixed | 0 | 0 | Fixed | 0 | star 4 max, temporary extrapolation |
| curve_mvp_third_75 | 1 | 9 | 24 | Add | 12 | 36 | Add | 12 | no star |
| curve_mvp_third_75 | 10 | 10 | 120 | Fixed | 0 | 192 | Fixed | 0 | star 1 |
| curve_mvp_third_75 | 11 | 24 | 288 | Multiply | 1.08 | 264 | Multiply | 1.20 | after star 1 |
| curve_mvp_third_75 | 25 | 25 | 912 | Fixed | 0 | 2880 | Fixed | 0 | star 2 |
| curve_mvp_third_75 | 26 | 49 | 984 | Multiply | 1.08 | 3456 | Multiply | 1.20 | after star 2 |
| curve_mvp_third_75 | 50 | 50 | 6240 | Fixed | 0 | 274800 | Fixed | 0 | star 3, temporary extrapolation |
| curve_mvp_third_75 | 51 | 74 | 6744 | Multiply | 1.08 | 330000 | Multiply | 1.20 | after star 3, temporary extrapolation |
| curve_mvp_third_75 | 75 | 75 | 42840 | Fixed | 0 | 0 | Fixed | 0 | star 4 max |

### ProductStarMilestoneTable

| milestoneId | productId | starIndex | requiredLevel | rewardMultiplier | diamondReward | showProductionSlotIds |
|---|---|---:|---:|---:|---:|---|
| cotton_candy_star_01 | cotton_candy | 1 | 10 | 2.0 | 1 |  |
| cotton_candy_star_02 | cotton_candy | 2 | 25 | 2.0 | 1 |  |
| bead_icecream_star_01 | bead_icecream | 1 | 10 | 2.0 | 1 |  |
| bead_icecream_star_02 | bead_icecream | 2 | 25 | 2.0 | 1 |  |
| bead_icecream_star_03 | bead_icecream | 3 | 50 | 2.0 | 1 |  |
| bead_icecream_star_04 | bead_icecream | 4 | 75 | 2.0 | 1 |  |
| chicken_skewer_star_01 | chicken_skewer | 1 | 10 | 2.0 | 1 | chicken_skewer_slot_02 |
| chicken_skewer_star_02 | chicken_skewer | 2 | 25 | 2.0 | 1 |  |
| chicken_skewer_star_03 | chicken_skewer | 3 | 50 | 2.0 | 1 |  |
| chicken_skewer_star_04 | chicken_skewer | 4 | 75 | 2.0 | 1 |  |
| popcorn_star_01 | popcorn | 1 | 10 | 2.0 | 1 |  |
| popcorn_star_02 | popcorn | 2 | 25 | 2.0 | 1 |  |
| popcorn_star_03 | popcorn | 3 | 50 | 2.0 | 1 |  |
| popcorn_star_04 | popcorn | 4 | 75 | 2.0 | 1 |  |
| hotdog_star_01 | hotdog | 1 | 10 | 2.0 | 1 | hotdog_slot_02 |
| hotdog_star_02 | hotdog | 2 | 25 | 2.0 | 1 |  |
| hotdog_star_03 | hotdog | 3 | 50 | 2.0 | 1 |  |
| hotdog_star_04 | hotdog | 4 | 75 | 2.0 | 1 |  |
| churros_star_01 | churros | 1 | 10 | 2.0 | 1 | churros_slot_02 |
| churros_star_02 | churros | 2 | 25 | 2.0 | 1 |  |
| churros_star_03 | churros | 3 | 50 | 2.0 | 1 |  |
| churros_star_04 | churros | 4 | 75 | 2.0 | 1 |  |

### ProductProductionSlotTable

| slotId | productId | slotIndex | requiredStarCount | activationMode | isDefaultActivated | pointKey |
|---|---|---:|---:|---|---|---|
| cotton_candy_slot_01 | cotton_candy | 1 | 0 | FreeTap | false | production_cotton_candy_01 |
| bead_icecream_slot_01 | bead_icecream | 1 | 0 | FreeTap | false | production_bead_icecream_01 |
| chicken_skewer_slot_01 | chicken_skewer | 1 | 0 | FreeTap | false | production_chicken_skewer_01 |
| chicken_skewer_slot_02 | chicken_skewer | 2 | 1 | FreeTap | false | production_chicken_skewer_02 |
| popcorn_slot_01 | popcorn | 1 | 0 | FreeTap | false | production_popcorn_01 |
| hotdog_slot_01 | hotdog | 1 | 0 | FreeTap | false | production_hotdog_01 |
| hotdog_slot_02 | hotdog | 2 | 1 | FreeTap | false | production_hotdog_02 |
| churros_slot_01 | churros | 1 | 0 | FreeTap | false | production_churros_01 |
| churros_slot_02 | churros | 2 | 1 | FreeTap | false | production_churros_02 |

### GlobalUpgradeTable

| upgradeId | displayName | effectType | baseValue | valuePerLevel | maxLevel | baseCostDiamond | costGrowth | iconKey |
|---|---|---|---:|---:|---:|---:|---:|---|
| ad_boost_duration | 광고 부스트 시간 증가 | AdBoostDurationSecAdd | 300 | 60 | 20 | 5 | 1.35 | icon_vault_ad_time |
| ad_boost_multiplier | 광고 Gold 배율 증가 | AdBoostMultiplierAdd | 2.0 | 0.5 | 10 | 10 | 1.45 | icon_vault_ad_multiplier |
| offline_efficiency | 오프라인 보상 효율 증가 | OfflineEfficiencyPct | 25 | 5 | 10 | 8 | 1.4 | icon_vault_offline |

### AdRewardConfig

| rewardId | rewardType | durationSec | multiplier | amount | dailyLimit | affectedByGlobalUpgrade | enabledInMvp |
|---|---|---:|---:|---:|---:|---|---|
| gold_boost_5m | GoldBoost | 300 | 2.0 | 0 | 0 | true | false |
| diamond_ad_50 | Diamond | 0 | 0 | 50 | 5 | false | false |

### FirebaseRemoteConfigKeys

| key | initialValue |
|---|---|
| min_supported_version | 0.1.0 |
| latest_version | 0.1.0 |
| force_update_required | false |
| maintenance_mode | false |
| maintenance_message |  |
| master_data_version | 1 |
| balance_version | 1 |
| event_config_version | 0 |
| android_store_url | TBD |
| ios_store_url | TBD |

### ChestTable

| chestId | displayName | costDiamond | itemCount | dropRateSetId |
|---|---|---:|---:|---|
| chest_normal | 노멀 상자 | 30 | 6 | drop_normal |
| chest_rare | 레어 상자 | 80 | 6 | drop_rare |
| chest_unique | 유니크 상자 | 150 | 6 | drop_unique |
| chest_legendary | 전설 상자 | 300 | 6 | drop_legendary |

`drop_legendary`는 Normal 40%, Rare 40%, Unique 18%, Legendary Blueprint 2%로 구성한다. Legendary 보상은 완제품 장비가 아니라 제작 문서다.

### ChestPityConfig

| pityId | chestId | targetRewardType | targetGrade | hardPityCount | softPityStartCount | rewardSelectionType | resetOnTargetReward |
|---|---|---|---|---:|---:|---|---|
| pity_legendary_blueprint | chest_legendary | EquipmentBlueprint | Legendary | 50 | 0 | RandomInGrade | true |

### ChestDropRateTable

| dropRateSetId | rewardType | grade | weightPct |
|---|---|---|---:|
| drop_normal | EquipmentItem | Normal | 100 |
| drop_rare | EquipmentItem | Normal | 80 |
| drop_rare | EquipmentItem | Rare | 20 |
| drop_unique | EquipmentItem | Normal | 50 |
| drop_unique | EquipmentItem | Rare | 42 |
| drop_unique | EquipmentItem | Unique | 8 |
| drop_legendary | EquipmentItem | Normal | 40 |
| drop_legendary | EquipmentItem | Rare | 40 |
| drop_legendary | EquipmentItem | Unique | 18 |
| drop_legendary | EquipmentBlueprint | Legendary | 2 |

### ShopItemTable

| shopItemId | displayName | shopItemType | linkedContentId | costType | costAmount | sortOrder |
|---|---|---|---|---|---:|---:|
| shop_chest_normal | 노멀 상자 | Chest | chest_normal | Diamond | 30 | 1 |
| shop_chest_rare | 레어 상자 | Chest | chest_rare | Diamond | 80 | 2 |
| shop_chest_unique | 유니크 상자 | Chest | chest_unique | Diamond | 150 | 3 |
| shop_chest_legendary | 전설 상자 | Chest | chest_legendary | Diamond | 300 | 4 |
| shop_income_2x_15m | 15분 수익 2배 | TimedBoost | income_2x_15m | Diamond | TBD | 10 |
| shop_income_5x_5m | 5분 수익 5배 | TimedBoost | income_5x_5m | Diamond | TBD | 11 |
| shop_character_rare_runner | 빠른 점원 | Character | char_rare_runner | Diamond | 500 | 20 |
| shop_character_unique_maker | 솜씨 좋은 점원 | Character | char_unique_maker | Diamond | 1200 | 21 |
| shop_character_epic_manager | 현장 매니저 | Character | char_epic_manager | Diamond | 2600 | 22 |
| shop_character_legendary_keeper | 전설 점장 | Character | char_legendary_keeper | Diamond | 4000 | 23 |

### TimedBoostConfig

| boostId | displayName | effectType | multiplier | durationSec | stackingGroup | sameBoostStackRule |
|---|---|---|---:|---:|---|---|
| income_2x_15m | 15분 수익 2배 | IncomeMultiplier | 2.0 | 900 | income_multiplier | ExtendDuration |
| income_5x_5m | 5분 수익 5배 | IncomeMultiplier | 5.0 | 300 | income_multiplier | ExtendDuration |

### CharacterTable

| characterId | displayName | grade | costDiamond | passiveOptionIds | iconKey | modelKey | unlockSource |
|---|---|---|---:|---|---|---|---|
| char_normal_default | 기본 점원 | Normal | 0 |  | icon_char_default | model_char_default | Default |
| char_rare_runner | 빠른 점원 | Rare | 500 | char_move_rare_001 | icon_char_runner | model_char_runner | Shop |
| char_unique_maker | 솜씨 좋은 점원 | Unique | 1200 | char_work_unique_001 | icon_char_maker | model_char_maker | Shop |
| char_epic_manager | 현장 매니저 | Epic | 2600 | char_move_epic_001,char_work_epic_001 | icon_char_manager | model_char_manager | Shop |
| char_legendary_keeper | 전설 점장 | Legendary | 4000 | char_dust_legendary_001,char_global_skill_legendary_001 | icon_char_keeper | model_char_keeper | Shop |
| char_mythic_special | 신화 점장 | Mythic | 0 | char_dust_mythic_001,char_global_skill_mythic_001 | icon_char_mythic | model_char_mythic | Event |

### CharacterPassiveOptionTable

| passiveOptionId | displayName | effectType | value | valueType | applyScope |
|---|---|---|---:|---|---|
| char_move_rare_001 | 주인공 이동속도 증가 | MainCharacterMoveSpeedPct | 5 | Percent | EquippedCharacterOnly |
| char_work_unique_001 | 주인공 작업속도 증가 | MainCharacterWorkSpeedPct | 5 | Percent | EquippedCharacterOnly |
| char_move_epic_001 | 주인공 이동속도 증가 | MainCharacterMoveSpeedPct | 8 | Percent | EquippedCharacterOnly |
| char_work_epic_001 | 주인공 작업속도 증가 | MainCharacterWorkSpeedPct | 5 | Percent | EquippedCharacterOnly |
| char_dust_legendary_001 | 장비 분해 Star Dust 증가 | EquipmentDismantleStarDustBonusPct | 1 | Percent | EquippedCharacterOnly |
| char_global_skill_legendary_001 | 공용 스킬 효과 증가 | GlobalSkillEffectBonusPct | 1 | Percent | EquippedCharacterOnly |
| char_dust_mythic_001 | 장비 분해 Star Dust 증가 | EquipmentDismantleStarDustBonusPct | 2 | Percent | EquippedCharacterOnly |
| char_global_skill_mythic_001 | 공용 스킬 효과 증가 | GlobalSkillEffectBonusPct | 2 | Percent | EquippedCharacterOnly |

### StaffTable

| staffId | displayName | role | addSource | moveTimePerTileSec | carryCapacity | maxCarryCapacity | workSpeedMultiplier | minProductionTimeSec | maxLevel | upgradeBaseCost | workTimeMultiplierPerLevel | costGrowth |
|---|---|---|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| generalist_staff | 범용 직원 | Generalist | StageUpgrade | 1.0 | 1 | 2 | 1.0 | 0.6 | 50 | 100 | 0.97 | 1.15 |

### CustomerTypeTable

| customerTypeId | displayName | orderQuantityMultiplier | expensiveProductWeight | moveSpeed | spawnWeight |
|---|---|---:|---:|---:|---:|
| normal | 일반 손님 | 1.0 | 1.0 | 1.5 | 100 |

### CustomerOrderConfig

| configId | firstOrderMode | eligibleProductRule | productSelectionRule | distinctProductCountRule | maxDistinctProductTypes | quantityRule | minQuantityPerProduct | maxQuantityPerProduct | workAssignmentRule | goldRewardTiming |
|---|---|---|---|---|---:|---|---:|---:|---|---|
| mvp_default_order | StartingStandOnly | UnlockedAndProductionSlotActivated | UniformEligibleProducts | UniformRange | 2 | UniformIntRange | 1 | 3 | OrderTakerThenProductUnitTasks | PerProductDelivered |

### StageUpgradeTable

| upgradeId | themeId | stageId | displayName | targetType | targetId | effectType | effectValue | cost | maxPurchaseCount | sortOrder |
|---|---|---|---|---|---|---|---:|---:|---:|---:|
| cotton_candy_speed_001 | theme_amusement | amusement_001 | 솜사탕 생산속도 증가 | Product | cotton_candy | ProductionSpeedPct | 100 | 50 | 1 | 1 |
| cotton_candy_reward_001 | theme_amusement | amusement_001 | 솜사탕 판매가격 증가 | Product | cotton_candy | RewardMultiplier | 2 | 1800 | 1 | 2 |
| amusement_001_customer_001 | theme_amusement | amusement_001 | 손님 증가 | Customer |  | CustomerCountAdd | 1 | 1200000 | 1 | 3 |
