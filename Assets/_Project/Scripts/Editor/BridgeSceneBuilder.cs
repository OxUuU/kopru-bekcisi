#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using KopruBekcisi.CameraRig;
using KopruBekcisi.Gameplay.Day;
using KopruBekcisi.Gameplay.Inspection;
using KopruBekcisi.UI;

namespace KopruBekcisi.EditorTools
{
    public static class BridgeSceneBuilder
    {
        const string BridgeScenePath = "Assets/_Project/Scenes/Bridge.unity";
        const string SpritesFolder = "Assets/_Project/Art/Sprites";

        [MenuItem("KopruBekcisi/Setup/Build Bridge Scene (parallax + desk UI)")]
        public static void Build()
        {
            EnsureFolder(SpritesFolder);

            var bg = CreateColorSprite("placeholder_bg", new Color(0.10f, 0.12f, 0.20f));
            var mid = CreateColorSprite("placeholder_mid", new Color(0.20f, 0.28f, 0.36f));
            var fg = CreateColorSprite("placeholder_fg", new Color(0.40f, 0.32f, 0.24f));
            var probe = CreateColorSprite("placeholder_probe", new Color(0.95f, 0.85f, 0.20f));

            var scene = EditorSceneManager.OpenScene(BridgeScenePath, OpenSceneMode.Single);

            foreach (var go in scene.GetRootGameObjects())
                Object.DestroyImmediate(go);

            var camGO = new GameObject("Main Camera") { tag = "MainCamera" };
            var cam = camGO.AddComponent<UnityEngine.Camera>();
            cam.orthographic = true;
            cam.backgroundColor = new Color(0.05f, 0.05f, 0.07f, 1f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.transform.position = new Vector3(0f, 4f, -10f);
            camGO.AddComponent<AudioListener>();

            var ppc = camGO.AddComponent<PixelPerfectCamera>();
            ppc.assetsPPU = 16;
            ppc.refResolutionX = 640;
            ppc.refResolutionY = 360;
            ppc.pixelSnapping = true;
            cam.orthographicSize = (360f / 2f) / 16f;

            camGO.AddComponent<CameraTestMover>();

            var lightGO = new GameObject("Global Light 2D");
            var light = lightGO.AddComponent<Light2D>();
            light.lightType = Light2D.LightType.Global;
            light.color = new Color(0.92f, 0.88f, 0.82f);
            light.intensity = 1.0f;

            CreateParallaxLayer("BG_Layer", bg, new Vector3(0, 7, 10), new Vector3(80, 12, 1), 0.2f, -10, cam.transform);
            CreateParallaxLayer("MID_Layer", mid, new Vector3(0, 5, 5), new Vector3(60, 8, 1), 0.5f, -5, cam.transform);
            CreateParallaxLayer("FG_Layer", fg, new Vector3(0, 3, 1), new Vector3(40, 4, 1), 0.85f, 0, cam.transform);

            for (int i = -3; i <= 3; i++)
            {
                var probeGO = new GameObject($"Probe_{i}");
                var sr = probeGO.AddComponent<SpriteRenderer>();
                sr.sprite = probe;
                sr.sortingOrder = 5;
                probeGO.transform.position = new Vector3(i * 4f, 6f, 0f);
                probeGO.transform.localScale = Vector3.one * 0.5f;
            }

            var dmGO = new GameObject("DayManager");
            var dayManager = dmGO.AddComponent<DayManager>();

            var inspGO = new GameObject("InspectionController");
            var inspection = inspGO.AddComponent<InspectionController>();

            BuildDeskUI(dayManager, inspection);

            var dbgGO = new GameObject("DebugConsole");
            var dbg = dbgGO.AddComponent<DebugConsole>();
            BindField(dbg, "dayManager", dayManager);

            var codexGO = new GameObject("CodexPanel");
            var codex = codexGO.AddComponent<CodexPanel>();
            BindField(codex, "dayManager", dayManager);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            Debug.Log("[BridgeSceneBuilder] ✓ Bridge sahnesi inşa edildi: parallax + Desk UI + DayManager + DebugConsole.");
        }

        static void BuildDeskUI(DayManager dayManager, InspectionController inspection)
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

            // Duvar parçaları (sol, sağ, üst) — ortadaki pencere boşluğunu çevreler
            var wallColor = new Color(0.42f, 0.36f, 0.30f, 1f);
            var leftWall = CreateColoredRect("LeftWall", canvasGO.transform, wallColor, new Vector2(0, 0.50f), new Vector2(0.18f, 0.96f));
            var rightWall = CreateColoredRect("RightWall", canvasGO.transform, wallColor, new Vector2(0.82f, 0.50f), new Vector2(1, 0.96f));
            var topWall = CreateColoredRect("TopWall", canvasGO.transform, wallColor, new Vector2(0.18f, 0.93f), new Vector2(0.82f, 0.96f));

            // Pencere çerçevesi (4 ince siyah çubuk — parallax bölgesini sınırlar)
            var frameColor = new Color(0.05f, 0.04f, 0.03f, 1f);
            CreateColoredRect("WindowBorder_T", canvasGO.transform, frameColor, new Vector2(0.18f, 0.92f), new Vector2(0.82f, 0.93f));
            CreateColoredRect("WindowBorder_B", canvasGO.transform, frameColor, new Vector2(0.18f, 0.50f), new Vector2(0.82f, 0.51f));
            CreateColoredRect("WindowBorder_L", canvasGO.transform, frameColor, new Vector2(0.18f, 0.50f), new Vector2(0.19f, 0.93f));
            CreateColoredRect("WindowBorder_R", canvasGO.transform, frameColor, new Vector2(0.81f, 0.50f), new Vector2(0.82f, 0.93f));

            // Pencere camını biraz aydınlat (hafif sıcak hue üst yarı)
            var windowGlow = CreateColoredRect("WindowGlow", canvasGO.transform, new Color(0.95f, 0.88f, 0.65f, 0.05f), new Vector2(0.19f, 0.51f), new Vector2(0.81f, 0.92f));

            var deskBg = CreateUIRect("DeskBackground", canvasGO.transform);
            var deskImg = deskBg.gameObject.AddComponent<Image>();
            deskImg.color = new Color(0.10f, 0.07f, 0.05f, 0.95f);
            SetAnchor(deskBg, new Vector2(0, 0), new Vector2(1, 0.5f));
            deskBg.offsetMin = Vector2.zero; deskBg.offsetMax = Vector2.zero;

            var documentPanel = CreateUIRect("DocumentPanel", deskBg);
            var docImg = documentPanel.gameObject.AddComponent<Image>();
            docImg.color = new Color(0.92f, 0.86f, 0.74f, 1f);
            SetAnchor(documentPanel, new Vector2(0.05f, 0.10f), new Vector2(0.55f, 0.95f));
            documentPanel.offsetMin = Vector2.zero; documentPanel.offsetMax = Vector2.zero;

            var documentText = CreateText("DocumentText", documentPanel, "PASAPORT\n\n(belge buraya)", 26, TextAnchor.UpperLeft);
            documentText.color = new Color(0.15f, 0.10f, 0.05f);
            SetAnchor(documentText.rectTransform, new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.95f));
            documentText.rectTransform.offsetMin = Vector2.zero; documentText.rectTransform.offsetMax = Vector2.zero;

