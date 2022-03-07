using AwariGameWpf.EventArguments;
using AwariGameWpf.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwariGameWpf.Model
{
    public class AwariModel
    {
        private IDataAccess dataAccess;
        public Dictionary<int, int> bowls;
        private Stack<bool> prevPlayer;
        private int bowlAmount;
        private int oppositeBowl;
        private int buttonPosition;
        private int counter;
        public string winner;
        public bool activePlayer;
        public bool canAgain;
        private bool notAdd;
        private bool previousPlayer;

        public event EventHandler<GameStartedEventArgs> GameStarted;
        public event EventHandler<GameOverEventArgs> GameOver;

        public AwariModel(IDataAccess dA)
        {
            dataAccess = dA;
            bowls = new Dictionary<int, int>();
            prevPlayer = new Stack<bool>();
            bowlAmount = 0;
            oppositeBowl = 0;
            buttonPosition = 0;
            counter = 0;
            winner = "";
            activePlayer = true;
            canAgain = false;
            notAdd = false;

            prevPlayer.Push(false);
        }

        public void StartNewGame(int size)
        {
            bowlAmount = size + 2;
            activePlayer = true;
            canAgain = false;
            bowls.Clear();

            for (int i = 0; i < bowlAmount; i++)
            {
                if (i == (bowlAmount - 2) / 2 || i == bowlAmount - 2 + 1)
                {
                    bowls.Add(i, 0);
                }
                else
                {
                    bowls.Add(i, 6);
                }
            }

            if (GameStarted is not null)
            {
                GameStarted(this, new GameStartedEventArgs(bowls, activePlayer, canAgain));
            }
        }

        public void CheckGameOver()
        {
            bowlAmount = bowls.Count;

            bool redEmpty = true;
            bool blueEmpty = true;

            for (int i = 0; i < (bowlAmount - 2) / 2; i++)
            {
                if (bowls[i] != 0)
                {
                    redEmpty = false;
                }
            }

            for (int i = (bowlAmount - 2) / 2 + 1; i < bowlAmount - 2 + 1; i++)
            {
                if (bowls[i] != 0)
                {
                    blueEmpty = false;
                }
            }

            if (redEmpty || blueEmpty)
            {
                if (bowls[(bowlAmount - 2) / 2] > bowls[bowlAmount - 2 + 1])
                {
                    winner = "Red";
                }
                else if (bowls[(bowlAmount - 2) / 2] < bowls[bowlAmount - 2 + 1])
                {
                    winner = "Blue";
                }
                else
                {
                    winner = "Tie";
                }
            }

            if (redEmpty && GameOver is not null || blueEmpty && GameOver is not null)
            {
                GameOver(this, new GameOverEventArgs(winner, bowlAmount - 2));
            }
        }

        public void Step(int buttonPos, int c)
        {
            buttonPosition = buttonPos;
            counter = c;
            previousPlayer = prevPlayer.Pop();

            while (counter > 0)
            {
                notAdd = false;
                if (counter == 1)
                {
                    CanAgain();
                }

                if (counter == 1)
                {
                    if (activePlayer && bowls.ElementAt(buttonPosition + 1).Key != bowls.ElementAt(buttonPos).Key && bowls[buttonPosition + 1] == 0 && bowls.ElementAt(buttonPosition + 1).Key < bowls.ElementAt((bowls.Count - 2) / 2).Key)
                    {
                        bowls[bowls.ElementAt((bowls.Count - 2) / 2).Key] = bowls.ElementAt((bowls.Count - 2) / 2).Value + 1;

                        oppositeBowl = bowls.ElementAt((bowls.Count - 2) / 2).Key - bowls.ElementAt(buttonPosition + 1).Key;
                        bowls[bowls.ElementAt((bowls.Count - 2) / 2).Key] = bowls[bowls.ElementAt((bowls.Count - 2) / 2).Key] + bowls[bowls.ElementAt((bowls.Count - 2) / 2 + oppositeBowl).Key];

                        bowls[bowls.ElementAt((bowls.Count - 2) / 2 + oppositeBowl).Key] = 0;
                        notAdd = true;
                        counter--;
                    }
                    else if (!activePlayer && bowls.ElementAt(buttonPosition + 1).Key != bowls.ElementAt(buttonPos).Key && bowls[buttonPosition + 1] == 0 && bowls.ElementAt(buttonPosition + 1).Key > bowls.ElementAt((bowls.Count - 2) / 2).Key)
                    {
                        bowls[bowls.ElementAt(bowls.Count - 2 + 1).Key] = bowls.ElementAt(bowls.Count - 2 + 1).Value + 1;

                        oppositeBowl = bowls.ElementAt(bowls.Count - 2 + 1).Key - bowls.ElementAt(buttonPosition + 1).Key;
                        bowls[bowls.ElementAt(bowls.Count - 2 + 1).Key] = bowls[bowls.ElementAt(bowls.Count - 2 + 1).Key] + bowls[bowls.ElementAt(oppositeBowl - 1).Key];

                        bowls[bowls.ElementAt(oppositeBowl - 1).Key] = 0;
                        notAdd = true;
                        counter--;
                    }
                }

                if (buttonPosition == bowls.Count - 2)
                {
                    bowls[buttonPosition + 1] = bowls[buttonPosition + 1] + 1;
                    counter--;
                    buttonPosition = -1;
                }
                else if (buttonPosition + 1 == bowls.ElementAt(buttonPos).Key)
                {
                    buttonPosition++;
                }
                else if (!notAdd)
                {
                    bowls[buttonPosition + 1] = bowls[buttonPosition + 1] + 1;
                    counter--;
                    buttonPosition++;
                }
            }

            prevPlayer.Push(activePlayer);
        }

        public bool WhoIsNext()
        {
            if (activePlayer && !canAgain)
            {
                activePlayer = false;
            }
            else if (activePlayer && canAgain)
            {
                activePlayer = true;
            }
            else if (!activePlayer && !canAgain)
            {
                activePlayer = true;
            }
            else if (!activePlayer && canAgain)
            {
                activePlayer = false;
            }

            return activePlayer;
        }

        public bool CanAgain()
        {
            if (activePlayer && bowls.ElementAt(buttonPosition + 1).Key == bowls.ElementAt((bowls.Count - 2) / 2).Key && !canAgain)
            {
                canAgain = true;
            }
            else if (!activePlayer && bowls.ElementAt(buttonPosition + 1).Key == bowls.ElementAt(bowls.Count - 2 + 1).Key && !canAgain)
            {
                canAgain = true;
            }
            else if (activePlayer && previousPlayer == activePlayer)
            {
                canAgain = false;
            }
            else if (!activePlayer && previousPlayer == activePlayer)
            {
                canAgain = false;
            }

            return canAgain;
        }

        public async Task SaveGame(string path)
        {
            await dataAccess.SaveAsync(path, bowls, activePlayer, canAgain);
        }

        public async Task LoadGame(string path)
        {
            (bowls, activePlayer, canAgain) = await dataAccess.LoadAsync(path);

            if (GameStarted is not null)
            {
                GameStarted(this, new GameStartedEventArgs(bowls, activePlayer, canAgain));
            }
        }
    }
}
