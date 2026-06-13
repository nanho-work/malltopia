# Malltopia Next Patch Plan

Version: 0.1  
Patch Target: v0.2 - Unity Data & Economy Foundation  
Status: Needs confirmation after concept review

## 1. Objective

다음 코드 패치는 보이는 매장 루프를 만들기 전에 데이터와 경제 계산 기반을 만든다.

목표:

- 문서에 정의된 MVP 데이터를 Unity 코드로 표현한다.
- 메인 캐릭터 자동 에이전트 기본 능력치 데이터를 Unity 코드로 표현한다.
- 테마/스테이지/스테이지 업그레이드 데이터를 Unity 코드로 표현한다.
- 상품 판매가와 업그레이드 비용 계산을 구현한다.
- 상품 별 마일스톤과 생산 슬롯 표시/활성화 조건 계산을 구현한다.
- 별 마일스톤 달성 시 Diamond 보상 데이터를 표현한다.
- 직원 생산 시간 배율과 업그레이드 비용 계산을 구현한다.
- Gold 축약 표기 규칙을 구현한다.
- Gold 계열 값은 MVP에서 `double` 기반 `CurrencyAmount` 규칙으로 처리한다.
- 장비 시스템 확장을 위한 최소 데이터 자리를 준비한다.
- 캐릭터 구매/선택 확장을 위한 최소 데이터 자리를 준비한다.
- Gold 획득/소비 로직을 구현한다.
- Diamond 획득/저장 로직을 준비한다.
- Gold 기반 액션 가능 여부 계산을 준비한다.
- 전역/기능성 알림 확인 상태 저장 모델을 준비한다.
- 저장 데이터 모델을 준비한다.
- Firebase Remote Config 기반 버전 관리 설정 자리를 준비한다.

## 2. In Scope

다음 항목을 포함한다.

- Unity 프로젝트 기본 폴더 구조
- `MainCharacterConfigData`
- `ThemeData`
- `StageData`
- `StageLayoutPointData`
- `ProductData`
- `ProductStarMilestoneData`
- `ProductProductionSlotData`
- `StaffData`
- `GlobalUpgradeData` placeholder
- `ChestData` placeholder
- `ChestDropRateData` placeholder
- `ChestPityConfigData` placeholder
- `ShopItemData` placeholder
- `TimedBoostConfigData` placeholder
- `CharacterData` placeholder
- `CharacterPassiveOptionData` placeholder
- `EquipmentItemData` placeholder
- `EquipmentBlueprintData` placeholder
- `EquipmentCraftRecipeData` placeholder
- `EquipmentDismantleConfigData` placeholder
- `EquipmentOptionData` placeholder
- `EquipmentOptionPoolData` placeholder
- `StageUpgradeData`
- `CustomerTypeData`
- `CustomerOrderConfigData`
- `NumberFormatConfigData`
- `EconomyConfigData`
- `OfflineRewardConfigData`
- `FirebaseRemoteConfigDefaultsData` placeholder
- `VersionGateConfigData` placeholder
- `RemoteConfigService` local-default implementation
- `VersionGateService`
- `GameSaveData`
- `MainCharacterSaveData`
- `ProductSaveData`
- `StaffSaveData`
- `StageUpgradeSaveData`
- `EconomyService`
- `ActionAvailabilityService`
- `NoticeService`
- 상품 판매가 계산
- 상품 업그레이드 비용 계산
- 상품 별 개수 계산
- 별 마일스톤 Diamond 보상 계산
- 생산 슬롯 활성화 가능 여부 계산
- 메인 캐릭터 기본 능력치 참조
- 상품 매대 해금 상태 모델
- 생산 테이블 활성화 상태 모델
- 직원 생산 시간 배율 계산
- 직원 업그레이드 비용 계산
- Gold 추가, 소비, 구매 가능 여부 검사
- Diamond 추가, 소비 가능 여부 검사
- Gold 기반 해금/업그레이드 가능 여부 계산
- 확인한 신규 기능 알림 ID 저장
- Gold 값의 음수, NaN, Infinity 방어

