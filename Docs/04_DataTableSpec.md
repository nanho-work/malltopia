# Malltopia Data Table Specification

Version: 0.1  
Status: MVP data contract  

## 1. Purpose

Malltopia MVP는 서버 DB를 사용하지 않는다. 이 문서는 Unity 클라이언트에서 사용할 마스터 데이터와 저장 데이터의 구조를 정의한다.

권장 방식:

- 마스터 데이터: ScriptableObject 또는 CSV/JSON
- 저장 데이터: 로컬 JSON
- 런타임 상태: C# class instance

초기 개발은 ScriptableObject를 기준으로 한다. 밸런스 조정이 잦아지면 CSV/Google Sheets 기반으로 옮길 수 있다.

## 2. Naming Rules

ID 규칙:

- 소문자 영문
- 단어 구분은 underscore
- 표시 이름은 별도 필드 사용

예시:

- `stage_001`
- `snack`
- `drink`
- `ramen`
- `cashier`
- `restocker`

## 3. Table List

| Table | Purpose | MVP Required |
|---|---|---|
| StageTable | 스테이지 기본 정보 | Yes |
| ProductTable | 상품 기본 정보 | Yes |
| StaffTable | 직원 기본 정보 | Yes |
| StoreUpgradeTable | 매장 업그레이드 정보 | Yes |
| CustomerTypeTable | 손님 타입 정보 | Yes |
| EconomyConfig | 경제 공식 기준값 | Yes |
| OfflineRewardConfig | 오프라인 보상 기준값 | Yes |
| AdRewardConfig | 광고 보상 기준값 | Stub |
| SaveData | 유저 저장 데이터 | Yes |

## 4. StageTable

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| stageId | string | Yes | stage_001 | 고유 ID |
| displayName | string | Yes | 동네 구멍가게 | 표시 이름 |
| order | int | Yes | 1 | 스테이지 순서 |
| productIds | string[] | Yes | snack,drink,ramen | 포함 상품 |
| defaultMaxCustomers | int | Yes | 4 | 기본 동시 손님 수 |
| defaultSpawnIntervalSec | float | Yes | 3.0 | 기본 손님 생성 주기 |
| unlockCost | int | Yes | 0 | 스테이지 해금 비용 |

## 5. ProductTable

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| productId | string | Yes | snack | 고유 ID |
| displayName | string | Yes | 과자 | 표시 이름 |
| stageId | string | Yes | stage_001 | 소속 스테이지 |
| unlockCost | int | Yes | 0 | 해금 비용 |
| basePrice | int | Yes | 5 | 레벨 1 판매가 |
| baseStock | int | Yes | 5 | 최대 진열 재고 |
| restockTimeSec | float | Yes | 3.0 | 기본 재입고 시간 |
| maxLevel | int | Yes | 100 | 최대 레벨 |
| upgradeBaseCost | int | Yes | 10 | 첫 업그레이드 비용 |
| priceGrowth | float | Yes | 1.10 | 판매가 증가율 |
| costGrowth | float | Yes | 1.12 | 업그레이드 비용 증가율 |
| shelfPrefabKey | string | No | shelf_snack | 프리팹 참조 키 |
| iconKey | string | No | icon_snack | UI 아이콘 키 |

## 6. StaffTable

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| staffId | string | Yes | cashier | 고유 ID |
| displayName | string | Yes | 계산원 | 표시 이름 |
| role | enum | Yes | Cashier | 직원 역할 |
| hireCost | int | Yes | 150 | 고용 비용 |
| baseWorkTimeSec | float | Yes | 2.2 | 기본 작업 시간 |
| minWorkTimeSec | float | Yes | 0.6 | 최소 작업 시간 |
| maxLevel | int | Yes | 50 | 최대 레벨 |
| upgradeBaseCost | int | Yes | 100 | 첫 업그레이드 비용 |
| workTimeMultiplier | float | Yes | 0.97 | 레벨당 작업 시간 배율 |
| costGrowth | float | Yes | 1.15 | 업그레이드 비용 증가율 |
| prefabKey | string | No | staff_cashier | 프리팹 참조 키 |
| iconKey | string | No | icon_cashier | UI 아이콘 키 |

### StaffRole Enum

| Value | Description |
|---|---|
| Cashier | 계산 처리 |
| Restocker | 재고 보충 |

## 7. StoreUpgradeTable

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| level | int | Yes | 2 | 매장 레벨 |
| upgradeCost | int | Yes | 200 | 해당 레벨로 올리는 비용 |
| maxCustomers | int | Yes | 5 | 동시 손님 수 |
| spawnIntervalSec | float | Yes | 2.8 | 손님 생성 주기 |

레벨 1은 기본값이며 비용은 0으로 둔다.

## 8. CustomerTypeTable

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| customerTypeId | string | Yes | normal | 고유 ID |
| displayName | string | Yes | 일반 손님 | 표시 이름 |
| purchaseQuantity | int | Yes | 1 | 구매 수량 |
| expensiveProductWeight | float | No | 1.0 | 고가 상품 선호도 |
| moveSpeed | float | Yes | 1.5 | 이동 속도 |
| prefabKey | string | No | customer_normal | 프리팹 참조 키 |

