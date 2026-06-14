# Malltopia Balance Guide

Version: 0.1  
Status: Initial MVP tuning baseline  

## 1. Purpose

이 문서는 MVP 경제 밸런스의 기준값을 정의한다. 모든 숫자는 1차 기준값이며, 실제 플레이 테스트 후 조정한다.

MVP 밸런스 목표:

- 첫 스테이지는 손님 1명과 메인 캐릭터 1명만으로 시작한다.
- 상품 생산 시간, 작업자 이동 시간, 손님 방문 속도, 업그레이드 비용이 자연스럽게 진행 시간을 만든다.
- 첫 스테이지 안에서 생산속도 2배, 판매가격 2배, 손님 +1 업그레이드의 의미가 보여야 한다.
- 모든 상품 최대 레벨 달성 후 다음 스테이지 버튼이 점멸한다.
- 초반에는 메인 캐릭터 혼자 주문/생산/전달을 처리하고, 직원 +1 이후 같은 루프를 병렬 처리하는 느낌을 준다.

## 2. Currency

| Currency | Use | MVP |
|---|---|---|
| Gold | 상품, 직원, 스테이지 성장 | Included |
| Diamond | 별 보상, 장기 프리미엄/전역 성장 | Earn and save only |

## 3. Stage Structure Baseline

테마별 목표 스테이지 수는 8개로 둔다. MVP에서는 놀이동산 테마의 1~3스테이지를 먼저 구현해 루프와 밸런스를 검증한다.

놀이동산 테마 예시:

| Stage ID | Stage No | Grid | Scroll | Counter Width | Product Stands | Starting Gold | Starting Stand | Max Product Level | Max Stars | Next Stage Cost Rule |
|---|---:|---|---|---:|---:|---:|---|---:|---:|---|
| amusement_001 | 1 | 11x20 | FixedNoScroll | 3 | 1 | 5 | 솜사탕 | 25 | 2 | max final upgrade cost x1.5 |
| amusement_002 | 2 | 11x20 | FixedNoScroll | 5 | 2 | 5 | 구슬아이스크림 | 75 | 4 | max final upgrade cost x1.5 |
| amusement_003 | 3 | 11x20 | FixedNoScroll | 7 | 3 | 5 | 팝콘 | 75 | 4 | max final upgrade cost x1.5 |
| amusement_008 | 8 | TBD | TBD | TBD | up to 10 | 5 | TBD | 350 | 12 | max final upgrade cost x1.5 |

MVP Stage 1~3은 폰 세로 화면을 11x20 그리드로 보고 한 화면에 모두 표시한다. x=5를 화면 중앙으로 두며, 맵 스크롤은 사용하지 않는다. 화면 위쪽 약 2/3는 손님 공간, 아래쪽 약 1/3는 주인공/직원 작업 공간으로 둔다.

매대와 생산 슬롯은 해금 여부와 관계없이 작업자 경로의 장애물이다. 생산 슬롯을 아직 활성화하지 않았더라도 미래 배치가 예약된 칸이면 주인공과 직원은 그 칸을 통과하지 않는다. 따라서 실제 진행 시간은 상품 생산 시간뿐 아니라 카운터 서비스 지점과 생산 지점 사이의 우회 동선 길이에도 영향을 받는다.

스테이지 클리어 조건:

```text
모든 상품 매대 해금 && 모든 상품 매대 maxLevel 달성
```

생산 슬롯 활성화 여부는 스테이지 클리어 조건에 포함하지 않는다.

각 스테이지 시작값:

| Config | Value |
|---|---:|
| startingGold | 5 |
| startingCustomers | 1 |
| startingStaff | 0 |
| startingMainCharacter | 1 |

각 스테이지의 첫 매대 해금 비용은 기본 5 Gold로 시작한다. 단, 첫 매대는 손님에게 첫 주문을 받기 전까지 해금할 수 없다. 첫 손님 주문 후 해금 가능 화살표가 표시되고, 유저는 스테이지 시작 Gold 5를 사용해 첫 매대를 열고 이후 판매 수익으로 레벨업과 추가 매대를 진행한다. 2번 이후 모든 매대는 Gold가 충분해도 1번 매대 해금 전에는 해금할 수 없다. 이 시작 Gold는 이후 전역 업그레이드로 늘릴 수 있는 후보로 둔다.

매대 해금 비용 초안:

| Stand Order | Unlock Cost |
|---:|---:|
| 1 | 5 |
| 2 | 250 |
| 3 | 12.5k |

## 4. Product Station Balance

MVP 상품 생산 시간은 스테이지가 아니라 해당 스테이지 안의 상품 순번을 기준으로 통일한다.

```text
baseProductionTimeSec = 5 + ((standOrder - 1) * 4)
```

예시:

| Stand Order | Production Time |
|---:|---:|
| 1 | 5 sec |
| 2 | 9 sec |
| 3 | 13 sec |

따라서 모든 스테이지의 첫 상품은 5초, 두 번째 상품은 9초, 세 번째 상품은 13초로 시작한다. 생산속도 업그레이드, 장비, 캐릭터, 직원 능력치가 이 시간을 줄인다.

| Product ID | Stage ID | Name | Unlock Cost | Base Price | Production Time Sec | Max Level | Max Stars | Base Slots | Max Slots | Upgrade Base Cost | Level Curve |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---:|---|
| cotton_candy | amusement_001 | 솜사탕 | 5 | 2 | 5.0 | 25 | 2 | 1 | 1 | 3 | curve_mvp_starter_25 |
| bead_icecream | amusement_002 | 구슬아이스크림 | 5 | 2 | 5.0 | 75 | 4 | 1 | 1 | 3 | curve_mvp_starter_75 |
| chicken_skewer | amusement_002 | 닭꼬치 | 250 | 320 | 9.0 | 75 | 4 | 1 | 2 | 150 | curve_mvp_second_75 |
| popcorn | amusement_003 | 팝콘 | 5 | 2 | 5.0 | 75 | 4 | 1 | 1 | 3 | curve_mvp_starter_75 |
| hotdog | amusement_003 | 핫도그 | 250 | 320 | 9.0 | 75 | 4 | 1 | 2 | 150 | curve_mvp_second_75 |
| churros | amusement_003 | 츄러스 | 12.5k | 24 | 13.0 | 75 | 4 | 1 | 2 | 36 | curve_mvp_third_75 |

