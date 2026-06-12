# Malltopia MVP Specification

Version: 0.1  
Status: Implementation reference  

## 1. Goal

MVP의 목표는 Stage 1 동네 구멍가게에서 다음 루프가 안정적으로 반복되는 상태를 만드는 것이다.

```text
손님 입장 -> 상품 구매 -> 계산 -> Gold 획득 -> 업그레이드 -> 손님 증가 -> 수익 증가
```

MVP는 재미 검증용 수직 슬라이스다. 전체 게임의 모든 시스템을 구현하지 않는다.

## 2. Scene

MVP는 단일 메인 씬으로 구성한다.

권장 씬 이름:

- `MainStore`

씬 구성 요소:

- 입구 지점
- 출구 지점
- 상품 진열대 3개
- 계산대 1개
- 직원 대기 지점
- 손님 이동 영역
- UI Canvas
- GameManager
- SaveManager

## 3. Store Layout

Stage 1 기본 배치:

| Object | Count | Description |
|---|---:|---|
| Entrance | 1 | 손님 생성 위치 |
| Exit | 1 | 손님 퇴장 위치 |
| Snack Shelf | 1 | 기본 해금 상품 |
| Drink Shelf | 1 | Gold로 해금 |
| Ramen Shelf | 1 | Gold로 해금 |
| Checkout Counter | 1 | 계산 처리 위치 |
| Staff Waiting Point | 1 | 직원 기본 대기 위치 |

## 4. MVP Entities

### Customer

일반 손님 1종만 구현한다.

상태:

- Spawned
- FindingProduct
- MovingToShelf
- PickingProduct
- MovingToCheckout
- WaitingForCheckout
- Paying
- Leaving

### Product Shelf

상품 진열대는 상품 재고를 가진다.

상태:

- Locked
- Available
- OutOfStock
- Restocking

### Cashier

계산원은 계산대에 대기 중인 손님을 순서대로 처리한다.

계산원이 없어도 계산대는 기본 처리 시간으로 자동 결제한다. 계산원은 결제 처리 시간을 줄이는 업그레이드 요소다. MVP에서는 계산원 1명만 고용 가능해도 충분하다.

### Restocker

진열 직원은 재고가 부족한 진열대를 찾아 재입고한다.

진열 직원이 없어도 진열대는 상품별 기본 재입고 시간으로 자동 보충된다. 진열 직원은 재고 부족 시간을 줄이는 업그레이드 요소다. MVP에서는 진열 직원 1명만 고용 가능해도 충분하다.

## 5. Functional Requirements

| ID | Requirement | MVP Rule |
|---|---|---|
| FR-001 | 손님 자동 생성 | 현재 손님 수가 최대치보다 낮으면 일정 주기로 생성 |
| FR-002 | 상품 선택 | 해금되어 있고 재고가 있는 상품 중 하나 선택 |
| FR-003 | 상품 구매 | 손님이 진열대에 도착하면 재고 1 감소 |
| FR-004 | 계산 대기 | 구매 완료 손님은 계산대 대기열에 들어감 |
| FR-005 | 계산 처리 | 계산대는 기본 속도로 결제하고, 계산원 고용 시 더 빠르게 처리 |
| FR-006 | Gold 획득 | 결제 완료 시 상품 판매가만큼 Gold 증가 |
| FR-007 | 상품 해금 | Gold를 지불하고 잠긴 상품 해금 |
| FR-008 | 상품 업그레이드 | Gold를 지불하고 상품 레벨 증가 |
| FR-009 | 직원 고용 | Gold를 지불하고 계산원/진열 직원 고용 |
| FR-010 | 직원 업그레이드 | Gold를 지불하고 직원 효율 증가 |
| FR-011 | 매장 업그레이드 | Gold를 지불하고 최대 손님 수/입장 주기 개선 |
| FR-012 | 저장 | Gold, 해금 상태, 레벨, 직원 상태 저장 |
| FR-013 | 오프라인 보상 | 마지막 저장 시간 기준으로 보상 계산 |
| FR-014 | 재입고 | 진열대는 기본 재입고를 수행하고, 진열 직원 고용 시 더 빠르게 보충 |

