using UnityEngine;

namespace Malltopia
{
    public enum PrototypeClickAction
    {
        None,
        SwitchStage,
        GrantGold,
        ResetStage,
        UnlockProduct,
        ActivateSlot,
        UpgradeProduct,
        ToggleStageUpgradeMenu,
        BuyStageUpgrade,
        BuyNextStage,
        OpenVault,
        OpenCharacter
    }

    public sealed class PrototypeClickTarget : MonoBehaviour
    {
        public StagePrototypeBootstrap bootstrap;
        public PrototypeClickAction action;
        public int stageNumber;
        public int productIndex;
        public int slotIndex;
    }
}
