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
                TxtUserId.Text = user.Id.ToString();
                TxtUserRole.Text = user.Role;
                TxtUserBalance.Text = $"{user.Balance:N2} руб.";
                UserDetailsPanel.Visibility = Visibility.Visible;
            }
            else
            {
                SelectedUserText.Text = "Выберите пользователя";
                UserDetailsPanel.Visibility = Visibility.Collapsed;
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

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListBox.SelectedItem is User selectedUser)
            {

                if (Session.CurrentUser != null && selectedUser.Id == Session.CurrentUser.Id)
                {
                    MessageBox.Show("Вы не можете удалить собственную учетную запись администратора!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show($"Вы уверены, что хотите БЕЗВОЗВРАТНО удалить пользователя {selectedUser.Nickname}?",
                                           "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var db = new AppDbContext())
                        {

                            var userInDb = db.Users.Find(selectedUser.Id);
                            if (userInDb != null)
                            {
                                db.Users.Remove(userInDb);
                                db.SaveChanges();
                                MessageBox.Show("Пользователь успешно удален.");
                                LoadUsers(); // Обновляем список слева
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}\nВозможно, у пользователя есть связанные данные (покупки).");
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
