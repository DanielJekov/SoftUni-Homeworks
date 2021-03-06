﻿using Microsoft.EntityFrameworkCore;
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
            Console.WriteLine(DeleteProjectById(context));
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

        public static string GetEmployee147(SoftUniContext context)
        {
            int currIdToTakeEmployee = 147;

            var takenByIDEmployee = context.Employees
                .Select(x => new Employee
                {
                    EmployeeId = x.EmployeeId,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    JobTitle = x.JobTitle,
                    EmployeesProjects = x.EmployeesProjects.Select(p => new EmployeeProject
                    {
                        Project = p.Project
                    })
                    .OrderBy(x => x.Project.Name)
                    .ToList()
                })
                .FirstOrDefault(x => x.EmployeeId == currIdToTakeEmployee);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{takenByIDEmployee.FirstName} {takenByIDEmployee.LastName} - {takenByIDEmployee.JobTitle}");
            foreach (var item in takenByIDEmployee.EmployeesProjects)
            {
                sb.AppendLine($"{item.Project.Name}");
            }

            return sb.ToString();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departmentsWithMoreThan5Emplooyes = context.Departments
                                                     .Where(x => x.Employees.Count > 5)
                                                     .OrderBy(x => x.Employees.Count)
                                                     .ThenBy(x => x.Name)
                                                     .Select(x => new
                                                     {
                                                         x.Name,
                                                         x.Manager.FirstName,
                                                         x.Manager.LastName,
                                                         Employees = x.Employees.Select(e => new
                                                         {
                                                             e.FirstName,
                                                             e.LastName,
                                                             e.JobTitle
                                                         })
                                                                                 .OrderBy(e => e.FirstName)
                                                                                 .ThenBy(e => e.LastName)
                                                                                 .ToList()
                                                     })
                                                     .ToList();

            var sb = new StringBuilder();
            foreach (var currDepartment in departmentsWithMoreThan5Emplooyes)
            {
                sb.AppendLine($"{currDepartment.Name} - {currDepartment.FirstName} {currDepartment.LastName}");
                foreach (var employee in currDepartment.Employees)
                {
                    sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
                }
            }
            return sb.ToString();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            var lastest10Projects = context.Projects
                                    .Select(x => new
                                    {
                                        x.Name,
                                        x.Description,
                                        x.StartDate,
                                    })
                                    .OrderByDescending(x => x.StartDate)
                                    .Take(10)
                                    .OrderBy(x => x.Name)
                                    .ToList();

            var sb = new StringBuilder();
            foreach (var currProject in lastest10Projects)
            {
                sb.AppendLine($"{currProject.Name}");
                sb.AppendLine($"{currProject.Description}");
                sb.AppendLine($"{currProject.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            var employeesByGivenDepartment = context.Employees
                                              .Where(x => x.Department.Name.Contains("Engineering") ||
                                               x.Department.Name.Contains("Information Services") ||
                                               x.Department.Name.Contains("Tool Design") ||
                                               x.Department.Name.Contains("Marketing"))
                                              .OrderBy(x => x.FirstName)
                                              .ThenBy(x => x.LastName)
                                              .ToList();

            var sb = new StringBuilder();

            foreach (var employeeByGivenDepartment in employeesByGivenDepartment)
            {
                employeeByGivenDepartment.Salary *= 1.12m;
                sb.AppendLine($"{employeeByGivenDepartment.FirstName} {employeeByGivenDepartment.LastName} (${employeeByGivenDepartment.Salary:f2})");
            }
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employeesStartsWithSa = context.Employees
                                        .Where(x => x.FirstName.StartsWith("Sa"))
                                        .Select(x => new
                                        {
                                            x.FirstName,
                                            x.LastName,
                                            x.JobTitle,
                                            x.Salary
                                        })
                                        .OrderBy(x => x.FirstName)
                                        .ThenBy(x => x.LastName)
                                        .ToList();

            var sb = new StringBuilder();
            foreach (var emp in employeesStartsWithSa)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} - {emp.JobTitle} - (${emp.Salary:F2})");
            }
            return sb.ToString().Trim();
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            int givenId = 2;

            var mapingTable = context.EmployeesProjects
                    .Where(x => x.ProjectId == givenId)
                    .ToList();

            mapingTable.Clear();
            context.SaveChanges();

            var projectToDelete = context.Projects.Where(x => x.ProjectId == givenId).ToList();
            projectToDelete.Clear();
            context.SaveChanges();

            var take10Project = context.Projects
                                       .Select(x => new
                                       {
                                           x.Name
                                       })
                                       .Take(10)
                                       .ToList();

            var sb = new StringBuilder();
            foreach (var proj in take10Project)
            {
                sb.AppendLine($"{proj.Name}");
            }
            return sb.ToString().Trim();
        }

        public static string RemoveTown(SoftUniContext context)
        {
            string townToDelete = "Seattle";
            var town = context.Towns.Include(x => x.Addresses).FirstOrDefault(x => x.Name == townToDelete);
            var allAddressesIds = town.Addresses.Select(x => x.AddressId).ToList();

            var employees = context.Employees
                 .Where(x => x.Address.Town.Name == townToDelete)
                 .ToList();

            foreach (var emp in employees)
            {
                emp.AddressId = null;
            }
            context.SaveChanges();

            foreach (var addressId in allAddressesIds)
            {
                var address = context.Addresses
                    .FirstOrDefault(x => x.AddressId == addressId);

                context.Addresses.Remove(address);
            }
            context.SaveChanges();

            context.Towns.Remove(town);
            context.SaveChanges();

            return $"{allAddressesIds.Count()} addresses in {townToDelete} were deleted";
        }
    }
}
