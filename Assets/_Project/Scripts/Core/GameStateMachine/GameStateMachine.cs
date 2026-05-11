using System;
using System.Collections.Generic;

namespace KopruBekcisi.Core
{
    public class GameStateMachine
    {
        public GameState Current { get; private set; }
        public event Action<GameState, GameState> StateChanged;

        readonly HashSet<(GameState from, GameState to)> _allowed;

        public GameStateMachine(GameState initial, IEnumerable<(GameState, GameState)> transitions)
        {
            Current = initial;
            _allowed = new HashSet<(GameState, GameState)>(transitions);
        }

        public bool CanTransition(GameState to) => _allowed.Contains((Current, to));

        public bool TryTransition(GameState to)
        {
            if (!CanTransition(to)) return false;
            var prev = Current;
            Current = to;
            StateChanged?.Invoke(prev, to);
            return true;
        }

        // Sahne durumu state ile uyuşmadığında bypass yapmak için (Editor'da Bridge'den
        // direkt Play'e basıldığında vb).
        public void ForceTransition(GameState to)
        {
            if (Current == to) return;
            var prev = Current;
            Current = to;
            StateChanged?.Invoke(prev, to);
        }
    }
}