## 3. Out of Scope

다음 항목은 v0.2에 포함하지 않는다.

- 손님 생성
- 손님 이동
- 메인 캐릭터 자동 이동 런타임 동작
- 주문 접수/생산/전달 런타임 동작
- 상품 매대 런타임 동작
- 생산 테이블 런타임 동작
- 복수 손님 주문 대기열
- 직원 이동
- UI 화면
- 저장 파일 입출력
- 오프라인 보상 계산
- 실제 Firebase SDK 연동
- 장비 드랍
- 장비 강화
- 장비 합성/제작
- 장비 분해
- Star Dust 사용
- 캐릭터 구매/교체 실제 UI
- 상자 구매/오픈
- 상점 시간제 부스터 구매/적용
- S급 상품 판정
- 광고 SDK
- 애니메이션

## 4. Folder Targets

권장 생성 경로:

```text
Assets/
  Scripts/
    Characters/
    Core/
    Data/
    Equipment/
    Economy/
    Firebase/
    MainCharacter/
    Save/
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
```

## 5. Class Targets

| Class | Type | Purpose |
|---|---|---|
| MainCharacterConfigData | ScriptableObject | 메인 캐릭터 자동 에이전트 기본 능력치 |
| ThemeData | ScriptableObject | 테마 마스터 데이터 |
| StageData | ScriptableObject | 스테이지 마스터 데이터 |
| StageClearRewardData | ScriptableObject | 스테이지 클리어 보상 데이터 자리 |
| StageLayoutPointData | ScriptableObject | 11x20 고정 그리드의 입장, 퇴장, 카운터 주문 위치, 생산 슬롯 위치 데이터 |
| ProductData | ScriptableObject | 상품 마스터 데이터 |
| ProductLevelCurveData | ScriptableObject | 상품 레벨별 판매가/업그레이드 비용 곡선 데이터 |
| ProductStarMilestoneData | ScriptableObject | 상품 별 마일스톤 데이터 |
| ProductProductionSlotData | ScriptableObject | 상품 생산 슬롯 표시/활성화 데이터 |
| StaffData | ScriptableObject | 직원 마스터 데이터 |
| GlobalUpgradeData | ScriptableObject | 금고/전역 업그레이드 데이터 자리 |
| ChestData | ScriptableObject | Diamond 상자 데이터 자리 |
| ChestDropRateData | ScriptableObject | 상자 등급별 드랍 확률 데이터 자리 |
| ChestPityConfigData | ScriptableObject | 전설 상자 50회 천장 데이터 자리 |
| ShopItemData | ScriptableObject | 상점 아이템 데이터 자리 |
| TimedBoostConfigData | ScriptableObject | 시간제 수익 부스터 데이터 자리 |
| CharacterData | ScriptableObject | 구매/선택 가능한 캐릭터 데이터 자리 |
| CharacterPassiveOptionData | ScriptableObject | 캐릭터 패시브 옵션 데이터 자리 |
| EquipmentItemData | ScriptableObject | 장비 아이템 데이터 자리 |
| EquipmentBlueprintData | ScriptableObject | 전설 이상 장비 제작 문서 데이터 자리 |
| EquipmentCraftRecipeData | ScriptableObject | 장비 합성/제작 레시피 데이터 자리 |
| EquipmentDismantleConfigData | ScriptableObject | 장비 분해 보상 데이터 자리 |
| EquipmentOptionData | ScriptableObject | 장비 옵션 데이터 자리 |
| EquipmentOptionPoolData | ScriptableObject | 등급/부위별 옵션 등장 풀 데이터 자리 |
| StageUpgradeData | ScriptableObject | 하단 스테이지 업그레이드 데이터 |
| CustomerTypeData | ScriptableObject | 손님 타입 데이터 |
| CustomerOrderConfigData | ScriptableObject | 첫 주문, 주문 가능 상품, 균등 랜덤 주문, 상품 1개 단위 작업 규칙 |
| NumberFormatConfigData | ScriptableObject | Gold 축약 표기 설정 |
| EconomyConfigData | ScriptableObject | 경제 공통 설정 |
| OfflineRewardConfigData | ScriptableObject | 오프라인 보상 설정 |
| FirebaseRemoteConfigDefaultsData | ScriptableObject | Firebase Remote Config 기본값 자리 |
| VersionGateConfigData | ScriptableObject | 최소 지원 버전, 최신 버전, 점검 모드 판단 데이터 자리 |
| EconomyService | C# class | Gold 획득/소비 처리 |
| ActionAvailabilityService | C# class | Gold 기반 액션 가능 여부 계산 |
| NoticeService | C# class | 전역/기능성 알림 확인 상태 관리 |
| RemoteConfigService | C# class | Firebase SDK 연동 전 로컬 기본 Remote Config 값을 제공 |
| VersionGateService | C# class | Remote Config 값을 기준으로 앱 진입 가능 여부 판단 |
| NumberFormatService | C# class | k, m, b, t, aa 단위 표기 처리 |
| GameSaveData | Serializable class | 전체 저장 데이터 |
| MainCharacterSaveData | Serializable class | 메인 캐릭터 저장 데이터 |
| ProductSaveData | Serializable class | 상품 저장 데이터 |
| StaffSaveData | Serializable class | 직원 저장 데이터 |
| StageUpgradeSaveData | Serializable class | 스테이지 업그레이드 구매 상태 |

