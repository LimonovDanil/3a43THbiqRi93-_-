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
    public partial class OrderConfirmationWindow : Window
    {
        public OrderConfirmationWindow(Orders order, Products product, int quantity)
        {
            InitializeComponent();

            txtOrderInfo.Text = $"Номер заказа: {order.Id}\n\n" +
                               $"Товар: {product.Name}\n" +
                               $"Количество: {quantity}\n" +
                               $"Сумма: {order.TotalAmount:C}\n\n" +
                               $"Статус: {order.Status}";
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