MVP에서는 `normal` 하나만 사용한다.

## 9. EconomyConfig

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| startingGold | int | Yes | 0 | 새 게임 시작 Gold |
| defaultCheckoutTimeSec | float | Yes | 3.5 | 계산원 없을 때 계산 시간 |
| productPickDurationSec | float | Yes | 0.6 | 상품 집는 시간 |
| leaveDelaySec | float | Yes | 0.2 | 결제 후 퇴장 전 지연 |
| goldRoundingUnit | int | Yes | 1 | Gold 반올림 단위 |
| autosaveIntervalSec | float | Yes | 10.0 | 자동 저장 주기 |

## 10. OfflineRewardConfig

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| minOfflineSec | int | Yes | 60 | 보상 표시 최소 오프라인 시간 |
| maxOfflineSec | int | Yes | 10800 | 최대 인정 시간 |
| efficiency | float | Yes | 0.25 | 오프라인 효율 |

## 11. AdRewardConfig

MVP에서는 실제 광고 SDK를 붙이지 않는다. 다만 이후 확장을 위해 설정 구조만 둘 수 있다.

| Field | Type | Required | Example | Description |
|---|---|---|---|---|
| rewardId | string | Yes | gold_boost_30m | 보상 ID |
| rewardType | enum | Yes | GoldBoost | 보상 타입 |
| durationSec | int | No | 1800 | 지속 시간 |
| multiplier | float | No | 2.0 | 배율 |
| amount | int | No | 0 | 지급량 |
| enabledInMvp | bool | Yes | false | MVP 활성 여부 |

## 12. SaveData

저장 데이터는 마스터 데이터와 분리한다.

상품의 현재 재고, 손님 위치, 계산 대기열 같은 런타임 상태는 저장하지 않는다. 재접속 시에는 마스터 데이터와 진행 상태를 기준으로 매장 루프를 새로 시작한다.

```json
{
  "version": 1,
  "gold": 0,
  "currentStageId": "stage_001",
  "storeLevel": 1,
  "products": [
    {
      "productId": "snack",
      "unlocked": true,
      "level": 1
    }
  ],
  "staff": [
    {
      "staffId": "cashier",
      "hired": false,
      "level": 1
    }
  ],
  "lastSavedAtUtc": "2026-06-12T00:00:00Z",
  "totalPlayTimeSec": 0
}
```

### SaveData Fields

| Field | Type | Required | Description |
|---|---|---|---|
| version | int | Yes | 저장 데이터 버전 |
| gold | long | Yes | 현재 Gold |
| currentStageId | string | Yes | 현재 스테이지 |
| storeLevel | int | Yes | 매장 업그레이드 레벨 |
| products | ProductSaveData[] | Yes | 상품 진행 상태 |
| staff | StaffSaveData[] | Yes | 직원 진행 상태 |
| lastSavedAtUtc | string | Yes | 마지막 저장 UTC 시각 |
| totalPlayTimeSec | long | Yes | 누적 플레이 시간 |

## 13. ProductSaveData

| Field | Type | Required | Description |
|---|---|---|---|
| productId | string | Yes | 상품 ID |
| unlocked | bool | Yes | 해금 여부 |
| level | int | Yes | 현재 레벨 |

## 14. StaffSaveData

| Field | Type | Required | Description |
|---|---|---|---|
| staffId | string | Yes | 직원 ID |
| hired | bool | Yes | 고용 여부 |
| level | int | Yes | 현재 레벨 |

## 15. Initial Data

### StageTable

| stageId | displayName | order | productIds | defaultMaxCustomers | defaultSpawnIntervalSec | unlockCost |
|---|---|---:|---|---:|---:|---:|
| stage_001 | 동네 구멍가게 | 1 | snack,drink,ramen | 4 | 3.0 | 0 |

### ProductTable

| productId | displayName | stageId | unlockCost | basePrice | baseStock | restockTimeSec | maxLevel | upgradeBaseCost | priceGrowth | costGrowth |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---:|
| snack | 과자 | stage_001 | 0 | 5 | 5 | 3.0 | 100 | 10 | 1.10 | 1.12 |
| drink | 음료 | stage_001 | 50 | 10 | 5 | 4.0 | 100 | 25 | 1.11 | 1.13 |
| ramen | 라면 | stage_001 | 250 | 25 | 4 | 6.0 | 100 | 80 | 1.12 | 1.14 |

### StaffTable

| staffId | displayName | role | hireCost | baseWorkTimeSec | minWorkTimeSec | maxLevel | upgradeBaseCost | workTimeMultiplier | costGrowth |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|
| cashier | 계산원 | Cashier | 150 | 2.2 | 0.6 | 50 | 100 | 0.97 | 1.15 |
| restocker | 진열 직원 | Restocker | 120 | 2.5 | 0.7 | 50 | 90 | 0.97 | 1.15 |