## 6. Formula Targets

### Agent Effective Stat

```text
effectiveStat = baseStat * (1 + characterPassivePct / 100) * (1 + equipmentBonusPct / 100)
```

MVP에서는 캐릭터 패시브와 장비 보너스를 0으로 둬도 된다.

### Product Level Base Sale Price

```text
levelBaseSaleGold = ProductLevelCurveTable value for current level
```

`ProductLevelCurveTable`은 구간별 `Add`, `Multiply`, `Fixed` 성장 타입을 지원한다.

```text
Add: value = startValue + ((level - levelStart) * growthValue)
Multiply: value = startValue * growthValue ^ (level - levelStart)
Fixed: value = startValue
```

### Product Star Count

```text
starCount(level) = count(milestone.requiredLevel <= level)
```

### Newly Reached Star Milestones

```text
newStars = milestones where previousLevel < requiredLevel <= currentLevel
diamondReward = sum(newStars.diamondReward)
```

### Product Sale Price

```text
salePrice(level, starCount) =
  levelBaseSaleGold
  * 2 ^ starCount
  * productRewardUpgradeMultiplier
  * equipmentIncomeMultiplier
  * characterPassiveIncomeMultiplier
  * timedBoostMultiplier
  * doubleProductMultiplier
  * sGradeProductMultiplier
```

### Product Upgrade Cost

```text
upgradeCost(level) = ProductLevelCurveTable upgrade cost for current level
```

### Effective Production Time

```text
workerWorkTimeMultiplier(level) = workTimeMultiplierPerLevel ^ (level - 1)
productionSpeedMultiplier = 1 + (productionSpeedBonusPct / 100)
effectiveProductionTime = max(minProductionTimeSec, productBaseProductionTimeSec * workerWorkTimeMultiplier(level) / productionSpeedMultiplier)
```

### Staff Upgrade Cost

```text
upgradeCost(level) = round(upgradeBaseCost * costGrowth ^ (level - 1))
```

### Production Slot Availability

```text
canShowSlot = starCount >= requiredStarCount && !slotActivated
activationCost = 0
activationMode = FreeTap
```

### Number Formatting

```text
formatGold(value) = 999, 1k, 1m, 1b, 1t, 1aa, 1ab...
```

### Currency Amount

```text
CurrencyAmount = double
```

MVP에서는 빠른 구현과 큰 수 표기를 위해 `double`을 사용한다. 후반 경제에서 정밀도가 문제가 되면 BigNumber 구조로 교체한다.

