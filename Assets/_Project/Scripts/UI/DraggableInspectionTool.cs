using UnityEngine;
using UnityEngine.EventSystems;
using KopruBekcisi.Gameplay.Inspection;

namespace KopruBekcisi.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class DraggableInspectionTool : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] InspectionController inspection;
        [SerializeField] InspectionTool tool = InspectionTool.Magnifier;
        [SerializeField] RectTransform documentRect;
        [SerializeField] Canvas canvas;
        [SerializeField] CanvasGroup canvasGroup;

        RectTransform _rt;
        Vector2 _originalAnchoredPos;
        Transform _originalParent;
        int _originalSiblingIndex;
        bool _wasOverDoc;

        bool _captured;

        void Awake()
        {
            _rt = GetComponent<RectTransform>();
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            if (inspection == null) inspection = FindFirstObjectByType<InspectionController>();
            if (canvas == null) canvas = GetComponentInParent<Canvas>();

            // İlk pozisyonu yakala (NPC değişince buraya snap edilecek)
            CaptureOriginal();

            // NPC değiştiğinde / Skip'lendiğinde tool kendini reset etsin
            var dm = FindFirstObjectByType<KopruBekcisi.Gameplay.Day.DayManager>();
            if (dm != null) dm.OnNPCApproached += (n, d) => ResetPosition();
        }

        void CaptureOriginal()
        {
            if (_captured) return;
            _originalAnchoredPos = _rt.anchoredPosition;
            _originalParent = _rt.parent;
            _originalSiblingIndex = _rt.GetSiblingIndex();
            _captured = true;
        }

        public void ResetPosition()
        {
            if (!_captured) return;
            if (_originalParent != null)
            {
                _rt.SetParent(_originalParent, false);
                _rt.SetSiblingIndex(_originalSiblingIndex);
            }
            _rt.anchoredPosition = _originalAnchoredPos;
            if (canvasGroup != null) canvasGroup.blocksRaycasts = true;
            inspection?.ClearTool();
            _wasOverDoc = false;
        }

        public void OnPointerDown(PointerEventData e)
        {
            CaptureOriginal();
            if (canvas != null)
            {
                _rt.SetParent(canvas.transform, true);
                _rt.SetAsLastSibling();
            }
            if (canvasGroup != null) canvasGroup.blocksRaycasts = false;
            _wasOverDoc = false;
        }

        public void OnDrag(PointerEventData e)
        {
            if (canvas == null) return;
            var canvasRect = canvas.transform as RectTransform;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, e.position, e.pressEventCamera, out var local))
                _rt.anchoredPosition = local;

            bool overDoc = documentRect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(documentRect, e.position, e.pressEventCamera);

            if (overDoc != _wasOverDoc)
            {
                _wasOverDoc = overDoc;
                if (overDoc) inspection?.SetTool(tool);
                else inspection?.ClearTool();
            }
        }

        public void OnPointerUp(PointerEventData e) => ResetPosition();
    }
}
