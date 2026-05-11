using System;

namespace KopruBekcisi.Karma
{
    public class KarmaService
    {
        public int Score { get; private set; }
        public event Action<int> KarmaChanged;

        public void Adjust(int delta)
        {
            if (delta == 0) return;
            Score += delta;
            KarmaChanged?.Invoke(Score);
        }
    }
}
