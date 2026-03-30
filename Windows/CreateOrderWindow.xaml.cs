using CoffeeShop.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class CreateOrderWindow : Window
    {
        private Products _product;
        private int _quantity = 1;
        private decimal _unitPrice;
        public CreateOrderWindow(Products product)
        {
            InitializeComponent();
            _product = product;
            _unitPrice = product.Price;

            LoadProductInfo();
            UpdateTotal();
        }
        private void LoadProductInfo()
        {
            txtProductName.Text = _product.Name;
            txtProductPrice.Text = $"Цена: {_unitPrice:C}";
        }
        private void UpdateTotal()
        {
            decimal total = _unitPrice * _quantity;
            txtTotal.Text = $"{total:C}";
        }
        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            _quantity++;
            txtQuantity.Text = _quantity.ToString();
            UpdateTotal();
        }
        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (_quantity > 1)
            {
                _quantity--;
                txtQuantity.Text = _quantity.ToString();
                UpdateTotal();
            }
        }
   private void txtQuantity_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (int.TryParse(txtQuantity.Text, out int newQuantity))
            {
                if (newQuantity >= 1 && newQuantity <= 100)
                {
                    _quantity = newQuantity;
                    UpdateTotal();
                }
                else
                {
                    txtQuantity.Text = _quantity.ToString();
                }
            }
            else if (!string.IsNullOrEmpty(txtQuantity.Text))
            {
                txtQuantity.Text = _quantity.ToString();
            }
        }

        private void txtQuantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.CurrentUser == null)
                {
                    MessageBox.Show("Пожалуйста, авторизуйтесь для оформления заказа!",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    DialogResult = false;
                    Close();
                    return;
                }
                Orders order = new Orders
                {
                    OrderDate = DateTime.Now,
                    ClientId = App.CurrentUser.Id,
                    BaristaId = null,
                    TotalAmount = _unitPrice * _quantity,
                    Status = "Новый"
                };
                DBClass.connect.Orders.Add(order);
                DBClass.connect.SaveChanges();
                OrderItems orderItem = new OrderItems
                {
                    OrderId = order.Id,
                    ProductId = _product.Id,
                    Quantity = _quantity,
                    PriceAtMoment = _unitPrice
                };
                DBClass.connect.OrderItems.Add(orderItem);
                if (!string.IsNullOrEmpty(txtComment.Text) || !string.IsNullOrEmpty(txtWishes.Text))
                {
                    string comment = $"Комментарий: {txtComment.Text}\nПожелания: {txtWishes.Text}";
                }
                DBClass.connect.SaveChanges();
                MessageBox.Show($"Заказ №{order.Id} успешно оформлен!\n\n" +
                              $"Товар: {_product.Name}\n" +
                              $"Количество: {_quantity}\n" +
                              $"Сумма: {(_unitPrice * _quantity):C}\n\n" +
                              $"Статус: {order.Status}",
                              "Заказ оформлен",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оформлении заказа:\n{ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите отменить оформление заказа?",
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
