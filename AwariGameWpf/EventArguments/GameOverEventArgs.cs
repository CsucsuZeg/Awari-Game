using System;

namespace AwariGameWpf.EventArguments
{
    public class GameOverEventArgs : EventArgs
    {
        public string Winner { get; set; }

        public int Size { get; set; }

        public GameOverEventArgs(string winner, int size)
        {
            Winner = winner;
            Size = size;
        }
    }
}
