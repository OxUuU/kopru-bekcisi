using UnityEngine;
using UnityEngine.UI;

namespace KopruBekcisi.UI
{
    public class MarekNote : MonoBehaviour
    {
        [SerializeField] Button button;
        bool _opened;

        void Awake()
        {
            if (button == null) button = GetComponent<Button>();
            if (button != null) button.onClick.AddListener(Open);
        }

        void Open()
        {
            if (DialogPopup.Instance == null) return;
            string body = _opened
                ? "Notu tekrar okuyorsun. Marek'in eli titrek; mürekkep yer yer akmış."
                : "<i>Aren —\n\n" +
                  "Sygn'da gözleyiciler var. Hepsi kapıdan geçmiyor; bazıları suya iner, bazıları kömür gibi yanar. Köprünün altını kontrol et. Mistveil mültecisi gibi görünenlerin bazısı bizi seyrediyor.\n\n" +
                  "Engizisyon yarın geliyor. Sözüme güven: sahte mühür arama, parlak altın ara. Eğer o altını alırsan, beni de almışsın olur. Almazsan, beni geri çağıracaksın.\n\n" +
                  "— M.V.</i>\n\n" +
                  "(Sayfanın kenarında üç çizgi: tarikat sembolü mü?)";
            _opened = true;
            DialogPopup.Instance.Show("MAREK'İN NOTU (masa altı)", body, "Notu yerine bırak");
        }
    }
}
