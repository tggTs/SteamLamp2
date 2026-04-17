using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using System.IO;

namespace SteamLamp.UI.Forms
{
    /// <summary>
    /// Логика взаимодействия для AdminDataPage.xaml
    /// </summary>
    public partial class AdminDataPage : UserControl 
    {
        public AdminDataPage()
        {
            InitializeComponent();
            LoadUsers();
        }
        private void LoadUsers()
        {
            using (var db = new AppDbContext())
            {
                UsersListBox.ItemsSource = db.Users.ToList();
            }
        }
        private void UsersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersListBox.SelectedItem is User user)
            {
                SelectedUserText.Text = user.Nickname;
            }
        }
        private void ExportJson_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListBox.SelectedItem is User selectedUser)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "JSON Files (*.json)|*.json";
                sfd.FileName = $"backup_{selectedUser.Nickname}_{DateTime.Now:ddMMyy}";

                if (sfd.ShowDialog() == true)
                {
                    try
                    {

                        var options = new JsonSerializerOptions { WriteIndented = true };
                        string json = JsonSerializer.Serialize(selectedUser, options);

                        File.WriteAllText(sfd.FileName, json);
                        MessageBox.Show("Файл успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при экспорте: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Сначала выберите пользователя из списка.");
            }
        }
    }
}
