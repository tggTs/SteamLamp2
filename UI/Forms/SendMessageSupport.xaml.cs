using System.Windows;
using System.Windows.Controls;

namespace SteamLamp
{
    public partial class SendMessageSupport : UserControl
    {
        public SendMessageSupport()
        {
            InitializeComponent();
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SupportMessageInput.Text))
            {
                SteamMessageBox.Show("Пожалуйста, опишите вашу проблему перед отправкой.", "Внимание", Window.GetWindow(this));
                return;
            }
            SteamMessageBox.Show("Ваше обращение успешно отправлено в службу поддержки SteamLamp!", "Успех", Window.GetWindow(this));

            SupportMessageInput.Clear();
            TopicComboBox.SelectedIndex = 0;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            SupportMessageInput.Clear();
            var mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.OpenStore_Click(null, null);
        }
    }
}