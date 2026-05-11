using System;
using System.Collections.Generic;

namespace KopruBekcisi.Faction
{
    public enum FactionId
    {
        Crown,
        Inquisition,
        HiddenHand,
        OldCults
    }

    public class FactionService
    {
        readonly Dictionary<FactionId, int> _reputation = new()
        {
            { FactionId.Crown, 0 },
            { FactionId.Inquisition, 0 },
            { FactionId.HiddenHand, 0 },
            { FactionId.OldCults, 0 }
        };

        public event Action<FactionId, int> ReputationChanged;

        public int Get(FactionId id) => _reputation[id];

        public void Adjust(FactionId id, int delta)
        {
            if (delta == 0) return;
            _reputation[id] += delta;
            ReputationChanged?.Invoke(id, _reputation[id]);
        }
    }
}
