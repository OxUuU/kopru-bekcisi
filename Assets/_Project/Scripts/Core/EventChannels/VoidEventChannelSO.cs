using System;
using UnityEngine;

namespace KopruBekcisi.Core.EventChannels
{
    [CreateAssetMenu(menuName = "KopruBekcisi/Events/Void Event Channel", fileName = "VoidEventChannel")]
    public class VoidEventChannelSO : ScriptableObject
    {
        public event Action OnRaised;

        public void Raise() => OnRaised?.Invoke();
    }
}
