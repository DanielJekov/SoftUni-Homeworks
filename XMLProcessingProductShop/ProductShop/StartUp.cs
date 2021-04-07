using ProductShop.Data;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System;
using System.Collections.Generic;
using ProductShop.Dtos.Export;
using System.Xml.Linq;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //var usersXml = File.ReadAllText(@"../../../Datasets\users.xml");
            //Console.WriteLine(ImportUsers(context, usersXml));

            //var productsXml = File.ReadAllText(@"../../../Datasets\products.xml");
            //Console.WriteLine(ImportProducts(context, productsXml));

            //var categoryXml = File.ReadAllText(@"../../../Datasets\categories.xml");
            //Console.WriteLine(ImportCategories(context, categoryXml));

            //var categoryProductsXml = File.ReadAllText(@"../../../Datasets\categories-products.xml");
            //Console.WriteLine(ImportCategoryProducts(context, categoryProductsXml));

            var result = XDocument.Parse(GetUsersWithProducts(context));
            Console.WriteLine(result);
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            const string root = "Users";
            var xmlDeserializer = new XmlSerializer(typeof(UserDto[]), new XmlRootAttribute(root));
            var textReader = new StringReader(inputXml);

            var usersDtos = xmlDeserializer
                .Deserialize(textReader) as UserDto[];

            var users = usersDtos
                .Select(x => new User
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age
                })
                .ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            const string root = "Products";
            var xmlDeserialize = new XmlSerializer(typeof(ProductDto[]), new XmlRootAttribute(root));
            var textReader = new StringReader(inputXml);

            var productsDtos = xmlDeserialize.Deserialize(textReader) as ProductDto[];

            var products = productsDtos
                .Select(x => new Product
                {
                    Name = x.Name,
                    Price = x.Price,
                    SellerId = x.SellerId,
                    BuyerId = x.BuyerId,
                })
                .ToList();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            const string root = "Categories";
            var xmlDeserializer = new XmlSerializer(typeof(CategoryDto[]), new XmlRootAttribute(root));
            var textReader = new StringReader(inputXml);

            var categoriesDtos = xmlDeserializer.Deserialize(textReader) as CategoryDto[];

            var categories = categoriesDtos
                .Select(x => new Category
                {
                    Name = x.Name
                })
                .Where(x => x.Name != null)
                .ToList();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}"; ;
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            const string root = "CategoryProducts";
            var xmlDeserializer = new XmlSerializer(typeof(CategoryProductDto[]), new XmlRootAttribute(root));
            var textReader = new StringReader(inputXml);

            var categoryProductsDtos = xmlDeserializer.Deserialize(textReader) as CategoryProductDto[];

            var validCategories = context.Categories
                .Select(x => new
                {
                    Id = x.Id
                })
                .ToList();

            var validProducts = context.Products
                .Select(x => new
                {
                    Id = x.Id
                })
                .OrderBy(x => x.Id)
                .ToList();
            ;
            List<CategoryProduct> categoryProducts = new List<CategoryProduct>();
            foreach (var item in categoryProductsDtos)
            {
                if (validCategories.Any(x => x.Id == item.CategoryId)
                    && validProducts.Any(p => p.Id == item.ProductId))
                {
                    categoryProducts.Add(new CategoryProduct
                    {
                        ProductId = item.ProductId,
                        CategoryId = item.CategoryId,
                    });
                }
            }

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();
            return $"Successfully imported {categoryProducts.Count}"; ;
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            const string root = "Products";

            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .Select(x => new ProductExportDto
                {
                    Name = x.Name,
                    Price = x.Price,
                    BuyerNames = x.Buyer.FirstName + " " + x.Buyer.LastName
                })
                .OrderBy(x => x.Price)
                .Take(10)
                .ToList()
                .ToArray();

            var result = XmlSerializer(products, root);

            return result;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            const string root = "Users";

            var soldProducts = context.Users
                .Where(x => x.ProductsSold.Any())
                .Select(x => new SoldProductExportDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold.Select(p => new NestedProductsSold
                    {
                        Name = p.Name,
                        Price = p.Price
                    })
                    .ToList()
                })
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Take(5)
                .ToList()
                .ToArray();

            var result = XmlSerializer<SoldProductExportDto>(soldProducts, root);

            return result;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            const string root = "Categories";

            var categories = context.Categories
                .Select(x => new CategoryByProductExportDto
                {
                    Name = x.Name,
                    Count = x.CategoryProducts.Count,
                    AveragePrice = x.CategoryProducts.Average(s => s.Product.Price),
                    TotalRevenue = x.CategoryProducts.Sum(s => s.Product.Price)
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.TotalRevenue)
                .ToList()
                .ToArray();

            var result = XmlSerializer<CategoryByProductExportDto>(categories, root);

            return result;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            const string root = "Users";

            var serializer = new UserExportDto()
            {
                CountOfUsers = context.Users.Count(x => x.FirstName != null),
                Users = context.Users
                .Where(x => x.ProductsSold.Count > 0)
                .OrderByDescending(x => x.ProductsSold.Count)
                .Select(x => new NestedUsersExportDto()
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age,
                    SoldProducts = new ProductSoldDto()
                    {
                        CountOfProductsSoldByUser = x.ProductsSold.Count(p => p.Buyer != null),
                        ProductsSoldByUser = x.ProductsSold
                        .Where(p => p.Buyer != null)
                        .Select(p => new NestedProductsSold()
                        {
                            Name = p.Name,
                            Price = p.Price,
                        })
                        .OrderByDescending(p => p.Name)
                        .ThenByDescending(p => p.Price)
                        //.Take(10)
                        .ToList()
                        .ToArray()
                    }
                })
                //.Take(10) 
                .ToList()
                .ToArray()

            };

            var result = XmlSerializer1<UserExportDto>(serializer, root);

            return result;
        }

        private static string XmlSerializer<T>(T[] collection, string root)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var xmlSerializer = new XmlSerializer(typeof(T), new XmlRootAttribute(root));
            var textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, collection, ns);

            return textWriter.ToString();
        }
        private static string XmlSerializer1<T>(T singleItem, string root)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var xmlSerializer = new XmlSerializer(typeof(T), new XmlRootAttribute(root));
            var textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, singleItem, ns);

            return textWriter.ToString();
        }
    }
}