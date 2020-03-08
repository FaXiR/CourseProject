using System;
using System.Collections.Generic;
using System.Data;
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
        /// Упрощенное взаимодействие с БД
        /// </summary>
        private UsingAccess UsAc;

        /// <summary>
        /// Путь до БД
        /// </summary>
        private string BDWay = Environment.CurrentDirectory + "\\db.mdb";

        /// <summary>
        /// ФИО авторизованного пользователя
        /// </summary>
        private string UserFIO = null;

        #region базовый код
        /// <summary>
        /// Логика взаимодействия для MainWindow.xaml
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            CreateConnection();
            AutorizationUser();

            FoundDealInList(null);
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
        #endregion

        #region общие методы
        /// <summary>
        /// Событие автогенерации колонок. Отлавливает и корректирует поля Даты.
        /// </summary>
        private void DataGrid_OnAutoGenerating(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd.MM.yyyy";
        }

        /// <summary>
        /// Событие клика по записи. Задает index выбранной записи
        /// </summary>
        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataGrid DG = (DataGrid)sender;

            //Получение имени
            string name = DG.Name;
            //Получение номера записи
            int index = DG.SelectedIndex;

            switch (name)
            {
                case "F_DataGrid_Deallist":
                    if (index == -1)
                    {
                        Title_SelectDealIndex = null;
                    }
                    else
                    {
                        Title_SelectDealIndex = ((DataView)DG.ItemsSource).Table.Rows[index]["Номер дела"].ToString();
                    }
                    break;
                case "F_DataGrid_Deal":
                    //TEXTBLOX.Text = index.ToString();
                    break;
                case "F_DataGrid_Document":
                    //TEXTBLOX.Text =index.ToString();
                    break;
            }
        }
        #endregion

        #region код для списка дел
        /// <summary>
        /// Возвращает или задает index выбранного дела (Индекс = номер дела)
        /// </summary>
        private string Title_SelectDealIndex
        {
            get
            {
                return _selectDealIndex;
            }
            set
            {
                if (value == null)
                {
                    F_GridDealList_TextBlock_TitleSelectDeal.Text = value;
                    F_GridDealList_TitleSelectDeal.Visibility = Visibility.Hidden;
                }
                else
                {
                    F_GridDealList_TextBlock_TitleSelectDeal.Text = value;
                    F_GridDealList_TitleSelectDeal.Visibility = Visibility.Visible;
                }

                _selectDealIndex = value;
            }
        }
        private string _selectDealIndex = null;

        /// <summary>
        /// Задает число найденных дел
        /// </summary>
        private string Title_DealListCount
        {
            set
            {
                if (value == null)
                {
                    F_GridDealList_TextBlock_TitleCountDeal.Text = null;
                }
                else
                {
                    F_GridDealList_TextBlock_TitleCountDeal.Text = "найдено " + value;
                }

            }
        }

        /// <summary>
        /// Поиск записей в таблице Дело
        /// </summary>
        /// <param name="found">значение поиска по номеру дела</param>
        private void FoundDealInList(string found)
        {
            if (found == null)
            {
                Table.Deal.Where = null;
            }
            else
            {
                Table.Deal.Where = $@"Номер_дела Like ""%{found}%""";
            }

            Table.Deal.UpdateTable();
            F_DataGrid_Deallist.ItemsSource = Table.Deal.DVTable;
            Title_DealListCount = Table.Deal.DVTable.Count.ToString();
        }

        /// <summary>
        /// Событие нажатия кнопки сброса списка дел
        /// </summary>
        private void F_GridDealList_ResetDealList(object sender, RoutedEventArgs e)
        {
            FoundDealInList(null);
        }

        /// <summary>
        /// Событие нажатия кнопки для поиска дела
        /// </summary>
        private void F_GridDealList_FoundInDealList(object sender, RoutedEventArgs e)
        {
            FoundDealInList(F_GridDealList_TextBoxFound.Text);
        }

        /// <summary>
        /// Событие нажатия кнопки в поле поиска дела. Отлов кнопки Enter
        /// </summary>
        private void F_GridDealList_TextBoxFoundKeyPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FoundDealInList(F_GridDealList_TextBoxFound.Text);
            }
        }

        /// <summary>
        /// Событие нажатия кнопки удаления записи
        /// </summary>
        private void F_GridDealList_DeleteDeal(object sender, RoutedEventArgs e)
        {
            if (Title_SelectDealIndex == null)
            {
                return;
            }

            try
            {
                Table.Deal.DeleteFrom($@"Номер_дела = ""{Title_SelectDealIndex}""");
            }
            finally
            {
                MessageBox.Show("Запись удалена, обновите таблицу");
            }
        }

        private void F_GridDealList_AddDeal(object sender, RoutedEventArgs e)
        {
            Windows.AddDeal addDeal = new Windows.AddDeal();
            string TimeDeal = null;

            //Получение результата
            if (addDeal.ShowDialog() == true)
            {
                TimeDeal = addDeal.Deal;
            }
            else
            {
                MessageBox.Show("Запись была отменена");
                return;
            }

            //Проверка записи на повтор
            if (UsAc.Execute(@"SELECT * FROM Дело where Дело.Номер_дела = """ + TimeDeal + @"""").Count != 0)
            {
                MessageBox.Show("Дело с таким номером уже существует");
                return;
            }

            //Создание записи
            Table.Deal.InsertInto("Номер_дела", $@"""{TimeDeal}""");

            //Переход к записи



            /*
            UsAc.RequestWithResponse(@"INSERT INTO Дело (Номер_дела) Values (""" + TimeBusiness + @""")");
            DataView timedTab = UsAc.Request($@"SELECT * FROM Дело where Дело.Номер_дела = ""{TimeBusiness}""");

            TableRowsToFieldViewBusiness(timedTab);

            Update($@"SELECT Номер_документа, Название_документа, Число_страниц FROM Документ where Документ.Номер_дела = ""{TimeBusiness}""", ref tab2, ref DaGr2);
            ViewBusinessShow();
            */
        }

        #endregion
    }
}
