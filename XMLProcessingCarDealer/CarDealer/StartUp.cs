using System;
using CarDealer.Data;
using CarDealer.DataTransferObjects.Input;
using CarDealer.Models;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using CarDealer.DataTransferObjects.Output;
using Microsoft.EntityFrameworkCore;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();
            //
            //var supplierXml = File.ReadAllText(@"../../../Datasets\suppliers.xml");
            //Console.WriteLine(ImportSuppliers(context, supplierXml));
            //
            //var partXml = File.ReadAllText(@"../../../Datasets\parts.xml");
            //Console.WriteLine(ImportParts(context, partXml));
            //
            //var carXml = File.ReadAllText(@"../../../Datasets\cars.xml");
            //Console.WriteLine(ImportCars(context, carXml));
            //
            //var customersXml = File.ReadAllText(@"../../../Datasets\customers.xml");
            //Console.WriteLine(ImportCustomers(context, customersXml));
            //
            //var salesXml = File.ReadAllText(@"../../../Datasets\sales.xml");
            //Console.WriteLine(ImportSales(context, salesXml));

            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(SupplierInputModel[]), new XmlRootAttribute("Suppliers"));

            var textRead = new StringReader(inputXml);

            var suppliersDto = xmlSerializer
                .Deserialize(textRead) as SupplierInputModel[];

            var suppliers = suppliersDto
                .Select(x => new Supplier
                {
                    Name = x.Name,
                    IsImporter = x.IsImporter,
                })
                .ToList();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(PartInputModel[]), new XmlRootAttribute("Parts"));
            var textRead = new StringReader(inputXml);

            var partsDto = xmlSerializer.Deserialize(textRead) as PartInputModel[];

            var parts = partsDto
                .Select(x => new Part
                {
                    Name = x.Name,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    SupplierId = x.SupplierId
                })
                .ToList();

            int counter = 0;
            foreach (var part in parts)
            {
                if (context.Suppliers.Any(x => x.Id == part.SupplierId))
                {
                    context.Parts.Add(part);
                    counter++;
                }
            }
            context.SaveChanges();

            return $"Successfully imported {counter}"; ;
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            const string root = "Cars";
            var xmlSerializer = new XmlSerializer(typeof(CarInputModel[]), new XmlRootAttribute(root));
            var textRead = new StringReader(inputXml);

            var carsDto = xmlSerializer.Deserialize(textRead) as CarInputModel[];

            var cars = new List<Car>();
            var allParts = context.Parts.Select(x => x.Id).ToList();

            foreach (var currentCar in carsDto)
            {
                var distinctedParts = currentCar.CarPartsInputModel.Select(x => x.Id).Distinct();
                var parts = distinctedParts.Intersect(allParts);

                var car = new Car
                {
                    Make = currentCar.Make,
                    Model = currentCar.Model,
                    TravelledDistance = currentCar.TraveledDistance,
                };

                foreach (var part in parts)
                {
                    var partCar = new PartCar
                    {
                        PartId = part
                    };

                    car.PartCars.Add(partCar);
                }
                cars.Add(car);
            }
            ;
            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            const string root = "Customers";
            var xmlSerializer = new XmlSerializer(typeof(CustomersInputModel[]), new XmlRootAttribute(root));
            var textRead = new StringReader(inputXml);

            var customersDtos = xmlSerializer.Deserialize(textRead) as CustomersInputModel[];

            var customers = customersDtos
                .Select(x => new Customer
                {
                    Name = x.Name,
                    BirthDate = x.BirthDate,
                    IsYoungDriver = x.IsYoungDriver,
                })
                .ToList();

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}"; ;
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            const string root = "Sales";
            var xmlSerializer = new XmlSerializer(typeof(SalesInputModel[]), new XmlRootAttribute(root));
            var textRead = new StringReader(inputXml);

            var salesDtos = xmlSerializer.Deserialize(textRead) as SalesInputModel[];

            var carsAvaivable = context.Cars.Select(x => x.Id).ToList();
            var sales = new List<Sale>();

            foreach (var currentSale in salesDtos)
            {
                if (carsAvaivable.Any(x => x == currentSale.CarId))
                {
                    var sale = new Sale()
                    {
                        CarId = currentSale.CarId,
                        CustomerId = currentSale.CustomerId,
                        Discount = currentSale.Discount
                    };

                    sales.Add(sale);
                }
            }

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}"; ;
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            const string root = "cars";

            var cars = context.Cars
                .Where(x => x.TravelledDistance > 2_000_000)
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .Select(x => new CarOutputModel
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .ToList()
                .Take(10)
                .ToArray();

            var result = XmlSerialization<CarOutputModel>(cars, root);

            return result;
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            const string root = "cars";
            const string carMake = "BMW";

            var cars = context.Cars
                .Where(x => x.Make == carMake)
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .Select(x => new BmwOutputModel
                {
                    Id = x.Id,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .ToList()
                .ToArray();

            var result = XmlSerialization<BmwOutputModel>(cars, root);

            return result;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            const bool notImporting = false;
            const string root = "suppliers";

            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == notImporting)
                .Select(x => new LocalSuppliersOutputModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count(),
                })
                .ToList()
                .ToArray();

            var result = XmlSerialization<LocalSuppliersOutputModel>(suppliers, root);

            return result;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            const string root = "cars";
            var cars = context.Cars
                .Select(x => new CarsWithPartsOutputModel
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance,
                    CarParts = x.PartCars.Select(p => new PartsFromCarOutputModel
                    {
                        Name = p.Part.Name,
                        Price = p.Part.Price,
                    })
                    .OrderByDescending(p => p.Price)
                    .ToList()
                    .ToArray()
                })
                .OrderByDescending(x => x.TravelledDistance)
                .ThenBy(x => x.Model)
                .Take(5)
                .ToList()
                .ToArray();

            var result = XmlSerialization<CarsWithPartsOutputModel>(cars, root);

            return result;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            const string root = "customers";

            var customers = context.Sales
                .Where(x => x.Customer != null)
                .Select(x => new CustomerWithTotalBoughts
                {
                    FullName = x.Customer.Name,
                    BoughtCars = x.Customer.Sales.Count,
                    SpentMoney = x.Car.PartCars.Sum(p => p.Part.Price)
                })
                .OrderByDescending(x => x.SpentMoney)
                .ToList()
                .ToArray();

            var result = XmlSerialization<CustomerWithTotalBoughts>(customers, root);
            return result;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            const string root = "sales";

            var sales = context.Sales
                .Select(x => new SaleWithAplliedDiscoutOutputModel
                {
                    Car = new CarOutputModel
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance
                    },
                    Discount = x.Discount,
                    CustomerName = x.Customer.Name,
                    Price = x.Car.PartCars.Sum(p => p.Part.Price),
                    PriceWithDiscount = x.Car.PartCars.Sum(p => p.Part.Price) - x.Car.PartCars.Sum(p => p.Part.Price) * x.Discount /100,
                })
                .ToList()
                .ToArray();

            var result = XmlSerialization<SaleWithAplliedDiscoutOutputModel>(sales, root);
            return result;
        }

        private static string XmlSerialization<T>(T[] collectionToSerialize, string root)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T[]), new XmlRootAttribute(root));
            var textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, collectionToSerialize, ns);

            return textWriter.ToString();
        }
    }
}