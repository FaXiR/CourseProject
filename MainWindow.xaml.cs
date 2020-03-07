using System;
using System.Collections.Generic;
using System.IO;
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
using CourseProject.Modules;

namespace CourseProject
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Упрощенное взаимодействие с таблицами
        /// </summary>
        private Tables Table;

        /// <summary>
        /// ФИО авторизованного пользователя
        /// </summary>
        private string UserFIO = null;

        /// <summary>
        /// Упрощенное взаимодействие с БД
        /// </summary>
        private UsingAccess UsAc;

        /// <summary>
        /// Путь до БД
        /// </summary>
        private string BDWay = Environment.CurrentDirectory + "\\db.mdb";

        /// <summary>
        /// Логика взаимодействия для MainWindow.xaml
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            CreateConnection();
            AutorizationUser();
        }

        /// <summary>
        /// Создание подключения
        /// </summary>
        /// <returns>Успех подключения</returns>
        private void CreateConnection()
        {
            //Определение пути до БД
            try
            {
                //Чтение пути до БД из файла
                string way = File.ReadAllLines("db.txt", Encoding.GetEncoding(1251))[0];
                if (way != "")
                {
                    BDWay = way;
                }
            }
            catch { }

            //Подключение к БД
            try
            {
                UsAc = new UsingAccess(BDWay, null, null, null);
                UsAc.AutoOpen = true;
            }
            catch
            {
                MessageBox.Show("Не удалось подключится к базе данных, пожалуйста, обратитесь к администратору");
                this.Close();
                return;
            }

            //Присоединение таблиц
            Table = new Tables(UsAc);
        }

        /// <summary>
        /// Подключение к БД и авторизация пользователя
        /// </summary>
        private void AutorizationUser()
        {
            //Авторизация пользователя
            var window = new Windows.AuthorizationWindow(Table.Users);
            if (window.ShowDialog() == true)
            {
                UserFIO = window.FIO;
                this.Show();
            }
            else
            {
                //Вход был отменен
                this.Close();
                return;
            }
        }

        /// <summary>
        /// Событие при закрытии приложения
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Если подключения к БД нет или пользователь не авторизован - закрыть приложение без раздумий
            if (UsAc == null || UserFIO == null)
            {
                return;                
            }

            //Опрос пользователя
            if (MessageBox.Show("Выйти из программы?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
            {
                try
                {
                    UsAc.ConnectClose();
                }
                finally
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
