using Lab2.Models;
using Microsoft.IdentityModel.Tokens;

namespace Lab2
{
    internal class Program
    {
        static void Print<T>(string sqlText, IEnumerable<T>? items)
        {
            Console.WriteLine(sqlText);
            Console.WriteLine("Записи: ");
            if(!items.IsNullOrEmpty())
            {
                foreach (var item in items)
                {
                    Console.WriteLine(item!.ToString());
                }
            }
            else
            {
                Console.WriteLine("Пусто");
            }
            Console.WriteLine();
            Console.WriteLine("Нажмите любую клавишу");
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
                             where w.WorkExperience < 10
                             select new
                             {
                                 Имя_Работника = e.Name,
                                 Должность_Работника = e.Role,
                                 Опыт_работы = w.WorkExperience
                             };

            comment = "5. Результат выполнения запроса на выборку данных из двух таблиц, связанных между собой отношением 'один-ко-многим' и отфильтрованным по некоторому условию";
            Print(comment, queryLINQ5.ToList());
        }

        static void Insert(CinemaContext db)
        {
            Genre genre = new Genre
            {
                Name = "New genre 1",
                Description = "New description"
            };
            db.Genres.Add(genre);
            db.SaveChanges();
            string comment = "Выберка жанров после вставки нового жанра";
            var queryLINQ1 = from g in db.Genres
                             where g.Name == "New genre 1"
                             select new
                             {
                                 Нзавание_Жанра = g.Name,
                                 Описание_Жанра = g.Description
                             };
            Print(comment, queryLINQ1.ToList());

            Movie movie = new Movie
            {
                Title = "New movie 1",
                Duration = new TimeOnly(2,13,15),
                ProductionCompany = "New production company",
                Country = "New country",
                AgeRestriction = 18,
                Description = "New description",
                GenreId = genre.GenreId,
            };
            db.Movies.Add(movie);
            db.SaveChanges();
            comment = "Выберка фильмов после вставки нового фильма";
            var queryLINQ2 = from m in db.Movies
                             where m.Title == "New movie 1"
                             select new
                             {
                                 Название_Фильма = m.Title,
                                 Продолжительность_Фильма = m.Description,
                                 Компания_производитель = m.ProductionCompany,
                                 Страна = m.Country,
                                 Возрастное_ограничение = m.AgeRestriction,
                                 Описание_Фильма = m.Description,
                                 Нзавание_Жанра = m.Genre.Name,
                             };
            Print(comment, queryLINQ2.ToList());
        }

        static void Delete(CinemaContext db)
        {
            string genreName = "New genre 1";
            var genre = db.Genres.Where(g => g.Name == genreName);

            if (genre != null)
            {
                db.Genres.RemoveRange(genre);
                db.SaveChanges();
            }
            string comment = "Выберка жанров после удаления жанра";
            var queryLINQ1 = from g in db.Genres
                             where g.Name == "New genre 1"
                             select new
                             {
                                 Нзавание_Жанра = g.Name,
                                 Описание_Жанра = g.Description
                             };
            Print(comment, queryLINQ1.ToList());

            string movieTitle = "New movie 1";
            var movies = db.Movies.Where(m => m.Title == movieTitle);

            if(movies != null)
            {
                db.Movies.RemoveRange(movies);
                db.SaveChanges();
            }
            comment = "Выберка фильмов после удаления фильмов";
            var queryLINQ2 = from m in db.Movies
                             where m.Title == "New movie 1"
                             select new
                             {
                                 Название_Фильма = m.Title,
                                 Продолжительность_Фильма = m.Description,
                                 Компания_производитель = m.ProductionCompany,
                                 Страна = m.Country,
                                 Возрастное_ограничение = m.AgeRestriction,
                                 Описание_Фильма = m.Description,
                                 Нзавание_Жанра = m.Genre.Name,
                             };
            Print(comment, queryLINQ2.ToList());
        }

        static void Update(CinemaContext db)
        {
            int workExp = 8;
            var workLogs = db.WorkLogs.Where(w => w.WorkExperience == workExp);
            if(!workLogs.IsNullOrEmpty())
            {
                foreach( var w in workLogs)
                {
                    w.WorkHours = 13;
                }
                db.SaveChanges();
            }

            string comment = "Выберка после обновления";
            var queryLINQ1 = from w in db.WorkLogs
                             where w.WorkExperience == workExp
                             select new
                             {
                                 Имя_Работника = w.Employee.Name,
                                 Опыт_работы = w.WorkExperience,
                                 Количество_часов = w.WorkHours
                             };
            Print(comment, queryLINQ1.ToList());
        }

        static void Main(string[] args)
        {
            using(var db = new CinemaContext())
            {
                Console.WriteLine("Будет выполнена выборка данных (нажмите любую клавишу) ============");
                Console.ReadKey();
                Select(db);

                Console.WriteLine("Будет выполнена вставка данных (нажмите любую клавишу) ============");
                Console.ReadKey();
                Insert(db);

                Console.WriteLine("Будет выполнено удаление данных (нажмите любую клавишу) ============");
                Console.ReadKey();
                Delete(db);

                Console.WriteLine("Будет выполнено обновление данных (нажмите любую клавишу) ============");
                Console.ReadKey();
                Update(db);
            }
        }
    }
}