## 6. UI Requirements

### Main HUD

필수 표시:

- 현재 Gold
- 현재 Stage 이름
- 설정 버튼

### Product Panel

각 상품별 표시:

- 상품 이름
- 현재 레벨
- 현재 판매가
- 업그레이드 비용
- 업그레이드 버튼
- 잠금 상태일 경우 해금 비용

### Staff Panel

직원별 표시:

- 직원 이름
- 고용 여부
- 현재 레벨
- 현재 효율
- 고용 또는 업그레이드 비용

### Store Upgrade Panel

표시:

- 최대 손님 수
- 손님 입장 주기
- 다음 업그레이드 비용

### Offline Reward Popup

재접속 시 표시:

- 오프라인 시간
- 획득 Gold
- 수령 버튼

## 7. Save Requirements

저장 대상:

- 현재 Gold
- 현재 Stage ID
- 상품 해금 여부
- 상품 레벨
- 직원 고용 여부
- 직원 레벨
- 매장 업그레이드 레벨
- 마지막 저장 시각
- 누적 플레이 시간

MVP 저장 방식:

- 로컬 JSON 파일
- PlayerPrefs는 MVP 저장 데이터에 사용하지 않음
- 출시 전에는 변조 방지 또는 서버 검증 검토

## 8. Offline Reward Requirements

조건:

- 앱 시작 시 마지막 저장 시각이 존재해야 한다.
- 오프라인 시간이 60초 이상일 때만 팝업을 표시한다.
- 최대 인정 시간은 3시간이다.

계산:

```text
effectiveOfflineSec = min(offlineSec, 10800)
reward = estimatedGoldPerSec * effectiveOfflineSec * 0.25
```

`estimatedGoldPerSec`은 MVP에서는 현재 상품 판매가, 손님 생성 주기, 계산 속도, 해금 상품 수를 기반으로 단순 추정한다.

## 9. MVP Exclusions

MVP에서는 다음 기능을 구현하지 않는다.

- 탭 부스트
- 수동 재입고
- 수동 계산
- VIP 손님
- 소비왕
- 단체 고객
- 상자
- 카드 장비
- 이벤트
- 실제 IAP
- 실제 광고 SDK
- 여러 매장 동시 운영

## 10. Acceptance Criteria

MVP는 다음 항목을 모두 만족해야 완료로 본다.

- 새 게임 시작 후 과자 판매 루프가 자동으로 돌아간다.
- 음료와 라면을 Gold로 해금할 수 있다.
- 상품 업그레이드 후 판매가가 증가한다.
- 계산대가 기본 속도로 결제를 자동 처리한다.
- 계산원을 고용하면 결제 속도가 증가한다.
- 진열대가 기본 속도로 재고를 자동 보충한다.
- 진열 직원을 고용하면 재고 보충 속도가 증가한다.
- 매장 업그레이드 후 손님 수 또는 입장 속도가 증가한다.
- 앱을 종료하고 다시 실행해도 진행 상태가 유지된다.
- 오프라인 보상이 계산되어 지급된다.
- 5분 이상 플레이했을 때 명확한 성장감이 있다.

## 11. Implementation Defaults

아래 항목은 MVP 코드 구현 기준으로 사용한다.

- 손님 이동은 waypoint 기반으로 시작한다.
- 상품 선택은 재고 있는 상품 중 랜덤으로 시작한다.
- 계산원은 기본 1명 무료 제공 대신, 계산대 자체가 기본 처리 능력을 갖게 한다.
- 진열 직원 고용 전에도 느린 자동 재입고는 가능하게 한다.
- 저장은 로컬 JSON 파일로 구현한다.

## 12. Deferred Decisions

아래 항목은 MVP 구현 후 플레이 테스트 단계에서 결정한다.

- Stage 2 이후 상품 수와 배치
- 고가 상품 구매 가중치
- 실제 광고 SDK와 보상형 광고 정책
- Diamond 도입 시점
- NavMesh 전환 여부