            var greetingText = CreateText("GreetingText", deskBg, "(NPC selamı)", 24, TextAnchor.UpperLeft);
            greetingText.color = new Color(0.85f, 0.80f, 0.65f);
            SetAnchor(greetingText.rectTransform, new Vector2(0.60f, 0.75f), new Vector2(0.98f, 0.95f));
            greetingText.rectTransform.offsetMin = Vector2.zero; greetingText.rectTransform.offsetMax = Vector2.zero;

            var feedbackText = CreateText("FeedbackText", deskBg, "", 22, TextAnchor.UpperLeft);
            feedbackText.color = new Color(0.85f, 0.80f, 0.65f);
            SetAnchor(feedbackText.rectTransform, new Vector2(0.60f, 0.55f), new Vector2(0.98f, 0.74f));
            feedbackText.rectTransform.offsetMin = Vector2.zero; feedbackText.rectTransform.offsetMax = Vector2.zero;

            var approve = CreateButton("ApproveButton", deskBg, "İçeri Al", new Color(0.30f, 0.55f, 0.30f), 0.60f, 0.50f, 0.78f, 0.62f);
            var deny = CreateButton("DenyButton", deskBg, "Geri Gönder", new Color(0.60f, 0.55f, 0.30f), 0.79f, 0.50f, 0.97f, 0.62f);
            var detain = CreateButton("DetainButton", deskBg, "Zindana At", new Color(0.50f, 0.30f, 0.55f), 0.60f, 0.36f, 0.78f, 0.48f);
            var execute = CreateButton("ExecuteButton", deskBg, "İdam Et", new Color(0.65f, 0.20f, 0.20f), 0.79f, 0.36f, 0.97f, 0.48f);

