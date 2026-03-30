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
    public partial class ClientPage : Page
    {
        public ClientPage()
        {
            InitializeComponent();
            Loaded += ClientPage_Loaded;
        }

        private void ClientPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ClientFrame.Navigate(new ProductsPage());
        }
    }
}
