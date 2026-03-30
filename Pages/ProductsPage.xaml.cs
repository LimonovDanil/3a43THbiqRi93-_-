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
        private readonly List<string> _imageFiles = new List<string>
        {
            "1.jpg", "2.png", "3.jpg", "4.jpg", "5.jpg", "6.jpg", "7.jpg", "8.jpeg", "9.jpg", "10.jpg",
            "11.jpg", "12.jpeg", "13.png", "14.jpg", "15.jpg", "16.jpg", "17.jpg", "18.jpg", "19.jpg", "20.jpg",
            "21.jpg", "22.jpg", "23.jpg", "24.png", "25.jpg", "26.jpg", "27.jpg", "28.jpg"
        };

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
                // Получаем только не удалённые товары
                var products = DBClass.connect.Products.Where(p => !(bool)p.IsDeleted).ToList();

                // Остальная фильтрация (по категории и поиску)
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

                // Добавляем путь к изображению
                var productsWithImage = products.Select(p => new ProductWithImage
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Categories = p.Categories,
                    CategoryId = p.CategoryId,
                    ImagePath = GetImagePathForProduct(p.Id)
                }).ToList();

                dgProducts.ItemsSource = productsWithImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}");
            }
        }
        private string GetImagePathForProduct(int productId)
        {
            int index = (productId - 1) % _imageFiles.Count;
            string fileName = _imageFiles[index];      
            return $"pack://application:,,,/Images/{fileName}";
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
            var product = button?.Tag as ProductWithImage;
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

                var realProduct = DBClass.connect.Products.FirstOrDefault(p => p.Id == product.Id);
                if (realProduct != null)
                {
                    var window = new CreateOrderWindow(realProduct);
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
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button?.Tag as ProductWithImage;
            if (product != null)
            {
                var realProduct = DBClass.connect.Products.FirstOrDefault(p => p.Id == product.Id);
                if (realProduct != null)
                {
                    var window = new ProductEditWindow(realProduct);
                    window.Owner = Application.Current.MainWindow;
                    if (window.ShowDialog() == true)
                    {
                        LoadProducts();
                    }
                }
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button?.Tag as ProductWithImage;
            if (product == null) return;

            var realProduct = DBClass.connect.Products.FirstOrDefault(p => p.Id == product.Id);
            if (realProduct == null) return;

            var result = MessageBox.Show($"Удалить товар '{realProduct.Name}' из каталога?",
                                         "Подтверждение",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                // Мягкое удаление
                realProduct.IsDeleted = true;
                DBClass.connect.SaveChanges();

                MessageBox.Show("Товар удалён из каталога.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadProducts(); // обновляем список
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении товара:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

   
    public class ProductWithImage : Products
    {
        public string ImagePath { get; set; }
    }
}