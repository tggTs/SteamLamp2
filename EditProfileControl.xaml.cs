using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SteamLamp
{
    public partial class EditProfileControl : UserControl
    {
        private byte[] _selectedAvatarBytes = null;
        public EditProfileControl()
        {
            InitializeComponent();

            if (Session.CurrentUser != null)
            {
                EditNickname.Text = Session.CurrentUser.Nickname;
                EditBio.Text = Session.CurrentUser.Bio;
                if (Session.CurrentUser.Avatar != null)
                {
                    CurrentAvatarPreviewImage.Source = BytesToImage(Session.CurrentUser.Avatar);
                }
            }
        }

        private void Field_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableSaveButton();
        }

        private void EnableSaveButton()
        {
            if (SaveBtn == null) return;
            SaveBtn.IsEnabled = true;
            SaveBtn.Background = new SolidColorBrush(Color.FromRgb(92, 154, 0));
            SaveBtn.Foreground = Brushes.White;
        }

        private void BtnOpenAvatarStore_Click(object sender, RoutedEventArgs e)
        {
            AvatarPickerOverlay.Visibility = Visibility.Visible;
            EditMainPanel.Visibility = Visibility.Collapsed;
        }

        private void AvatarSelect_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                string fileName = btn.Tag.ToString();
                string resourcePath = $"/assets/{fileName}";
                try
                {
                    Uri uri = new Uri(resourcePath, UriKind.Relative);
                    var streamResource = Application.GetResourceStream(uri);

                    if (streamResource == null)
                    {
                        MessageBox.Show($"Файл {fileName} не найден в ресурсах. Проверь Build Action!");
                        return;
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        streamResource.Stream.CopyTo(ms);
                        _selectedAvatarBytes = ms.ToArray();
                    }
                    CurrentAvatarPreviewImage.Source = BytesToImage(_selectedAvatarBytes);
                    CloseAvatarPicker_Click(null, null);
                    EnableSaveButton();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при выборе аватара: " + ex.Message);
                }
            }
        }

        private void CloseAvatarPicker_Click(object sender, RoutedEventArgs e)
        {
            AvatarPickerOverlay.Visibility = Visibility.Collapsed;
            EditMainPanel.Visibility = Visibility.Visible;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var user = db.Users.Find(Session.CurrentUser.Id);
                if (user != null)
                {
                    user.Nickname = EditNickname.Text;
                    user.Bio = EditBio.Text;
                    if (_selectedAvatarBytes != null)
                    {
                        user.Avatar = _selectedAvatarBytes;
                    }
                    db.SaveChanges();
                    Session.CurrentUser = user;
                    var win = Window.GetWindow(this) as MainWindow;
                    if (win != null)
                    {
                        win.AccountMenuButton.Content = user.Nickname + " ▼";
                        win.OpenProfile_Click(null, null);
                    }
                }
            }
        }
        private BitmapImage BytesToImage(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(bytes))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
    }
}