## 5. Product Sale And Cost Rules

판매가는 `레벨 기본 판매가`와 `별 보너스`를 분리해서 계산한다. 별 보너스가 이미 x2를 주기 때문에, 별 달성 레벨에서 기본 판매가까지 동시에 크게 튀게 만들지 않는다.

### Level Base Sale Price

```text
levelBaseSaleGold = value from ProductLevelCurveTable
```

MVP 초반 1~9레벨은 튜토리얼 체감을 위해 `+1` 증가처럼 읽히는 구간으로 둘 수 있다. 10레벨 이후, 특히 25레벨 이후 구간은 관측값 기준으로 수익과 레벨업 비용을 각각 다른 복리 배율로 증가시킨다.

### Star Reward Multiplier

```text
starMultiplier(starCount) = 2 ^ starCount
```

### Final Sale Price

```text
salePrice =
  levelBaseSaleGold
  * starMultiplier(starCount)
  * productRewardUpgradeMultiplier
  * equipmentIncomeMultiplier
  * characterPassiveIncomeMultiplier
  * timedBoostMultiplier
  * doubleProductMultiplier
  * sGradeProductMultiplier
```

기본값:

| Multiplier | Default | Note |
|---|---:|---|
| productRewardUpgradeMultiplier | 1.0 | 스테이지 업그레이드에서 판매가격 x2 구매 시 2.0 |
| equipmentIncomeMultiplier | 1.0 | 장비 기본 수익 증가 |
| characterPassiveIncomeMultiplier | 1.0 | 캐릭터 패시브 수익 증가 |
| timedBoostMultiplier | 1.0 | 광고/상점 시간제 부스터 |
| doubleProductMultiplier | 1.0 or 2.0 | 더블 상품 발동 시 2.0 |
| sGradeProductMultiplier | 1.0 or 3.0 | S급 상품 발동 시 3.0 |

더블과 S급이 동시에 발동하면 곱연산으로 처리한다. 예를 들어 10 Gold 상품에 판매가격 x2, 더블 x2, S급 x3이 동시에 적용되면 `10 * 2 * 2 * 3 = 120 Gold`가 된다.

### Upgrade Cost

```text
upgradeCostGold = value from ProductLevelCurveTable
```

`level`은 현재 레벨이다. 예를 들어 레벨 1 상품을 레벨 2로 올릴 때 `level = 1` 비용을 사용한다.

### Observed Reference Curve

참고 게임 관측값이다. 아래 수익은 2배 수익 버프가 켜진 상태의 표시값이다.

| Level | Display Sale With x2 Boost | Sale Without Boost | Level Base Sale Before Star | Upgrade Cost | Notes |
|---:|---:|---:|---:|---:|---|
| 35 | 1.31k | 655 | 163.75 | 1.48k | 2 stars, star x4 |
| 36 | 1.41k | 705 | 176.25 | 1.78k |  |
| 37 | 1.53k | 765 | 191.25 | 2.13k |  |
| 38 | 1.65k | 825 | 206.25 | 2.56k |  |
| 39 | 1.78k | 890 | 222.5 | 3.07k |  |
| 40 | 1.93k | 965 | 241.25 | 3.68k |  |
| 41 | 2.08k | 1.04k | 260 | 4.41k |  |

관측 증가율:

| Value | Approx Growth Per Level |
|---|---:|
| Display sale | x1.08 |
| Upgrade cost | x1.20 |

따라서 25레벨 이후 기준 공식은 아래처럼 둔다.

```text
levelBaseSaleGold =
  segmentStartSaleGold * saleGrowthPerLevel ^ (level - segmentStartLevel)

upgradeCostGold =
  segmentStartUpgradeCostGold * costGrowthPerLevel ^ (level - segmentStartLevel)
```

### Second Stand Observed Curve

2번 매대는 해금 비용 250 Gold, 레벨 1 판매 수익 320 Gold, 레벨 1 업그레이드 비용 150 Gold로 관측됐다. 아래 수익은 2배 버프가 끝난 상태의 표시값이다.

| Level | Display Sale | Upgrade Cost | Notes |
|---:|---:|---:|---|
| 1 | 320 | 150 | unlocked from 250 Gold |
| 2 | 345 | 181 |  |
| 3 | 373 | 217 |  |
| 4 | 403 | 260 |  |
| 5 | 435 | 312 |  |
| 6 | 470 | 374 |  |
| 7 | 507 | 488 | cost value needs one more check |
| 8 | 548 | 538 |  |
| 9 | 592 | 645 |  |
| 10 | 1.27k | 774 | star 1, sale x2, production line +1 |
| 11 | 1.38k | 929 |  |
| 12 | 1.49k | 1.12k |  |
| 13 | 1.61k | 1.34k |  |
| 14 | 1.74k | 1.61k |  |
| 15 | 1.87k | 1.93k |  |
| 16 | 2.03k | 2.32k |  |
| 17 | 2.19k | 2.78k |  |
| 18 | 2.36k | 3.33k |  |
| 19 | 2.55k | 4.00k |  |
| 20 | 2.76k | 4.80k |  |
| 21 | 2.98k | 5.76k |  |
| 22 | 3.22k | 6.91k |  |
| 23 | 3.47k | 8.29k |  |
| 24 | 3.75k | 9.94k |  |
| 25 | 8.11k | 12.0k | star 2, sale x2 |

2번 매대 1~25레벨은 아래 공식으로 거의 맞는다.

