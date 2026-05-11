using UnityEngine;

namespace KopruBekcisi.Core.Boot
{
    [DefaultExecutionOrder(-2000)]
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] GameObject gameDirectorPrefab;

        void Awake()
        {
            if (GameDirector.Instance != null) return;
            if (gameDirectorPrefab == null)
            {
                Debug.LogError("[Bootstrapper] gameDirectorPrefab atanmamış. Boot sahnesinde Bootstrapper'a prefab referansını ver.");
                return;
            }
            Instantiate(gameDirectorPrefab);
        }
    }
}
