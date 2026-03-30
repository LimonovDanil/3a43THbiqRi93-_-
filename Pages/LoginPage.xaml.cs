using CoffeeShop.DB;
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


namespace CoffeeShop.Pages
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                txtError.Text = "Введите логин и пароль!";
                return;
            }
            try
            {
                var user = DBClass.connect.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

                if (user != null)
                {
                    App.CurrentUser = user;


                    var mainWindow = Application.Current.MainWindow as MainWindow;
                    if (mainWindow != null)
                    {
                        if (user.RoleId == 1) 
                        {
                            mainWindow.MainFrame.Navigate(new AdminPage());
                        }
                        else if (user.RoleId == 2) 
                        {
                            mainWindow.MainFrame.Navigate(new BaristaPage());
                        }
                        else 
                        {
                            mainWindow.MainFrame.Navigate(new ClientPage());
                        }
                    }
                }
                else
                {
                    txtError.Text = "Неверный логин или пароль!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Navigate(new RegisterPage());
            }
        }
    }
}
