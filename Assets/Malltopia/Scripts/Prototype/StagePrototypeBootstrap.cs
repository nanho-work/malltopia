using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Malltopia
{
    public sealed class StagePrototypeBootstrap : MonoBehaviour
    {
        [Header("Prototype")]
        [Range(1, 3)]
        public int startStageNumber = 1;
        public int gridWidth = 11;
        public int gridHeight = 20;
        public float cellSize = 1f;
        public float moveSecondsPerTile = 1.2f;
        public bool autoStartCustomerLoop = true;

        [Header("Prototype Camera")]
        [Tooltip("How many grid rows fit vertically in the Game view. Smaller values zoom in.")]
        [Min(4f)]
        public float cameraVisibleRows = 16f;
        [Tooltip("Positive values move the camera view toward the entrance/customer area.")]
        public float cameraVerticalOffsetCells = 1.5f;

        [Header("Prototype Assets")]
        public GameObject customerPrefab;
        [Tooltip("Customer visual target height in grid cells. 2 means about two cells tall.")]
        [Min(0.1f)]
        public float customerVisualHeightCells = 2f;
        [Tooltip("Extra multiplier after fitting customerVisualHeightCells. Usually keep this at 1.")]
        [Min(0.1f)]
        public float customerPrefabScale = 1f;
        public Vector3 customerPrefabLocalOffset = new Vector3(0f, -0.36f, -0.55f);
        public Vector3 customerPrefabLocalEuler = Vector3.zero;

        [Header("Upgrade Hold")]
        public float holdStartDelaySec = 0.35f;
        public float initialRepeatIntervalSec = 0.12f;
        public float minRepeatIntervalSec = 0.02f;
        public float timeToMinIntervalSec = 2f;

        private readonly List<ProductRuntimeState> products = new List<ProductRuntimeState>();
        private readonly List<CustomerRuntimeState> activeCustomers = new List<CustomerRuntimeState>();
        private readonly List<WorkerRuntimeState> workers = new List<WorkerRuntimeState>();
        private readonly List<ProductWorkTask> workTasks = new List<ProductWorkTask>();
        private readonly List<StageUpgradeRuntimeState> stageUpgrades = new List<StageUpgradeRuntimeState>();
        private readonly List<Material> createdMaterials = new List<Material>();
        private readonly List<PrototypeHitTarget> hitTargets = new List<PrototypeHitTarget>();

        private Transform staticRoot;
        private Transform dynamicRoot;
        private Transform uiRoot;
        private Font uiFont;
        private PrototypeStageSpec currentStage;
        private PrototypeThemeSkin currentThemeSkin;
        private TextMesh statusText;
        private TextMesh toastText;
        private TextMesh workText;
        private Sprite footerButtonBaseSprite;
        private Sprite footerButtonWideSprite;
        private Sprite footerCharacterSprite;
        private GameObject cachedCustomerPrefab;
        private double gold;
        private int diamond;
        private int currentStageNumber;
        private float toastHideAt;
        private bool firstOrderReceived;
        private bool upgradeMenuOpen;
        private bool[] orderSlotOccupied;
        private int maxCustomers = 1;
        private int nextCustomerId = 1;
        private int nextWorkerId = 1;
        private bool isHoldingProductUpgrade;
        private int heldProductIndex = -1;
        private float holdStartedAt;
        private float nextHoldUpgradeAt;

        private static readonly int[] StarLevels =
        {
            10, 25, 50, 75, 100, 130, 170, 200, 235, 270, 310, 350
        };

        private const string FooterButtonBaseSpritePath = "Assets/Malltopia/Art/UI/Icons/button_footer_base.png";
        private const string FooterButtonWideSpritePath = "Assets/Malltopia/Art/UI/Icons/button_footer_base_pressed.png";
        private const string FooterCharacterSpritePath = "Assets/Malltopia/Art/UI/Icons/footer_character.png";
        private const string CustomerPrefabPath = "Assets/Malltopia/Art/Models/Customers/CustomerChibi.fbx";

        private void OnValidate()
        {
            cellSize = Mathf.Max(0.01f, cellSize);
            customerVisualHeightCells = Mathf.Max(0.1f, customerVisualHeightCells);
            customerPrefabScale = Mathf.Max(0.1f, customerPrefabScale);

            if (Application.isPlaying)
            {
                RefreshCustomerVisuals();
                ApplyPrototypeCamera(Camera.main);
            }
        }

        private void Start()
        {
            LoadStage(startStageNumber);
        }

        private void Update()
        {
            HandlePointerInput();

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                LoadStage(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                LoadStage(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                LoadStage(3);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                LoadStage(currentStageNumber);
            }

            if (toastText != null && Time.time >= toastHideAt)
            {
                toastText.text = string.Empty;
            }
        }

        public void HandlePrototypeClick(PrototypeClickTarget target)
        {
            HandlePrototypeAction(target.action, target.stageNumber, target.productIndex, target.slotIndex);
        }

        private void HandlePrototypeAction(PrototypeClickAction action, int stageNumber, int productIndex, int slotIndex)
        {
            switch (action)
            {
                case PrototypeClickAction.SwitchStage:
                    LoadStage(stageNumber);
                    break;
                case PrototypeClickAction.GrantGold:
                    gold += GetDebugGoldAmount();
                    RefreshStaticLayer();
                    ShowToast("+ Gold");
                    break;
                case PrototypeClickAction.ResetStage:
                    LoadStage(currentStageNumber);
                    break;
                case PrototypeClickAction.UnlockProduct:
                    TryUnlockProduct(productIndex);
                    break;
                case PrototypeClickAction.ActivateSlot:
                    TryActivateSlot(productIndex, slotIndex);
                    break;
                case PrototypeClickAction.UpgradeProduct:
                    TryUpgradeProduct(productIndex);
                    break;
                case PrototypeClickAction.ToggleStageUpgradeMenu:
                    upgradeMenuOpen = !upgradeMenuOpen;
                    RefreshStaticLayer();
                    break;
                case PrototypeClickAction.BuyStageUpgrade:
                    TryBuyStageUpgrade(productIndex);
                    break;
                case PrototypeClickAction.BuyNextStage:
                    TryBuyNextStage();
                    break;
                case PrototypeClickAction.OpenVault:
                    ShowToast("금고 준비중");
                    break;
                case PrototypeClickAction.OpenCharacter:
                    ShowToast("캐릭터 준비중");
                    break;
            }
        }

        private void LoadStage(int stageNumber)
        {
            StopAllCoroutines();
            EndProductUpgradeHold();
            currentStageNumber = Mathf.Clamp(stageNumber, 1, 3);
            currentStage = CreateStageSpec(currentStageNumber);
            currentThemeSkin = CreateAmusementParkThemeSkin();
            gold = currentStage.startingGold;
            diamond = 0;
            firstOrderReceived = false;
            upgradeMenuOpen = false;
            maxCustomers = 1;
            nextCustomerId = 1;
            nextWorkerId = 1;
            orderSlotOccupied = new bool[currentStage.customerOrderCells.Length];
            products.Clear();
            activeCustomers.Clear();
            workers.Clear();
            workTasks.Clear();
            stageUpgrades.Clear();

            for (int i = 0; i < currentStage.products.Length; i++)
            {
                products.Add(new ProductRuntimeState(currentStage.products[i]));
            }

            CreateStageUpgrades();
            ClearGeneratedScene();
            staticRoot = CreateRoot("Static Prototype");
            dynamicRoot = CreateRoot("Dynamic Prototype");
            CreateCamera();
            RefreshStaticLayer();
            CreateMainCharacter();
            UpdateStatus();
            ShowToast("Stage " + currentStageNumber);

            if (autoStartCustomerLoop)
            {
                StartCoroutine(CustomerSpawnerLoop());
            }
        }

        private void RefreshStaticLayer()
        {
            if (staticRoot == null)
            {
                staticRoot = CreateRoot("Static Prototype");
            }

            ClearChildren(staticRoot);
            hitTargets.Clear();
            BuildFloor();
            BuildCounter();
            BuildProducts();
            if (upgradeMenuOpen)
            {
                BuildStageUpgradeMenu();
            }
            BuildOverlayText();
            BuildFooterUi();
            UpdateStatus();
        }

        private void BuildFloor()
        {
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    Color color = GetFloorColor(y);
                    CreateCell("Floor " + x + "," + y, new GridPosition(x, y), color, 0.95f, staticRoot);
                }
            }

            CreateText("Entrance Label", "입구/출구", GridToWorld(new GridPosition(5, 0)) + new Vector3(0f, 0.1f, -0.6f), 0.07f, Color.black, staticRoot);
            CreateText("Customer Area Label", "손님 공간", GridToWorld(new GridPosition(5, 6)) + new Vector3(0f, 0f, -0.6f), 0.09f, new Color(0.24f, 0.3f, 0.38f), staticRoot);
            CreateText("Worker Area Label", "작업 공간", GridToWorld(new GridPosition(5, 18)) + new Vector3(0f, 0f, -0.6f), 0.075f, new Color(0.23f, 0.28f, 0.34f), staticRoot);

            BuildThemeDecorations();
            BuildCustomerSpaceMarkers();
        }

        private void BuildThemeDecorations()
        {
            PrototypeThemeSkin skin = GetThemeSkin();

            CreateBox("Theme Entrance Arch", GridToWorld(new GridPosition(5, 0)) + new Vector3(0f, 0f, -0.08f), new Vector3(4.2f, 0.22f, 0.08f), skin.accentColor, staticRoot);
            CreateText("Theme Name", skin.displayName, GridToWorld(new GridPosition(5, 1)) + new Vector3(0f, 0.16f, -0.65f), 0.055f, skin.titleColor, staticRoot);

            CreateThemeDecoration("Ticket Booth L", new GridPosition(1, 2), "티켓", skin.ticketBoothColor, 0.72f);
            CreateThemeDecoration("Ticket Booth R", new GridPosition(9, 2), "티켓", skin.ticketBoothColor, 0.72f);
            CreateThemeDecoration("Balloon L", new GridPosition(1, 5), "풍선", skin.balloonColor, 0.55f);
            CreateThemeDecoration("Balloon R", new GridPosition(9, 5), "풍선", skin.balloonColor, 0.55f);
            CreateThemeDecoration("Bench L", new GridPosition(2, 8), "벤치", skin.benchColor, 0.78f);
            CreateThemeDecoration("Bench R", new GridPosition(8, 8), "벤치", skin.benchColor, 0.78f);

            for (int x = 0; x < gridWidth; x += 2)
            {
                CreateCell("Theme Stripe Top", new GridPosition(x, 1), skin.pathStripeColor, 0.34f, staticRoot);
            }
        }

        private void CreateThemeDecoration(string name, GridPosition position, string label, Color color, float size)
        {
            CreateCell(name, position, color, size, staticRoot);
            CreateText(name + " Label", label, GridToWorld(position) + new Vector3(0f, 0f, -0.65f), 0.04f, Color.black, staticRoot);
        }

        private void BuildCustomerSpaceMarkers()
        {
            GridPosition[] markers =
            {
                new GridPosition(3, 3),
                new GridPosition(7, 3),
                new GridPosition(2, 6),
                new GridPosition(8, 6),
                new GridPosition(4, 8),
                new GridPosition(6, 8)
            };

            for (int i = 0; i < markers.Length; i++)
            {
                CreateCell("Future Customer Decoration", markers[i], GetThemeSkin().customerDecorationColor, 0.46f, staticRoot);
            }
        }

        private void BuildCounter()
        {
            for (int i = 0; i < currentStage.counterCells.Length; i++)
            {
                CreateCell("Counter", currentStage.counterCells[i], GetThemeSkin().counterColor, 0.98f, staticRoot);
            }

            for (int i = 0; i < currentStage.customerOrderCells.Length; i++)
            {
                CreateCell("Customer Order Position", currentStage.customerOrderCells[i], GetThemeSkin().customerOrderColor, 0.66f, staticRoot);
                CreateText("Order Label", "손님", GridToWorld(currentStage.customerOrderCells[i]) + new Vector3(0f, 0f, -0.6f), 0.055f, Color.black, staticRoot);
            }
        }

        private void BuildProducts()
        {
            for (int i = 0; i < products.Count; i++)
            {
                ProductRuntimeState state = products[i];
                bool unlockAllowed = IsProductUnlockAllowed(i);
                Color standColor = state.IsUnlocked ? GetProductStandColor(i) : GetThemeSkin().lockedStandColor;
                PrototypeClickAction standAction = state.IsUnlocked ? PrototypeClickAction.UpgradeProduct : PrototypeClickAction.UnlockProduct;
                GameObject stand = CreateCell("Stand " + state.Spec.displayName, state.Spec.standCell, standColor, 0.88f, staticRoot);
                AddClickTarget(stand, standAction, i, 0);
                AddHitTarget(GridToWorld(state.Spec.standCell), new Vector2(0.95f, 0.95f), standAction, 0, i, 0);

                string shortName = GetShortProductName(state.Spec.displayName);
                string lockedReason = i == 0 ? "주문 대기" : "1번 매대 필요";
                string standLabel = state.IsUnlocked
                    ? shortName + "\nLv" + state.Level + " ★" + state.Stars
                    : shortName + "\n" + (unlockAllowed ? "해금 " + FormatGold(state.Spec.unlockCost) : lockedReason);
                CreateText("Stand Label", standLabel, GridToWorld(state.Spec.standCell) + new Vector3(0f, 0f, -0.7f), 0.045f, Color.black, staticRoot);

                if (!state.IsUnlocked && unlockAllowed && gold >= state.Spec.unlockCost)
                {
                    CreateUpgradeBadge("Unlock Badge", state.Spec.standCell, staticRoot, standAction, i, 0);
                }
                else if (state.IsUnlocked && state.Level < state.Spec.maxLevel && gold >= GetUpgradeCost(state))
                {
                    CreateUpgradeBadge("Upgrade Badge", state.Spec.standCell, staticRoot, standAction, i, 0);
                }

                for (int slotIndex = 0; slotIndex < state.Spec.slotCells.Length; slotIndex++)
                {
                    if (!IsSlotVisible(state, slotIndex))
                    {
                        continue;
                    }

                    bool active = slotIndex < state.ActiveSlotCount;
                    Color slotColor = active ? GetThemeSkin().activeSlotColor : GetThemeSkin().inactiveSlotColor;
                    GameObject slot = CreateCell("Production Slot", state.Spec.slotCells[slotIndex], slotColor, 0.74f, staticRoot);
                    AddClickTarget(slot, PrototypeClickAction.ActivateSlot, i, slotIndex);
                    AddHitTarget(GridToWorld(state.Spec.slotCells[slotIndex]), new Vector2(0.9f, 0.9f), PrototypeClickAction.ActivateSlot, 0, i, slotIndex);
                    CreateText("Slot Label", active ? "생산" : "터치", GridToWorld(state.Spec.slotCells[slotIndex]) + new Vector3(0f, 0f, -0.7f), 0.05f, Color.black, staticRoot);

                    if (!active)
                    {
                        CreateUpgradeBadge("Slot Badge", state.Spec.slotCells[slotIndex], staticRoot, PrototypeClickAction.ActivateSlot, i, slotIndex);
                    }
                }
            }

            for (int i = 0; i < currentStage.staffWaitingCells.Length; i++)
            {
                CreateCell("Staff Wait", currentStage.staffWaitingCells[i], GetThemeSkin().staffWaitColor, 0.54f, staticRoot);
                CreateText("Staff Wait Label", "대기", GridToWorld(currentStage.staffWaitingCells[i]) + new Vector3(0f, 0f, -0.65f), 0.045f, Color.black, staticRoot);
            }
        }

        private void BuildStageUpgradeMenu()
        {
            int visibleRows = stageUpgrades.Count;
            float panelHeight = 1.1f + visibleRows * 0.55f;
            Vector3 panelPosition = new Vector3(0f, -5.95f, -0.95f);
            CreateBox("Stage Upgrade Panel", panelPosition, new Vector3(8.2f, panelHeight, 0.08f), new Color(0.96f, 0.96f, 0.93f), staticRoot);
            AddHitTarget(panelPosition, new Vector2(8.3f, panelHeight), PrototypeClickAction.None, 0, -1, 0);
            CreateText("Stage Upgrade Title", "업그레이드", panelPosition + new Vector3(0f, panelHeight * 0.5f - 0.35f, -0.6f), 0.075f, Color.black, staticRoot);

            for (int i = 0; i < visibleRows; i++)
            {
                StageUpgradeRuntimeState upgrade = stageUpgrades[i];
                Vector3 rowPosition = panelPosition + new Vector3(0f, panelHeight * 0.5f - 0.9f - i * 0.52f, -0.2f);
                bool complete = upgrade.PurchasedCount >= upgrade.MaxPurchaseCount;
                bool canBuy = CanBuyStageUpgrade(i);
                Color rowColor = complete
                    ? new Color(0.74f, 0.78f, 0.78f)
                    : canBuy ? new Color(0.74f, 0.9f, 0.78f) : new Color(0.86f, 0.86f, 0.86f);

                CreateBox("Stage Upgrade Row", rowPosition, new Vector3(7.45f, 0.45f, 0.07f), rowColor, staticRoot);
                AddHitTarget(rowPosition, new Vector2(7.5f, 0.5f), PrototypeClickAction.BuyStageUpgrade, 0, i, 0);

                string priceText = complete ? "완료" : FormatGold(upgrade.Cost);
                string countText = upgrade.MaxPurchaseCount > 1 ? " " + upgrade.PurchasedCount + "/" + upgrade.MaxPurchaseCount : string.Empty;
                string rowText = upgrade.DisplayName + countText + "  " + upgrade.Description + "  " + priceText;
                CreateText("Stage Upgrade Row Text", rowText, rowPosition + new Vector3(0f, 0f, -0.6f), 0.038f, Color.black, staticRoot);
            }
        }

        private void BuildFooterUi()
        {
            EnsureUiRoot();
            ClearChildren(uiRoot);
            EnsureEventSystem();

            GameObject canvasObject = new GameObject("Footer Canvas");
            canvasObject.transform.SetParent(uiRoot, false);
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 50;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(390f, 844f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObject.AddComponent<GraphicRaycaster>();

            GameObject footer = CreateUiPanel("Footer Bar", canvasObject.transform, new Color(0.08f, 0.09f, 0.1f, 0.96f));
            RectTransform footerRect = footer.GetComponent<RectTransform>();
            footerRect.anchorMin = new Vector2(0f, 0f);
            footerRect.anchorMax = new Vector2(1f, 0f);
            footerRect.pivot = new Vector2(0.5f, 0f);
            footerRect.anchoredPosition = Vector2.zero;
            footerRect.sizeDelta = new Vector2(0f, 76f);

            HorizontalLayoutGroup layout = footer.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 10, 10);
            layout.spacing = 9f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            Sprite smallButtonSprite = GetFooterButtonBaseSprite();
            Sprite wideButtonSprite = GetFooterButtonWideSprite();

            CreateFooterButton(footer.transform, "✈", string.Empty, 56f, PrototypeClickAction.BuyNextStage, new Color(0.12f, 0.55f, 0.78f), smallButtonSprite, null);
            CreateFooterButton(footer.transform, "▣", string.Empty, 56f, PrototypeClickAction.OpenVault, new Color(0.12f, 0.55f, 0.78f), smallButtonSprite, null);
            CreateFooterButton(footer.transform, "▶", "부스터 x6\n+Gold", 130f, PrototypeClickAction.GrantGold, new Color(0.08f, 0.47f, 0.68f), wideButtonSprite, null);
            CreateFooterButton(footer.transform, "●", string.Empty, 56f, PrototypeClickAction.OpenCharacter, new Color(0.12f, 0.55f, 0.78f), smallButtonSprite, GetFooterCharacterSprite());
            CreateFooterButton(footer.transform, "↑", string.Empty, 56f, PrototypeClickAction.ToggleStageUpgradeMenu, upgradeMenuOpen ? new Color(0.2f, 0.7f, 0.98f) : new Color(0.12f, 0.55f, 0.78f), smallButtonSprite, null);
        }

        private void CreateFooterButton(Transform parent, string iconText, string labelText, float preferredWidth, PrototypeClickAction action, Color color, Sprite backgroundSprite, Sprite iconSprite)
        {
            GameObject buttonObject = CreateUiPanel("Footer Button " + iconText, parent, color);
            Image background = buttonObject.GetComponent<Image>();
            if (backgroundSprite != null)
            {
                background.sprite = backgroundSprite;
                background.type = Image.Type.Simple;
                background.preserveAspect = false;
                background.color = Color.white;
            }

            LayoutElement layout = buttonObject.AddComponent<LayoutElement>();
            layout.preferredWidth = preferredWidth;
            layout.minWidth = preferredWidth;

            Button button = buttonObject.AddComponent<Button>();
            ColorBlock colors = button.colors;
            Color normalColor = backgroundSprite != null ? Color.white : color;
            colors.normalColor = normalColor;
            colors.highlightedColor = Color.Lerp(normalColor, Color.white, 0.18f);
            colors.pressedColor = Color.Lerp(normalColor, Color.black, 0.18f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
            button.targetGraphic = background;
            button.onClick.AddListener(() => HandlePrototypeAction(action, 0, -1, 0));

            RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(preferredWidth, 56f);

            bool hasLabel = !string.IsNullOrEmpty(labelText);
            RectTransform iconRect;
            bool usesImageIcon = iconSprite != null;
            if (iconSprite != null)
            {
                Image icon = CreateUiImage("Icon Image", buttonObject.transform, iconSprite, Color.white);
                icon.preserveAspect = true;
                iconRect = icon.GetComponent<RectTransform>();
            }
            else
            {
                Text icon = CreateUiText("Icon", buttonObject.transform, iconText, 28, Color.white, TextAnchor.MiddleCenter);
                iconRect = icon.GetComponent<RectTransform>();
            }

            iconRect.anchorMin = new Vector2(0f, hasLabel ? 0.32f : 0f);
            iconRect.anchorMax = new Vector2(1f, 1f);
            float iconPadding = usesImageIcon ? 10f : 0f;
            iconRect.offsetMin = new Vector2(iconPadding, iconPadding);
            iconRect.offsetMax = new Vector2(-iconPadding, -iconPadding);

            if (hasLabel)
            {
                Text label = CreateUiText("Label", buttonObject.transform, labelText, 14, new Color(0.82f, 0.9f, 0.95f), TextAnchor.MiddleCenter);
                RectTransform labelRect = label.GetComponent<RectTransform>();
                labelRect.anchorMin = new Vector2(0f, 0f);
                labelRect.anchorMax = new Vector2(1f, 0.48f);
                labelRect.offsetMin = new Vector2(4f, 1f);
                labelRect.offsetMax = new Vector2(-4f, 0f);
            }
        }

        private GameObject CreateUiPanel(string name, Transform parent, Color color)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.localScale = Vector3.one;
            Image image = panel.AddComponent<Image>();
            image.color = color;
            return panel;
        }

        private Image CreateUiImage(string name, Transform parent, Sprite sprite, Color color)
        {
            GameObject imageObject = new GameObject(name);
            imageObject.transform.SetParent(parent, false);
            RectTransform rect = imageObject.AddComponent<RectTransform>();
            rect.localScale = Vector3.one;
            Image image = imageObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private Sprite GetFooterButtonBaseSprite()
        {
            if (footerButtonBaseSprite == null)
            {
                footerButtonBaseSprite = LoadPrototypeSprite(FooterButtonBaseSpritePath);
            }

            return footerButtonBaseSprite;
        }

        private Sprite GetFooterButtonWideSprite()
        {
            if (footerButtonWideSprite == null)
            {
                footerButtonWideSprite = LoadPrototypeSprite(FooterButtonWideSpritePath);
            }

            return footerButtonWideSprite != null ? footerButtonWideSprite : GetFooterButtonBaseSprite();
        }

        private Sprite GetFooterCharacterSprite()
        {
            if (footerCharacterSprite == null)
            {
                footerCharacterSprite = LoadPrototypeSprite(FooterCharacterSpritePath);
            }

            return footerCharacterSprite;
        }

        private Sprite LoadPrototypeSprite(string assetPath)
        {
#if UNITY_EDITOR
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite != null)
            {
                return sprite;
            }

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture == null)
            {
                return null;
            }

            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
#else
            return null;
#endif
        }

        private Text CreateUiText(string name, Transform parent, string text, int fontSize, Color color, TextAnchor alignment)
        {
            GameObject textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            RectTransform rect = textObject.AddComponent<RectTransform>();
            rect.localScale = Vector3.one;
            Text uiText = textObject.AddComponent<Text>();
            uiText.text = text;
            uiText.font = GetUiFont();
            uiText.fontSize = fontSize;
            uiText.color = color;
            uiText.alignment = alignment;
            uiText.horizontalOverflow = HorizontalWrapMode.Wrap;
            uiText.verticalOverflow = VerticalWrapMode.Truncate;
            return uiText;
        }

        private void BuildOverlayText()
        {
            statusText = CreateText("Status", string.Empty, new Vector3(-5.05f, 10.78f, -0.7f), 0.075f, Color.black, staticRoot);
            statusText.anchor = TextAnchor.MiddleLeft;

            workText = CreateText("Work Text", string.Empty, new Vector3(0f, 10.78f, -0.7f), 0.065f, new Color(0.05f, 0.27f, 0.45f), staticRoot);

            toastText = CreateText("Toast", string.Empty, new Vector3(2.75f, 10.78f, -0.7f), 0.065f, new Color(0.1f, 0.46f, 0.16f), staticRoot);
            toastText.anchor = TextAnchor.MiddleLeft;
        }

        private IEnumerator CustomerSpawnerLoop()
        {
            while (true)
            {
                if (activeCustomers.Count < maxCustomers)
                {
                    int orderSlotIndex;
                    if (TryReserveOrderSlot(out orderSlotIndex))
                    {
                        StartCoroutine(CustomerRoutine(orderSlotIndex));
                    }
                }

                yield return new WaitForSeconds(0.35f);
            }
        }

        private IEnumerator CustomerRoutine(int orderSlotIndex)
        {
            CustomerRuntimeState runtime = CreateCustomer(orderSlotIndex);
            activeCustomers.Add(runtime);
            yield return MoveTo(runtime.Agent, GridToWorld(runtime.OrderCell), 1.2f);
            yield return new WaitForSeconds(0.4f);

            CreateCustomerOrder(runtime);
            SetCustomerBubble(runtime, GetOrderSummary(runtime));

            if (!firstOrderReceived)
            {
                firstOrderReceived = true;
                RefreshStaticLayer();
                ShowToast("첫 주문: 해금 가능");
            }

            while (runtime.RemainingItems > 0)
            {
                RefreshCustomerWaitingBubble(runtime);
                yield return null;
            }

            SetCustomerBubble(runtime, "완료");
            yield return new WaitForSeconds(0.2f);
            yield return MoveTo(runtime.Agent, GridToWorld(currentStage.exitCell), 1.3f);
            ReleaseOrderSlot(orderSlotIndex);
            activeCustomers.Remove(runtime);

            if (runtime.Agent != null)
            {
                Destroy(runtime.Agent);
            }
        }

        private bool TryReserveOrderSlot(out int orderSlotIndex)
        {
            orderSlotIndex = -1;
            if (orderSlotOccupied == null)
            {
                return false;
            }

            for (int i = 0; i < orderSlotOccupied.Length; i++)
            {
                if (orderSlotOccupied[i])
                {
                    continue;
                }

                orderSlotOccupied[i] = true;
                orderSlotIndex = i;
                return true;
            }

            return false;
        }

        private void ReleaseOrderSlot(int orderSlotIndex)
        {
            if (orderSlotOccupied != null && orderSlotIndex >= 0 && orderSlotIndex < orderSlotOccupied.Length)
            {
                orderSlotOccupied[orderSlotIndex] = false;
            }
        }

        private void CreateCustomerOrder(CustomerRuntimeState customerState)
        {
            customerState.OrderItems.Clear();

            List<int> eligibleProducts = GetEligibleOrderProductIndexes();
            if (!firstOrderReceived || eligibleProducts.Count == 0)
            {
                AddCustomerOrderItem(customerState, 0, 1);
            }
            else
            {
                int distinctProductCount = Random.Range(1, Mathf.Min(2, eligibleProducts.Count) + 1);
                for (int i = 0; i < distinctProductCount; i++)
                {
                    int randomIndex = Random.Range(0, eligibleProducts.Count);
                    int productIndex = eligibleProducts[randomIndex];
                    eligibleProducts.RemoveAt(randomIndex);
                    AddCustomerOrderItem(customerState, productIndex, Random.Range(1, 4));
                }
            }

            customerState.RemainingItems = 0;
            for (int i = 0; i < customerState.OrderItems.Count; i++)
            {
                CustomerOrderItem item = customerState.OrderItems[i];
                customerState.RemainingItems += item.Quantity;
                for (int quantityIndex = 0; quantityIndex < item.Quantity; quantityIndex++)
                {
                    workTasks.Add(new ProductWorkTask(customerState, item.ProductIndex));
                }
            }
        }

        private void AddCustomerOrderItem(CustomerRuntimeState customerState, int productIndex, int quantity)
        {
            if (!IsValidProductIndex(productIndex))
            {
                return;
            }

            customerState.OrderItems.Add(new CustomerOrderItem(productIndex, quantity));
        }

        private List<int> GetEligibleOrderProductIndexes()
        {
            List<int> eligible = new List<int>();
            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].IsUnlocked && products[i].ActiveSlotCount > 0)
                {
                    eligible.Add(i);
                }
            }

            return eligible;
        }

        private WorkAssignment TakeNextAvailableWorkTask(WorkerRuntimeState worker)
        {
            for (int i = 0; i < workTasks.Count; i++)
            {
                ProductWorkTask task = workTasks[i];
                if (task.Customer == null || task.Customer.Agent == null || task.Customer.RemainingItems <= 0 || !IsValidProductIndex(task.ProductIndex))
                {
                    workTasks.RemoveAt(i);
                    i--;
                    continue;
                }

                ProductRuntimeState product = products[task.ProductIndex];
                if (!CanServe(product))
                {
                    continue;
                }

                ProductionWorkTarget target = GetProductionWorkTarget(product, worker.GridPosition);
                if (!target.IsValid)
                {
                    continue;
                }

                product.SetSlotOccupied(target.SlotIndex, true);
                workTasks.RemoveAt(i);
                return new WorkAssignment(task, target);
            }

            return null;
        }

        private string GetOrderSummary(CustomerRuntimeState customerState)
        {
            if (customerState.OrderItems.Count == 0)
            {
                return "...";
            }

            List<string> parts = new List<string>();
            for (int i = 0; i < customerState.OrderItems.Count; i++)
            {
                CustomerOrderItem item = customerState.OrderItems[i];
                parts.Add(GetShortProductName(products[item.ProductIndex].Spec.displayName) + " x" + item.Quantity);
            }

            return string.Join("\n", parts.ToArray());
        }

        private void RefreshCustomerWaitingBubble(CustomerRuntimeState customerState)
        {
            if (customerState == null || customerState.RemainingItems <= 0)
            {
                return;
            }

            for (int i = 0; i < customerState.OrderItems.Count; i++)
            {
                CustomerOrderItem item = customerState.OrderItems[i];
                if (!CanServe(products[item.ProductIndex]))
                {
                    ProductRuntimeState product = products[item.ProductIndex];
                    string orderName = GetShortProductName(product.Spec.displayName);
                    SetCustomerBubble(customerState, product.IsUnlocked ? orderName + "\n생산칸" : orderName + "\n해금");
                    return;
                }
            }

            SetCustomerBubble(customerState, GetOrderSummary(customerState) + "\n남음 " + customerState.RemainingItems);
        }

        private void HandlePointerInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverUi(-1))
                {
                    EndProductUpgradeHold();
                }
                else
                {
                    HandlePointerDown(Input.mousePosition);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                EndProductUpgradeHold();
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    if (IsPointerOverUi(touch.fingerId))
                    {
                        EndProductUpgradeHold();
                    }
                    else
                    {
                        HandlePointerDown(touch.position);
                    }
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    EndProductUpgradeHold();
                }
            }

            UpdateProductUpgradeHold();
        }

        private void HandlePointerDown(Vector2 screenPoint)
        {
            PrototypeHitTarget target;
            if (!TryGetHitTarget(screenPoint, out target))
            {
                if (upgradeMenuOpen)
                {
                    upgradeMenuOpen = false;
                    RefreshStaticLayer();
                }

                EndProductUpgradeHold();
                return;
            }

            if (upgradeMenuOpen &&
                target.Action != PrototypeClickAction.None &&
                target.Action != PrototypeClickAction.BuyStageUpgrade &&
                target.Action != PrototypeClickAction.ToggleStageUpgradeMenu)
            {
                upgradeMenuOpen = false;
                RefreshStaticLayer();
                EndProductUpgradeHold();
                return;
            }

            HandlePrototypeAction(target.Action, target.StageNumber, target.ProductIndex, target.SlotIndex);

            if (target.Action == PrototypeClickAction.UpgradeProduct && CanRepeatProductUpgrade(target.ProductIndex))
            {
                BeginProductUpgradeHold(target.ProductIndex);
            }
            else
            {
                EndProductUpgradeHold();
            }
        }

        private bool TryGetHitTarget(Vector2 screenPoint, out PrototypeHitTarget target)
        {
            target = default(PrototypeHitTarget);
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                return false;
            }

            Vector3 world = mainCamera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, -mainCamera.transform.position.z));
            for (int i = hitTargets.Count - 1; i >= 0; i--)
            {
                if (!hitTargets[i].Contains(world))
                {
                    continue;
                }

                target = hitTargets[i];
                return true;
            }

            return false;
        }

        private void BeginProductUpgradeHold(int productIndex)
        {
            isHoldingProductUpgrade = true;
            heldProductIndex = productIndex;
            holdStartedAt = Time.time;
            nextHoldUpgradeAt = Time.time + Mathf.Max(0f, holdStartDelaySec);
        }

        private void EndProductUpgradeHold()
        {
            isHoldingProductUpgrade = false;
            heldProductIndex = -1;
        }

        private bool IsPointerOverUi(int pointerId)
        {
            if (EventSystem.current == null)
            {
                return false;
            }

            return pointerId < 0
                ? EventSystem.current.IsPointerOverGameObject()
                : EventSystem.current.IsPointerOverGameObject(pointerId);
        }

        private void UpdateProductUpgradeHold()
        {
            if (!isHoldingProductUpgrade || Time.time < nextHoldUpgradeAt)
            {
                return;
            }

            if (!TryUpgradeProduct(heldProductIndex))
            {
                EndProductUpgradeHold();
                return;
            }

            nextHoldUpgradeAt = Time.time + GetHoldRepeatInterval();
        }

        private float GetHoldRepeatInterval()
        {
            float heldSeconds = Mathf.Max(0f, Time.time - holdStartedAt - holdStartDelaySec);
            float ratio = Mathf.Clamp01(heldSeconds / Mathf.Max(0.01f, timeToMinIntervalSec));
            return Mathf.Lerp(initialRepeatIntervalSec, minRepeatIntervalSec, ratio);
        }

        private IEnumerator WorkerLoop(WorkerRuntimeState worker)
        {
            while (true)
            {
                WorkAssignment assignment = TakeNextAvailableWorkTask(worker);
                if (assignment == null)
                {
                    yield return null;
                    continue;
                }

                yield return ServeWorkTask(worker, assignment);
            }
        }

        private IEnumerator ServeWorkTask(WorkerRuntimeState worker, WorkAssignment assignment)
        {
            ProductWorkTask task = assignment.Task;
            if (task.Customer == null || task.Customer.Agent == null || !IsValidProductIndex(task.ProductIndex))
            {
                ReleaseWorkAssignment(assignment);
                yield break;
            }

            ProductRuntimeState product = products[task.ProductIndex];
            ProductionWorkTarget target = assignment.Target;
            if (!target.IsValid)
            {
                ReleaseWorkAssignment(assignment);
                yield break;
            }

            yield return MoveWorkerTo(worker, target.WorkCell, 1f);
            yield return PlayProduction(product, target.SlotIndex);
            ReleaseWorkAssignment(assignment);
            yield return MoveWorkerTo(worker, currentStage.serviceCell, 1f);

            double reward = GetReward(product);
            gold += reward;
            task.Customer.RemainingItems = Mathf.Max(0, task.Customer.RemainingItems - 1);
            RefreshCustomerWaitingBubble(task.Customer);
            RefreshStaticLayer();
            ShowToast("+" + FormatGold(reward));
        }

        private void ReleaseWorkAssignment(WorkAssignment assignment)
        {
            if (assignment == null || assignment.Task == null || !IsValidProductIndex(assignment.Task.ProductIndex) || !assignment.Target.IsValid)
            {
                return;
            }

            products[assignment.Task.ProductIndex].SetSlotOccupied(assignment.Target.SlotIndex, false);
        }

        private IEnumerator PlayProduction(ProductRuntimeState product, int slotIndex)
        {
            float duration = product.Spec.productionSeconds / Mathf.Max(0.1f, product.ProductionSpeedMultiplier);
            float elapsed = 0f;
            Vector3 barPosition = GridToWorld(product.Spec.slotCells[slotIndex]) + new Vector3(0f, 0.48f, -0.8f);
            GameObject barBackground = CreateBox("Production Bar BG", barPosition, new Vector3(0.72f, 0.1f, 0.05f), new Color(0.2f, 0.2f, 0.2f), dynamicRoot);
            GameObject bar = CreateBox("Production Bar", barPosition + new Vector3(-0.36f, 0f, -0.05f), new Vector3(0.01f, 0.1f, 0.05f), new Color(0.18f, 0.9f, 0.38f), dynamicRoot);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float ratio = Mathf.Clamp01(elapsed / duration);
                bar.transform.localScale = new Vector3(0.72f * ratio, 0.1f, 0.05f);
                bar.transform.position = barPosition + new Vector3((-0.36f + 0.36f * ratio), 0f, -0.05f);
                if (workText != null)
                {
                    workText.text = "생산 " + duration.ToString("0.#", CultureInfo.InvariantCulture) + "초";
                }

                yield return null;
            }

            if (workText != null)
            {
                workText.text = string.Empty;
            }

            Destroy(barBackground);
            Destroy(bar);
        }

        private void CreateMainCharacter()
        {
            WorkerRuntimeState mainWorker = CreateWorker("Main Character", currentStage.mainStartCell, new Color(0.2f, 0.46f, 1f), "주");
            workers.Add(mainWorker);
            StartCoroutine(WorkerLoop(mainWorker));
        }

        private CustomerRuntimeState CreateCustomer(int orderSlotIndex)
        {
            CustomerRuntimeState runtime = new CustomerRuntimeState();
            runtime.Id = nextCustomerId++;
            runtime.OrderSlotIndex = orderSlotIndex;
            runtime.OrderCell = currentStage.customerOrderCells[orderSlotIndex];
            runtime.Agent = CreateCustomerAgent("Customer " + runtime.Id, GridToWorld(currentStage.entranceCell));
            GameObject bubble = CreateBox("Order Bubble", runtime.Agent.transform.position + new Vector3(0.3f, 0.72f, -0.6f), new Vector3(1.35f, 0.55f, 0.05f), Color.white, dynamicRoot);
            bubble.transform.SetParent(runtime.Agent.transform, true);
            runtime.BubbleText = CreateText("Order Bubble Text", string.Empty, bubble.transform.position + new Vector3(0f, 0f, -0.7f), 0.045f, Color.black, dynamicRoot);
            runtime.BubbleText.transform.SetParent(runtime.Agent.transform, true);
            return runtime;
        }

        private WorkerRuntimeState CreateWorker(string name, GridPosition startCell, Color color, string label)
        {
            WorkerRuntimeState runtime = new WorkerRuntimeState();
            runtime.Id = nextWorkerId++;
            runtime.GridPosition = startCell;
            runtime.Agent = CreateAgent(name, GridToWorld(startCell), color, label);
            return runtime;
        }

        private GameObject CreateCustomerAgent(string name, Vector3 position)
        {
            GameObject prefab = GetCustomerPrefab();
            if (prefab == null)
            {
                return CreateAgent(name, position, new Color(1f, 0.78f, 0.27f), "손");
            }

            GameObject agent = new GameObject(name);
            agent.transform.SetParent(dynamicRoot, false);
            agent.transform.position = position + new Vector3(0f, 0f, -0.35f);

            GameObject visual = Instantiate(prefab, agent.transform);
            visual.name = "CustomerChibi Visual";
            visual.transform.localPosition = customerPrefabLocalOffset;
            visual.transform.localRotation = Quaternion.Euler(customerPrefabLocalEuler);
            FitCustomerVisualToCellHeight(visual);

            PrototypeFaceCamera faceCamera = visual.AddComponent<PrototypeFaceCamera>();
            faceCamera.eulerOffset = customerPrefabLocalEuler;
            return agent;
        }

        private void RefreshCustomerVisuals()
        {
            for (int i = 0; i < activeCustomers.Count; i++)
            {
                GameObject agent = activeCustomers[i].Agent;
                if (agent == null)
                {
                    continue;
                }

                PrototypeFaceCamera faceCamera = agent.GetComponentInChildren<PrototypeFaceCamera>();
                if (faceCamera == null)
                {
                    continue;
                }

                Transform visual = faceCamera.transform;
                visual.localPosition = customerPrefabLocalOffset;
                visual.localRotation = Quaternion.Euler(customerPrefabLocalEuler);
                faceCamera.eulerOffset = customerPrefabLocalEuler;
                FitCustomerVisualToCellHeight(visual.gameObject);
            }
        }

        private void FitCustomerVisualToCellHeight(GameObject visual)
        {
            if (visual == null)
            {
                return;
            }

            visual.transform.localScale = Vector3.one;
            Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                visual.transform.localScale = Vector3.one * Mathf.Max(0.01f, customerPrefabScale);
                return;
            }

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            float currentHeight = Mathf.Max(0.01f, bounds.size.y);
            float targetHeight = Mathf.Max(0.1f, customerVisualHeightCells) * cellSize;
            float scale = targetHeight / currentHeight * Mathf.Max(0.01f, customerPrefabScale);
            visual.transform.localScale = Vector3.one * scale;
        }

        private GameObject GetCustomerPrefab()
        {
            if (customerPrefab != null)
            {
                return customerPrefab;
            }

#if UNITY_EDITOR
            if (cachedCustomerPrefab == null)
            {
                cachedCustomerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CustomerPrefabPath);
            }

            return cachedCustomerPrefab;