```text
levelBaseSaleGold = 320 * 1.08 ^ (level - 1)
upgradeCostGold = 150 * 1.20 ^ (level - 1)
displaySaleGold = levelBaseSaleGold * starMultiplier(starCount)
```

### Late Theme Stage 1 Observation Memo

아래 값은 후순위 테마 또는 높은 계정 성장 상태에서 관측한 Stage 1 / 1번 매대 데이터다. 캐릭터 능력치, 테마 진행도, 전역 성장으로 인해 절대값이 커진 상태이므로 MVP 기본값으로 직접 사용하지 않는다. 대신 성장 비율, 별 구간, 클리어 보상 구조를 참고한다.

관측 컨텍스트:

| Item | Observed Value | Memo |
|---|---:|---|
| startingGold | 25k | 캐릭터/테마 성장 반영 가능성이 큼 |
| firstStandUnlockCost | 370 | MVP의 5 Gold와 직접 비교 금지 |
| maxLevel | 25 | Stage 1 기준 |
| maxStars | 2 | 10레벨, 25레벨 |
| nextStageCost | 62.7k | 스테이지 이동 비용 |
| clearReward | Common Chest x1 | MVP에서는 보상 데이터 자리만 준비 후보 |

관측값:

| Level | Display Sale | Upgrade Cost | Notes |
|---:|---:|---:|---|
| 1 | 6 | 222 | 이전 관측 |
| 2 | 7 | 280 | 이전 관측 |
| 3 | 8 | 353 |  |
| 4 | 9 | 445 |  |
| 5 | 10 | 560 |  |
| 6 | 11 | 706 |  |
| 7 | 12 | 889 |  |
| 8 | 13 | 1.12k |  |
| 9 | 14 | 1.42k |  |
| 10 | 31 | 1.78k | star 1, sale x2 |
| 11 | 34 | 2.24k |  |
| 12 | 37 | 2.83k |  |
| 13 | 40 | 3.56k |  |
| 14 | 43 | 4.48k |  |
| 15 | 46 | 5.56k |  |
| 16 | 50 | 7.12k |  |
| 17 | 54 | 8.96k |  |
| 18 | 59 | 11.3k |  |
| 19 | 63 | 14.3k |  |
| 20 | 69 | 18.0k |  |
| 21 | 74 | 22.6k |  |
| 22 | 80 | 28.5k |  |
| 23 | 86 | 35.9k |  |
| 24 | 93 | 45.2k |  |
| 25 | 202 |  | star 2, max |

관측 해석:

| Range | Sale Growth | Upgrade Cost Growth | Memo |
|---|---:|---:|---|
| 1~9 | about x1.11 | about x1.26 | 초반 튜토리얼 구간은 비용이 빠르게 상승 |
| 10~24 | about x1.08 | about x1.26 | star 1 이후 수익 성장률이 안정화 |
| 25 | x2 star bonus | max | star 2 달성, Stage 1 클리어 조건 충족 |

설계 메모:

- 후순위 테마의 절대값은 `themeScale`, `stageScale`, 캐릭터/장비/전역 성장의 결과로 본다.
- MVP 기본값은 낮게 유지하고, 후반 테마는 스케일 배율 또는 테이블 override로 키운다.
- 모든 레벨 값을 수동 입력하지 않는다. 매대별 `unlockCost`, `level1Sale`, `level1UpgradeCost`, `saleGrowth`, `costGrowth`, `starMilestones`, `maxLevel`만 있으면 곡선 생성이 가능하다.
- Stage clear reward는 스테이지 데이터의 보상 테이블로 분리하는 후보로 둔다.

### MVP Starter Curve Draft

첫 상품용 기준 곡선이다. 별이 붙는 레벨은 별 배율이 상승하는 지점이므로 `levelBaseSaleGold`를 직전 구간 끝값에 가깝게 두고, 표시 판매가가 별 배율로 크게 오르는 느낌을 만든다.

| Curve ID | Level Range | Level Base Sale Rule | Upgrade Cost Rule | Star Note |
|---|---|---|---|---|
| curve_mvp_starter_25 | 1~9 | `2 + (level - 1)` | `3 + (level - 1)` | no star |
| curve_mvp_starter_25 | 10 | `10` | `16` | star 1, displayed sale x2 |
| curve_mvp_starter_25 | 11~24 | `24 * 1.08 ^ (level - 11)` | `22 * 1.20 ^ (level - 11)` | star 1 유지 |
| curve_mvp_starter_25 | 25 | `76` | `0` | star 2, max |

Stage 2~3의 75레벨 곡선은 위 흐름을 4별까지 확장한다.

| Curve ID | Level Range | Level Base Sale Rule | Upgrade Cost Rule | Star Note |
|---|---|---|---|---|
| curve_mvp_starter_75 | 1~9 | `2 + (level - 1)` | `3 + (level - 1)` | no star |
| curve_mvp_starter_75 | 10 | `10` | `16` | star 1, displayed sale x2 |
| curve_mvp_starter_75 | 11~24 | `24 * 1.08 ^ (level - 11)` | `22 * 1.20 ^ (level - 11)` | star 1 유지 |
| curve_mvp_starter_75 | 25 | `76` | `240` | star 2, displayed sale x4 |
| curve_mvp_starter_75 | 26~49 | `82 * 1.08 ^ (level - 26)` | `288 * 1.20 ^ (level - 26)` | star 2 유지, Lv35~41 관측값 기준 |
| curve_mvp_starter_75 | 50 | `520` | `22.9k` | star 3, displayed sale x8 |
| curve_mvp_starter_75 | 51~74 | `562 * 1.08 ^ (level - 51)` | `27.5k * 1.20 ^ (level - 51)` | star 3 유지, 임시 외삽 |
| curve_mvp_starter_75 | 75 | `3.57k` | `0` | star 4, max |
| curve_mvp_second_75 | 1~74 | `320 * 1.08 ^ (level - 1)` | `150 * 1.20 ^ (level - 1)` | 1~25 관측값 기준, 26~74 임시 외삽 |
| curve_mvp_second_75 | 75 | `95.2k` | `0` | star 4, max, 임시 외삽 |

