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
    public partial class ProductEditWindow : Window
    {
        private Products _product;
        private bool _isEdit;

        public ProductEditWindow(Products product = null)
        {
            InitializeComponent();
            LoadCategories();

            if (product != null)
            {
                _isEdit = true;
                _product = product;
                LoadProductData();
                Title = "✏️ Редактирование товара";
            }
            else
            {
                _isEdit = false;
                _product = new Products();
                Title = "➕ Добавление товара";
            }
        }

        private void LoadCategories()
        {
            try
            {
                var categories = DBClass.connect.Categories.ToList();
                cmbCategory.ItemsSource = categories;
                cmbCategory.SelectedValuePath = "Id";
                cmbCategory.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}");
            }
        }

        private void LoadProductData()
        {
            txtName.Text = _product.Name;
            txtPrice.Text = _product.Price.ToString();
            cmbCategory.SelectedValue = _product.CategoryId;
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            string name = string.IsNullOrEmpty(txtName.Text) ? "Название товара" : txtName.Text;
            string price = string.IsNullOrEmpty(txtPrice.Text) ? "0" : txtPrice.Text;
            string category = cmbCategory.SelectedItem != null ?
                ((Categories)cmbCategory.SelectedItem).Name : "Категория не выбрана";

            txtPreview.Text = $"{name}\nЦена: {price} ₽\nКатегория: {category}";
        }

        private void txtName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void txtPrice_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdatePreview();

            // Валидация цены
            if (!string.IsNullOrEmpty(txtPrice.Text))
            {
                if (decimal.TryParse(txtPrice.Text, out decimal price))
                {
                    if (price < 0)
                    {
                        txtError.Text = "Цена не может быть отрицательной!";
                        txtPrice.Background = System.Windows.Media.Brushes.LightPink;
                    }
                    else
                    {
                        txtError.Text = "";
                        txtPrice.Background = System.Windows.Media.Brushes.White;
                    }
                }
                else
                {
                    txtError.Text = "Введите корректную цену!";
                    txtPrice.Background = System.Windows.Media.Brushes.LightPink;
                }
            }
        }

        private void txtPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Проверка заполнения полей
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                txtError.Text = "Введите название товара!";
                txtName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                txtError.Text = "Введите цену товара!";
                txtPrice.Focus();
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price))
            {
                txtError.Text = "Введите корректную цену!";
                txtPrice.Focus();
                return;
            }

            if (price <= 0)
            {
                txtError.Text = "Цена должна быть больше 0!";
                txtPrice.Focus();
                return;
            }

            if (cmbCategory.SelectedItem == null)
            {
                txtError.Text = "Выберите категорию товара!";
                cmbCategory.Focus();
                return;
            }

            try
            {
                _product.Name = txtName.Text.Trim();
                _product.Price = price;
                _product.CategoryId = (int)cmbCategory.SelectedValue;

                if (!_isEdit)
                {
                    DBClass.connect.Products.Add(_product);
                }

                DBClass.connect.SaveChanges();

                MessageBox.Show($"Товар успешно {(_isEdit ? "обновлен" : "добавлен")}!",
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
