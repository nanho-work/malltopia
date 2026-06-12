# Malltopia Balance Guide

Version: 0.1  
Status: Initial MVP tuning baseline  

## 1. Purpose

이 문서는 MVP 경제 밸런스의 기준값을 정의한다. 모든 숫자는 1차 기준값이며, 실제 플레이 테스트 후 조정한다.

MVP 밸런스 목표:

- 첫 30초 안에 첫 수익을 본다.
- 첫 1분 안에 첫 업그레이드를 한다.
- 첫 3분 안에 두 번째 상품을 해금한다.
- 첫 7분 안에 직원 고용을 경험한다.
- 첫 10분 안에 매장이 더 바빠졌다는 느낌을 준다.

## 2. Currency

| Currency | Use | MVP |
|---|---|---|
| Gold | 상품, 직원, 매장 성장 | Included |
| Diamond | 프리미엄 재화 | Excluded |

## 3. Product Balance

| Product ID | Name | Unlock Cost | Base Price | Base Stock | Restock Time Sec | Max Level | Upgrade Base Cost | Price Growth | Cost Growth |
|---|---|---:|---:|---:|---:|---:|---:|---:|---:|
| snack | 과자 | 0 | 5 | 5 | 3.0 | 100 | 10 | 1.10 | 1.12 |
| drink | 음료 | 50 | 10 | 5 | 4.0 | 100 | 25 | 1.11 | 1.13 |
| ramen | 라면 | 250 | 25 | 4 | 6.0 | 100 | 80 | 1.12 | 1.14 |

## 4. Product Formulas

### Sale Price

```text
salePrice(level) = round(basePrice * priceGrowth ^ (level - 1))
```

### Upgrade Cost

```text
upgradeCost(level) = round(upgradeBaseCost * costGrowth ^ (level - 1))
```

`level`은 현재 레벨이다. 예를 들어 레벨 1 상품을 레벨 2로 올릴 때 `level = 1` 비용을 사용한다.

## 5. Customer Flow

| Config | Initial Value | Description |
|---|---:|---|
| spawnIntervalSec | 3.0 | 손님 생성 주기 |
| maxCustomers | 4 | 동시 손님 수 |
| productPickDurationSec | 0.6 | 상품 집는 시간 |
| defaultCheckoutTimeSec | 3.5 | 계산원 없을 때 기본 계산 시간 |
| cashierCheckoutTimeSec | 2.2 | 계산원 고용 후 레벨 1 계산 시간 |
| leaveDelaySec | 0.2 | 결제 후 퇴장 전 짧은 지연 |

## 6. Customer Purchase Rule

MVP 기본 규칙:

```text
구매 가능한 상품 = 해금됨 && 현재 재고 > 0
손님은 구매 가능한 상품 중 하나를 랜덤 선택
구매 수량은 1개
```

고가 상품 구매 가중치는 MVP 이후에 추가한다.

## 7. Staff Balance

| Staff ID | Name | Hire Cost | Base Work Time Sec | Max Level | Upgrade Base Cost | Work Time Multiplier Per Level | Cost Growth |
|---|---|---:|---:|---:|---:|---:|---:|
| cashier | 계산원 | 150 | 2.2 | 50 | 100 | 0.97 | 1.15 |
| restocker | 진열 직원 | 120 | 2.5 | 50 | 90 | 0.97 | 1.15 |

## 8. Staff Formulas

### Work Time

```text
workTime(level) = max(minWorkTime, baseWorkTime * workTimeMultiplier ^ (level - 1))
```

MVP 기본값:

| Staff | minWorkTime |
|---|---:|
| cashier | 0.6 sec |
| restocker | 0.7 sec |

### Staff Upgrade Cost

```text
upgradeCost(level) = round(upgradeBaseCost * costGrowth ^ (level - 1))
```

## 9. Store Upgrade Balance

| Level | Upgrade Cost | Max Customers | Spawn Interval Sec |
|---:|---:|---:|---:|
| 1 | 0 | 4 | 3.0 |
| 2 | 200 | 5 | 2.8 |
| 3 | 500 | 6 | 2.6 |
| 4 | 1,200 | 7 | 2.4 |
| 5 | 2,800 | 8 | 2.2 |
| 6 | 6,000 | 9 | 2.0 |

MVP에서는 Store Level 6까지만 있어도 충분하다.

## 10. Default Automation

MVP는 수동 탭 조작이 없으므로, 직원이 없어도 기본 운영이 멈추면 안 된다.

권장 기본값:

| System | Before Hire | After Hire |
|---|---|---|
| Checkout | 계산대 기본 처리, 3.5 sec | 계산원 처리, 2.2 sec |
| Restock | 진열대 자체 재입고, base restock time | 진열 직원이 부족 재고 우선 보충 |

직원은 "필수 잠금 해제"가 아니라 "효율 증가"로 작동해야 초반이 막히지 않는다.

재입고는 상품별 재고가 0이 되었을 때 시작한다. 기본 재입고가 완료되면 해당 상품의 재고는 `baseStock`까지 회복된다. 진열 직원이 고용되어 있으면 가장 재고가 낮은 해금 상품을 우선 대상으로 삼고, 직원 작업 시간이 완료되면 해당 상품의 재고가 `baseStock`까지 회복된다.

## 11. Offline Reward

| Config | Value |
|---|---:|
| minOfflineSec | 60 |
| maxOfflineSec | 10,800 |
| offlineEfficiency | 0.25 |

계산:

```text
offlineGold = estimatedGoldPerSec * offlineSec * offlineEfficiency
```

`estimatedGoldPerSec`은 현재 상품 판매가, 손님 생성 주기, 계산 속도, 해금 상품 수를 기반으로 단순 추정한다.

## 12. Early Game Target Timeline

| Time | Expected Player State |
|---|---|
| 0:00 | 과자 진열대 운영 시작 |
| 0:30 | 첫 상품 업그레이드 가능 |
| 1:00 | 과자 레벨 3 이상 |
| 3:00 | 음료 해금 |
| 5:00 | 음료 업그레이드 진행 |
| 7:00 | 진열 직원 또는 계산원 고용 |
| 10:00 | 라면 해금 준비 또는 매장 업그레이드 진행 |

## 13. Tuning Notes

플레이 테스트에서 확인할 항목:

- 손님 대기열이 너무 길어 답답하지 않은가
- 재고 부족 시간이 너무 길지 않은가
- 첫 직원 고용까지 시간이 너무 길지 않은가
- 업그레이드 비용이 너무 급격히 오르지 않는가
- 상품 3개가 모두 의미 있게 수익에 기여하는가
- 10분 플레이 후 매장이 충분히 바빠 보이는가