## 7. Acceptance Criteria

v0.2는 다음 조건을 만족하면 완료로 본다.

- Unity에서 컴파일 가능한 데이터 클래스가 있다.
- 메인 캐릭터 자동 에이전트 기본 능력치 데이터가 있다.
- 테마와 스테이지 데이터가 있다.
- 스테이지 클리어 보상 데이터 자리가 있다.
- 상품 데이터가 판매가와 업그레이드 비용을 계산할 수 있다.
- 상품 별 마일스톤 데이터가 레벨 기준 별 개수를 계산할 수 있다.
- 상품 별 마일스톤 데이터가 Diamond 보상을 표현할 수 있다.
- 생산 슬롯 데이터가 별 조건과 무료 터치 활성화 방식을 표현할 수 있다.
- 직원 데이터가 상품 기본 생산 시간에 적용할 생산 시간 배율과 업그레이드 비용을 계산할 수 있다.
- 스테이지 업그레이드 데이터가 생산속도, 판매가격, 손님 +1 효과를 표현할 수 있다.
- Gold 축약 표기 서비스가 `1.8k`, `1.2m`, `4b` 같은 표기를 만들 수 있다.
- Gold 계산에서 음수, NaN, Infinity가 저장되지 않는다.
- 장비 아이템, 장비 옵션, 등급/부위별 옵션 풀 데이터 자리가 있다.
- 전설 제작 문서, 장비 제작 레시피, 장비 분해 보상 데이터 자리가 있다.
- 상점 아이템과 시간제 수익 부스터 데이터 자리가 있다.
- 캐릭터와 캐릭터 패시브 옵션 데이터 자리가 있다.
- `EconomyService`가 Gold 추가, 소비, 구매 가능 여부를 처리한다.
- `EconomyService`가 Diamond 추가와 소비 가능 여부를 처리할 준비가 되어 있다.
- Firebase Remote Config 기본 키와 버전 게이트 판단 데이터 자리가 있다.
- `ActionAvailabilityService`가 매대 해금, 상품 레벨업, 생산 슬롯 활성화, 스테이지 업그레이드 구매 가능 여부를 계산할 수 있다.
- 저장 데이터가 `seenNoticeIds`를 표현할 수 있다.
- 저장 데이터 모델이 JSON 직렬화 가능한 구조다.
- 저장 데이터가 상품 매대 해금 여부와 생산 테이블 활성화 여부를 표현할 수 있다.
- 저장 데이터가 활성화된 생산 슬롯 ID 목록을 표현할 수 있다.
- 저장 데이터가 활성 시간제 부스터 ID와 종료 시각을 표현할 수 있다.
- 저장 데이터가 보유 캐릭터와 선택 캐릭터를 표현할 수 있다.
- 저장 데이터가 현재 Theme, Stage, 손님 수, 직원 수, 스테이지 업그레이드 구매 상태를 표현할 수 있다.
- 문서의 Stage 1~3 초기 데이터를 코드 또는 Unity asset으로 만들 준비가 되어 있다.

## 8. Recommended Test Cases

에디터 테스트 또는 간단한 런타임 테스트로 확인한다.