`curve_mvp_second_75`는 2번 매대 관측값을 기준으로 한다. `curve_mvp_third_75`는 아직 관측값이 부족하므로 임시값이며, 3번 매대 실제 데이터 수집 후 교체한다.

## 6. Product Star Milestones

별 마일스톤은 상품별로 정의한다. 별을 획득하면 해당 상품 보상이 기존 대비 2배 증가한다.

공통 별 기준:

| Star | Required Level |
|---:|---:|
| 1 | 10 |
| 2 | 25 |
| 3 | 50 |
| 4 | 75 |
| 5 | 100 |
| 6 | 130 |
| 7 | 170 |
| 8 | 200 |
| 9 | 235 |
| 10 | 270 |
| 11 | 310 |
| 12 | 350 |

상품은 스테이지와 상품별 `maxStarCount`까지만 위 기준을 사용한다.

초기 기준:

| Product ID | Star | Required Level | Reward Multiplier On Reach | Diamond Reward | Shows Slot Availability |
|---|---:|---:|---:|---:|---|
| cotton_candy | 1 | 10 | x2 | 1 | none |
| cotton_candy | 2 | 25 | x2 | 1 |  |
| bead_icecream | 1 | 10 | x2 | 1 | none |
| bead_icecream | 2 | 25 | x2 | 1 | none |
| bead_icecream | 3 | 50 | x2 | 1 | none |
| bead_icecream | 4 | 75 | x2 | 1 | none |
| chicken_skewer | 1 | 10 | x2 | 1 | slot_2 |
| chicken_skewer | 2 | 25 | x2 | 1 | none |
| chicken_skewer | 3 | 50 | x2 | 1 | none |
| chicken_skewer | 4 | 75 | x2 | 1 | none |
| popcorn | 1 | 10 | x2 | 1 | none |
| popcorn | 2 | 25 | x2 | 1 | none |
| popcorn | 3 | 50 | x2 | 1 | none |
| popcorn | 4 | 75 | x2 | 1 | slot_3 |
| hotdog | 1 | 10 | x2 | 1 | slot_2 |
| hotdog | 2 | 25 | x2 | 1 | none |
| hotdog | 3 | 50 | x2 | 1 | none |
| hotdog | 4 | 75 | x2 | 1 | none |
| churros | 1 | 10 | x2 | 1 | slot_2 |
| churros | 2 | 25 | x2 | 1 | none |
| churros | 3 | 50 | x2 | 1 | none |
| churros | 4 | 75 | x2 | 1 | none |

후반 스테이지는 같은 공통 기준을 사용하되, 상품별 `maxLevel`과 `maxStarCount`를 높여 12별까지 확장한다.

Stage 1 솜사탕 별 진행:

| Level Range | UI State |
|---|---|
| 1~9 | 별 없음, 1번째 별 그래프 진행 |
| 10 | 별 1개 활성화, Diamond +1, 그래프 초기화 |
| 11~24 | 별 1개 유지, 2번째 별 그래프 진행 |
| 25 | 별 2개 활성화, Diamond +1, 최대 레벨 표시 |

## 7. Production Slot Balance

생산 슬롯은 한 번에 한 에이전트가 점유할 수 있는 작업 위치다. 슬롯이 늘어나면 같은 상품을 병렬 생산할 수 있다.

| Slot | Visibility Rule | Activation Rule |
|---:|---|---|
| 1 | 상품 매대 해금 후 생산 테이블 활성화칸 표시 | 무료 터치 활성화 |
| 2 | 상품별 별 조건 충족 후 활성화칸 표시 | 무료 터치 활성화 |
| 3+ | 중후반 스테이지의 별 조건 충족 후 표시 | 무료 터치 활성화 |

생산 슬롯은 Gold 구매 대상이 아니다. 슬롯을 표시만 해두고 활성화하지 않는 선택도 가능해야 한다.

1번 매대는 추가 생산 라인을 제공하지 않는다. 2번 매대와 3번 매대는 1별, 즉 10레벨 달성 후 두 번째 생산 슬롯 활성화칸이 표시되고 무료 터치로 활성화된다. 추가 생산 슬롯 활성화 여부는 유저 선택이며 스테이지 클리어 조건에 포함하지 않는다.

직원/주인공 작업 배정은 생산 슬롯 예약을 기준으로 판단한다. 앞선 주문의 상품 슬롯이 모두 예약 또는 점유 중이면, 작업자는 그 상품 앞에서 기다리지 않고 현재 생산 가능한 다른 상품 작업을 가져간다. 이 규칙은 생산 슬롯 수와 직원 수를 늘렸을 때 병렬 생산 체감이 사라지지 않게 하기 위한 기본 전제다.

## 8. Customer Flow

| Config | Initial Value | Description |
|---|---:|---|
| nextCustomerSpawnDelaySec | 0.2 | maxCustomers=1 기본 상태에서 손님 퇴장 후 다음 손님이 등장하기까지의 짧은 지연 |
| maxCustomers | 1 | 손님 +1 전에는 동시에 1명만 존재 |
| customerPickProductDurationSec | 0.6 | 손님이 주문 위치에 도착한 뒤 주문을 확정하기까지의 시간 |
| customerWaitRule | UntilProductDelivered | 주문 후 상품 수령 전까지 매장 안에서 대기 |
| generalistStaffWorkSpeedMultiplier | 1.0 | 범용 직원 레벨 1 작업속도 배율 |
| leaveDelaySec | 0.2 | 상품 수령 후 퇴장 전 짧은 지연 |

## 9. Main Character Agent Baseline

