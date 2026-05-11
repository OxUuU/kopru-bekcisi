using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KopruBekcisi.Core;
using KopruBekcisi.Economy;

namespace KopruBekcisi.Home
{
    public enum HomeNeed { Food, Medicine, Candle, School }

    public class HomeManager : MonoBehaviour
    {
        [SerializeField] Text statusText;
        [SerializeField] Text familyStatusText;
        [SerializeField] Text feedbackText;
        [SerializeField] Button foodButton;
        [SerializeField] Button medicineButton;
        [SerializeField] Button candleButton;
        [SerializeField] Button schoolButton;
        [SerializeField] Button sleepButton;

        readonly Dictionary<HomeNeed, int> _costs = new()
        {
            { HomeNeed.Food, 5 },
            { HomeNeed.Medicine, 15 },
            { HomeNeed.Candle, 3 },
            { HomeNeed.School, 8 },
        };

        readonly HashSet<HomeNeed> _purchased = new();
        EconomyService _economy;

        void Start()
        {
            _economy = GameDirector.Instance?.Economy;
            if (_economy != null) _economy.GoldChanged += _ => UpdateStatus();

            if (foodButton) foodButton.onClick.AddListener(() => Buy(HomeNeed.Food, "Bir akşam yemeği. Idris'in iştahı yok ama Lina yiyor."));
            if (medicineButton) medicineButton.onClick.AddListener(() => Buy(HomeNeed.Medicine, "Thal'eros şifa otu. Idris'in öksürüğü hafifliyor."));
            if (candleButton) candleButton.onClick.AddListener(() => Buy(HomeNeed.Candle, "Yarın için yeni mum. Masada uzun saatler aydınlık olur."));
            if (schoolButton) schoolButton.onClick.AddListener(() => Buy(HomeNeed.School, "Lina'nın haftalık okul ücreti ödendi."));
            if (sleepButton) sleepButton.onClick.AddListener(SleepAndAdvance);

            UpdateStatus();
            UpdateFamily();
        }

        void Buy(HomeNeed need, string flavor)
        {
            if (_purchased.Contains(need))
            {
                ShowFeedback($"{NeedName(need)} zaten bu akşam alındı.");
                return;
            }
            int cost = _costs[need];
            if (_economy == null || !_economy.TrySpend(cost))
            {
                ShowFeedback($"{NeedName(need)} için yeterli altın yok ({cost} altın gerek).");
                return;
            }
            _purchased.Add(need);
            ShowFeedback($"-{cost} altın. {flavor}");
            UpdateStatus();
            UpdateFamily();
        }

        void SleepAndAdvance()
        {
            ShowFeedback("Aren mumu söndürdü. Yarın yeni bir gün.");
            // Vertical slice: gün sonunda oyun kapanır gibi davranır.
            // Multi-day için GameDirector.RequestState(GameState.NextDay) eklenebilir.
        }

        void UpdateStatus()
        {
            if (statusText && _economy != null)
                statusText.text = $"Ev — Akşam | Altın: {_economy.Gold}";
        }

        void UpdateFamily()
        {
            if (familyStatusText == null) return;
            string idris = _purchased.Contains(HomeNeed.Medicine) ? "<b>Idris:</b> öksürük hafifledi" : "<b>Idris:</b> öksürük şiddetli, ilaç gerek";
            string lina = _purchased.Contains(HomeNeed.School) ? "<b>Lina:</b> okula hazır, mutlu" : "<b>Lina:</b> okul ücreti ödenmedi";
            string food = _purchased.Contains(HomeNeed.Food) ? "Mutfak: dolu" : "Mutfak: boş";
            string candle = _purchased.Contains(HomeNeed.Candle) ? "Yarın masada mum var" : "Yarın masa karanlık olacak";
            familyStatusText.text = $"{idris}\n{lina}\n{food}\n{candle}";
        }

        void ShowFeedback(string s)
        {
            if (feedbackText) feedbackText.text = s;
        }

        static string NeedName(HomeNeed n) => n switch
        {
            HomeNeed.Food => "Yiyecek",
            HomeNeed.Medicine => "İlaç",
            HomeNeed.Candle => "Mum",
            HomeNeed.School => "Okul ücreti",
            _ => n.ToString(),
        };
    }
}
