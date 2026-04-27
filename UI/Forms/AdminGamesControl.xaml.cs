using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SteamLamp.UI.Forms
{
    public partial class AdminGamesControl : UserControl
    {
        public AdminGamesControl()
        {
            InitializeComponent();
            RefreshData();
        }

        private void RefreshData()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    GamesListBox.ItemsSource = db.Games.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }
        private void GamesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GamesListBox.SelectedItem is Game selectedGame)
            {
                GameDetailsPanel.Visibility = Visibility.Visible;

                SelectedGameTitle.Text = selectedGame.Title;
                TxtGameId.Text = selectedGame.Id.ToString();

                EditTitle.Text = selectedGame.Title;
                EditDeveloper.Text = selectedGame.Developer;
                EditPrice.Text = selectedGame.Price?.ToString();
                EditDescription.Text = selectedGame.Description;
                EditImagePath.Text = selectedGame.ImagePath;
            }
            else
            {
                GameDetailsPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            if (!(GamesListBox.SelectedItem is Game selectedGame)) return;

            try
            {
                using (var db = new AppDbContext())
                {
                    var dbGame = db.Games.Find(selectedGame.Id);
                    if (dbGame != null)
                    {
                        dbGame.Title = EditTitle.Text.Trim();
                        dbGame.Developer = EditDeveloper.Text.Trim();
                        dbGame.Price = EditPrice.Text.Trim();
                        dbGame.Description = EditDescription.Text.Trim(); 
                        dbGame.ImagePath = EditImagePath.Text.Trim();
                        db.SaveChanges();
                        MessageBox.Show("Данные игры успешно обновлены!");
                        RefreshData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }
        private void DeleteGame_Click(object sender, RoutedEventArgs e)
        {
            if (!(GamesListBox.SelectedItem is Game selectedGame)) return;

            if (MessageBox.Show($"Удалить {selectedGame.Title} навсегда?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new AppDbContext())
                    {
                        var dbGame = db.Games.Find(selectedGame.Id);
                        if (dbGame != null)
                        {
                            db.Games.Remove(dbGame);
                            db.SaveChanges();
                            RefreshData();
                            MessageBox.Show("Игра удалена.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message);
                }
            }
        }
    }
}