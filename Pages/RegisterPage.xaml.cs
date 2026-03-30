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
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;
            string fullName = txtFullName.Text.Trim();
            if (string.IsNullOrEmpty(login))
            {
                txtError.Text = "Введите логин!";
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                txtError.Text = "Введите пароль!";
                return;
            }
            if (password != confirmPassword)
            {
                txtError.Text = "Пароли не совпадают!";
                return;
            }
            if (string.IsNullOrEmpty(fullName))
            {
                txtError.Text = "Введите полное имя!";
                return;
            }
            try
            {
                var existingUser = DBClass.connect.Users.FirstOrDefault(u => u.Login == login);
                if (existingUser != null)
                {
                    txtError.Text = "Пользователь с таким логином уже существует!";
                    return;
                }
                Users newUser = new Users
                {
                    Login = login,
                    Password = password,
                    FullName = fullName,
                    RoleId = 3
                };
                DBClass.connect.Users.Add(newUser);
                DBClass.connect.SaveChanges();

                MessageBox.Show("Регистрация успешно завершена!", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Navigate(new LoginPage());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Navigate(new LoginPage());
            }
        }
    }
}