| Config | Initial Value | Description |
|---|---:|---|
| moveTimePerTileSec | 1.0 | 매대 1칸 기준 이동 시간 |
| workSpeedMultiplier | 1.0 | 상품 생산 시간에 곱하는 기본 작업속도 배율 |
| deliveryHandoffTimeSec | 0.5 | 손님에게 상품을 건네는 짧은 처리 시간 |
| orderTakeTimeSec | 0.4 | 주문 접수 시간 |
| baseIncomeBonusPct | 0 | 장비 없는 기본 수익 보너스 |
| sGradeChancePct | 0 | MVP 기본 S급 상품 확률 |

상품 생산 시간은 메인 캐릭터가 아니라 `ProductTable.baseProductionTimeSec`를 기준으로 한다. 메인 캐릭터, 직원, 장비, 캐릭터 패시브의 생산속도 증가는 이 기준 시간을 줄이는 배율로 적용한다.

## 10. Customer Purchase Rule

MVP 기본 규칙:

```text
첫 손님 주문 가능 상품 = 스테이지 첫 상품
일반 주문 가능 상품 = 매대 해금됨 && 생산 슬롯 활성화됨
한 주문의 상품 종류 수 = uniform random integer between 1 and min(2, 주문 가능 상품 수)
상품 선택 = 주문 가능 상품 풀에서 균등 랜덤, 중복 없는 선택
상품별 주문 수량 = uniform random integer between 1 and 3
```

첫 손님은 첫 상품을 주문해 첫 매대 해금칸 점멸을 유도한다. 2번 이후 모든 매대는 1번 매대가 해금된 뒤에만 해금 가능 상태가 된다. 두 번째 이후 상품은 해금되지 않은 상태에서 주문 대상이 되지 않는다. 주문 가능한 상품이 늘어나면 상품 선택 확률은 열린 상품 수 기준으로 다시 균등 분배된다. 손님 +1 업그레이드 전에는 `one-in-one-out`으로 처리한다. 손님 1명이 입장, 주문, 상품 수령, 퇴장을 완료한 뒤 다음 손님이 등장한다. 상품을 기다리는 동안의 대기는 줄서기/대기열이 아니라 현재 주문 손님이 매장 안에 머무르는 상태다.

손님 +1 이후 주문 위치는 카운터 앞 일렬 공간을 우선 사용한다. 좌우 공간이 있으면 좌우로 확장하고, 카운터 앞 공간이 부족하면 뒤쪽 줄로 fallback한다.

고가 상품 구매 가중치는 MVP 이후에 추가한다.

## 11. Staff Balance

| Staff ID | Name | Stage ID | Add Source | Move Time Per Tile | Work Speed Multiplier | Min Production Time | Max Level | Upgrade Base Cost | Work Time Multiplier Per Level | Cost Growth |
|---|---|---|---|---:|---:|---:|---:|---:|---:|---:|
| generalist_staff | 범용 직원 | current_stage | Upgrade List | 1.0 | 1.0 | 0.6 | 50 | 100 | 0.97 | 1.15 |

MVP의 범용 직원은 주문 접수, 생산, 전달을 모두 수행할 수 있다. 맵이 카운터/생산 구역으로 분리되는 스테이지부터 서비스 직원과 생산 직원으로 나눌 수 있다.

작업자는 주문 접수와 포장/생산을 분리해서 처리한다. 주문 접수는 빈 작업자 1명이 손님 1명의 전체 주문을 받는다. 주문 접수 후에는 주문 안의 상품을 상품 1개 단위 작업으로 쪼개고, 빈 메인 캐릭터 또는 직원이 하나씩 처리한다. 생산된 상품은 반드시 해당 주문을 낸 손님에게 전달한다.

기본 운반/포장 수량은 작업자 1회당 상품 1개다. 장비나 자원 업그레이드로 `1회 포장 시 2개 획득 확률`을 추가할 수 있지만 최대 수량은 2개까지다. Gold는 주문 전체 완료가 아니라 상품 1개 전달마다 즉시 지급한다.

## 12. Staff Formulas

### Effective Production Time

```text
workerWorkTimeMultiplier(level) = workTimeMultiplierPerLevel ^ (level - 1)
productionSpeedMultiplier = 1 + (productionSpeedBonusPct / 100)

effectiveProductionTime =
  max(
    minProductionTimeSec,
    product.baseProductionTimeSec
      * workerWorkTimeMultiplier(level)
      / productionSpeedMultiplier
  )
```

메인 캐릭터와 직원 모두 `ProductTable.baseProductionTimeSec`를 기준으로 생산한다. 직원 레벨, 스테이지 업그레이드, 장비 옵션, 캐릭터 패시브는 이 기준 시간을 줄이는 배율로 적용한다.

### Staff Upgrade Cost

```text
upgradeCost(level) = round(upgradeBaseCost * costGrowth ^ (level - 1))
```

## 13. Stage Upgrade List Balance

하단 메뉴 업그레이드 리스트 예시:

| Upgrade ID | Display | Effect | Cost |
|---|---|---|---:|
| cotton_candy_speed_001 | 솜사탕 생산속도 증가 | production speed x2 | 50 |
| cotton_candy_reward_001 | 솜사탕 판매가격 증가 | sale price x2 | 1.8k |
| customer_count_001 | 손님 증가 | max customers +1 | 1.2m |

Stage 1 업그레이드 리스트에는 직원 +1을 넣지 않는다. 직원 +1은 Stage 2 이후부터 도입하는 후보로 둔다.

## 14. Upgrade Input Balance

매대 레벨업은 단일 터치와 길게 누르기 반복 입력을 지원한다.

초기 기준:

| Input | Value |
|---|---:|
| holdStartDelaySec | 0.35 |
| initialRepeatIntervalSec | 0.12 |
| minRepeatIntervalSec | 0.02 |
| timeToMinIntervalSec | 2.0 |
| repeatScope | product station level up only |

반복 레벨업은 매대 전용으로 시작한다. 스테이지 업그레이드, 직원 +1, 손님 +1, 생산 슬롯 활성화는 실수 방지를 위해 단일 터치를 기본으로 한다.

