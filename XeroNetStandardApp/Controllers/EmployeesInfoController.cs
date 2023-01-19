using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xero.NetStandard.OAuth2.Model.PayrollAu;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Config;
using Microsoft.Extensions.Options;

namespace XeroNetStandardApp.Controllers
{
    public class EmployeesInfo : ApiAccessorController<PayrollAuApi>
    {
        public EmployeesInfo(IOptions<XeroConfiguration> xeroConfig) : base(xeroConfig) { }

        // GET: /EmployeesInfo/
        public async Task<IActionResult> Index()
        {
            var response = await Api.GetEmployeesAsync(XeroToken.AccessToken, TenantId);

            return View(response._Employees);
        }

        // GET: /Contacts#Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Contacts#Create
        [HttpPost]
        public async Task<IActionResult> Create(string firstName, string lastName)
        {
            var homeAddress = new HomeAddress
            {
                AddressLine1 = "6 MeatMe Street",
                AddressLine2 = " ",
                Region = State.VIC,
                City = "Long Island",
                PostalCode = "9999",
                Country = "New York"
            };

            var employee = new Employee
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = DateTime.Today.AddYears(-20),
                HomeAddress = homeAddress
            };

            var employees = new List<Employee> { employee };

            await Api.CreateEmployeeAsync(XeroToken.AccessToken, TenantId, employees);

            return RedirectToAction("Index", "EmployeesInfo");
        }
    }
}