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
using CoffeeShop.Windows;

namespace CoffeeShop.Pages
{
    public partial class ProductsPage : Page
    {
        public ProductsPage()
        {
            InitializeComponent();
            LoadCategories();
            LoadProducts();
            if (App.CurrentUser != null && App.CurrentUser.RoleId == 1)
            {
                btnAdd.Visibility = Visibility.Visible;
            }
        }
        private void LoadCategories()
        {
            try
            {
                var categories = DBClass.connect.Categories.ToList();
                categories.Insert(0, new Categories { Id = 0, Name = "Все категории" });
                cmbCategory.ItemsSource = categories;
                cmbCategory.SelectedValuePath = "Id";
                cmbCategory.DisplayMemberPath = "Name";
                cmbCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}");
            }
        }
        private void LoadProducts()
        {
            try
            {
                var products = DBClass.connect.Products.ToList();
                if (cmbCategory.SelectedItem != null && (int)cmbCategory.SelectedValue != 0)
                {
                    int categoryId = (int)cmbCategory.SelectedValue;
                    products = products.Where(p => p.CategoryId == categoryId).ToList();
                }
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    string search = txtSearch.Text.ToLower();
                    products = products.Where(p => p.Name.ToLower().Contains(search)).ToList();
                }

                dgProducts.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}");
            }
        }
        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadProducts();
        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadProducts();
        }
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            cmbCategory.SelectedIndex = 0;
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var window = new ProductEditWindow();
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true)
            {
                LoadProducts();
            }
        }
        private void BuyProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button?.Tag as Products;
            if (product != null)
            {
                if (App.CurrentUser == null)
                {
                    MessageBox.Show("Пожалуйста, авторизуйтесь для оформления заказа!",
                                  "Требуется авторизация",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                    var mainWindow = Application.Current.MainWindow as MainWindow;
                    if (mainWindow != null)
                    {
                        mainWindow.MainFrame.Navigate(new LoginPage());
                    }
                    return;
                }
                if (App.CurrentUser.RoleId != 3)
                {
                    MessageBox.Show("Только клиенты могут оформлять заказы!",
                                  "Доступ запрещен",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                    return;
                }
                var window = new CreateOrderWindow(product);
                window.Owner = Application.Current.MainWindow;

                if (window.ShowDialog() == true)
                {
                    LoadProducts();
                    MessageBox.Show("Заказ успешно создан! Вы можете отслеживать его статус в разделе 'Мои заказы'.",
                                  "Успех",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }
            }
        }
        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button?.Tag as Products;

            if (product != null)
            {
                var window = new ProductEditWindow(product);
                window.Owner = Application.Current.MainWindow;
                if (window.ShowDialog() == true)
                {
                    LoadProducts();
                }
            }
        }
        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button?.Tag as Products;

            if (product != null)
            {
                var result = MessageBox.Show($"Удалить товар '{product.Name}'?",
                                           "Подтверждение",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var hasOrders = DBClass.connect.OrderItems.Any(oi => oi.ProductId == product.Id);

                        if (hasOrders)
                        {
                            var confirmResult = MessageBox.Show(
                                "Этот товар есть в заказах. При удалении информация о нем в заказах сохранится, " +
                                "но сам товар исчезнет из каталога. Продолжить?",
                                "Предупреждение",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning);

                            if (confirmResult != MessageBoxResult.Yes)
                                return;
                        }

                        DBClass.connect.Products.Remove(product);
                        DBClass.connect.SaveChanges();

                        MessageBox.Show("Товар успешно удален!", "Успех",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadProducts();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}