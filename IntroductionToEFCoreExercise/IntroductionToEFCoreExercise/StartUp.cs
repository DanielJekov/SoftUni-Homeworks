using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var context = new SoftUniContext();
            Console.WriteLine(GetAddressesByTown(context));
        }
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees.OrderBy(x => x.EmployeeId).ToList();
            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:F2}");

            }
            return sb.ToString();
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees.Where(x => x.Salary > 50000).OrderBy(x => x.FirstName).ToList();
            var sb = new StringBuilder();
            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:f2}");
            }
            return sb.ToString();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees.Select(x => new
            {
                x.FirstName,
                x.LastName,
                x.Salary,
                x.Department.Name,
            })
                                             .Where(x => x.Name.Contains("Research and Development"))
                                             .OrderBy(x => x.Salary)
                                             .ThenByDescending(x => x.FirstName).ToList();

            var sb = new StringBuilder();
            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.Name} - ${employee.Salary:f2}");
            }

            return sb.ToString();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var address = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.Addresses.Add(address);
            context.SaveChanges();

            var takenByLastName = context.Employees
                                         .FirstOrDefault(x => x.LastName == "Nakov");

            takenByLastName.AddressId = address.AddressId;
            context.SaveChanges();

            var addressesForResult = context.Employees.Select(x => new
            {
                x.Address.AddressText,
                x.Address.AddressId,
            })
                                                       .OrderByDescending(x => x.AddressId)
                                                       .Take(10)
                                                       .ToList();
            var sb = new StringBuilder();

            foreach (var currAddress in addressesForResult)
            {
                sb.AppendLine(currAddress.AddressText);
            }
            return sb.ToString();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees.Include(x => x.EmployeesProjects)
                                             .ThenInclude(x => x.Project)
                                             .Where(x => x.EmployeesProjects.Any(p => p.Project.StartDate.Year >= 2001 && p.Project.StartDate.Year <= 2003))
                                             .Select(x => new
                                             {
                                                 employeeFirstName = x.FirstName,
                                                 employeeLastName = x.LastName,
                                                 managerFirstName = x.Manager.FirstName,
                                                 managerLastName = x.Manager.LastName,
                                                 Projects = x.EmployeesProjects.Select(p => new
                                                 {
                                                     ProjectName = p.Project.Name,
                                                     StartDate = p.Project.StartDate,
                                                     EndDate = p.Project.EndDate
                                                 })
                                             })
                                             .Take(10)
                                             .ToList();

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.employeeFirstName} {employee.employeeLastName} - Manager: {employee.managerFirstName} {employee.managerLastName}");
                foreach (var item in employee.Projects)
                {
                    string endDate = "not finished";
                    if (item.EndDate != null)
                    {
                        DateTime endDate2 = (DateTime)item.EndDate;
                        endDate = endDate2.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    }

                    sb.AppendLine($"--{item.ProjectName} - {item.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)} - {endDate}");
                }
            }
            return sb.ToString();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses.Select(x => new
                                                            {
                                                                AddressText = x.AddressText,
                                                                TownName = x.Town.Name,
                                                                CountOfEmployeesLivingOnAddress = x.Employees.Count()
                                                            })
                                              .OrderByDescending(x => x.CountOfEmployeesLivingOnAddress)
                                              .ThenBy(x => x.TownName)
                                              .Take(10)
                                              .ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var currAddress in addresses)
            {
                sb.AppendLine($"{currAddress.AddressText}, {currAddress.TownName} - {currAddress.CountOfEmployeesLivingOnAddress} employees");
            }

            return sb.ToString();
        }
    }
}
