using CoffeeShop.DB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CoffeeShop
{
    public partial class App : Application
    {
        public static Users CurrentUser { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Создаем главное окно и показываем страницу входа
            MainWindow mainWindow = new MainWindow();
            mainWindow.MainFrame.Navigate(new Pages.LoginPage());
            mainWindow.Show();
        }
    }
}
