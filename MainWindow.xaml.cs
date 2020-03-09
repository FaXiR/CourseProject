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
                case "F_DataGrid_Document":
                    if (index == -1)
                    {
                        Title_SelectDocument = null;
                    }
                    else
                    {
                        Title_SelectDocument = ((DataView)DG.ItemsSource).Table.Rows[index]["Номер"].ToString();
                    }
                    break;
                default:
                    MessageBox.Show("#231644 Невозможно определить принадлежность к таблице");
                    break;
            }
        }

        /// <summary>
        /// Создание чек суммы по всем полям
        /// </summary>
        private string CreateCheckSum(params string[] str)
        {
            string result = null;

            foreach (string s in str)
            {
                result += s.GetHashCode().ToString();
            }

            return result;
        }

        /// <summary>
        /// Создание чек суммы из полей обзора дела
        /// </summary>
        /// <returns></returns>
        private string CreateCheckSumToDeal()
        {
            return CreateCheckSum(F_GridDeal_DateStorage.Text, F_GridDeal_DateOpen.Text, F_GridDeal_DateClose.Text, F_GridDeal_ReasonOpen.Text, F_GridDeal_assure.Text, F_GridDeal_Comment.Text);
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
                Title_SelectDealIndex = null;
            }
        }

        /// <summary>
        /// Событие нажатия кнопки добавления записи
        /// </summary>
        private void F_GridDealList_AddDeal(object sender, RoutedEventArgs e)
        {
            Windows.AddDeal addDeal = new Windows.AddDeal();
            string TimeDeal;

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

            if (TimeDeal == "")
            {
                MessageBox.Show("Нельзя добавить пустую запись");
                return;
            }

            var TimeTable = UsAc.Execute(@"SELECT * FROM Дело where Дело.Номер_дела = """ + TimeDeal + @"""");

            //Проверка записи на повтор
            if (TimeTable.Count == 0)
            {
                //Создание записи
                Table.Deal.InsertInto("Номер_дела", $@"""{TimeDeal}""");

                TimeTable = UsAc.Execute(@"SELECT * FROM Дело where Дело.Номер_дела = """ + TimeDeal + @"""");
            }
            else
            {
                var enter = MessageBox.Show("Запись уже существует, перейти к ней?", "Повторная запись", MessageBoxButton.YesNo, MessageBoxImage.Information);

                if (enter == MessageBoxResult.Yes)
                {
                    //Ничего, т.к. далее переход к записи
                }
                else if (enter == MessageBoxResult.No)
                {
                    return;
                }
            }

            //Переход к записи
            EnterViewDeal(TimeDeal, TimeTable);
        }

        /// <summary>
        /// Событие нажатия кнопки редактирования
        /// </summary>
        private void F_GridDealList_Edit(object sender, RoutedEventArgs e)
        {
            if (Title_SelectDealIndex == null)
            {
                return;
            }

            var TimeTable = UsAc.Execute($@"SELECT * FROM Дело where Дело.Номер_дела = ""{Title_SelectDealIndex}""");

            //Переход к записи
            EnterViewDeal(Title_SelectDealIndex, TimeTable);
        }

        /// <summary>
        /// Передача параметров в обзор дела/Список документов
        /// </summary>
        /// <param name="deal">Номер дела</param>
        /// <param name="table">Таблица из которой берутся параметры. Null если таблица новая</param>
        private void EnterViewDeal(string deal, DataView table)
        {
            Title_SelectDeal = deal;
            FoundDocumentInList(deal, null);

            if (table == null)
            {
                F_GridDeal_DateStorage.Text = null;
                F_GridDeal_DateOpen.Text = null;
                F_GridDeal_DateClose.Text = null;
                F_GridDeal_ReasonOpen.Text = null;
                F_GridDeal_assure.Text = null;
                F_GridDeal_Comment.Text = null;
            }
            else
            {
                F_GridDeal_DateStorage.Text = table.Table.Rows[0]["Дата_введения_на_хранение"].ToString();
                F_GridDeal_DateOpen.Text = table.Table.Rows[0]["Дата_открытия"].ToString();
                F_GridDeal_DateClose.Text = table.Table.Rows[0]["Дата_закрытия"].ToString();
                F_GridDeal_ReasonOpen.Text = table.Table.Rows[0]["Причина_открытия"].ToString();
                F_GridDeal_assure.Text = table.Table.Rows[0]["Заверитель"].ToString();
                F_GridDeal_Comment.Text = table.Table.Rows[0]["Комментарии"].ToString();
            }

            selectDealChecksum = CreateCheckSumToDeal();

            F_GridDealList.Visibility = Visibility.Hidden;
            F_GridDeal.Visibility = Visibility.Visible;
            F_GridDocument.Visibility = Visibility.Hidden;
        }
        #endregion

        #region код для дела/списка документов
        /// <summary>
        /// Возвращает или задает index выбранного дела (Индекс = номер дела)
        /// </summary>
        private string Title_SelectDeal
        {
            get
            {
                return _selectDeal;
            }
            set
            {
                _selectDeal = value;
                F_GridDeal_TextBlock_TitleSelectDeal.Text = value;
            }
        }
        private string _selectDeal = null;

        /// <summary>
        /// Возвращает или задает index выбранного документа (Индекс = номер документа)
        /// </summary>
        private string Title_SelectDocument
        {
            get
            {
                return _selectDocumentIndex;
            }
            set
            {
                if (value == null)
                {
                    F_GridDocumentList_TextBlock_TitleSelectDocument.Text = value;
                    F_GridDocumentList_TitleSelectDeal.Visibility = Visibility.Hidden;
                }
                else
                {
                    F_GridDocumentList_TextBlock_TitleSelectDocument.Text = value;
                    F_GridDocumentList_TitleSelectDeal.Visibility = Visibility.Visible;
                }

                _selectDocumentIndex = value;
            }
        }
        private string _selectDocumentIndex = null;

        /// <summary>
        /// Задает число найденных документов
        /// </summary>
        private string Title_DocumentListCount
        {
            set
            {
                if (value == null)
                {
                    F_GridDeal_TextBlock_TitleCountDocument.Text = null;
                }
                else
                {
                    F_GridDeal_TextBlock_TitleCountDocument.Text = "найдено " + value;
                }
            }
        }

        /// <summary>
        /// Используется для проверки изменений записи, если они не было сохранены
        /// </summary>
        private string selectDealChecksum = null;


        /// <summary>
        /// События нажатия кнопки назад
        /// </summary>
        private void F_GridDeal_Back(object sender, RoutedEventArgs e)
        {
            string CheckSum = CreateCheckSumToDeal();

            if (selectDealChecksum != CheckSum)
            {
                var saved = MessageBox.Show("Сохранить изменения?", "Сохранение", MessageBoxButton.YesNoCancel, MessageBoxImage.Information);

                if (saved == MessageBoxResult.Yes)
                {
                    SaveChangeForDeal();
                }
                else if (saved == MessageBoxResult.No)
                {
                    //Ничего...
                }
                else if (saved == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            F_GridDealList.Visibility = Visibility.Visible;
            F_GridDeal.Visibility = Visibility.Hidden;
            F_GridDocument.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Событие нажатия кнопки сброса списка документов
        /// </summary>
        private void F_GridDocumentList_ResetDealList(object sender, RoutedEventArgs e)
        {
            FoundDocumentInList(Title_SelectDeal, null);
        }

        /// <summary>
        /// Событие нажатия кнопки для поиска документа
        /// </summary>
        private void F_GridDeal_FoundInDealList(object sender, RoutedEventArgs e)
        {
            FoundDocumentInList(Title_SelectDeal, F_GridDeal_TextBoxFound.Text);
        }

        /// <summary>
        /// Событие нажатия кнопки в поле поиска документа. Отлов кнопки Enter
        /// </summary>
        private void F_GridDeal_TextBoxFoundKeyPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FoundDocumentInList(Title_SelectDeal, F_GridDeal_TextBoxFound.Text);
            }
        }

        /// <summary>
        /// Событие нажатия кнопки удаления записи в списке документов
        /// </summary>
        private void F_GridDeal_DeleteDeal(object sender, RoutedEventArgs e)
        {
            if (Title_SelectDocument == null)
            {
                return;
            }

            try
            {
                Table.Document.DeleteFrom($@"Номер_дела = ""{Title_SelectDeal}"" and Номер_документа = {Title_SelectDocument}");
            }
            finally
            {
                MessageBox.Show("Запись удалена, обновите таблицу");
                Title_SelectDocument = null;
            }
        }

        /// <summary>
        /// Событие нажатия кнопки редактирования
        /// </summary>
        private void F_GridDeal_Edit(object sender, RoutedEventArgs e)
        {
            if (Title_SelectDocument == null)
            {
                return;
            }

            return; 
            //TODO: пилить
            var TimeTable = UsAc.Execute($@"SELECT * FROM Дело where Дело.Номер_дела = ""{Title_SelectDealIndex}""");

            //Переход к записи
            EnterViewDeal(Title_SelectDealIndex, TimeTable);
        }

        /// <summary>
        /// Событие нажатия кнопки добавления записи
        /// </summary>
        private void F_GridDeal_AddDeal(object sender, RoutedEventArgs e)
        {
            return;
            //TODO: пилить
            Windows.AddDeal addDeal = new Windows.AddDeal();
            string TimeDeal;

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

            if (TimeDeal == "")
            {
                MessageBox.Show("Нельзя добавить пустую запись");
                return;
            }

            var TimeTable = UsAc.Execute(@"SELECT * FROM Дело where Дело.Номер_дела = """ + TimeDeal + @"""");

            //Проверка записи на повтор
            if (TimeTable.Count == 0)
            {
                //Создание записи
                Table.Deal.InsertInto("Номер_дела", $@"""{TimeDeal}""");

                TimeTable = UsAc.Execute(@"SELECT * FROM Дело where Дело.Номер_дела = """ + TimeDeal + @"""");
            }
            else
            {
                var enter = MessageBox.Show("Запись уже существует, перейти к ней?", "Повторная запись", MessageBoxButton.YesNo, MessageBoxImage.Information);

                if (enter == MessageBoxResult.Yes)
                {
                    //Ничего, т.к. далее переход к записи
                }
                else if (enter == MessageBoxResult.No)
                {
                    return;
                }
            }

            //Переход к записи
            EnterViewDeal(TimeDeal, TimeTable);
        }

        /// <summary>
        /// Метод сохранения изменения для дела
        /// </summary>
        private void SaveChangeForDeal()
        {
            string set = $@"Заверитель = ""{F_GridDeal_assure.Text}"", ";
            set += $@"Причина_открытия = ""{F_GridDeal_ReasonOpen.Text}"", ";

            if (F_GridDeal_DateStorage.Text == "")
            {
                set += $@"Дата_введения_на_хранение = null, ";
            }
            else
            {
                set += $@"Дата_введения_на_хранение = ""{F_GridDeal_DateStorage.Text}"", ";
            }

            if (F_GridDeal_DateOpen.Text == "")
            {
                set += $@"Дата_открытия = null, ";
            }
            else
            {
                set += $@"Дата_открытия = ""{F_GridDeal_DateOpen.Text}"", ";
            }

            if (F_GridDeal_DateClose.Text == "")
            {
                set += $@"Дата_закрытия = null, ";
            }
            else
            {
                set += $@"Дата_закрытия = ""{F_GridDeal_DateClose.Text}"", ";
            }

            set += $@"Комментарии = ""{F_GridDeal_Comment.Text}""";

            Table.Deal.Update(set, $@"Дело.Номер_дела = ""{Title_SelectDeal}""");
        }

        /// <summary>
        /// Поиск записей в таблице Документ
        /// </summary>
        /// <param name="deal">значение поиска по номеру дела</param>
        /// <param name="number">значение поиска по номеру документа</param>
        private void FoundDocumentInList(string deal, string number)
        {
            if (number == null)
            {
                Table.Document.Where = $@"Номер_дела = ""{deal}""";
            }
            else
            {
                if (!int.TryParse(number, out int num))
                {
                    return;
                }

                Table.Document.Where = $@"Номер_дела = ""{deal}"" and Номер_документа = {number}";
            }

            Table.Document.UpdateTable();
            F_DataGrid_Document.ItemsSource = Table.Document.DVTable;
            Title_DocumentListCount = Table.Document.DVTable.Count.ToString();
        }
        #endregion
    }
}
