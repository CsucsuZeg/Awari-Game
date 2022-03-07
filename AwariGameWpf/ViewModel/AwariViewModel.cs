using AwariGameWpf.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AwariGameWpf.ViewModel
{
    public class AwariViewModel: ViewModelBase
    {
        private AwariModel _model;
        private Dictionary<int, int> bowls;
        private bool activePlayer;
        private bool redActivePlayer;
        private bool blueActivePlayer;

        public Dictionary<int, int> Bowls { get => bowls; set { bowls = value; OnPropertyChanged(); } }

        public bool CanAgain { get; set; }

        public ObservableCollection<BowlViewModel> BlueBowls { get; set; }
        public ObservableCollection<BowlViewModel> RedBowls { get; set; }
        public ObservableCollection<BowlViewModel> MainBowls { get; set; }

        public DelegateCommand StartNewGameCommand { get; set; }
        public DelegateCommand LoadGameCommand { get; set; }
        public DelegateCommand SaveGameCommand { get; set; }

        public event EventHandler LoadGame;
        public event EventHandler SaveGame;

        public AwariViewModel(AwariModel model)
        {
            _model = model;
            _model.GameStarted += onGameStarted;

            BlueBowls = new ObservableCollection<BowlViewModel>();
            RedBowls = new ObservableCollection<BowlViewModel>();
            MainBowls = new ObservableCollection<BowlViewModel>();

            StartNewGameCommand = new DelegateCommand(n => _model.StartNewGame(Convert.ToInt32(n)));
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());

            _model.StartNewGame(8);
        }

        private void onGameStarted(object sender, EventArguments.GameStartedEventArgs e)
        {
            Bowls = e.Bowls;
            activePlayer = e.ActivePlayer;
            redActivePlayer = activePlayer ? true : false;
            blueActivePlayer = activePlayer ? false : true;
            CanAgain = e.CanAgain;

            BlueBowls.Clear();
            RedBowls.Clear();
            MainBowls.Clear();

            for(int i = 0; i < Bowls.Count; i++)
            {
                if (i == (Bowls.Count - 2) / 2)
                {
                    var bowl = new BowlViewModel(i, Bowls.ElementAt(i).Value, "IndianRed", redActivePlayer, blueActivePlayer);
                    MainBowls.Add(bowl);
                }
                else if (i == (Bowls.Count - 2) + 1)
                {
                    var bowl = new BowlViewModel(i, Bowls.ElementAt(i).Value, "Aqua", redActivePlayer, blueActivePlayer);
                    MainBowls.Add(bowl);
                }
                else if(i < (Bowls.Count - 2) / 2)
                {
                    var bowl = new BowlViewModel(i, Bowls.ElementAt(i).Value, "IndianRed", redActivePlayer, blueActivePlayer);
                    bowl.BowlPressed += OnBowlPressed;
                    RedBowls.Add(bowl);
                }
                else
                {
                    var bowl = new BowlViewModel(i, Bowls.ElementAt(i).Value, "Aqua", redActivePlayer, blueActivePlayer);
                    bowl.BowlPressed += OnBowlPressed;
                    BlueBowls.Add(bowl);
                }
            }
        }

        private void OnBowlPressed(object sender, EventArgs e)
        {
            var bowl = sender as BowlViewModel;
            if (bowl is not null)
            {
                _model.Step(bowl.Id, bowl.RocksInBowl);

                bowls[bowls.ElementAt(bowl.Id).Key] = 0;
                bowl.RocksInBowl = 0;

                activePlayer = _model.WhoIsNext();
                redActivePlayer = activePlayer ? true : false;
                blueActivePlayer = activePlayer ? false : true;

                for (int i = 0; i < bowls.Count; i++)
                {
                    if(i == (Bowls.Count - 2) / 2)
                    {
                        MainBowls[0].RocksInBowl = bowls.ElementAt(i).Value;
                    }
                    else if(i == (Bowls.Count - 2) + 1)
                    {
                        MainBowls[1].RocksInBowl = bowls.ElementAt(i).Value;
                    }
                    else if (i < (Bowls.Count - 2) / 2)
                    {
                        RedBowls[i].RocksInBowl = bowls.ElementAt(i).Value;
                        if(RedBowls[i].RocksInBowl == 0)
                        {
                            RedBowls[i].RedActivePlayer = false;
                            RedBowls[i].BlueActivePlayer = false;
                        }
                        else
                        {
                            RedBowls[i].RedActivePlayer = redActivePlayer;
                            RedBowls[i].BlueActivePlayer = blueActivePlayer;
                        }
                    }
                    else
                    {
                        BlueBowls[i - RedBowls.Count - 1].RocksInBowl = bowls.ElementAt(i).Value;
                        if (BlueBowls[i - RedBowls.Count - 1].RocksInBowl == 0)
                        {
                            BlueBowls[i - RedBowls.Count - 1].RedActivePlayer = false;
                            BlueBowls[i - RedBowls.Count - 1].BlueActivePlayer = false;
                        }
                        else
                        {
                            
                            BlueBowls[i - RedBowls.Count - 1].RedActivePlayer = redActivePlayer;
                            BlueBowls[i - RedBowls.Count - 1].BlueActivePlayer = blueActivePlayer;
                        }
                    }
                }

                _model.CheckGameOver();
            }
        }

        private void OnLoadGame()
        {
            if(LoadGame is not null)
            {
                LoadGame(this, EventArgs.Empty);
            }
        }

        private void OnSaveGame()
        {
            if (SaveGame is not null)
            {
                SaveGame(this, EventArgs.Empty);
            }
        }
    }
}
