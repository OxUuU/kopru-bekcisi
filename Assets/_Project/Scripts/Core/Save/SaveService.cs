using System.IO;
using UnityEngine;

namespace KopruBekcisi.Core.Save
{
    public static class SaveService
    {
        const string FileName = "savegame.json";

        public static string GetSavePath() => Path.Combine(Application.persistentDataPath, FileName);

        public static void Save<T>(T payload)
        {
            var json = JsonUtility.ToJson(payload, prettyPrint: true);
            File.WriteAllText(GetSavePath(), json);
        }

        public static T Load<T>() where T : new()
        {
            var path = GetSavePath();
            if (!File.Exists(path)) return new T();
            return JsonUtility.FromJson<T>(File.ReadAllText(path));
        }

        public static bool Exists() => File.Exists(GetSavePath());

        public static void Delete()
        {
            var path = GetSavePath();
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
