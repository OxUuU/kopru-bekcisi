using System;

namespace KopruBekcisi.Economy
{
    public class EconomyService
    {
        public int Gold { get; private set; }
        public event Action<int> GoldChanged;

        public EconomyService(int starting = 0) { Gold = starting; }

        public void Add(int amount)
        {
            if (amount == 0) return;
            Gold += amount;
            GoldChanged?.Invoke(Gold);
        }

        public bool TrySpend(int amount)
        {
            if (amount < 0) throw new ArgumentException("Negatif harcama yok.", nameof(amount));
            if (Gold < amount) return false;
            Gold -= amount;
            GoldChanged?.Invoke(Gold);
            return true;
        }
    }
}
