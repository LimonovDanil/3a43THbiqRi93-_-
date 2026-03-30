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
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            Loaded += AdminPage_Loaded;
        }

        private void AdminPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Показываем товары по умолчанию
            AdminFrame.Navigate(new ProductsPage());
        }
    }
}