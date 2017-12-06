using dayThreeRegister.Controllers;
using dayThreeRegister.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dayThreeRegister.Models
{
    interface ICustomerRepository
    {
        IEnumerable<Customer> GetAllCustomers(DatabaseContext databaseContext);
        Customer GetCustomerById(int id, DatabaseContext databaseContext);
        void AddCustomer(Customer newCustomer, DatabaseContext databaseContext);
        void RemoveCustomer(Customer removeCustomer, DatabaseContext databaseContext);
        string UpdateCustomer(Customer editCustomer, ValuesController controller, string capitalizedPropertyName, string value, DatabaseContext databaseContext);
        void SeedCustomers(string dataLocation, DatabaseContext databaseContext);
    }
}
