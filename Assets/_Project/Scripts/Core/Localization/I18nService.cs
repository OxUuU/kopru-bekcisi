using System.Collections.Generic;

namespace KopruBekcisi.Core.Localization
{
    /// <summary>
    /// Vertical slice için hard-coded TR string sözlüğü.
    /// M10 sonrası Unity Localization Package ile değiştirilecek.
    /// Tüm UI metinleri buradan key ile alınmalı (literal string yazılmamalı).
    /// </summary>
    public static class I18nService
    {
        public enum Lang { TR, EN }

        public static Lang Current = Lang.TR;

        static readonly Dictionary<string, string> TR = new()
        {
            // Verdict aksiyonları
            { "verdict.approve", "İçeri Al" },
            { "verdict.deny", "Geri Gönder" },
            { "verdict.detain", "Zindana At" },
            { "verdict.execute", "İdam Et" },

            // Inspection tools
            { "tool.magnifier", "🔍\nMercek" },
            { "tool.lantern", "🔦\nFener" },

            // Status / phases
            { "status.day", "Gün" },
            { "status.gold", "Altın" },
            { "status.karma", "Karma" },
            { "status.queue", "Sırada" },

            // Home
            { "home.food", "Yiyecek (5 altın)" },
            { "home.medicine", "İlaç (15 altın)" },
            { "home.candle", "Mum (3 altın)" },
            { "home.school", "Okul Ücreti (8 altın)" },
            { "home.sleep", "Mumu Söndür ve Uyu" },

            // Dialog
            { "dialog.ok", "Tamam" },
            { "dialog.cancel", "Vazgeç" },
        };

        static readonly Dictionary<string, string> EN = new()
        {
            { "verdict.approve", "Approve" },
            { "verdict.deny", "Deny" },
            { "verdict.detain", "Detain" },
            { "verdict.execute", "Execute" },
            { "tool.magnifier", "🔍\nLens" },
            { "tool.lantern", "🔦\nLantern" },
            { "status.day", "Day" },
            { "status.gold", "Gold" },
            { "status.karma", "Karma" },
            { "status.queue", "Queue" },
            { "home.food", "Food (5g)" },
            { "home.medicine", "Medicine (15g)" },
            { "home.candle", "Candle (3g)" },
            { "home.school", "School (8g)" },
            { "home.sleep", "Snuff & Sleep" },
            { "dialog.ok", "OK" },
            { "dialog.cancel", "Cancel" },
        };

        public static string T(string key)
        {
            var dict = Current == Lang.EN ? EN : TR;
            return dict.TryGetValue(key, out var v) ? v : key;
        }
    }
}
