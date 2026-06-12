# Malltopia Next Patch Plan

Version: 0.1  
Patch Target: v0.2 - Unity Data & Economy Foundation  
Status: Ready for implementation  

## 1. Objective

다음 코드 패치는 보이는 매장 루프를 만들기 전에 데이터와 경제 계산 기반을 만든다.

목표:

- 문서에 정의된 MVP 데이터를 Unity 코드로 표현한다.
- 상품 판매가와 업그레이드 비용 계산을 구현한다.
- 직원 작업 시간과 업그레이드 비용 계산을 구현한다.
- Gold 획득/소비 로직을 구현한다.
- 저장 데이터 모델을 준비한다.

## 2. In Scope

다음 항목을 포함한다.

- Unity 프로젝트 기본 폴더 구조
- `ProductData`
- `StaffData`
- `StageData`
- `StoreUpgradeData`
- `CustomerTypeData`
- `EconomyConfigData`
- `OfflineRewardConfigData`
- `GameSaveData`
- `ProductSaveData`
- `StaffSaveData`
- `EconomyService`
- 상품 판매가 계산
- 상품 업그레이드 비용 계산
- 직원 작업 시간 계산
- 직원 업그레이드 비용 계산
- Gold 추가, 소비, 구매 가능 여부 검사

## 3. Out of Scope

다음 항목은 v0.2에 포함하지 않는다.

- 손님 생성
- 손님 이동
- 상품 진열대 런타임 동작
- 계산대 대기열
- 직원 이동
- UI 화면
- 저장 파일 입출력
- 오프라인 보상 계산
- 광고 SDK
- 애니메이션

## 4. Folder Targets

권장 생성 경로:

```text
Assets/
  Scripts/
    Core/
    Data/
    Economy/
    Save/
  Data/
    Customers/
    Economy/
    Products/
    Staff/
    Stages/
    Store/
```

## 5. Class Targets

| Class | Type | Purpose |
|---|---|---|
| ProductData | ScriptableObject | 상품 마스터 데이터 |
| StaffData | ScriptableObject | 직원 마스터 데이터 |
| StageData | ScriptableObject | 스테이지 마스터 데이터 |
| StoreUpgradeData | ScriptableObject | 매장 업그레이드 데이터 |
| CustomerTypeData | ScriptableObject | 손님 타입 데이터 |
| EconomyConfigData | ScriptableObject | 경제 공통 설정 |
| OfflineRewardConfigData | ScriptableObject | 오프라인 보상 설정 |
| EconomyService | C# class | Gold 획득/소비 처리 |
| GameSaveData | Serializable class | 전체 저장 데이터 |
| ProductSaveData | Serializable class | 상품 저장 데이터 |
| StaffSaveData | Serializable class | 직원 저장 데이터 |

## 6. Formula Targets

### Product Sale Price

```text
salePrice(level) = round(basePrice * priceGrowth ^ (level - 1))
```

### Product Upgrade Cost

```text
upgradeCost(level) = round(upgradeBaseCost * costGrowth ^ (level - 1))
```

### Staff Work Time

```text
workTime(level) = max(minWorkTime, baseWorkTime * workTimeMultiplier ^ (level - 1))
```

### Staff Upgrade Cost

```text
upgradeCost(level) = round(upgradeBaseCost * costGrowth ^ (level - 1))
```

## 7. Acceptance Criteria

v0.2는 다음 조건을 만족하면 완료로 본다.

- Unity에서 컴파일 가능한 데이터 클래스가 있다.
- 상품 데이터가 판매가와 업그레이드 비용을 계산할 수 있다.
- 직원 데이터가 작업 시간과 업그레이드 비용을 계산할 수 있다.
- `EconomyService`가 Gold 추가, 소비, 구매 가능 여부를 처리한다.
- 저장 데이터 모델이 JSON 직렬화 가능한 구조다.
- 문서의 Stage 1 초기 데이터를 코드 또는 Unity asset으로 만들 준비가 되어 있다.

## 8. Recommended Test Cases

에디터 테스트 또는 간단한 런타임 테스트로 확인한다.

- 과자 레벨 1 판매가가 5인지 확인
- 과자 레벨 1 업그레이드 비용이 10인지 확인
- 계산원 레벨 1 작업 시간이 2.2초인지 확인
- 계산원 작업 시간이 최소 0.6초 아래로 내려가지 않는지 확인
- Gold 100에서 30 소비 시 70이 되는지 확인
- Gold 20에서 30 소비 시 실패하고 20이 유지되는지 확인
