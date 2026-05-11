using System;
using UnityEngine;

namespace KopruBekcisi.Core.EventChannels
{
    [CreateAssetMenu(menuName = "KopruBekcisi/Events/Game State Event Channel", fileName = "GameStateEventChannel")]
    public class GameStateEventChannelSO : ScriptableObject
    {
        public event Action<GameState, GameState> OnStateChanged;

        public void Raise(GameState from, GameState to) => OnStateChanged?.Invoke(from, to);
    }
}