- 솜사탕 레벨 1 판매가가 2인지 확인
- 솜사탕 레벨 1 업그레이드 비용이 3인지 확인
- 각 스테이지 시작 Gold가 5인지 확인
- 첫 매대 해금 비용이 5인지 확인
- 두 번째 매대 해금 비용이 250인지 확인
- 세 번째 매대 해금 비용이 12500인지 확인
- 다음 스테이지 이동 비용 규칙이 가장 비싼 최종 레벨업 비용 x1.5인지 확인
- Stage 1~3이 11x20 `FixedNoScroll` 그리드를 사용하는지 확인
- Stage 1~3의 카운터 폭이 각각 3, 5, 7인지 확인
- Stage 1~3의 입구, 출구, 카운터, 매대, 생산 슬롯, 주인공 시작 위치, 직원 대기 위치가 `StageLayoutPointData`로 표현되는지 확인
- 첫 손님 주문이 첫 상품으로 고정되는지 확인
- 첫 상품 이후 주문 가능 상품이 해금되고 생산 슬롯이 활성화된 상품으로 제한되는지 확인
- 상품 종류 수, 상품 선택, 상품별 1~3개 수량이 균등 랜덤 규칙으로 표현되는지 확인
- 주문 접수 후 상품 1개 단위 포장/생산 작업으로 분해되는지 확인
- Gold가 주문 전체 완료가 아니라 상품 1개 전달마다 지급되는지 확인
- 카운터 주문 위치가 기본 앞줄, 좌우, 뒤쪽 fallback 우선순위를 표현하는지 확인
- 주인공과 직원의 기본 운반 수량이 1, 최대 확장 수량이 2로 표현되는지 확인
- 2번 매대 레벨 1 판매가가 320인지 확인
- 2번 매대 레벨 1 업그레이드 비용이 150인지 확인
- 2번 매대 레벨 10에서 별 1개, 표시 수익 약 1.27k, 레벨업 비용 약 774인지 확인
- 2번 매대 레벨 25에서 별 2개, 표시 수익 약 8.11k, 레벨업 비용 약 12.0k인지 확인
- 솜사탕 레벨 10에서 별 1개가 계산되는지 확인
- 솜사탕 레벨 25에서 별 2개가 계산되는지 확인
- 솜사탕 별 1개 달성 후 판매가에 x2가 적용되는지 확인
- 1번 매대 35레벨, 별 2개, 2배 버프 상태의 표시 수익이 약 1.31k인지 확인
- 1번 매대 35레벨 레벨업 비용이 약 1.48k인지 확인
- 1번 매대 35~41레벨 구간에서 표시 수익이 레벨당 약 x1.08 증가하는지 확인
- 1번 매대 35~41레벨 구간에서 레벨업 비용이 레벨당 약 x1.20 증가하는지 확인
- 솜사탕 별 1개 달성 후 Diamond 보상 1개가 계산되는지 확인
- 솜사탕 별 2개 달성 후 누적 Diamond 보상 2개가 계산되는지 확인
- 기본 메인 캐릭터의 매대 1칸 이동 시간이 1.0초인지 확인
- 기본 메인 캐릭터 운반량이 1인지 확인
- 스테이지 첫 상품 생산 시간이 5초인지 확인
- 스테이지 두 번째 상품 생산 시간이 9초인지 확인
- Stage 2 상품 최대 레벨이 75, 최대 별이 4개인지 확인
- 1번 매대에는 추가 생산 슬롯이 없는지 확인
- 2번/3번 매대의 두 번째 생산 슬롯이 1별 달성 후 표시되는지 확인
- 스테이지 세 번째 상품 생산 시간이 13초인지 확인
- 범용 직원 레벨 1 작업속도 배율이 1.0인지 확인
- 범용 직원 생산 시간이 최소 0.6초 아래로 내려가지 않는지 확인
- Stage 1 솜사탕 생산속도 업그레이드가 +100%, 즉 2배 증가로 표현되는지 확인
- Stage 1 솜사탕 판매가격 업그레이드가 x2로 표현되는지 확인
- Stage 1 업그레이드 리스트에 직원 +1이 포함되지 않는지 확인
- 1800 Gold가 `1.8k`로 표시되는지 확인
- 1200000 Gold가 `1.2m`으로 표시되는지 확인
- Gold 100에서 30 소비 시 70이 되는지 확인
- Gold 20에서 30 소비 시 실패하고 20이 유지되는지 확인
- Gold가 충분할 때 매대 레벨업 가능 상태가 true인지 확인
- Gold가 부족할 때 매대 레벨업 가능 상태가 false인지 확인
- 확인한 신규 기능 알림 ID가 저장 모델에 들어가는지 확인
