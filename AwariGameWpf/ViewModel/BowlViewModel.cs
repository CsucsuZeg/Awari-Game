using System;

namespace AwariGameWpf.ViewModel
{
    public class BowlViewModel: ViewModelBase
    {
        private int rocksInBowl;
        private bool redActivePlayer;
        private bool blueActivePlayer;

        public int RocksInBowl { get => rocksInBowl; set { rocksInBowl = value; OnPropertyChanged(); } }
        public string BowlColor { get; set; }
        public int Id { get; set; }
        public bool RedActivePlayer { get => redActivePlayer; set { redActivePlayer = value; OnPropertyChanged(); } }
        public bool BlueActivePlayer { get => blueActivePlayer; set { blueActivePlayer = value; OnPropertyChanged(); } }

        public DelegateCommand ClickCommand { get; set; }

        public event EventHandler<EventArgs> BowlPressed;


        public BowlViewModel(int id, int rocksInBowl, string bowlColor, bool redActive, bool blueActive)
        {
            Id = id;
            RocksInBowl = rocksInBowl;
            BowlColor = bowlColor;
            if(RocksInBowl == 0)
            {
                redActivePlayer = false;
                blueActivePlayer = false;
            }
            else
            {
                redActivePlayer = redActive;
                blueActivePlayer = blueActive;
            }
            ClickCommand = new DelegateCommand(_ => BowlPressed?.Invoke(this, EventArgs.Empty));
        }
    }
}
