namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Z.EntityFramework.Plus;
    using BookShop.Models;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
           // DbInitializer.ResetDatabase(db);

           Console.WriteLine(IncreasePricesById(db));
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var ageRestriction = Enum.Parse<AgeRestriction>(command, true);

            var books = context.Books
                .Where(x => x.AgeRestriction == ageRestriction)
                .Select(x => x.Title)
                .OrderBy(title => title)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book}");
            }

            return sb.ToString().Trim();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var EditionType = Enum.Parse<EditionType>("Gold", false);
            var GoldenBooks = context.Books
                .Where(x => x.Copies < 5000 && x.EditionType == EditionType)
                .Select(x => new
                {
                    x.BookId,
                    x.Title
                })
                .OrderBy(x => x.BookId)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in GoldenBooks)
            {
                sb.AppendLine($"{book.Title}");
            }

            return sb.ToString().Trim();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.Price > 40)
                .Select(x => new { x.Title, x.Price })
                .OrderByDescending(x => x.Price)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            }
            return sb.ToString().Trim();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(x => x.ReleaseDate.Value.Year != year && x.ReleaseDate != null)
                .Select(x => new { x.BookId, x.Title })
                .OrderBy(x => x.BookId)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title}");
            }
            return sb.ToString().Trim();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            //string[] inputSplitted = input
            //    .Split(" ", StringSplitOptions.RemoveEmptyEntries)
            //    .Select(x => x.ToLower())
            //    .ToArray();

            //string inputSplitted = input.ToLower();



            //var sb = new StringBuilder();

            //foreach (var book in books)
            //{
            //    sb.AppendLine($"{book}");
            //}
            return null;
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {

            var targetDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(x => x.ReleaseDate.Value < targetDate)

                .Select(x => new
                {
                    x.Title,
                    x.EditionType,
                    x.Price,
                    x.ReleaseDate.Value
                })
                .OrderByDescending(x => x.Value)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:F2}");
            }
            return sb.ToString().Trim();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(x => x.FirstName.EndsWith(input))
                .Select(x => new { FullName = x.FirstName + " " + x.LastName })
                .OrderBy(x => x.FullName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var author in authors)
            {
                sb.AppendLine($"{author.FullName}");
            }
            return sb.ToString().Trim();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(x => x.Title.ToLower().Contains(input.ToLower()))
                .Select(x => new { x.Title })
                .OrderBy(x => x.Title)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title}");
            }

            return sb.ToString().Trim();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .Include(x => x.Author)
                .Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(x => new
                {
                    x.BookId,
                    x.Title,
                    FullName = x.Author.FirstName + " " + x.Author.LastName
                })
                .OrderBy(x => x.BookId)
                .ToList();


            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} ({book.FullName})");
            }

            return sb.ToString().Trim();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context.Books.Where(x => x.Title.Length > lengthCheck).Count();
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var booksCount = context.Authors
                .Include(x => x.Books)
                .Select(x => new
                {
                    FullName = x.FirstName + " " + x.LastName,
                    BooksCount = x.Books.Sum(x => x.Copies)
                })
                .OrderByDescending(x => x.BooksCount)
                .ToList();

            var sb = new StringBuilder();

            foreach (var item in booksCount)
            {
                sb.AppendLine($"{item.FullName} - {item.BooksCount}");
            }

            return sb.ToString().Trim();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var totalProfit = context.Categories
                .Select(x => new
                {
                    CategoryName = x.Name,
                    SumOfTotalCopies = x.CategoryBooks
                    .Select(x => x.Book.Copies * x.Book.Price)
                    .Sum()
                })
                .OrderByDescending(x => x.SumOfTotalCopies)
                .ThenBy(x => x.CategoryName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var item in totalProfit)
            {
                sb.AppendLine($"{item.CategoryName} ${item.SumOfTotalCopies:F2}");
            }

            return sb.ToString().Trim();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var mostRecentBooks = context.Categories
                .Select(x => new
                {
                    CategoryName = x.Name,
                    Top3Realised = x.CategoryBooks
                    .Select(x => new
                    {
                        BookName = x.Book.Title,
                        RealiseDate = x.Book.ReleaseDate.Value
                    })
                    .OrderByDescending(x => x.RealiseDate)
                    .Take(3)
                    .ToList()
                })
                .OrderBy(x => x.CategoryName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var currCategory in mostRecentBooks)
            {
                sb.AppendLine($"--{currCategory.CategoryName}");
                foreach (var books in currCategory.Top3Realised)
                {
                    sb.AppendLine($"{books.BookName} ({books.RealiseDate.Year})");
                }
            }

            return sb.ToString().Trim();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var Inreaces = context.Books
                .Where(x => x.ReleaseDate.Value.Year < 2010)
                .ToList();

            foreach (var item in Inreaces)
            {
                item.Price += 5;
            }

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            return context.Books
                .Where(x => x.Copies < 4200)
                .Delete();
        }

        public static int IncreasePricesById(BookShopContext context)
        {
            return context.Books
                .Include(x => x.BookCategories)
                .ThenInclude(x => x.Category)
               .Where(x => x.Author.LastName == "Powell")
                .Update(x => new Book { Price = x.Price - 3000 });

        }
    }
}
