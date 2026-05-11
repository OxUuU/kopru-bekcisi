using UnityEngine;
using KopruBekcisi.Core;
using KopruBekcisi.UI;

namespace KopruBekcisi.Faction
{
    /// <summary>
    /// Home sahnesinde gece tetiklenen fraksiyon olayı.
    /// M9 vertical slice'ında: Aydınlık Engizisyonu temsilcisi ziyareti.
    /// </summary>
    public class NightVisitor : MonoBehaviour
    {
        bool _shown;

        void Start()
        {
            // Bir frame sonra göster ki UI Canvas/DialogPopup hazır olsun
            Invoke(nameof(TriggerVisit), 0.4f);
        }

        void TriggerVisit()
        {
            if (_shown) return;
            _shown = true;
            if (DialogPopup.Instance == null) return;

            var dir = GameDirector.Instance;
            int karma = dir?.Karma?.Score ?? 0;

            string title = "GECE ZİYARETİ — Engizisyon Yargıcı Aldreth";
            string body;

            if (karma <= -2)
            {
                body =
                    "Kapı çalınır. Eşikte siyah cüppeli bir adam: <b>Yargıç Aldreth</b>.\n\n" +
                    "<i>\"Bekçi Kross. Kayıtlarımı inceledim. Bugün Aydınlık'ın gözünde temiz değilsin. " +
                    "Suçluyu serbest bıraktığını duydum. Bu bir kez olur. İki kez olursa, " +
                    "bir sonraki ziyaretim öğrenci ile değil, demir ile olur.\"</i>\n\n" +
                    "Idris perdenin arkasından sessizce öksürür. Aldreth onu duymamış gibi yapar.";
                DialogPopup.Instance.Show(title, body, "Başını eğip onaylar", () => RecordReputation(-2));
            }
            else if (karma >= 2)
            {
                body =
                    "Kapı nazikçe çalınır. Genç bir noviş: <b>Aldreth'in habercisi</b>.\n\n" +
                    "<i>\"Yargıç Aldreth selamını yolluyor. Bugünkü işin Aydınlık'a hizmet etti. " +
                    "Krallık adına bu küçük desteği kabul et.\"</i>\n\n" +
                    "Genç adam bir kese altın bırakır ve kaybolur.";
                if (dir != null) dir.Economy.Add(20);
                DialogPopup.Instance.Show(title, body, "Keseyi al", () => RecordReputation(2));
            }
            else
            {
                body =
                    "Kapı çalınır. <b>Yargıç Aldreth</b> kapıda — yüzü ne sıcak ne soğuk.\n\n" +
                    "<i>\"Köprünün üstünde bir gözüm seni izliyor, Bekçi Kross. " +
                    "Yarın daha dikkatli ol. Mistveil'den gelen herkes Aydınlık'a düşman değildir, " +
                    "ama köprü hatırlamaz; yargı hatırlar.\"</i>\n\n" +
                    "Aldreth Marek'in adını anmaz, ama bakışında bir şey saklı.";
                DialogPopup.Instance.Show(title, body, "Yargıcı uğurla", () => RecordReputation(0));
            }
        }

        void RecordReputation(int delta)
        {
            // GameDirector'da FactionService yok şu an — basit Karma adjust
            var dir = GameDirector.Instance;
            if (dir != null && delta != 0) dir.Karma.Adjust(delta);
            Debug.Log($"[NightVisitor] Engizisyon ziyareti tamamlandı. (karma delta: {delta})");
        }
    }
}
