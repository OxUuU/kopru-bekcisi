using System;
using System.Collections.Generic;

namespace KopruBekcisi.Core.Save
{
    [Serializable]
    public class SaveData
    {
        public int version = 1;
        public int dayNumber = 1;
        public int gold = 20;
        public int karma = 0;
        public List<string> seenKingdoms = new();
        public string savedAtIso;
    }
}
