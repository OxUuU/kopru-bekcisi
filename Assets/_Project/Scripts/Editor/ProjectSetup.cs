#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using KopruBekcisi.Core;
using KopruBekcisi.Core.Boot;

namespace KopruBekcisi.EditorTools
{
    public static class ProjectSetup
    {
        const string SetupKey = "KopruBekcisi.SetupDone.v1";
        const string ScenesFolder = "Assets/_Project/Scenes";
        const string PrefabsFolder = "Assets/_Project/Prefabs";
        const string GameDirectorPrefabPath = PrefabsFolder + "/GameDirector.prefab";

        [InitializeOnLoadMethod]
        static void AutoSetupOnLoad()
        {
            if (EditorPrefs.GetBool(SetupKey, false)) return;
            EditorApplication.delayCall += () =>
            {
                try { SetupAll(); }
                catch (System.Exception e) { Debug.LogError("[ProjectSetup] Auto setup failed: " + e); }
            };
        }

        [MenuItem("KopruBekcisi/Setup/Run All")]
        public static void SetupAll()
        {
            Debug.Log("[ProjectSetup] Setup başlıyor...");

            EnsureFolder(ScenesFolder);
            EnsureFolder(PrefabsFolder);

            string bootPath = CreateOrGetEmptyScene("Boot");
            string bridgePath = CreateOrGetEmptyScene("Bridge");
            string homePath = CreateOrGetEmptyScene("Home");

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(bootPath, true),
                new EditorBuildSettingsScene(bridgePath, true),
                new EditorBuildSettingsScene(homePath, true),
            };

            var gdPrefab = EnsureGameDirectorPrefab();
            EnsureBootstrapperInBoot(bootPath, gdPrefab);

            EditorPrefs.SetBool(SetupKey, true);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[ProjectSetup] ✓ 3 sahne, GameDirector prefab, Bootstrapper, Build Settings hazır.");
        }

        [MenuItem("KopruBekcisi/Setup/Reset Flag")]
        public static void ResetFlag()
        {
            EditorPrefs.DeleteKey(SetupKey);
            Debug.Log("[ProjectSetup] Flag sıfırlandı. Yeniden açılışta otomatik çalışır.");
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

        static string CreateOrGetEmptyScene(string sceneName)
        {
            string path = $"{ScenesFolder}/{sceneName}.unity";
            if (File.Exists(path)) return path;

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, path);
            return path;
        }

        static GameObject EnsureGameDirectorPrefab()
        {
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(GameDirectorPrefabPath);
            if (existing != null) return existing;

            var go = new GameObject("GameDirector");
            go.AddComponent<GameDirector>();
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, GameDirectorPrefabPath);
            Object.DestroyImmediate(go);
            return prefab;
        }

        static void EnsureBootstrapperInBoot(string bootScenePath, GameObject gameDirectorPrefab)
        {
            var scene = EditorSceneManager.OpenScene(bootScenePath, OpenSceneMode.Single);
            var existing = Object.FindObjectsByType<Bootstrapper>(FindObjectsSortMode.None);

            Bootstrapper bs;
            if (existing.Length > 0)
            {
                bs = existing[0];
            }
            else
            {
                var go = new GameObject("Bootstrapper");
                bs = go.AddComponent<Bootstrapper>();
            }

            var so = new SerializedObject(bs);
            var prop = so.FindProperty("gameDirectorPrefab");
            if (prop != null)
            {
                prop.objectReferenceValue = gameDirectorPrefab;
                so.ApplyModifiedProperties();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
    }
}
#endif
