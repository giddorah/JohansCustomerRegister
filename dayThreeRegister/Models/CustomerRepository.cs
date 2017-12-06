using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dayThreeRegister.Entities;
using System.Reflection;
using dayThreeRegister.Controllers;

namespace dayThreeRegister.Models
{
    public class CustomerRepository : ICustomerRepository
    {
        public void AddCustomer(Customer newCustomer, DatabaseContext databaseContext)
        {
            databaseContext.Add(newCustomer);
            databaseContext.SaveChanges();
        }

        public IEnumerable<Customer> GetAllCustomers(DatabaseContext databaseContext)
        {
            databaseContext.Database.EnsureCreated();
            return databaseContext.GetAllCustomers();
        }

        public Customer GetCustomerById(int id, DatabaseContext databaseContext)
        {
            var listOfCUstomers = GetAllCustomers(databaseContext);

            var result = databaseContext.GetCustomerById(id);

            return result;
        }

        public void RemoveCustomer(Customer removeCustomer, DatabaseContext databaseContext)
        {
            databaseContext.Remove(removeCustomer);
            databaseContext.SaveChanges();
        }

        public string UpdateCustomer(Customer editCustomer, ValuesController controller, string capitalizedPropertyName, string value, DatabaseContext databaseContext)
        {
            editCustomer.DateEdited = DateTime.Now;
            Type type = editCustomer.GetType();
            var checkModel = new EditCustomer();
            foreach (PropertyInfo property in type.GetProperties())
            {
                if (property.Name.ToString().Equals(capitalizedPropertyName))
                {
                    if (capitalizedPropertyName == "Age")
                    {
                        try
                        {
                            property.SetValue(editCustomer, int.Parse(value));
                        }
                        catch (Exception)
                        {
                            return $"Error: Age can't be empty";
                        }
                        
                    }
                    else
                    {
                        property.SetValue(editCustomer, value);
                    }
                } 
            }

            checkModel.FirstName = editCustomer.FirstName;
            checkModel.LastName = editCustomer.LastName;
            checkModel.Age = editCustomer.Age;
            checkModel.Email = editCustomer.Email;
            checkModel.Gender = editCustomer.Gender;

            if (controller.TryValidateModel(checkModel))
            {
                databaseContext.SaveChanges();
                return $"Updated customer {checkModel.FirstName} {checkModel.LastName} ";
            }
            else
            {
                var returnMessage = controller.TryValidateModel(checkModel).ToString();
                return "Error: Model does not pass validation.";
            }
        }
        public void UpdateAddress(Address editAddress, string capitalizedPropertyName, string value, DatabaseContext databaseContext)
        {
            Type type = editAddress.GetType();
            foreach (PropertyInfo property in type.GetProperties())
            {
                if (property.Name.ToString().Equals(capitalizedPropertyName))
                {
                    if (capitalizedPropertyName == "Number" || capitalizedPropertyName == "PostalCode")
                    {
                        property.SetValue(editAddress, int.Parse(value));
                    }
                    else
                    {
                        property.SetValue(editAddress, value);
                    }
                }
            }
            databaseContext.SaveChanges();
        }

        public void SeedCustomers(string dataLocation, DatabaseContext databaseContext)
        {
            databaseContext.Customers.RemoveRange(databaseContext.Customers);
            var dataSet = CustomersToSeed(dataLocation);

            databaseContext.Customers.AddRange(dataSet);
            databaseContext.SaveChanges();
        }

        public List<Customer> CustomersToSeed(string dataLocation)
        {
            var databaseCustomers = new List<Customer>();
            var dataSet = System.IO.File.ReadAllLines(dataLocation);

            foreach (var customer in dataSet)
            {
                string[] splitString = customer.Split(",");
                databaseCustomers.Add(new Customer
                {
                    FirstName = splitString[1],
                    LastName = splitString[2],
                    Gender = splitString[3],
                    Email = splitString[4],
                    Age = int.Parse(splitString[5]),
                    DateCreated = DateTime.Now
                });
            }

            return databaseCustomers;
        }

        internal void AddAddress(Address addressToAdd, int custId, DatabaseContext databaseContext)
        {
            databaseContext.Add(addressToAdd);
            databaseContext.SaveChanges();

            var address = databaseContext.Addresses.Last();
            var addressId = address.Id;
            var newRelation = new CustomerToAddressRelations() { CustId = custId, AdressId = addressId };
            databaseContext.Add(newRelation);
            databaseContext.SaveChanges();
        }

        internal void RemoveCustomer(int addressId, int custId, DatabaseContext databaseContext)
        {
            databaseContext.RemoveAddress(custId, addressId);
            databaseContext.SaveChanges();
        }
    }
}
