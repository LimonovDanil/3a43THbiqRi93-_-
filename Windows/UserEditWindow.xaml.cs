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
using System.Windows.Shapes;

namespace CoffeeShop.Windows
{
    public partial class UserEditWindow : Window
    {
        private Users _user;
        private bool _isEdit;
        private bool _passwordChanged = false;

        public UserEditWindow(Users user = null)
        {
            InitializeComponent();
            LoadRoles();

            if (user != null)
            {
                _isEdit = true;
                _user = user;
                LoadUserData();
                Title = "✏️ Редактирование пользователя";
            }
            else
            {
                _isEdit = false;
                _user = new Users();
                Title = "➕ Добавление пользователя";
            }
        }

        private void LoadRoles()
        {
            try
            {
                var roles = DBClass.connect.Roles.ToList();
                cmbRole.ItemsSource = roles;
                cmbRole.SelectedValuePath = "Id";
                cmbRole.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки ролей: {ex.Message}");
            }
        }

        private void LoadUserData()
        {
            txtLogin.Text = _user.Login;
            txtFullName.Text = _user.FullName;
            cmbRole.SelectedValue = _user.RoleId;
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            string roleName = cmbRole.SelectedItem != null ?
                ((Roles)cmbRole.SelectedItem).Name : "Не выбрана";

            string passwordStatus = _passwordChanged ? "Пароль будет изменен" : "Пароль не изменен";

            txtInfo.Text = $"Роль: {roleName}\n{passwordStatus}";
        }

        private void txtLogin_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLogin.Text) && txtLogin.Text.Length < 3)
            {
                txtError.Text = "Логин должен содержать минимум 3 символа!";
                txtLogin.Background = System.Windows.Media.Brushes.LightPink;
            }
            else
            {
                if (txtError.Text == "Логин должен содержать минимум 3 символа!")
                    txtError.Text = "";
                txtLogin.Background = System.Windows.Media.Brushes.White;
            }
        }

        private void txtFullName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtFullName.Text) && txtFullName.Text.Length < 3)
            {
                txtError.Text = "Имя должно содержать минимум 3 символа!";
                txtFullName.Background = System.Windows.Media.Brushes.LightPink;
            }
            else
            {
                if (txtError.Text == "Имя должно содержать минимум 3 символа!")
                    txtError.Text = "";
                txtFullName.Background = System.Windows.Media.Brushes.White;
            }
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _passwordChanged = true;
            UpdateInfo();

            if (!string.IsNullOrEmpty(txtPassword.Password) && txtPassword.Password.Length < 3)
            {
                txtError.Text = "Пароль должен содержать минимум 3 символа!";
                txtPassword.Background = System.Windows.Media.Brushes.LightPink;
            }
            else
            {
                if (txtError.Text == "Пароль должен содержать минимум 3 символа!")
                    txtError.Text = "";
                txtPassword.Background = System.Windows.Media.Brushes.White;
            }

            CheckPasswordMatch();
        }

        private void txtConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CheckPasswordMatch();
        }

        private void CheckPasswordMatch()
        {
            if (_passwordChanged && txtPassword.Password != txtConfirmPassword.Password)
            {
                txtError.Text = "Пароли не совпадают!";
                txtConfirmPassword.Background = System.Windows.Media.Brushes.LightPink;
            }
            else
            {
                if (txtError.Text == "Пароли не совпадают!")
                    txtError.Text = "";
                txtConfirmPassword.Background = System.Windows.Media.Brushes.White;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                txtError.Text = "Введите логин!";
                txtLogin.Focus();
                return;
            }

            if (txtLogin.Text.Length < 3)
            {
                txtError.Text = "Логин должен содержать минимум 3 символа!";
                txtLogin.Focus();
                return;
            }

 
            if (!_isEdit && string.IsNullOrEmpty(txtPassword.Password))
            {
                txtError.Text = "Введите пароль!";
                txtPassword.Focus();
                return;
            }


            if (_passwordChanged)
            {
                if (string.IsNullOrEmpty(txtPassword.Password))
                {
                    txtError.Text = "Введите пароль!";
                    txtPassword.Focus();
                    return;
                }

                if (txtPassword.Password.Length < 3)
                {
                    txtError.Text = "Пароль должен содержать минимум 3 символа!";
                    txtPassword.Focus();
                    return;
                }

                if (txtPassword.Password != txtConfirmPassword.Password)
                {
                    txtError.Text = "Пароли не совпадают!";
                    txtConfirmPassword.Focus();
                    return;
                }
            }

  
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                txtError.Text = "Введите полное имя!";
                txtFullName.Focus();
                return;
            }

            if (txtFullName.Text.Length < 3)
            {
                txtError.Text = "Имя должно содержать минимум 3 символа!";
                txtFullName.Focus();
                return;
            }

  
            if (cmbRole.SelectedItem == null)
            {
                txtError.Text = "Выберите роль!";
                cmbRole.Focus();
                return;
            }

            try
            {
             
                var existingUser = DBClass.connect.Users
                    .FirstOrDefault(u => u.Login.ToLower() == txtLogin.Text.Trim().ToLower()
                                        && (!_isEdit || u.Id != _user.Id));

                if (existingUser != null)
                {
                    txtError.Text = "Пользователь с таким логином уже существует!";
                    txtLogin.Focus();
                    return;
                }

                _user.Login = txtLogin.Text.Trim();

                if (_passwordChanged || !_isEdit)
                {
                    _user.Password = txtPassword.Password;
                }

                _user.FullName = txtFullName.Text.Trim();
                _user.RoleId = (int)cmbRole.SelectedValue;

                if (!_isEdit)
                {
                    DBClass.connect.Users.Add(_user);
                }

                DBClass.connect.SaveChanges();

                MessageBox.Show($"Пользователь успешно {(_isEdit ? "обновлен" : "добавлен")}!",
                              "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите отменить изменения?",
                                        "Подтверждение",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DialogResult = false;
                Close();
            }
        }
    }
}
