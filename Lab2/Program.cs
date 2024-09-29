namespace Lab2
{
    internal class Program
    {
        static void Print<T>(string sqlText, IEnumerable<T> items)
        {
            Console.WriteLine(sqlText);
            Console.WriteLine("Записи: ");
            foreach (var item in items)
            {
                Console.WriteLine(item!.ToString());
            }
            Console.WriteLine();
            Console.ReadKey();
        }

        static void Select(CinemaContext db)
        {
            var queryLINQ1 = from g in db.Genres
                             select new
                             {
                                 Название_Жанра = g.Name,
                                 Описание_Жанра = g.Description,
                             };
            string comment = "1. Результат выполнения запроса на выборку всех данных из таблицы, стоящей в схеме базы данных на стороне отношения 'Один'";
            Print(comment, queryLINQ1.Take(10).ToList());

            var queryLINQ2 = from emp in db.Employees
                             where emp.Name == "Employee 81"
                             select new
                             {
                                 Имя_Работника = emp.Name,
                                 Должность_Работника = emp.Role
                             };
            comment = "2. Результат выполнения запроса на выборку данных из таблицы, стоящей в схеме базы данных на стороне отношения 'Один', отфильтрованные по определенному условию";
            Print(comment, queryLINQ2.ToList());

            var queryLINQ3 = db.Movies.
                             Join(db.Genres, m => m.GenreId, g => g.GenreId, (m,g) => new { m.GenreId, m.AgeRestriction, g.Name }).
                             GroupBy(m => new { m.GenreId, m.Name}).
                             Select(gr => new
                             {
                                 Название_Жанра = gr.Key.Name,
                                 Средний_возраст = gr.Average(m => m.AgeRestriction),
                             });
            comment = "3. Результат выполнения запроса на выборку данных из таблицы, стоящей в схеме базы данных на стороне отношения 'Многие', сгрупированных по любому из полей данных с выводом итогового результата";
            Print(comment, queryLINQ3.Take(10).ToList());

            var queryLINQ4 = db.Employees.
                             Join(db.WorkLogs, e => e.EmployeeId, w => w.EmployeeId, (e, w) => new { e.Name, w.WorkExperience }).
                             Select(gr => new
                             {
                                 Имя_Работника = gr.Name,
                                 Опыт_Работы = gr.WorkExperience,
                             });
            comment = "4. Результат выполнения запроса на выборку данных двух полей из двух таблиц, связанных между собой отношением 'один-ко-многим'";
            Print(comment, queryLINQ4.Take(10).ToList());

            var queryLINQ5 = from e in db.Employees
                             join w in db.WorkLogs
                             on e.EmployeeId equals w.EmployeeId
                             where w.WorkExperience <= 15
                             select new
                             {
                                 Имя_Работника = e.Name,
                                 Должность_Работника = e.Role,
                                 Опыт_работы = w.WorkExperience
                             };

            comment = "5. Результат выполнения запроса на выборку данных из двух таблиц, связанных между собой отношением 'один-ко-многим' и отфильтрованным по некоторому условию";
            Print(comment, queryLINQ5.ToList());
        }

        static void Main(string[] args)
        {
            using(var db = new CinemaContext())
            {
                Console.WriteLine("Будет выполнена выборка данных (нажмите любую клавишу) ============");
                Console.ReadKey();
                Select(db);
            }
        }
    }
}
