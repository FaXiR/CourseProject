namespace CourseProject.Modules
{
    /// <summary>
    /// Хранит все таблицы для проекта CourseProject
    /// </summary>
    class Tables
    {
        /// <summary>
        /// Упрощенное взаимодействие с Access
        /// </summary>
        private UsingAccess UsAc;
        /// <summary>
        /// Хранит все таблицы для проекта CourseProject
        /// </summary>
        /// <param name="UsAc">Для обновления содержимого таблиц</param>
        public Tables(UsingAccess UsAc)
        {
            this.UsAc = UsAc;

            Deal = new UsingDataView(UsAc, "Номер_дела AS [Номер дела], Дата_введения_на_хранение AS [Введено на хранение], Причина_открытия AS [Причина открытия], Заверитель", "Дело", null, null);
            Document = new UsingDataView(UsAc, "Номер_документа as [Номер], Название_документа as [Название], Число_страниц as [Число страниц]", "Документ", null, null);
            DocumentContent = new UsingDataView(UsAc, "*", "Содержимое_документа", null, null);
            Users = new UsingDataView(UsAc, "*", "Пользователи", null, null);
        }
        /// <summary>
        /// Дело
        /// </summary>
        public UsingDataView Deal;
        /// <summary>
        /// Документ
        /// </summary>
        public UsingDataView Document;
        /// <summary>
        /// Содержимое документа
        /// </summary>
        public UsingDataView DocumentContent;
        /// <summary>
        /// Пользователи
        /// </summary>
        public UsingDataView Users;
    }
}
