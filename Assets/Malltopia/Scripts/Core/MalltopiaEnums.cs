namespace Malltopia
{
    public enum StageScrollMode
    {
        FixedNoScroll
    }

    public enum StageClearConditionType
    {
        AllProductsMaxLevel
    }

    public enum NextStageCostRule
    {
        MaxFinalUpgradeCostMultiplier
    }

    public enum StageLayoutPointType
    {
        Entrance,
        Exit,
        CustomerPath,
        CustomerOrder,
        CustomerOrderSide,
        CustomerOrderBack,
        Counter,
        WorkerService,
        WorkerPath,
        ProductStand,
        ProductionSlot,
        ReservedObstacle,
        MainCharacterStart,
        StaffWaiting,
        DecorationArea
    }

    public enum LevelGrowthType
    {
        Add,
        Multiply,
        Fixed
    }

    public enum ProductionSlotActivationMode
    {
        FreeTap
    }

    public enum StaffRole
    {
        Generalist,
        Service,
        Production
    }

    public enum AddSource
    {
        StageUpgrade
    }

    public enum StageUpgradeTargetType
    {
        Product,
        Customer,
        Staff,
        Stage
    }

    public enum StageUpgradeEffectType
    {
        ProductionSpeedPct,
        RewardMultiplier,
        CustomerCountAdd,
        StaffCountAdd
    }

    public enum FirstOrderMode
    {
        StartingStandOnly
    }

    public enum EligibleProductRule
    {
        UnlockedAndProductionSlotActivated
    }

    public enum ProductSelectionRule
    {
        UniformEligibleProducts
    }

    public enum DistinctProductCountRule
    {
        UniformRange
    }

    public enum QuantityRule
    {
        UniformIntRange
    }

    public enum WorkAssignmentRule
    {
        OrderTakerThenProductUnitTasks
    }

    public enum GoldRewardTiming
    {
        PerProductDelivered
    }

    public enum RewardType
    {
        Chest,
        Diamond,
        Gold,
        Item
    }

    public enum VersionGateResult
    {
        Allowed,
        Maintenance,
        UpdateRequired
    }
}
