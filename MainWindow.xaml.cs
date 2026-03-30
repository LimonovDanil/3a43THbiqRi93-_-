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
using CoffeeShop.Pages;

namespace CoffeeShop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigated += MainFrame_Navigated;
        }

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
           
            bool isAuthPage = e.Content is LoginPage || e.Content is RegisterPage;

            headerRect.Visibility = isAuthPage ? Visibility.Collapsed : Visibility.Visible;
            headerPanel.Visibility = isAuthPage ? Visibility.Collapsed : Visibility.Visible;
            menuPanel.Visibility = isAuthPage ? Visibility.Collapsed : Visibility.Visible;

          
            if (!isAuthPage && App.CurrentUser != null)
            {
                UpdateMenuByRole();
                txtUserInfo.Text = $"Добро пожаловать, {App.CurrentUser.FullName}";
            }
        }

        private void UpdateMenuByRole()
        {
            if (App.CurrentUser.RoleId == 1) 
            {               
                btnProducts.Visibility = Visibility.Visible;
                
            }
            else if (App.CurrentUser.RoleId == 2) 
            {             
                btnProducts.Visibility = Visibility.Visible;

            }
            else 
            {
                btnProducts.Visibility = Visibility.Visible;
            }
        }

        private void btnProducts_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductsPage());
        }


        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            MainFrame.Navigate(new LoginPage());
        }
    }
}
