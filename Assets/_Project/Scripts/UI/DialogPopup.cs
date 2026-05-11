using System;
using UnityEngine;
using UnityEngine.UI;

namespace KopruBekcisi.UI
{
    public class DialogPopup : MonoBehaviour
    {
        [SerializeField] GameObject root;
        [SerializeField] Text titleText;
        [SerializeField] Text bodyText;
        [SerializeField] Button primaryButton;
        [SerializeField] Text primaryLabel;
        [SerializeField] Button secondaryButton;
        [SerializeField] Text secondaryLabel;

        Action _onPrimary;
        Action _onSecondary;

        public static DialogPopup Instance { get; private set; }

        void Awake()
        {
            Instance = this;
            if (root != null) root.SetActive(false);
            if (primaryButton != null) primaryButton.onClick.AddListener(() => Close(true));
            if (secondaryButton != null) secondaryButton.onClick.AddListener(() => Close(false));
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void Show(string title, string body, string primaryLbl = "Tamam", Action onPrimary = null,
            string secondaryLbl = null, Action onSecondary = null)
        {
            if (root != null) root.SetActive(true);
            if (titleText) titleText.text = title;
            if (bodyText) bodyText.text = body;
            if (primaryLabel) primaryLabel.text = primaryLbl;
            _onPrimary = onPrimary;

            bool hasSecondary = !string.IsNullOrEmpty(secondaryLbl);
            if (secondaryButton != null) secondaryButton.gameObject.SetActive(hasSecondary);
            if (secondaryLabel) secondaryLabel.text = secondaryLbl ?? "";
            _onSecondary = onSecondary;
        }

        void Close(bool primary)
        {
            if (root != null) root.SetActive(false);
            var primaryCb = _onPrimary;
            var secondaryCb = _onSecondary;
            _onPrimary = null;
            _onSecondary = null;
            if (primary) primaryCb?.Invoke();
            else secondaryCb?.Invoke();
        }
    }
}