            var canvasComp = canvasGO.GetComponent<Canvas>();
            // Tool'lar duvarda asılı — sol duvarın üst-orta kısmında
            var magImg = CreateDraggableTool("MagnifierTool", leftWall.transform, "🔍\nMercek", new Color(0.30f, 0.40f, 0.55f), 0.10f, 0.55f, 0.90f, 0.78f, InspectionTool.Magnifier, inspection, documentPanel, canvasComp);
            var lanImg = CreateDraggableTool("LanternTool", leftWall.transform, "🔦\nFener", new Color(0.55f, 0.45f, 0.20f), 0.10f, 0.28f, 0.90f, 0.51f, InspectionTool.Lantern, inspection, documentPanel, canvasComp);

            // Askı çubuğu (dekoratif, tool'ları "duvarda asılı" hissettirir)
            CreateColoredRect("ToolHook1", leftWall.transform, new Color(0.20f, 0.15f, 0.10f), new Vector2(0.45f, 0.78f), new Vector2(0.55f, 0.84f));
            CreateColoredRect("ToolHook2", leftWall.transform, new Color(0.20f, 0.15f, 0.10f), new Vector2(0.45f, 0.51f), new Vector2(0.55f, 0.57f));

            var statusBar = CreateUIRect("StatusBar", canvasGO.transform);
            var sbImg = statusBar.gameObject.AddComponent<Image>();
            sbImg.color = new Color(0.0f, 0.0f, 0.0f, 0.85f);
            SetAnchor(statusBar, new Vector2(0, 0.96f), new Vector2(1, 1));
            statusBar.offsetMin = Vector2.zero; statusBar.offsetMax = Vector2.zero;

            var statusText = CreateText("StatusText", statusBar, "Gün 1 | Altın: 0 | Karma: 0 | Sırada: 0", 22, TextAnchor.MiddleCenter);
            statusText.color = new Color(0.95f, 0.92f, 0.80f);
            SetAnchor(statusText.rectTransform, new Vector2(0, 0), new Vector2(1, 1));
            statusText.rectTransform.offsetMin = Vector2.zero; statusText.rectTransform.offsetMax = Vector2.zero;

            var deskCtrlGO = new GameObject("DeskUIController");
            var ctrl = deskCtrlGO.AddComponent<DeskUIController>();
            BindField(ctrl, "dayManager", dayManager);
            BindField(ctrl, "inspection", inspection);
            BindField(ctrl, "documentPanel", documentPanel.gameObject);
            BindField(ctrl, "documentText", documentText);
            BindField(ctrl, "greetingText", greetingText);
            BindField(ctrl, "feedbackText", feedbackText);
            BindField(ctrl, "statusText", statusText);
            BindField(ctrl, "approveButton", approve);
            BindField(ctrl, "denyButton", deny);
            BindField(ctrl, "detainButton", detain);
            BindField(ctrl, "executeButton", execute);
            BindField(ctrl, "magnifierIndicator", magImg);
            BindField(ctrl, "lanternIndicator", lanImg);

            BuildDialogPopup(canvasGO.transform);
            BuildMarekNote(rightWall.transform);
        }

        static RectTransform CreateColoredRect(string name, Transform parent, Color color, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            img.raycastTarget = false;
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            return rt;
        }

