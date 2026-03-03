using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SteamLamp
{
    public partial class EditProfileControl : UserControl
    {
        public EditProfileControl()
        {
            InitializeComponent();
            if (Session.CurrentUser != null)
            {
                EditNickname.Text = Session.CurrentUser.Nickname;
                EditBio.Text = Session.CurrentUser.Bio;
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
                var colorStr = btn.Tag.ToString();
                var bc = new BrushConverter();
                CurrentAvatarPreview.Background = (Brush)bc.ConvertFromString(colorStr);

                CloseAvatarPicker_Click(null, null);
                EnableSaveButton();
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
                    db.SaveChanges();
                    Session.CurrentUser = user;
                    var win = Window.GetWindow(this) as MainWindow;
                    if (win != null)
                    {
                        win.OpenProfile_Click(null, null);
                    }
                }
            }
        }
    }
}