using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dayThreeRegister.Entities
{
    public partial class DatabaseContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> context) : base(context)
        {

        }
        public List<Customer> GetAllCustomers()
        {
            var databaseListOfCustomers = new List<Customer>();

            foreach (var customer in Customers)
            {
                databaseListOfCustomers.Add(customer);
            }

            return databaseListOfCustomers;
        }
        public Customer GetCustomerById(int id)
        {
            var foundCustomer = Customers.SingleOrDefault(customer => customer.Id == id);

            return foundCustomer;
        }
    }
}
