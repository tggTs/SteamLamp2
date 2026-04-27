using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using SteamLamp.UI.Forms;

namespace SteamLamp.UI.Forms
{
    public partial class AddGameWindow : Window
    {
        private string previewPath = "";
        private string bannerPath = "";

        private DispatcherTimer checkTimer;
        private double currentProgress = 0;
        private const double TotalDurationSeconds = 5;

        public AddGameWindow()
        {
            InitializeComponent();
        }

        private void SelectPreview_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Images|*.jpg;*.png;*.jpeg" };
            if (ofd.ShowDialog() == true)
            {
                previewPath = ofd.FileName;
                LblPreviewPath.Text = Path.GetFileName(previewPath);
            }
        }

        private void SelectBanner_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Images|*.jpg;*.png;*.jpeg" };
            if (ofd.ShowDialog() == true)
            {
                bannerPath = ofd.FileName;
                LblBannerPath.Text = Path.GetFileName(bannerPath);
            }
        }

        private void SubmitGame_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTitle.Text) ||
                string.IsNullOrWhiteSpace(TxtDescription.Text) ||
                string.IsNullOrWhiteSpace(TxtDeveloper.Text) ||
                string.IsNullOrWhiteSpace(previewPath) ||
                string.IsNullOrWhiteSpace(bannerPath) ||
                !decimal.TryParse(TxtPrice.Text, out decimal price))
            {
                MessageBox.Show("Пожалуйста, заполните все текстовые поля, выберите изображения и укажите корректную цену!",
                                "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MainForm.Visibility = Visibility.Collapsed;
            ProgressOverlay.Visibility = Visibility.Visible;
            currentProgress = 0;
            PublishProgressBar.Value = 0;
            TimerText.Text = "0%";
            checkTimer = new DispatcherTimer();
            checkTimer.Interval = TimeSpan.FromMilliseconds(50);
            checkTimer.Tick += CheckTimer_Tick;
            checkTimer.Start();
        }

        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            double step = (50.0 / (TotalDurationSeconds * 1000.0)) * 100.0;
            currentProgress += step;

            if (currentProgress >= 100)
            {
                currentProgress = 100;
                checkTimer.Stop();

                PublishProgressBar.Value = currentProgress;
                TimerText.Text = "100%";

                FinalizePublishing();
            }
            else
            {
                PublishProgressBar.Value = currentProgress;
                TimerText.Text = $"{(int)currentProgress}%";
            }
        }

        private void FinalizePublishing()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    
                    string formattedPrice = TxtPrice.Text.Trim() + " руб.";

                    var newGame = new Game
                    {
                        Title = TxtTitle.Text,
                        Description = TxtDescription.Text,
                        Price = formattedPrice,
                        Developer = TxtDeveloper.Text,
                        ImagePath = previewPath,
                        DetailedImagePath = bannerPath
                    };

                    db.Games.Add(newGame);
                    db.SaveChanges();
                }

                MessageBox.Show("Проверка пройдена! Игра опубликована.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
                MainForm.Visibility = Visibility.Visible;
                ProgressOverlay.Visibility = Visibility.Collapsed;
            }
        }
    }
}