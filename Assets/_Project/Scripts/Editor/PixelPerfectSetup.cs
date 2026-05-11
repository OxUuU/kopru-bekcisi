#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace KopruBekcisi.EditorTools
{
    public static class PixelPerfectSetup
    {
        public const int RefWidth = 640;
        public const int RefHeight = 360;
        public const int PixelsPerUnit = 16;

        [MenuItem("KopruBekcisi/Setup/Configure Pixel Perfect Camera (current scene)")]
        public static void ConfigureCurrentScene()
        {
            var cam = UnityEngine.Camera.main;
            if (cam == null)
            {
                Debug.LogWarning("[PixelPerfectSetup] Aktif sahnede Main Camera yok.");
                return;
            }

            var ppc = cam.GetComponent<PixelPerfectCamera>();
            if (ppc == null) ppc = cam.gameObject.AddComponent<PixelPerfectCamera>();

            ppc.assetsPPU = PixelsPerUnit;
            ppc.refResolutionX = RefWidth;
            ppc.refResolutionY = RefHeight;
            ppc.pixelSnapping = true;

            cam.orthographic = true;
            cam.orthographicSize = (RefHeight / 2f) / PixelsPerUnit;
            cam.backgroundColor = new Color(0.05f, 0.05f, 0.07f, 1f);
            cam.clearFlags = CameraClearFlags.SolidColor;

            EditorSceneManager.MarkSceneDirty(cam.gameObject.scene);
            EditorSceneManager.SaveScene(cam.gameObject.scene);

            Debug.Log($"[PixelPerfectSetup] ✓ Main Camera config: {RefWidth}x{RefHeight} @ {PixelsPerUnit} PPU, ortho size {cam.orthographicSize:F2}");
        }
    }
}
#endif
