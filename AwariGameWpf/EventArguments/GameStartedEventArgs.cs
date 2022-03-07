using System;
using System.Collections.Generic;

namespace AwariGameWpf.EventArguments
{
    public class GameStartedEventArgs : EventArgs
    {
        public Dictionary<int, int> Bowls { get; set; }

        public bool ActivePlayer { get; set; }

        public bool CanAgain { get; set; }

        public GameStartedEventArgs(Dictionary<int, int> bowls, bool activePlayer, bool canAgain)
        {
            Bowls = bowls;
            ActivePlayer = activePlayer;
            CanAgain = canAgain;
        }
    }
}
