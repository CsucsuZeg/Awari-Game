using AwariGameWpf.Model;
using AwariGameWpf.Persistence;
using AwariGameWpf.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;

namespace AwariGameWpf
{
    public partial class App : Application
    {
        private MainWindow _mainWindow;
        private AwariViewModel _viewModel;
        private AwariModel _model;

        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            _mainWindow = new MainWindow();

            _model = new AwariModel(new DataAccess());
            _model.GameOver += OnGameFinished;

            _viewModel = new AwariViewModel(_model);
            _viewModel.SaveGame += new EventHandler(OnSaveGame);
            _viewModel.LoadGame += new EventHandler(OnLoadGame);

            _mainWindow.DataContext = _viewModel;
            _mainWindow.Show();
        }

        private async void OnSaveGame(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Awari game saving";
                saveFileDialog.Filter = "Awari Game|*.txt";
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        await _model.SaveGame(saveFileDialog.FileName);
                    }
                    catch (AwariDataException)
                    {
                        MessageBox.Show("Error while saving game!", "Awari", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error while saving game!", "Awari", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void OnLoadGame(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Awari game loading";
                openFileDialog.Filter = "Awari Game|*.txt";
                if (openFileDialog.ShowDialog() == true)
                {
                    await _model.LoadGame(openFileDialog.FileName);
                }
            }
            catch (AwariDataException)
            {
                MessageBox.Show("Error while loading game!", "Awari", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnGameFinished(object sender, EventArguments.GameOverEventArgs e)
        {
            if (!e.Winner.Equals("Tie"))
            {
                MessageBox.Show
                (
                    $"Congratulations {e.Winner} player, you won!",
                    "Game Over",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            else
            {
                MessageBox.Show
                (
                    "It's a tie!" + Environment.NewLine + "Round two? Go ahead and press OK to start!",
                    "Game Over",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            _model.StartNewGame(e.Size);
        }
    }
}