        static void BuildMarekNote(Transform parent)
        {
            var go = new GameObject("MarekNote", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var img = go.GetComponent<Image>();
            img.color = new Color(0.65f, 0.55f, 0.30f, 0.95f);
            var btn = go.GetComponent<Button>();
            var rt = go.GetComponent<RectTransform>();
            // Sağ duvarın orta kısmında, küçük bir not gibi asılı
            rt.anchorMin = new Vector2(0.10f, 0.55f); rt.anchorMax = new Vector2(0.90f, 0.78f);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;

            var lblGO = new GameObject("Label", typeof(RectTransform));
            lblGO.transform.SetParent(go.transform, false);
            var t = lblGO.AddComponent<Text>();
            t.font = GetDefaultFont();
            t.text = "📜\nM.V.\nnotu";
            t.fontSize = 22; t.alignment = TextAnchor.MiddleCenter; t.color = new Color(0.20f, 0.15f, 0.05f);
            t.fontStyle = FontStyle.Bold;
            t.horizontalOverflow = HorizontalWrapMode.Wrap; t.verticalOverflow = VerticalWrapMode.Overflow;
            t.raycastTarget = false;
            var lrt = t.rectTransform;
            lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero; lrt.offsetMax = Vector2.zero;

            var note = go.AddComponent<MarekNote>();
            BindField(note, "button", btn);
        }

        static void BuildDialogPopup(Transform canvas)
        {
            var rootRt = CreateUIRect("DialogPopup", canvas);
            var rootImg = rootRt.gameObject.AddComponent<Image>();
            rootImg.color = new Color(0, 0, 0, 0.7f);
            SetAnchor(rootRt, Vector2.zero, Vector2.one);
            rootRt.offsetMin = Vector2.zero; rootRt.offsetMax = Vector2.zero;

            var panel = CreateUIRect("Panel", rootRt);
            var panelImg = panel.gameObject.AddComponent<Image>();
            panelImg.color = new Color(0.10f, 0.08f, 0.07f, 0.97f);
            SetAnchor(panel, new Vector2(0.18f, 0.20f), new Vector2(0.82f, 0.85f));
            panel.offsetMin = Vector2.zero; panel.offsetMax = Vector2.zero;

            var title = CreateText("Title", panel, "Başlık", 32, TextAnchor.UpperCenter);
            title.color = new Color(0.95f, 0.85f, 0.55f);
            title.fontStyle = FontStyle.Bold;
            SetAnchor(title.rectTransform, new Vector2(0.05f, 0.85f), new Vector2(0.95f, 0.97f));
            title.rectTransform.offsetMin = Vector2.zero; title.rectTransform.offsetMax = Vector2.zero;

            var body = CreateText("Body", panel, "...", 24, TextAnchor.UpperLeft);
            body.color = new Color(0.92f, 0.90f, 0.80f);
            SetAnchor(body.rectTransform, new Vector2(0.06f, 0.20f), new Vector2(0.94f, 0.83f));
            body.rectTransform.offsetMin = Vector2.zero; body.rectTransform.offsetMax = Vector2.zero;

            var primary = CreateButton("PrimaryButton", panel, "Tamam", new Color(0.30f, 0.50f, 0.35f), 0.55f, 0.04f, 0.93f, 0.16f);
            var secondary = CreateButton("SecondaryButton", panel, "Vazgeç", new Color(0.55f, 0.30f, 0.30f), 0.07f, 0.04f, 0.45f, 0.16f);

            var primaryLabel = primary.GetComponentInChildren<Text>();
            var secondaryLabel = secondary.GetComponentInChildren<Text>();

            var dlg = rootRt.gameObject.AddComponent<DialogPopup>();
            BindField(dlg, "root", rootRt.gameObject);
            BindField(dlg, "titleText", title);
            BindField(dlg, "bodyText", body);
            BindField(dlg, "primaryButton", primary);
            BindField(dlg, "primaryLabel", primaryLabel);
            BindField(dlg, "secondaryButton", secondary);
            BindField(dlg, "secondaryLabel", secondaryLabel);

            rootRt.gameObject.SetActive(false);
        }

        static Image CreateDraggableTool(string name, Transform parent, string label, Color color,
            float aMinX, float aMinY, float aMaxX, float aMaxY,
            InspectionTool tool, InspectionController inspection, RectTransform docRect, Canvas canvas)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            go.transform.SetParent(parent, false);
            var img = go.GetComponent<Image>();
            img.color = color;
            img.raycastTarget = true;
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(aMinX, aMinY);
            rt.anchorMax = new Vector2(aMaxX, aMaxY);
            rt.offsetMin = new Vector2(6, 6); rt.offsetMax = new Vector2(-6, -6);

            var lblGO = new GameObject("Label", typeof(RectTransform));
            lblGO.transform.SetParent(go.transform, false);
            var t = lblGO.AddComponent<Text>();
            t.font = GetDefaultFont();
            t.text = label; t.fontSize = 20; t.color = Color.white;
            t.alignment = TextAnchor.MiddleCenter; t.fontStyle = FontStyle.Bold;
            t.horizontalOverflow = HorizontalWrapMode.Wrap; t.verticalOverflow = VerticalWrapMode.Overflow;
            t.raycastTarget = false;
            var lrt = t.rectTransform;
            lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero; lrt.offsetMax = Vector2.zero;

            var drag = go.AddComponent<DraggableInspectionTool>();
            var so = new SerializedObject(drag);
            so.FindProperty("inspection").objectReferenceValue = inspection;
            so.FindProperty("documentRect").objectReferenceValue = docRect;
            so.FindProperty("canvas").objectReferenceValue = canvas;
            so.FindProperty("canvasGroup").objectReferenceValue = go.GetComponent<CanvasGroup>();
            so.FindProperty("tool").enumValueIndex = (int)tool;
            so.ApplyModifiedProperties();

            return img;
        }

