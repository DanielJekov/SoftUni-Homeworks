using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var dbContext = new CarDealerContext();

            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();

            //var jsonSuppliers =
            //    File.ReadAllText(@"../../../Datasets\suppliers.json");
            //var jsonSales =
            //    File.ReadAllText(@"../../../Datasets\sales.json");
            //var jsonParts =     
            //    File.ReadAllText(@"../../../Datasets\parts.json");
            //var jsonCustomers = 
            //    File.ReadAllText(@"../../../Datasets\customers.json");
            //var jsonCars =      
            //    File.ReadAllText(@"../../../Datasets\cars.json");

            //Console.WriteLine(ImportSuppliers(dbContext, jsonSuppliers));
            //Console.WriteLine(ImportParts(dbContext, jsonParts));
            //Console.WriteLine(ImportCars(dbContext, jsonCars));
            //Console.WriteLine(ImportCustomers(dbContext, jsonCustomers)); 
            //Console.WriteLine(ImportSales(dbContext, jsonSales));

            Console.WriteLine(GetSalesWithAppliedDiscount(dbContext));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var parts = JsonConvert.DeserializeObject<List<Part>>(inputJson);

            var suppliersIds = context.Suppliers
                .Select(x => x.Id)
                .ToList();

            int counter = 0;
            foreach (var part in parts)
            {
                if (suppliersIds.Any(x => x == part.SupplierId))
                {
                    context.Parts.Add(part);
                    counter++;
                }
            }

            context.SaveChanges();

            return $"Successfully imported {counter}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var cars = JsonConvert.DeserializeObject<List<Car>>(inputJson);
            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var custromers = context.Customers
                .OrderBy(x => x.BirthDate)
                .ThenBy(x => x.IsYoungDriver)
                .Select(x => new
                {
                    Name = x.Name,
                    BirthDate = x.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    IsYoungDriver = x.IsYoungDriver
                })
                .ToList();

            var jsonResult = JsonConvert.SerializeObject(custromers, Formatting.Indented);

            return jsonResult;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.Make == "Toyota")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .Select(x => new
                {
                    Id = x.Id,
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance,
                })
                .ToList();

            var jsonResult = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return jsonResult;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count(),
                })
                .ToList();

            var jsonResult = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

            return jsonResult;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(x => new
                {
                    car = new
                    {
                        Make = x.Make,
                        Model = x.Model,
                        TravelledDistance = x.TravelledDistance
                    },


                    parts = x.PartCars.Select(y => new
                    {
                        Name = y.Part.Name,
                        Price = y.Part.Price.ToString("F2")
                    })
                    .ToList()
                })
                .ToList();



            var jsonResult = JsonConvert.SerializeObject(cars, Formatting.Indented);
            return jsonResult;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers.Where(x => x.Sales != null)
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Select(y => y.Car).Count(),
                    spentMoney = c.Sales.Sum(y => y.Car.PartCars.Sum(p => p.Part.Price))

                })
                .OrderByDescending(x => x.spentMoney)
                .ThenByDescending(x => x.boughtCars)
                .ToList();

            var jsonResult = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return jsonResult;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(x => new
                {
                    car = new
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance,
                    },

                    customerName = x.Customer.Name,
                    Discount = x.Discount.ToString("F2"),
                    price = x.Car.PartCars.Sum(p => p.Part.Price).ToString("F2"),
                    //PriceWithDiscount = (x.Car.PartCars.Sum(pc => pc.Part.Price) -
                                           // x.Car.PartCars.Sum(pc => pc.Part.Price) * x.Discount / 100).ToString("f2")
                })
                .Take(10)
                .ToList();

            var jsonResult = JsonConvert.SerializeObject(sales, Formatting.Indented);
            return jsonResult;
        }
    }
}