반복 레벨업 중 레벨, 판매 보상, 레벨업 비용, 별 진행도는 즉시 갱신한다. 별 달성이나 Diamond 지급 토스트가 발생해도 반복 레벨업은 멈추지 않는다. 새 토스트가 발생하면 기존 토스트를 즉시 대체하고 타이머를 다시 시작한다.

## 15. Number Formatting

Gold 표기는 짧은 축약 단위를 사용한다.

MVP 내부 계산은 `double` 기반 `CurrencyAmount` 규칙으로 시작한다. 후반 수치에서 정밀도 문제가 보이면 BigNumber 구조로 교체한다.

| Range | Suffix Example |
|---|---|
| 1 - 999 | 999 |
| 1,000 | 1k |
| 1,000,000 | 1m |
| 1,000,000,000 | 1b |
| 1,000,000,000,000 | 1t |
| Beyond | aa, ab, ac... |

## 16. Stage Capacity Upgrade Notes

MVP에는 별도의 매장 레벨을 두지 않는다. 손님 수, 직원 수, 다음 스테이지 이동은 현재 스테이지의 업그레이드 리스트와 `StageData`로 관리한다.

| Growth Type | Data Source | Effect | MVP Start |
|---|---|---|---:|
| Customer Capacity | StageUpgradeTable | 현재 스테이지 최대 손님 수 +1 | 1 |
| Staff Count | StageUpgradeTable | 현재 스테이지 직원 수 +1 | 0 |
| Next Stage Move | max(final upgrade cost) x 1.5 | 조건 충족 후 Gold 지불로 이동 | 산출값 |

## 17. Agent Automation

MVP는 탭 부스트가 없고, 캐릭터 이동은 자동 에이전트가 수행한다. 직원이 없어도 메인 캐릭터 혼자 기본 운영이 가능해야 한다.

권장 기본값:

| System | Before Hire | After Hire |
|---|---|---|
| Order/Delivery | 메인 캐릭터 단독 처리 | 범용 직원이 병렬 처리 |
| Production | 상품별 기본 생산 시간 사용 | 범용 직원이 빈 슬롯에서 병렬 생산 |

직원은 "게임 진행 필수 조건"이 아니라 "자동 에이전트 수와 속도를 늘리는 성장"으로 작동해야 한다.

직원 +1 업그레이드를 구매하면 해당 스테이지에서 주문/생산/전달 작업을 자동으로 수행한다.

직원 보너스 적용:

| Bonus Source | Scope | Example |
|---|---|---|
| Equipment Staff Bonus | Global or Stage | 장비 옵션에 따라 전체 직원 또는 특정 스테이지 직원 강화 |
| Main Upgrade Staff Bonus | Global | 전체 직원 이동속도 증가 |
| Stage Upgrade Staff Bonus | Stage Only | 현재 스테이지 직원 생산속도 증가 |

## 18. Equipment Balance Placeholder

장비 시스템은 장기 핵심 성장축이지만 MVP 세부 밸런스는 확정하지 않는다.

MVP에서 UI 기준으로 먼저 고려할 장비 슬롯:

| Slot | Example |
|---|---|
| Hat | 점원 모자, 캐릭터 머리 장식 |
| Top | 유니폼, 셔츠 |
| Bottom | 바지, 앞치마 하의 |
| Tool | 먼지털이개, 바코드 스캐너, 카드 단말기 |

추후 슬롯은 최대 4개까지 추가할 수 있다.

초기 방향:

| Grade | Base Income Growth | Sub Option Count | Option Pool Direction |
|---|---|---:|---|
| Normal | lowest | 0 | 기본 수익 증가만 |
| Rare | low | 0 | 기본 수익 증가만 |
| Unique | medium | 1 | 이동속도, 작업속도 중심 |
| Epic | high | 1 | 수익, 더블, S상품 확률 진입 |
| Legendary | very high | 2 | 주인공/직원 혼합, 더블, S상품, 수익 |
| Mythic | highest | 3 | 전체 수익, 전체 작업속도, S상품 보상, 이벤트 특수 옵션 |

장비 기본 수익 증가는 레벨업과 등급업으로 성장한다. 서브 옵션의 종류와 수치는 레벨업으로 변하지 않는다.

수익 중첩 공식:

```text
finalSaleGold =
  baseSaleGold
  * equipmentBaseIncomeMultiplier
  * incomeOptionMultiplier
  * doubleMultiplier
  * sGradeMultiplier
```

중첩 예시:

| Step | Value |
|---|---:|
| baseSaleGold | 10 |
| equipmentBaseIncomeMultiplier | x2 |
| doubleMultiplier | x2 |
| sGradeMultiplier | x3 |
| finalSaleGold | 120 |

더블과 S상품은 동시에 발동할 수 있다.

부위별 옵션 방향:

| Slot | Primary Direction | Secondary Direction |
|---|---|---|
| Tool | 주인공 수익, 주인공 더블, 주인공 S상품 | 주인공 작업속도, 주인공 즉시생산 |
| Hat | 전체 직원 작업속도, 전체 직원 이동속도 | 직원 더블, 직원 S상품, 직원 즉시생산 |
| Top | 전체 수익, 전체 작업자 작업속도 | 주인공/직원 수익 혼합, 손님 주문속도 |
| Bottom | 주인공 이동속도, 전체 직원 이동속도 | 전체 작업자 이동속도, 손님 이동속도 |

용어 기준:

| Term | Meaning |
|---|---|
| MainCharacter | 주인공만 |
| Staff | 해금/고용된 직원 전체, 주인공 제외 |
| AllWorkers | 주인공과 직원 전체 |
| Customer | 손님 |

옵션 타입 후보:

