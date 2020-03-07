using System.Windows;
using System.Windows.Input;
using CourseProject.Modules;

namespace CourseProject.Windows
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        #region переменные
        public string FIO = null;
        public string Login
        {
            get { return F_Login.Text.ToString(); }
            set { F_Login.Text = value; }
        }
        public string Password
        {
            get { return F_Password.Password.ToString(); }
            set { F_Password.Password = value; }
        }
        private UsingAccess UsAc;
        #endregion

        /// <summary>
        /// Окно авторизации
        /// </summary>
        /// <param name="usingAccess">Соеденение с Access для проверки Логина/Пароля</param>
        public AuthorizationWindow(UsingAccess usingAccess)
        {
            InitializeComponent();

            UsAc = usingAccess;
            F_Login.Focus();
        }

        /// <summary>
        /// Событие нажатия кнопки войти
        /// </summary>
        private void F_PressEnter(object sender, RoutedEventArgs e)
        {
            AttemptEnter();
        }

        /// <summary>
        /// Проверка полей на пустоту и попытка авторизации
        /// </summary>
        private void AttemptEnter()
        {
            if (F_Login.Text == "")
            {
                F_Login.Focus();
                F_Login.SelectAll();
            }
            else if (F_Password.Password == "")
            {
                F_Password.Focus();
                F_Password.SelectAll();
            }

            if (CheckLogPas())
            {
                this.DialogResult = true;
            }
            else
            {
                F_Password.Clear();
                F_Login.Focus();
                F_Login.SelectAll();
            }
        }

        /// <summary>
        /// Поиск логина/пароля в БД
        /// </summary>
        /// <returns>Наличие записи</returns>
        private bool CheckLogPas()
        {
            //Поиск записи в БД
            var FoundRole = UsAc.Execute($@"Select ФИО From Пользователи where Логин = ""{F_Login.Text}"" and Пароль = ""{F_Password.Password}""");
            if (FoundRole.Count == 0)
            {
                return false;
            }
            else
            {
                FIO = FoundRole.Table.Rows[0]["ФИО"].ToString();
                return true;                
            }
        }

        /// <summary>
        /// Событие нажатии кнопки в TextBox Логина
        /// </summary>
        private void F_KeyUp_Login(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                F_Password.Focus();
                F_Password.SelectAll();
            }
        }

        /// <summary>
        /// Событие нажатии кнопки в PasswordBox пароля
        /// </summary>
        private void F_KeyUp_Password(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AttemptEnter();
            }
        }
    }
}
