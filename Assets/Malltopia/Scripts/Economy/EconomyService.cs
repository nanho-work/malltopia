namespace Malltopia
{
    public class EconomyService
    {
        public double Gold { get; private set; }
        public int Diamond { get; private set; }
        public int StarDust { get; private set; }

        public EconomyService(double startingGold, int startingDiamond, int startingStarDust)
        {
            Gold = CurrencyMath.Sanitize(startingGold);
            Diamond = SanitizeInt(startingDiamond);
            StarDust = SanitizeInt(startingStarDust);
        }

        public bool CanSpendGold(double amount)
        {
            amount = CurrencyMath.Sanitize(amount);
            return Gold >= amount;
        }

        public bool TrySpendGold(double amount)
        {
            amount = CurrencyMath.Sanitize(amount);

            if (!CanSpendGold(amount))
            {
                return false;
            }

            Gold = CurrencyMath.Sanitize(Gold - amount);
            return true;
        }

        public void AddGold(double amount)
        {
            Gold = CurrencyMath.Sanitize(Gold + CurrencyMath.Sanitize(amount));
        }

        public bool CanSpendDiamond(int amount)
        {
            amount = SanitizeInt(amount);
            return Diamond >= amount;
        }

        public bool TrySpendDiamond(int amount)
        {
            amount = SanitizeInt(amount);

            if (!CanSpendDiamond(amount))
            {
                return false;
            }

            Diamond -= amount;
            return true;
        }

        public void AddDiamond(int amount)
        {
            Diamond += SanitizeInt(amount);
        }

        public void AddStarDust(int amount)
        {
            StarDust += SanitizeInt(amount);
        }

        private static int SanitizeInt(int value)
        {
            return value < 0 ? 0 : value;
        }
    }
}
