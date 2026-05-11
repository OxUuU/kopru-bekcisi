#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using KopruBekcisi.Faction;
using KopruBekcisi.Home;
using KopruBekcisi.UI;

namespace KopruBekcisi.EditorTools
{
    public static class HomeSceneBuilder
    {
        const string HomeScenePath = "Assets/_Project/Scenes/Home.unity";

        [MenuItem("KopruBekcisi/Setup/Build Home Scene (gece, aile, harcama)")]
        public static void Build()
        {
            var scene = EditorSceneManager.OpenScene(HomeScenePath, OpenSceneMode.Single);

            foreach (var go in scene.GetRootGameObjects())
                Object.DestroyImmediate(go);

            var camGO = new GameObject("Main Camera") { tag = "MainCamera" };
            var cam = camGO.AddComponent<UnityEngine.Camera>();
            cam.orthographic = true;
            cam.backgroundColor = new Color(0.04f, 0.03f, 0.05f, 1f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            camGO.AddComponent<AudioListener>();

            var lightGO = new GameObject("Candle Light");
            var light = lightGO.AddComponent<Light2D>();
            light.lightType = Light2D.LightType.Global;
            light.color = new Color(1.0f, 0.78f, 0.55f);
            light.intensity = 0.65f;

            BuildUI();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            Debug.Log("[HomeSceneBuilder] ✓ Home sahnesi inşa edildi.");
        }

        static void BuildUI()
        {
            var canvasGO = new GameObject("UI Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            var scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule));

            var bg = MakeRect("Background", canvasGO.transform);
            var bgImg = bg.gameObject.AddComponent<Image>();
            bgImg.color = new Color(0.06f, 0.05f, 0.04f, 1f);
            Anchor(bg, Vector2.zero, Vector2.one);

            var title = MakeText("Title", bg, "<b>EV — AKŞAM</b>\nIdris kaide önünde nefes alıyor. Lina uyumuş.", 32, TextAnchor.UpperLeft);
            title.color = new Color(0.95f, 0.85f, 0.65f);
            Anchor(title.rectTransform, new Vector2(0.05f, 0.78f), new Vector2(0.55f, 0.95f));

            var family = MakeText("FamilyStatus", bg, "(aile durumu)", 26, TextAnchor.UpperLeft);
            family.color = new Color(0.85f, 0.80f, 0.65f);
            Anchor(family.rectTransform, new Vector2(0.05f, 0.40f), new Vector2(0.55f, 0.77f));

            var feedback = MakeText("Feedback", bg, "", 24, TextAnchor.UpperLeft);
            feedback.color = new Color(0.75f, 0.85f, 0.70f);
            Anchor(feedback.rectTransform, new Vector2(0.05f, 0.20f), new Vector2(0.55f, 0.39f));

            var statusBar = MakeRect("StatusBar", canvasGO.transform);
            var sbImg = statusBar.gameObject.AddComponent<Image>();
            sbImg.color = new Color(0, 0, 0, 0.85f);
            Anchor(statusBar, new Vector2(0, 0.96f), new Vector2(1, 1));
            var statusText = MakeText("StatusText", statusBar, "Ev — Akşam | Altın: 0", 22, TextAnchor.MiddleCenter);
            statusText.color = new Color(0.95f, 0.92f, 0.80f);
            Anchor(statusText.rectTransform, Vector2.zero, Vector2.one);

            var food = MakeButton("FoodButton", bg, "Yiyecek (5 altın)", new Color(0.40f, 0.55f, 0.35f), 0.60f, 0.75f, 0.97f, 0.86f);
            var med = MakeButton("MedicineButton", bg, "İlaç (15 altın)", new Color(0.55f, 0.30f, 0.30f), 0.60f, 0.62f, 0.97f, 0.73f);
            var candle = MakeButton("CandleButton", bg, "Mum (3 altın)", new Color(0.65f, 0.55f, 0.25f), 0.60f, 0.49f, 0.97f, 0.60f);
            var school = MakeButton("SchoolButton", bg, "Okul Ücreti (8 altın)", new Color(0.40f, 0.45f, 0.60f), 0.60f, 0.36f, 0.97f, 0.47f);
            var sleep = MakeButton("SleepButton", bg, "Mumu Söndür ve Uyu", new Color(0.30f, 0.30f, 0.45f), 0.60f, 0.05f, 0.97f, 0.20f);

            var hmGO = new GameObject("HomeManager");
            var hm = hmGO.AddComponent<HomeManager>();
            Bind(hm, "statusText", statusText);
            Bind(hm, "familyStatusText", family);
            Bind(hm, "feedbackText", feedback);
            Bind(hm, "foodButton", food);
            Bind(hm, "medicineButton", med);
            Bind(hm, "candleButton", candle);
            Bind(hm, "schoolButton", school);
            Bind(hm, "sleepButton", sleep);

            BuildDialogPopup(canvasGO.transform);
            new GameObject("NightVisitor").AddComponent<NightVisitor>();
        }

        static void BuildDialogPopup(Transform canvas)
        {
            var rootRt = MakeRect("DialogPopup", canvas);
            var rootImg = rootRt.gameObject.AddComponent<Image>();
            rootImg.color = new Color(0, 0, 0, 0.75f);
            Anchor(rootRt, Vector2.zero, Vector2.one);

            var panel = MakeRect("Panel", rootRt);
            var panelImg = panel.gameObject.AddComponent<Image>();
            panelImg.color = new Color(0.10f, 0.08f, 0.07f, 0.97f);
            Anchor(panel, new Vector2(0.18f, 0.20f), new Vector2(0.82f, 0.85f));

            var title = MakeText("Title", panel, "Başlık", 30, TextAnchor.UpperCenter);
            title.color = new Color(0.95f, 0.85f, 0.55f); title.fontStyle = FontStyle.Bold;
            Anchor(title.rectTransform, new Vector2(0.05f, 0.85f), new Vector2(0.95f, 0.97f));

            var body = MakeText("Body", panel, "...", 22, TextAnchor.UpperLeft);
            body.color = new Color(0.92f, 0.90f, 0.80f);
            Anchor(body.rectTransform, new Vector2(0.06f, 0.20f), new Vector2(0.94f, 0.83f));

            var primary = MakeButton("PrimaryButton", panel, "Tamam", new Color(0.30f, 0.50f, 0.35f), 0.55f, 0.04f, 0.93f, 0.16f);
            var secondary = MakeButton("SecondaryButton", panel, "Vazgeç", new Color(0.55f, 0.30f, 0.30f), 0.07f, 0.04f, 0.45f, 0.16f);
            var pLbl = primary.GetComponentInChildren<Text>();
            var sLbl = secondary.GetComponentInChildren<Text>();

            var dlg = rootRt.gameObject.AddComponent<DialogPopup>();
            Bind(dlg, "root", rootRt.gameObject);
            Bind(dlg, "titleText", title);
            Bind(dlg, "bodyText", body);
            Bind(dlg, "primaryButton", primary);
            Bind(dlg, "primaryLabel", pLbl);
            Bind(dlg, "secondaryButton", secondary);
            Bind(dlg, "secondaryLabel", sLbl);

            rootRt.gameObject.SetActive(false);
        }

        static RectTransform MakeRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        static Font _font;
        static Font GetFont() => _font ??= Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        static Text MakeText(string name, Transform parent, string content, int size, TextAnchor align)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var t = go.AddComponent<Text>();
            t.font = GetFont(); t.text = content; t.fontSize = size; t.alignment = align;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.supportRichText = true;
            return t;
        }

        static Button MakeButton(string name, Transform parent, string label, Color color, float aMinX, float aMinY, float aMaxX, float aMaxY)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            var b = go.AddComponent<Button>();
            var c = b.colors; c.normalColor = color; c.highlightedColor = color * 1.2f; c.pressedColor = color * 0.7f; b.colors = c;
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(aMinX, aMinY);
            rt.anchorMax = new Vector2(aMaxX, aMaxY);
            rt.offsetMin = new Vector2(6, 6); rt.offsetMax = new Vector2(-6, -6);
            var lblGO = new GameObject("Label", typeof(RectTransform));
            lblGO.transform.SetParent(go.transform, false);
            var t = lblGO.AddComponent<Text>();
            t.font = GetFont(); t.text = label; t.fontSize = 24; t.alignment = TextAnchor.MiddleCenter;
            t.color = Color.white; t.fontStyle = FontStyle.Bold;
            t.horizontalOverflow = HorizontalWrapMode.Wrap; t.verticalOverflow = VerticalWrapMode.Overflow;
            var lrt = t.rectTransform;
            lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero; lrt.offsetMax = Vector2.zero;
            return b;
        }

        static void Anchor(RectTransform rt, Vector2 min, Vector2 max)
        {
            rt.anchorMin = min; rt.anchorMax = max;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        }

        static void Bind(Object target, string field, Object value)
        {
            var so = new SerializedObject(target);
            var p = so.FindProperty(field);
            if (p != null) { p.objectReferenceValue = value; so.ApplyModifiedProperties(); }
        }
    }
}
#endif