#else
            return null;
#endif
        }

        private GameObject CreateAgent(string name, Vector3 position, Color color, string label)
        {
            GameObject agent = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            agent.name = name;
            agent.transform.SetParent(dynamicRoot, false);
            agent.transform.position = position + new Vector3(0f, 0f, -0.35f);
            agent.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
            SetRendererColor(agent, color);
            CreateText(name + " Label", label, position + new Vector3(0f, -0.02f, -1f), 0.055f, Color.black, agent.transform);
            return agent;
        }

        private IEnumerator MoveTo(GameObject target, Vector3 destination, float speedMultiplier)
        {
            if (target == null)
            {
                yield break;
            }

            Vector3 finalDestination = new Vector3(destination.x, destination.y, target.transform.position.z);
            float speed = cellSize / Mathf.Max(0.1f, moveSecondsPerTile) * speedMultiplier;

            while (Vector3.Distance(target.transform.position, finalDestination) > 0.02f)
            {
                target.transform.position = Vector3.MoveTowards(target.transform.position, finalDestination, speed * Time.deltaTime);
                yield return null;
            }

            target.transform.position = finalDestination;
        }

        private IEnumerator MoveWorkerTo(WorkerRuntimeState worker, GridPosition destination, float speedMultiplier)
        {
            if (worker == null || worker.Agent == null)
            {
                yield break;
            }

            List<GridPosition> path = FindWorkerPath(worker.GridPosition, destination);
            if (path.Count == 0)
            {
                yield return MoveTo(worker.Agent, GridToWorld(destination), speedMultiplier);
                worker.GridPosition = destination;
                yield break;
            }

            for (int i = 1; i < path.Count; i++)
            {
                yield return MoveTo(worker.Agent, GridToWorld(path[i]), speedMultiplier);
                worker.GridPosition = path[i];
            }
        }

        private List<GridPosition> FindWorkerPath(GridPosition start, GridPosition destination)
        {
            List<GridPosition> result = new List<GridPosition>();
            if (!IsInsideGrid(start) || !IsInsideGrid(destination) || !IsWorkerWalkable(destination))
            {
                return result;
            }

            bool[,] visited = new bool[gridWidth, gridHeight];
            GridPosition[,] parent = new GridPosition[gridWidth, gridHeight];
            Queue<GridPosition> queue = new Queue<GridPosition>();
            queue.Enqueue(start);
            visited[start.x, start.y] = true;
            parent[start.x, start.y] = start;

            GridPosition[] directions =
            {
                new GridPosition(0, -1),
                new GridPosition(1, 0),
                new GridPosition(0, 1),
                new GridPosition(-1, 0)
            };

            while (queue.Count > 0)
            {
                GridPosition current = queue.Dequeue();
                if (IsSameGridPosition(current, destination))
                {
                    return RebuildPath(parent, start, destination);
                }

                for (int i = 0; i < directions.Length; i++)
                {
                    GridPosition next = new GridPosition(current.x + directions[i].x, current.y + directions[i].y);
                    if (!IsInsideGrid(next) || visited[next.x, next.y] || !IsWorkerWalkable(next))
                    {
                        continue;
                    }

                    visited[next.x, next.y] = true;
                    parent[next.x, next.y] = current;
                    queue.Enqueue(next);
                }
            }

            return result;
        }

        private List<GridPosition> RebuildPath(GridPosition[,] parent, GridPosition start, GridPosition destination)
        {
            List<GridPosition> path = new List<GridPosition>();
            GridPosition current = destination;
            path.Add(current);

            while (!IsSameGridPosition(current, start))
            {
                current = parent[current.x, current.y];
                path.Add(current);
            }

            path.Reverse();
            return path;
        }

        private ProductionWorkTarget GetProductionWorkTarget(ProductRuntimeState product, GridPosition workerPosition)
        {
            ProductionWorkTarget bestTarget = ProductionWorkTarget.Invalid;
            int bestPathLength = int.MaxValue;
            int bestServiceDistance = int.MaxValue;

            for (int slotIndex = 0; slotIndex < product.ActiveSlotCount; slotIndex++)
            {
                if (product.IsSlotOccupied(slotIndex))
                {
                    continue;
                }

                GridPosition slot = product.Spec.slotCells[slotIndex];
                GridPosition[] candidates =
                {
                    new GridPosition(slot.x, slot.y - 1),
                    new GridPosition(slot.x + 1, slot.y),
                    new GridPosition(slot.x, slot.y + 1),
                    new GridPosition(slot.x - 1, slot.y)
                };

                for (int i = 0; i < candidates.Length; i++)
                {
                    GridPosition candidate = candidates[i];
                    if (!IsWorkerWalkable(candidate))
                    {
                        continue;
                    }

                    List<GridPosition> path = FindWorkerPath(workerPosition, candidate);
                    if (path.Count == 0)
                    {
                        continue;
                    }

                    int serviceDistance = ManhattanDistance(candidate, currentStage.serviceCell);
                    if (path.Count < bestPathLength || (path.Count == bestPathLength && serviceDistance < bestServiceDistance))
                    {
                        bestPathLength = path.Count;
                        bestServiceDistance = serviceDistance;
                        bestTarget = new ProductionWorkTarget(slotIndex, candidate);
                    }
                }
            }

            return bestTarget;
        }

        private bool IsWorkerWalkable(GridPosition position)
        {
            return IsInsideGrid(position) &&
                   position.y >= currentStage.workerAreaMinY &&
                   !IsBlockedForWorker(position);
        }

        private bool IsBlockedForWorker(GridPosition position)
        {
            if (ContainsPosition(currentStage.counterCells, position))
            {
                return true;
            }

            for (int i = 0; i < currentStage.products.Length; i++)
            {
                PrototypeProductSpec product = currentStage.products[i];
                if (IsSameGridPosition(product.standCell, position) || ContainsPosition(product.slotCells, position))
                {
                    return true;
                }
            }

            return false;
        }

        private bool ContainsPosition(GridPosition[] positions, GridPosition position)
        {
            if (positions == null)
            {
                return false;
            }

            for (int i = 0; i < positions.Length; i++)
            {
                if (IsSameGridPosition(positions[i], position))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsInsideGrid(GridPosition position)
        {
            return position.x >= 0 && position.x < gridWidth && position.y >= 0 && position.y < gridHeight;
        }

        private bool IsSameGridPosition(GridPosition a, GridPosition b)
        {
            return a.x == b.x && a.y == b.y;
        }

        private int ManhattanDistance(GridPosition a, GridPosition b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        private void TryUnlockProduct(int productIndex)
        {
            if (!IsValidProductIndex(productIndex))
            {
                return;
            }

            ProductRuntimeState state = products[productIndex];
            if (state.IsUnlocked)
            {
                TryUpgradeProduct(productIndex);
                return;
            }

            if (!IsProductUnlockAllowed(productIndex))
            {
                ShowToast(productIndex == 0 ? "주문 후 해금" : "1번 매대 먼저");
                return;
            }

            if (gold < state.Spec.unlockCost)
            {
                ShowToast("Gold 부족");
                return;
            }

            gold -= state.Spec.unlockCost;
            state.Level = 1;
            RefreshStaticLayer();
            ShowToast(state.Spec.displayName + " 해금");
        }

        private void TryActivateSlot(int productIndex, int slotIndex)
        {
            if (!IsValidProductIndex(productIndex))
            {
                return;
            }

            ProductRuntimeState state = products[productIndex];
            if (!state.IsUnlocked || !IsSlotVisible(state, slotIndex) || slotIndex != state.ActiveSlotCount)
            {
                return;
            }

            state.ActiveSlotCount++;
            RefreshStaticLayer();
            ShowToast("생산칸 활성화");
        }

        private void CreateStageUpgrades()
        {
            for (int i = 0; i < currentStage.products.Length; i++)
            {
                PrototypeProductSpec product = currentStage.products[i];
                double speedCost = i == 0 ? 50d : product.unlockCost * 6d;
                double rewardCost = i == 0 ? 1800d : product.unlockCost * 20d;
                stageUpgrades.Add(new StageUpgradeRuntimeState(
                    GetShortProductName(product.displayName) + " 속도",
                    "생산 x2",
                    speedCost,
                    StageUpgradeType.ProductProductionSpeed,
                    i,
                    2d,
                    1));
                stageUpgrades.Add(new StageUpgradeRuntimeState(
                    GetShortProductName(product.displayName) + " 보상",
                    "수익 x2",
                    rewardCost,
                    StageUpgradeType.ProductReward,
                    i,
                    2d,
                    1));
            }

            int customerUpgradeCount = Mathf.Max(0, currentStage.customerOrderCells.Length - 1);
            stageUpgrades.Add(new StageUpgradeRuntimeState("손님 증가", "최대 +1", 1200d, StageUpgradeType.MaxCustomers, -1, 1d, customerUpgradeCount));

            if (currentStageNumber >= 2)
            {
                stageUpgrades.Add(new StageUpgradeRuntimeState("직원 증가", "작업자 +1", 4000d, StageUpgradeType.Staff, -1, 1d, currentStage.staffWaitingCells.Length));
            }
        }

        private void TryBuyStageUpgrade(int upgradeIndex)
        {
            if (upgradeIndex < 0 || upgradeIndex >= stageUpgrades.Count)
            {
                return;
            }

            StageUpgradeRuntimeState upgrade = stageUpgrades[upgradeIndex];
            if (upgrade.PurchasedCount >= upgrade.MaxPurchaseCount)
            {
                ShowToast("이미 완료");
                return;
            }

            if (upgrade.ProductIndex >= 0 && (!IsValidProductIndex(upgrade.ProductIndex) || !products[upgrade.ProductIndex].IsUnlocked))
            {
                ShowToast("매대 해금 필요");
                return;
            }

            if (gold < upgrade.Cost)
            {
                ShowToast("Gold 부족");
                return;
            }

            gold -= upgrade.Cost;
            upgrade.PurchasedCount++;
            ApplyStageUpgrade(upgrade);
            RefreshStaticLayer();
            ShowToast(upgrade.DisplayName);
        }

        private void ApplyStageUpgrade(StageUpgradeRuntimeState upgrade)
        {
            switch (upgrade.Type)
            {
                case StageUpgradeType.ProductProductionSpeed:
                    products[upgrade.ProductIndex].ProductionSpeedMultiplier *= (float)upgrade.EffectValue;
                    break;
                case StageUpgradeType.ProductReward:
                    products[upgrade.ProductIndex].RewardMultiplier *= upgrade.EffectValue;
                    break;
                case StageUpgradeType.MaxCustomers:
                    maxCustomers += Mathf.RoundToInt((float)upgrade.EffectValue);
                    break;
                case StageUpgradeType.Staff:
                    AddStaffWorker();
                    break;
            }
        }

        private bool CanBuyStageUpgrade(int upgradeIndex)
        {
            if (upgradeIndex < 0 || upgradeIndex >= stageUpgrades.Count)
            {
                return false;
            }

            StageUpgradeRuntimeState upgrade = stageUpgrades[upgradeIndex];
            if (upgrade.PurchasedCount >= upgrade.MaxPurchaseCount || gold < upgrade.Cost)
            {
                return false;
            }

            return upgrade.ProductIndex < 0 || (IsValidProductIndex(upgrade.ProductIndex) && products[upgrade.ProductIndex].IsUnlocked);
        }

        private void AddStaffWorker()
        {
            int staffIndex = workers.Count - 1;
            if (staffIndex < 0 || staffIndex >= currentStage.staffWaitingCells.Length)
            {
                ShowToast("직원 자리 부족");
                return;
            }

            WorkerRuntimeState worker = CreateWorker("Staff " + (staffIndex + 1), currentStage.staffWaitingCells[staffIndex], new Color(0.18f, 0.72f, 0.68f), "직");
            workers.Add(worker);
            StartCoroutine(WorkerLoop(worker));
        }

        private bool TryUpgradeProduct(int productIndex)
        {
            if (!IsValidProductIndex(productIndex))
            {
                return false;
            }

            ProductRuntimeState state = products[productIndex];
            if (!state.IsUnlocked)
            {
                return false;
            }

            if (state.Level >= state.Spec.maxLevel)
            {
                ShowToast("최대 레벨");
                return false;
            }

            double cost = GetUpgradeCost(state);
            if (gold < cost)
            {
                ShowToast("Gold 부족");
                return false;
            }

            int previousStars = state.Stars;
            gold -= cost;
            state.Level++;
            state.Stars = CalculateStars(state.Level, state.Spec.maxStars);

            if (state.Stars > previousStars)
            {
                diamond += state.Stars - previousStars;
                RefreshStaticLayer();
                ShowToast("★ 보너스 수익 2배 / Diamond +" + (state.Stars - previousStars));
            }
            else
            {
                RefreshStaticLayer();
                ShowToast("Lv " + state.Level);
            }

            return true;
        }

        private void TryBuyNextStage()
        {
            if (currentStageNumber >= 3)
            {
                ShowToast("MVP 마지막");
                return;
            }

            if (!AreAllProductsMaxed())
            {
                ShowToast("모든 매대 Max 필요");
                return;
            }

            double cost = GetNextStageCost();
            if (gold < cost)
            {
                ShowToast("이동 비용 " + FormatGold(cost));
                return;
            }

            gold -= cost;
            LoadStage(currentStageNumber + 1);
        }

        private bool AreAllProductsMaxed()
        {
            for (int i = 0; i < products.Count; i++)
            {
                if (!products[i].IsUnlocked || products[i].Level < products[i].Spec.maxLevel)
                {
                    return false;
                }
            }

            return products.Count > 0;
        }

        private double GetNextStageCost()
        {
            double highestFinalUpgradeCost = 0d;
            for (int i = 0; i < products.Count; i++)
            {
                highestFinalUpgradeCost = System.Math.Max(highestFinalUpgradeCost, GetUpgradeCost(products[i].Spec, products[i].Spec.maxLevel));
            }

            return System.Math.Ceiling(highestFinalUpgradeCost * 1.5d);
        }

        private int PickOrderProductIndex()
        {
            List<int> eligible = new List<int>();
            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].IsUnlocked && products[i].ActiveSlotCount > 0)
                {
                    eligible.Add(i);
                }
            }

            if (eligible.Count == 0)
            {
                return 0;
            }

            return eligible[Random.Range(0, eligible.Count)];
        }

        private bool IsProductUnlockAllowed(int productIndex)
        {
            if (productIndex == 0)
            {
                return firstOrderReceived;
            }

            return products.Count > 0 && products[0].IsUnlocked;
        }

        private bool CanRepeatProductUpgrade(int productIndex)
        {
            if (!IsValidProductIndex(productIndex))
            {
                return false;
            }

            ProductRuntimeState state = products[productIndex];
            return state.IsUnlocked && state.Level < state.Spec.maxLevel && gold >= GetUpgradeCost(state);
        }

        private bool CanServe(ProductRuntimeState product)
        {
            return product.IsUnlocked && product.ActiveSlotCount > 0;
        }

        private bool IsSlotVisible(ProductRuntimeState state, int slotIndex)
        {
            if (!state.IsUnlocked)
            {
                return false;
            }

            if (slotIndex == 0)
            {
                return true;
            }

            return state.Stars >= state.Spec.slotRequiredStars[slotIndex];
        }

        private double GetReward(ProductRuntimeState state)
        {
            double levelGrowth = 1d + Mathf.Max(0, state.Level - 1) * 0.18d;
            double starMultiplier = System.Math.Pow(2d, state.Stars);
            return state.Spec.baseReward * levelGrowth * starMultiplier * state.RewardMultiplier;
        }

        private double GetUpgradeCost(ProductRuntimeState state)
        {
            return GetUpgradeCost(state.Spec, state.Level);
        }

        private double GetUpgradeCost(PrototypeProductSpec spec, int level)
        {
            double baseCost = spec.baseReward * 1.35d;
            return System.Math.Ceiling(baseCost * System.Math.Pow(1.2d, level));
        }

        private int CalculateStars(int level, int maxStars)
        {
            int stars = 0;
            for (int i = 0; i < StarLevels.Length && i < maxStars; i++)
            {
                if (level >= StarLevels[i])
                {
                    stars++;
                }
            }

            return stars;
        }

        private double GetDebugGoldAmount()
        {
            if (currentStageNumber == 1)
            {
                return 1000d;
            }

            if (currentStageNumber == 2)
            {
                return 25000d;
            }

            return 250000d;
        }

        private void SetCustomerBubble(CustomerRuntimeState customerState, string text)
        {
            if (customerState != null && customerState.BubbleText != null)
            {
                customerState.BubbleText.text = text;
            }
        }

        private void ShowToast(string message)
        {
            if (toastText != null)
            {
                toastText.text = message;
            }

            toastHideAt = Time.time + 2f;
        }

        private void UpdateStatus()
        {
            if (statusText != null)
            {
                statusText.text = "S" + currentStageNumber + "  G " + FormatGold(gold) + "  D " + diamond +
                                  "  C " + activeCustomers.Count + "/" + maxCustomers +
                                  "  W " + workers.Count;
            }
        }

        private string FormatGold(double value)
        {
            if (value >= 1000000000000d)
            {
                return (value / 1000000000000d).ToString("0.##", CultureInfo.InvariantCulture) + "t";
            }

            if (value >= 1000000000d)
            {
                return (value / 1000000000d).ToString("0.##", CultureInfo.InvariantCulture) + "b";
            }

            if (value >= 1000000d)
            {
                return (value / 1000000d).ToString("0.##", CultureInfo.InvariantCulture) + "m";
            }

            if (value >= 1000d)
            {
                return (value / 1000d).ToString("0.##", CultureInfo.InvariantCulture) + "k";
            }

            return value.ToString("0", CultureInfo.InvariantCulture);
        }

        private bool IsValidProductIndex(int productIndex)
        {
            return productIndex >= 0 && productIndex < products.Count;
        }

        private string GetShortProductName(string displayName)
        {
            if (string.IsNullOrEmpty(displayName) || displayName.Length <= 2)
            {
                return displayName;
            }

            return displayName.Substring(0, 2);
        }

        private GameObject CreateCell(string name, GridPosition gridPosition, Color color, float size, Transform parent)
        {
            return CreateBox(name, GridToWorld(gridPosition), new Vector3(size, size, 0.05f), color, parent);
        }

        private GameObject CreateBox(string name, Vector3 position, Vector3 scale, Color color, Transform parent)
        {
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = name;
            box.transform.SetParent(parent, false);
            box.transform.position = position;
            box.transform.localScale = scale;
            SetRendererColor(box, color);
            return box;
        }

        private TextMesh CreateText(string name, string text, Vector3 position, float characterSize, Color color, Transform parent)
        {
            GameObject textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            textObject.transform.position = position;
            TextMesh mesh = textObject.AddComponent<TextMesh>();
            mesh.text = text;
            mesh.anchor = TextAnchor.MiddleCenter;
            mesh.alignment = TextAlignment.Center;
            mesh.characterSize = characterSize;
            mesh.fontSize = 64;
            mesh.color = color;
            return mesh;
        }

        private void CreateUpgradeBadge(string name, GridPosition gridPosition, Transform parent, PrototypeClickAction action, int productIndex, int slotIndex)
        {
            Vector3 position = GridToWorld(gridPosition) + new Vector3(0.36f, 0.36f, -0.85f);
            GameObject badge = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            badge.name = name;
            badge.transform.SetParent(parent, false);
            badge.transform.position = position;
            badge.transform.localScale = new Vector3(0.42f, 0.42f, 0.42f);
            SetRendererColor(badge, new Color(0.95f, 0.18f, 0.18f));
            AddClickTarget(badge, action, productIndex, slotIndex);
            AddHitTarget(position, new Vector2(0.8f, 0.8f), action, 0, productIndex, slotIndex);
            CreateText(name + " Arrow", "↑", position + new Vector3(0f, -0.01f, -0.45f), 0.07f, Color.white, parent);
        }

        private void AddClickTarget(GameObject target, PrototypeClickAction action, int productIndex, int slotIndex, int stageNumber = 0)
        {
            PrototypeClickTarget clickTarget = target.AddComponent<PrototypeClickTarget>();
            clickTarget.bootstrap = this;
            clickTarget.action = action;
            clickTarget.stageNumber = stageNumber;
            clickTarget.productIndex = productIndex;
            clickTarget.slotIndex = slotIndex;
        }

        private void AddHitTarget(Vector3 center, Vector2 size, PrototypeClickAction action, int stageNumber, int productIndex, int slotIndex)
        {
            hitTargets.Add(new PrototypeHitTarget(center, size, action, stageNumber, productIndex, slotIndex));
        }

        private void SetRendererColor(GameObject target, Color color)
        {
            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer == null)
            {
                return;
            }

            Material material = CreateColorMaterial(color);
            renderer.sharedMaterial = material;
        }

        private Material CreateColorMaterial(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }

            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }

            Material material = new Material(shader);
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }

            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", color);
            }

            createdMaterials.Add(material);
            return material;
        }

        private Color GetFloorColor(int y)
        {
            PrototypeThemeSkin skin = GetThemeSkin();
            if (y <= 0)
            {
                return skin.entranceFloorColor;
            }

            if (y <= 10)
            {
                return skin.customerFloorColor;
            }

            if (y == 11)
            {
                return skin.customerOrderFloorColor;
            }

            if (y == 12)
            {
                return skin.counterFloorColor;
            }

            if (y <= 16)
            {
                return skin.workerFloorColor;
            }

            if (y <= 19)
            {
                return skin.workerBackFloorColor;
            }

            return skin.workerBackFloorColor;
        }

        private PrototypeThemeSkin GetThemeSkin()
        {
            if (currentThemeSkin == null)
            {
                currentThemeSkin = CreateAmusementParkThemeSkin();
            }

            return currentThemeSkin;
        }

        private Color GetProductStandColor(int productIndex)
        {
            PrototypeThemeSkin skin = GetThemeSkin();
            if (skin.standColors == null || skin.standColors.Length == 0)
            {
                return new Color(1f, 0.82f, 0.36f);
            }

            return skin.standColors[Mathf.Abs(productIndex) % skin.standColors.Length];
        }

        private PrototypeThemeSkin CreateAmusementParkThemeSkin()
        {
            return new PrototypeThemeSkin
            {
                displayName = "AMUSE PARK",
                entranceFloorColor = new Color(0.74f, 0.9f, 0.76f),
                customerFloorColor = new Color(0.95f, 0.89f, 0.74f),
                customerOrderFloorColor = new Color(0.77f, 0.9f, 1f),
                counterFloorColor = new Color(1f, 0.83f, 0.5f),
                workerFloorColor = new Color(0.88f, 0.93f, 0.95f),
                workerBackFloorColor = new Color(0.78f, 0.85f, 0.89f),
                counterColor = new Color(0.96f, 0.65f, 0.32f),
                customerOrderColor = new Color(0.66f, 0.87f, 1f),
                customerDecorationColor = new Color(0.98f, 0.8f, 0.48f),
                lockedStandColor = new Color(0.68f, 0.68f, 0.68f),
                activeSlotColor = new Color(0.56f, 0.88f, 0.58f),
                inactiveSlotColor = new Color(0.96f, 0.94f, 0.88f),
                staffWaitColor = new Color(0.74f, 0.82f, 0.9f),
                accentColor = new Color(0.95f, 0.28f, 0.32f),
                titleColor = new Color(0.25f, 0.23f, 0.34f),
                ticketBoothColor = new Color(0.57f, 0.83f, 0.96f),
                balloonColor = new Color(0.98f, 0.42f, 0.56f),
                benchColor = new Color(0.77f, 0.56f, 0.36f),
                pathStripeColor = new Color(1f, 0.95f, 0.64f),
                standColors = new[]
                {
                    new Color(1f, 0.78f, 0.36f),
                    new Color(0.96f, 0.42f, 0.38f),
                    new Color(0.58f, 0.77f, 1f),
                    new Color(0.8f, 0.62f, 0.96f)
                }
            };
        }

        private Vector3 GridToWorld(GridPosition gridPosition)
        {
            float worldX = (gridPosition.x - (gridWidth - 1) * 0.5f) * cellSize;
            float worldY = ((gridHeight - 1) * 0.5f - gridPosition.y) * cellSize;
            return new Vector3(worldX, worldY, 0f);
        }

        private void CreateCamera()
        {
            Camera existingCamera = Camera.main;
            if (existingCamera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                existingCamera = cameraObject.AddComponent<Camera>();
                cameraObject.tag = "MainCamera";
            }

            existingCamera.transform.position = new Vector3(0f, -0.25f, -15f);
            existingCamera.transform.rotation = Quaternion.identity;
            existingCamera.orthographic = true;
            existingCamera.backgroundColor = new Color(0.12f, 0.13f, 0.15f);
            existingCamera.clearFlags = CameraClearFlags.SolidColor;
            ApplyPrototypeCamera(existingCamera);
        }

        private void ApplyPrototypeCamera(Camera targetCamera)
        {
            if (targetCamera == null)
            {
                return;
            }

            float visibleRows = Mathf.Clamp(cameraVisibleRows, 4f, Mathf.Max(4f, gridHeight + 4f));
            float offsetY = cameraVerticalOffsetCells * cellSize;
            targetCamera.transform.position = new Vector3(0f, offsetY, -15f);
            targetCamera.transform.rotation = Quaternion.identity;
            targetCamera.orthographic = true;
            targetCamera.orthographicSize = visibleRows * cellSize * 0.5f;
        }

        private Transform CreateRoot(string name)
        {
            GameObject rootObject = new GameObject(name);
            rootObject.transform.SetParent(transform, false);
            return rootObject.transform;
        }

        private void EnsureUiRoot()
        {
            if (uiRoot != null)
            {
                return;
            }

            uiRoot = CreateRoot("UI Prototype");
        }

        private void EnsureEventSystem()
        {
            if (EventSystem.current != null)
            {
                return;
            }

            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        private Font GetUiFont()
        {
            if (uiFont != null)
            {
                return uiFont;
            }

            uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (uiFont == null)
            {
                uiFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            return uiFont;
        }

        private void ClearGeneratedScene()
        {
            ClearChildren(transform);
            staticRoot = null;
            dynamicRoot = null;
            uiRoot = null;

            for (int i = 0; i < createdMaterials.Count; i++)
            {
                if (createdMaterials[i] != null)
                {
                    Destroy(createdMaterials[i]);
                }
            }

            createdMaterials.Clear();
        }

        private void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        private PrototypeStageSpec CreateStageSpec(int stageNumber)
        {
            if (stageNumber == 1)
            {
                return new PrototypeStageSpec
                {
                    stageNumber = 1,
                    displayName = "놀이동산 1 - 솜사탕",
                    startingGold = 5d,
                    entranceCell = new GridPosition(5, 0),
                    exitCell = new GridPosition(5, 0),
                    mainStartCell = new GridPosition(5, 13),
                    serviceCell = new GridPosition(5, 13),
                    workerAreaMinY = 13,
                    customerOrderCells = new[] { new GridPosition(5, 11), new GridPosition(4, 11), new GridPosition(6, 11) },
                    counterCells = CreateHorizontalCells(4, 6, 12),
                    staffWaitingCells = new[] { new GridPosition(4, 13) },
                    products = new[]
                    {
                        new PrototypeProductSpec("cotton_candy", "솜사탕", 5d, 5d, 5f, 25, 2, new GridPosition(4, 15), new[] { new GridPosition(5, 15) }, new[] { 0 })
                    }
                };
            }

            if (stageNumber == 2)
            {
                return new PrototypeStageSpec
                {
                    stageNumber = 2,
                    displayName = "놀이동산 2 - 간식 매장",
                    startingGold = 5d,
                    entranceCell = new GridPosition(5, 0),
                    exitCell = new GridPosition(5, 0),
                    mainStartCell = new GridPosition(5, 13),
                    serviceCell = new GridPosition(5, 13),
                    workerAreaMinY = 13,
                    customerOrderCells = new[] { new GridPosition(5, 11), new GridPosition(4, 11), new GridPosition(6, 11), new GridPosition(3, 11), new GridPosition(7, 11) },
                    counterCells = CreateHorizontalCells(3, 7, 12),
                    staffWaitingCells = new[] { new GridPosition(4, 13), new GridPosition(6, 13) },
                    products = new[]
                    {
                        new PrototypeProductSpec("bead_icecream", "구슬아이스크림", 5d, 5d, 5f, 75, 4, new GridPosition(3, 15), new[] { new GridPosition(4, 15) }, new[] { 0 }),
                        new PrototypeProductSpec("chicken_skewer", "닭꼬치", 250d, 40d, 9f, 75, 4, new GridPosition(6, 15), new[] { new GridPosition(7, 15), new GridPosition(7, 16) }, new[] { 0, 1 })
                    }
                };
            }

            return new PrototypeStageSpec
            {
                stageNumber = 3,
                displayName = "놀이동산 3 - 푸드 코트",
                startingGold = 5d,
                entranceCell = new GridPosition(5, 0),
                exitCell = new GridPosition(5, 0),
                mainStartCell = new GridPosition(5, 13),
                serviceCell = new GridPosition(5, 13),
                workerAreaMinY = 13,
                customerOrderCells = new[] { new GridPosition(5, 11), new GridPosition(4, 11), new GridPosition(6, 11), new GridPosition(3, 11), new GridPosition(7, 11), new GridPosition(2, 11), new GridPosition(8, 11) },
                counterCells = CreateHorizontalCells(2, 8, 12),
                staffWaitingCells = new[] { new GridPosition(4, 13), new GridPosition(6, 13), new GridPosition(5, 14) },
                products = new[]
                {
                    new PrototypeProductSpec("popcorn", "팝콘", 5d, 5d, 5f, 75, 4, new GridPosition(2, 15), new[] { new GridPosition(3, 15) }, new[] { 0 }),
                    new PrototypeProductSpec("hotdog", "핫도그", 250d, 40d, 9f, 75, 4, new GridPosition(5, 15), new[] { new GridPosition(6, 15), new GridPosition(6, 16) }, new[] { 0, 1 }),
                    new PrototypeProductSpec("churros", "츄러스", 12500d, 700d, 13f, 75, 4, new GridPosition(8, 15), new[] { new GridPosition(9, 15), new GridPosition(9, 16) }, new[] { 0, 1 })
                }
            };
        }

        private GridPosition[] CreateHorizontalCells(int startX, int endX, int y)
        {
            GridPosition[] cells = new GridPosition[endX - startX + 1];
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = new GridPosition(startX + i, y);
            }

            return cells;
        }

        private sealed class PrototypeStageSpec
        {
            public int stageNumber;
            public string displayName;
            public double startingGold;
            public GridPosition entranceCell;
            public GridPosition exitCell;
            public GridPosition mainStartCell;
            public GridPosition serviceCell;
            public int workerAreaMinY;
            public GridPosition[] customerOrderCells;
            public GridPosition[] counterCells;
            public GridPosition[] staffWaitingCells;
            public PrototypeProductSpec[] products;
        }

        private sealed class PrototypeThemeSkin
        {
            public string displayName;
            public Color entranceFloorColor;
            public Color customerFloorColor;
            public Color customerOrderFloorColor;
            public Color counterFloorColor;
            public Color workerFloorColor;
            public Color workerBackFloorColor;
            public Color counterColor;
            public Color customerOrderColor;
            public Color customerDecorationColor;
            public Color lockedStandColor;
            public Color activeSlotColor;
            public Color inactiveSlotColor;
            public Color staffWaitColor;
            public Color accentColor;
            public Color titleColor;
            public Color ticketBoothColor;
            public Color balloonColor;
            public Color benchColor;
            public Color pathStripeColor;
            public Color[] standColors;
        }

        private sealed class PrototypeProductSpec
        {
            public readonly string id;
            public readonly string displayName;
            public readonly double unlockCost;
            public readonly double baseReward;
            public readonly float productionSeconds;
            public readonly int maxLevel;
            public readonly int maxStars;
            public readonly GridPosition standCell;
            public readonly GridPosition[] slotCells;
            public readonly int[] slotRequiredStars;

            public PrototypeProductSpec(string id, string displayName, double unlockCost, double baseReward, float productionSeconds, int maxLevel, int maxStars, GridPosition standCell, GridPosition[] slotCells, int[] slotRequiredStars)
            {
                this.id = id;
                this.displayName = displayName;
                this.unlockCost = unlockCost;
                this.baseReward = baseReward;
                this.productionSeconds = productionSeconds;
                this.maxLevel = maxLevel;
                this.maxStars = maxStars;
                this.standCell = standCell;
                this.slotCells = slotCells;
                this.slotRequiredStars = slotRequiredStars;
            }
        }

        private sealed class ProductRuntimeState
        {
            public readonly PrototypeProductSpec Spec;
            private readonly bool[] slotOccupied;
            public int Level;
            public int Stars;
            public int ActiveSlotCount;
            public float ProductionSpeedMultiplier = 1f;
            public double RewardMultiplier = 1d;

            public ProductRuntimeState(PrototypeProductSpec spec)
            {
                Spec = spec;
                slotOccupied = new bool[spec.slotCells.Length];
            }

            public bool IsUnlocked
            {
                get { return Level > 0; }
            }

            public bool IsSlotOccupied(int slotIndex)
            {
                return slotIndex >= 0 && slotIndex < slotOccupied.Length && slotOccupied[slotIndex];
            }

            public void SetSlotOccupied(int slotIndex, bool occupied)
            {
                if (slotIndex >= 0 && slotIndex < slotOccupied.Length)
                {
                    slotOccupied[slotIndex] = occupied;
                }
            }
        }

        private sealed class CustomerRuntimeState
        {
            public int Id;
            public GameObject Agent;
            public TextMesh BubbleText;
            public int OrderSlotIndex;
            public GridPosition OrderCell;
            public readonly List<CustomerOrderItem> OrderItems = new List<CustomerOrderItem>();
            public int RemainingItems;
        }

        private readonly struct CustomerOrderItem
        {
            public readonly int ProductIndex;
            public readonly int Quantity;

            public CustomerOrderItem(int productIndex, int quantity)
            {
                ProductIndex = productIndex;
                Quantity = quantity;
            }
        }

        private sealed class ProductWorkTask
        {
            public readonly CustomerRuntimeState Customer;
            public readonly int ProductIndex;

            public ProductWorkTask(CustomerRuntimeState customer, int productIndex)
            {
                Customer = customer;
                ProductIndex = productIndex;
            }
        }

        private sealed class WorkAssignment
        {
            public readonly ProductWorkTask Task;
            public readonly ProductionWorkTarget Target;

            public WorkAssignment(ProductWorkTask task, ProductionWorkTarget target)
            {
                Task = task;
                Target = target;
            }
        }

        private sealed class WorkerRuntimeState
        {
            public int Id;
            public GameObject Agent;
            public GridPosition GridPosition;
        }

        private enum StageUpgradeType
        {
            ProductProductionSpeed,
            ProductReward,
            MaxCustomers,
            Staff
        }

        private sealed class StageUpgradeRuntimeState
        {
            public readonly string DisplayName;
            public readonly string Description;
            public readonly double Cost;
            public readonly StageUpgradeType Type;
            public readonly int ProductIndex;
            public readonly double EffectValue;
            public readonly int MaxPurchaseCount;
            public int PurchasedCount;

            public StageUpgradeRuntimeState(string displayName, string description, double cost, StageUpgradeType type, int productIndex, double effectValue, int maxPurchaseCount)
            {
                DisplayName = displayName;
                Description = description;
                Cost = cost;
                Type = type;
                ProductIndex = productIndex;
                EffectValue = effectValue;
                MaxPurchaseCount = maxPurchaseCount;
            }
        }

        private readonly struct ProductionWorkTarget
        {
            public static readonly ProductionWorkTarget Invalid = new ProductionWorkTarget(-1, new GridPosition(-1, -1));

            public readonly int SlotIndex;
            public readonly GridPosition WorkCell;

            public ProductionWorkTarget(int slotIndex, GridPosition workCell)
            {
                SlotIndex = slotIndex;
                WorkCell = workCell;
            }

            public bool IsValid
            {
                get { return SlotIndex >= 0; }
            }
        }

        private readonly struct PrototypeHitTarget
        {
            public readonly Vector3 Center;
            public readonly Vector2 Size;
            public readonly PrototypeClickAction Action;
            public readonly int StageNumber;
            public readonly int ProductIndex;
            public readonly int SlotIndex;

            public PrototypeHitTarget(Vector3 center, Vector2 size, PrototypeClickAction action, int stageNumber, int productIndex, int slotIndex)
            {
                Center = center;
                Size = size;
                Action = action;
                StageNumber = stageNumber;
                ProductIndex = productIndex;
                SlotIndex = slotIndex;
            }

            public bool Contains(Vector3 world)
            {
                return Mathf.Abs(world.x - Center.x) <= Size.x * 0.5f &&
                       Mathf.Abs(world.y - Center.y) <= Size.y * 0.5f;
            }
        }
    }
}