| Option Type | Target | Meaning |
|---|---|---|
| IncomeMultiplier | MainCharacter, Staff, AllWorkers | 판매 수익 배율 |
| MoveSpeedPct | MainCharacter, Staff, AllWorkers | 이동속도 증가 |
| WorkSpeedPct | MainCharacter, Staff, AllWorkers | 주문/생산/전달 작업속도 증가 |
| DoubleChancePct | MainCharacter, Staff, AllWorkers | 판매 시 더블 발동 확률 |
| SGradeChancePct | MainCharacter, Staff, AllWorkers | S상품 발생 확률 |
| SGradeRewardMultiplier | MainCharacter, Staff, AllWorkers | S상품 보상 배율 |
| InstantProductionChancePct | MainCharacter, Staff, AllWorkers | 생산 시간을 건너뛰고 즉시 생산할 확률 |
| CustomerMoveSpeedPct | Customer | 손님 입장/퇴장 이동속도 증가 |
| CustomerOrderSpeedPct | Customer | 손님 주문 대기 시간 감소 |
| ProductCategoryIncomeMultiplier | ProductCategory | 특정 상품군 수익 증가 |
| StageIncomeMultiplier | Stage | 특정 스테이지 수익 증가 |

즉시생산은 생산 시작 시점에 판정한다. 발동하면 해당 상품의 생산 시간만 0으로 처리하고, 이동과 전달 시간은 그대로 적용한다.

장비 성장 기준:

| Rule | Baseline |
|---|---|
| maxEquipmentLevel | 30 |
| levelUpMaterial | same grade or lower grade equipment |
| gradeUpMaterialBeforeLegendary | two level-30 duplicate items with same slot and same equipment family |
| legendaryCraftMaterial | legendary blueprint + 5 lower-grade material items |
| mythicSource | event chest or event crafting |
| duplicateItems | allowed |
| dismantleReward | Star Dust |
| starDustUse | main character level up in character status view |

후속 결정 필요:

- 장비 레벨업 경험치
- 등급별 재료 경험치
- 등급별 옵션 개수와 고유 옵션 수
- 직원 보너스 옵션의 최대치
- S급 상품 확률과 보상 배율

## 19. Chest Balance Placeholder

상자는 Diamond 사용처 후보이며 MVP에서는 실제 구매를 구현하지 않는다.

| Chest | Diamond Cost | Item Count | Normal | Rare | Unique | Legendary Blueprint | Mythic Blueprint | Note |
|---|---:|---:|---:|---:|---:|---:|---:|---|
| Normal | 30 | 6 | 100% | 0% | 0% | 0% | 0% | 일반 아이템만 |
| Rare | 80 | 6 | 80% | 20% | 0% | 0% | 0% |  |
| Unique | 150 | 6 | 50% | 42% | 8% | 0% | 0% |  |
| Legendary | 300 | 6 | 40% | 40% | 18% | 2% | 0% | Legendary는 완제품이 아니라 제작 문서 |
| Event | TBD | 6 | TBD | TBD | TBD | TBD | TBD | Mythic 제작 문서 가능 |

후속 결정 필요:

- Legendary 제작 문서별 필요한 하위 재료 구성을 장비마다 다르게 둘지
- Mythic 제작 문서를 이벤트 상자에서만 줄지

### Chest Pity Baseline

천장 시스템은 상자를 여러 번 열었는데도 핵심 보상이 나오지 않는 유저를 보호하는 보장 규칙이다. Legendary Chest에는 Hard Pity를 적용한다.

기본 규칙:

| Config | Value |
|---|---|
| targetChest | Legendary Chest |
| hardPityCount | 50 |
| targetReward | random Legendary Blueprint |
| counterIncrease | Legendary Chest opened and no Legendary Blueprint gained |
| counterReset | Legendary Blueprint gained by random drop or pity reward |
| softPity | none for first implementation |

동작 예시:

```text
전설 상자를 49회 열 때까지 Legendary Blueprint가 나오지 않음
-> 50번째 전설 상자는 랜덤 Legendary Blueprint 1개를 보장
-> Legendary Blueprint 획득 후 카운터 초기화
```

종류:

| Type | Meaning |
|---|---|
| Hard Pity | 정해진 횟수에 도달하면 보상 확정 |
| Soft Pity | 일정 횟수 이후부터 목표 보상 확률 점진 증가 |

상자 구매 UI에는 Legendary Chest 천장 진행률을 표시한다.

| UI | Rule |
|---|---|
| Progress Count | currentPityCount / 50 |
| Progress Percent | currentPityCount / 50 * 100 |
| Progress Bar | 전설 상자 구매 항목에 가로 그래프 표시 |
| Reset Timing | Legendary Blueprint 획득 직후 0/50으로 초기화 |

MVP에서는 구현하지 않는다. 상자 구매를 실제로 넣는 패치에서 위 규칙을 따른다.

## 20. Shop Timed Boost Balance Placeholder

상점에는 상자 외에도 시간제 수익 부스터를 둘 수 있다. MVP에서는 구현하지 않고, 상점 기능을 여는 패치에서 적용한다.

초기 후보:

| Boost ID | Display | Duration | Multiplier | Cost Type | Cost |
|---|---|---:|---:|---|---:|
| income_2x_15m | 15분 수익 2배 | 900 sec | x2 | Diamond | TBD |
| income_5x_5m | 5분 수익 5배 | 300 sec | x5 | Diamond | TBD |

중첩 규칙:

| Case | Rule |
|---|---|
| Different boost IDs active together | multipliers multiply |
| Same boost ID purchased while active | extend remaining duration |
| Boost expired | remove that multiplier |

공식:

```text
activeShopBoostMultiplier = product(active timed boost multipliers)
finalSaleGold = baseSaleGold * activeShopBoostMultiplier * otherMultipliers
```

예시:

```text
baseSaleGold = 10
income_2x_15m active = x2
income_5x_5m active = x5

finalSaleGold = 10 * 2 * 5 = 100
```

후속 결정 필요:

- 각 부스터의 Diamond 가격
- 광고 보상 부스터와 상점 구매 부스터를 같은 슬롯으로 볼지
- 오프라인 보상에도 시간제 부스터를 적용할지

## 21. Character Roster Balance Placeholder

캐릭터는 Diamond 사용처 후보지만 장비보다 약한 성장축으로 둔다. 캐릭터의 목적은 강한 수익 배율이 아니라 외형, 수집, 낮은 편의 패시브다.

