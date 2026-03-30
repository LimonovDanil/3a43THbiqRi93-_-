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
    public partial class CategoryEditWindow : Window
    {
        private Categories _category;
        private bool _isEdit;
        public CategoryEditWindow(Categories category = null)
        {
            InitializeComponent();

            if (category != null)
            {
                _isEdit = true;
                _category = category;
                LoadCategoryData();
                Title = "✏️ Редактирование категории";
            }
            else
            {
                _isEdit = false;
                _category = new Categories();
                Title = "➕ Добавление категории";
                txtStats.Text = "Новая категория";
            }
        }
        private void LoadCategoryData()
        {
            txtName.Text = _category.Name;
            UpdatePreview();
            UpdateStats();
        }
        private void UpdatePreview()
        {
            string name = string.IsNullOrEmpty(txtName.Text) ? "Название категории" : txtName.Text;
            txtPreview.Text = name;
        }
        private void UpdateStats()
        {
            try
            {
                if (_isEdit && _category.Id > 0)
                {
                    int productCount = DBClass.connect.Products
                        .Count(p => p.CategoryId == _category.Id);

                    txtStats.Text = $"Товаров в категории: {productCount}";

                    if (productCount > 0)
                    {
                        txtStats.Foreground = System.Windows.Media.Brushes.Orange;
                    }
                    else
                    {
                        txtStats.Foreground = System.Windows.Media.Brushes.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                txtStats.Text = "Ошибка загрузки статистики";
            }
        }
        private void txtName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdatePreview();

            if (!string.IsNullOrEmpty(txtName.Text) && txtName.Text.Length < 2)
            {
                txtError.Text = "Название должно содержать минимум 2 символа!";
                txtName.Background = System.Windows.Media.Brushes.LightPink;
            }
            else
            {
                txtError.Text = "";
                txtName.Background = System.Windows.Media.Brushes.White;
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                txtError.Text = "Введите название категории!";
                txtName.Focus();
                return;
            }

            if (txtName.Text.Length < 2)
            {
                txtError.Text = "Название должно содержать минимум 2 символа!";
                txtName.Focus();
                return;
            }

            try
            {
                var existingCategory = DBClass.connect.Categories
                    .FirstOrDefault(c => c.Name.ToLower() == txtName.Text.Trim().ToLower()
                                        && (!_isEdit || c.Id != _category.Id));

                if (existingCategory != null)
                {
                    txtError.Text = "Категория с таким названием уже существует!";
                    txtName.Focus();
                    return;
                }

                _category.Name = txtName.Text.Trim();

                if (!_isEdit)
                {
                    DBClass.connect.Categories.Add(_category);
                }

                DBClass.connect.SaveChanges();

                MessageBox.Show($"Категория успешно {(_isEdit ? "обновлена" : "добавлена")}!",
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
            var result = MessageBox.Show("Вы действительно хотите отменить изменения?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);      
            if (result == MessageBoxResult.Yes)
            {
                DialogResult = false;
                Close();
            }
        }
    }
}