        static RectTransform CreateUIRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        static Font _cachedFont;

        static Font GetDefaultFont()
        {
            if (_cachedFont != null) return _cachedFont;
            _cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (_cachedFont == null) Debug.LogWarning("[BridgeSceneBuilder] LegacyRuntime.ttf bulunamadı.");
            return _cachedFont;
        }

        static Text CreateText(string name, Transform parent, string content, int fontSize, TextAnchor align)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var text = go.AddComponent<Text>();
            text.text = content;
            text.font = GetDefaultFont();
            text.fontSize = fontSize;
            text.alignment = align;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.supportRichText = true;
            return text;
        }

        static Button CreateButton(string name, Transform parent, string label, Color color, float anchorMinX, float anchorMinY, float anchorMaxX, float anchorMaxY)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            var btn = go.AddComponent<Button>();
            var colors = btn.colors;
            colors.normalColor = color;
            colors.highlightedColor = color * 1.2f;
            colors.pressedColor = color * 0.7f;
            btn.colors = colors;

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(anchorMinX, anchorMinY);
            rt.anchorMax = new Vector2(anchorMaxX, anchorMaxY);
            rt.offsetMin = new Vector2(6, 6);
            rt.offsetMax = new Vector2(-6, -6);

            var labelGO = new GameObject("Label", typeof(RectTransform));
            labelGO.transform.SetParent(go.transform, false);
            var t = labelGO.AddComponent<Text>();
            t.text = label;
            t.font = GetDefaultFont();
            t.fontSize = 26;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = Color.white;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.fontStyle = FontStyle.Bold;
            var lrt = t.rectTransform;
            lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero; lrt.offsetMax = Vector2.zero;

            return btn;
        }

        static void SetAnchor(RectTransform rt, Vector2 min, Vector2 max)
        {
            rt.anchorMin = min;
            rt.anchorMax = max;
        }

        static void BindField(Object target, string fieldName, Object value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(fieldName);
            if (prop != null) { prop.objectReferenceValue = value; so.ApplyModifiedProperties(); }
            else Debug.LogWarning($"[BridgeSceneBuilder] '{fieldName}' field bulunamadı: {target.GetType().Name}");
        }

        static void CreateParallaxLayer(string name, Sprite sprite, Vector3 pos, Vector3 scale, float factor, int sortingOrder, Transform cameraTransform)
        {
            var go = new GameObject(name);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = sortingOrder;
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = new Vector2(scale.x, scale.y);
            go.transform.position = pos;

            var par = go.AddComponent<ParallaxController>();
            var so = new SerializedObject(par);
            so.FindProperty("cameraTransform").objectReferenceValue = cameraTransform;
            so.FindProperty("parallaxFactor").floatValue = factor;
            so.FindProperty("pixelSnap").boolValue = true;
            so.FindProperty("pixelsPerUnit").intValue = 16;
            so.ApplyModifiedProperties();
        }

        static Sprite CreateColorSprite(string name, Color color, int size = 32)
        {
            var path = $"{SpritesFolder}/{name}.png";
            if (!File.Exists(path))
            {
                var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
                var pixels = new Color[size * size];
                for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
                tex.SetPixels(pixels);
                tex.Apply();
                File.WriteAllBytes(path, tex.EncodeToPNG());
                Object.DestroyImmediate(tex);
                AssetDatabase.ImportAsset(path);
            }

            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 16;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.spriteImportMode = SpriteImportMode.Single;
                var settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteMeshType = SpriteMeshType.FullRect;
                importer.SetTextureSettings(settings);
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        static void EnsureFolder(string assetPath)
        {
            if (AssetDatabase.IsValidFolder(assetPath)) return;
            string[] parts = assetPath.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
#endif