초기 가격 후보:

| Grade | Diamond Cost | Passive Direction | Example Value |
|---|---:|---|---:|
| Normal | 0 | 기본 캐릭터 | 0 |
| Rare | 500 | 주인공 이동속도 | +5% |
| Unique | 1200 | 주인공 작업속도 | +5% |
| Epic | 2600 | 이동/작업 혼합 | +8% / +5% |
| Legendary | 4000 | 분해 보상, 공용 스킬 | +1% / +1% |
| Mythic | TBD | 이벤트/특수 유틸 | +2% / +2% |

밸런스 원칙:

| Rule | Reason |
|---|---|
| 캐릭터에는 큰 수익 배율을 넣지 않는다 | 장비와 상점 부스터의 역할을 침범하지 않기 위해 |
| 이동/작업속도는 초반 +5~12% 범위에서 시작한다 | 체감은 주되 필수 과금처럼 느껴지지 않게 하기 위해 |
| Legendary 이상은 유틸 보너스를 중심으로 둔다 | 진행 밸런스 파괴를 줄이기 위해 |
| 분해 보너스는 Star Dust 획득량 +1~2%로 시작한다 | 장기 성장 보조 효과로 충분하기 때문 |
| 공용 스킬 효과 보정은 +1~2%로 시작한다 | 금고/전역 업그레이드 밸런스를 크게 흔들지 않기 위해 |

광고 Diamond 보상 후보:

| Reward | Amount | Daily Limit | Daily Max |
|---|---:|---:|---:|
| Diamond Ad | 50 | 5 | 250 |

광고만으로 캐릭터를 구매할 때의 대략적인 목표:

| Character Grade | Cost | Ad-Only Time |
|---|---:|---:|
| Rare | 500 | 2 days |
| Unique | 1200 | 5 days |
| Epic | 2600 | 11 days |
| Legendary | 4000 | 16 days |

후속 결정 필요:

- 캐릭터를 계정 단위로 둘지, 테마별 보너스를 가진 캐릭터를 따로 둘지
- 캐릭터 레벨업 Star Dust 요구량과 레벨별 효과 증가폭을 어떻게 둘지
- 캐릭터 외형만 다른 스킨을 별도 상품으로 둘지

## 22. Global Upgrade Balance Placeholder

금고 또는 전역 업그레이드는 Diamond 사용처 후보다. MVP에서는 실제 구매를 구현하지 않고, 밸런스 방향만 남긴다.

예시:

| Upgrade ID | Effect | Level 1 | Level 2 | Level 3 |
|---|---|---:|---:|---:|
| ad_boost_duration | 광고 부스트 지속시간 | 5 min | 6 min | 7 min |
| ad_boost_multiplier | 광고 Gold 배율 | x2 | x2.5 | x3 |
| offline_efficiency | 오프라인 보상 효율 | 25% | 30% | 35% |
| starting_gold_bonus | 새 스테이지 시작 Gold 보너스 | 0 | TBD | TBD |

후속 결정 필요:

- 업그레이드 비용을 Diamond로만 할지, 상자/이벤트 보상과 섞을지
- 광고 지속시간과 광고 배율을 동시에 강화 가능하게 둘지
- 금고 업그레이드가 스테이지 진행 속도를 너무 빠르게 만들지

## 23. Offline Reward

| Config | Value |
|---|---:|
| minOfflineSec | 60 |
| maxOfflineSec | 10,800 |
| offlineEfficiency | 0.25 |

계산:

```text
offlineGold = estimatedGoldPerSec * offlineSec * offlineEfficiency
```

`estimatedGoldPerSec`은 현재 상품 판매가, 손님 퇴장 후 다음 손님 등장 지연, 주문/전달 처리 속도, 해금 상품 수를 기반으로 단순 추정한다.

## 24. Derived Progression Timing

MVP는 첫 판매, 첫 레벨업, 첫 별, 첫 직원, 첫 생산 슬롯, 스테이지 클리어까지의 시간을 고정 목표로 강제하지 않는다. 진행 시간은 아래 값들의 조합으로 자연스럽게 결정된다.

| Source | Effect |
|---|---|
| Product production time | 상품 순번별 5초, 9초, 13초 |
| Worker movement time | 매대 1칸 이동 1.0초 |
| Customer flow | 손님 퇴장 후 다음 손님 등장 지연과 최대 손님 수 |
| Upgrade cost | 상품/스테이지 업그레이드 비용 |
| Income growth | 상품 레벨, 별 보너스, 장비/캐릭터/부스터 보너스 |
| Parallelism | 생산 슬롯, 직원 수, 손님 수 |

플레이 테스트에서는 위 규칙으로 실제 시간이 어떻게 나오는지 관찰하고, 목표 감각에 맞지 않으면 생산 시간보다 가격, 비용, 다음 손님 등장 지연, 업그레이드 효과를 먼저 조정한다.

## 25. Tuning Notes

플레이 테스트에서 확인할 항목:

- 손님이 상품을 기다리는 시간이 너무 길어 답답하지 않은가
- 생산 대기 시간이 너무 길지 않은가
- 첫 판매, 첫 레벨업, 첫 별 획득 시점이 자연스럽고 보상감 있게 느껴지는가
- Diamond +1 지급이 헤더에서 즉시 인지되는가
- 추가 생산 슬롯을 열었을 때 병렬 생산 체감이 분명한가
- 메인 캐릭터와 직원 이동 동선이 보기 좋고 답답하지 않은가
- 직원 +1 구매까지 시간이 너무 길거나 너무 빠르지 않은가
- 업그레이드 비용이 너무 급격히 오르지 않는가
- Stage 1의 상품 1개 루프가 충분히 이해하기 쉬운가
- Stage 2 이후 여러 매대가 모두 의미 있게 수익에 기여하는가
- 10분 플레이 후 매장이 충분히 바빠 보이는가
