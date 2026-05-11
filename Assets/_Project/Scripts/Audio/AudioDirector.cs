using UnityEngine;
using UnityEngine.Audio;

namespace KopruBekcisi.Audio
{
    public class AudioDirector : MonoBehaviour
    {
        [SerializeField] AudioMixer mixer;
        [SerializeField, Range(0.0001f, 1f)] float master = 1f;
        [SerializeField, Range(0.0001f, 1f)] float music = 0.7f;
        [SerializeField, Range(0.0001f, 1f)] float sfx = 1f;
        [SerializeField, Range(0.0001f, 1f)] float ambient = 0.6f;
        [SerializeField, Range(0.0001f, 1f)] float ui = 1f;

        const string MasterParam = "MasterVol";
        const string MusicParam = "MusicVol";
        const string SfxParam = "SfxVol";
        const string AmbientParam = "AmbientVol";
        const string UiParam = "UiVol";

        void Start() => ApplyAll();

        public void SetMaster(float v) { master = v; if (mixer != null) mixer.SetFloat(MasterParam, LinearToDb(v)); }
        public void SetMusic(float v) { music = v; if (mixer != null) mixer.SetFloat(MusicParam, LinearToDb(v)); }
        public void SetSfx(float v) { sfx = v; if (mixer != null) mixer.SetFloat(SfxParam, LinearToDb(v)); }
        public void SetAmbient(float v) { ambient = v; if (mixer != null) mixer.SetFloat(AmbientParam, LinearToDb(v)); }
        public void SetUi(float v) { ui = v; if (mixer != null) mixer.SetFloat(UiParam, LinearToDb(v)); }

        void ApplyAll()
        {
            if (mixer == null) return;
            mixer.SetFloat(MasterParam, LinearToDb(master));
            mixer.SetFloat(MusicParam, LinearToDb(music));
            mixer.SetFloat(SfxParam, LinearToDb(sfx));
            mixer.SetFloat(AmbientParam, LinearToDb(ambient));
            mixer.SetFloat(UiParam, LinearToDb(ui));
        }

        static float LinearToDb(float v) => Mathf.Log10(Mathf.Clamp(v, 0.0001f, 1f)) * 20f;
    }
}
