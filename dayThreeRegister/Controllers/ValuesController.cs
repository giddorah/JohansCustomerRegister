using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using dayThreeRegister.Models;
using System.Globalization;
using System.Reflection;
using dayThreeRegister.Entities;
using System.ComponentModel.DataAnnotations;

namespace dayThreeRegister.Controllers
{
    public class CustomGenderAttribute : ValidationAttribute
    {
        public CustomGenderAttribute()
        {
            this.ErrorMessage = "Accepted values for gender is: Male or Female.";
        }

        public override bool IsValid(object value)
        {
            string gender = value as string;

            if (gender.ToLower() == "male" || gender.ToLower() == "female")
            {
                return true;
            }
            else
            {
                return false;
            }
        }    
    }

    public class CustomNameAttribute : ValidationAttribute
    {
        public CustomNameAttribute()
        {
            this.ErrorMessage = "Accepted values for name is: A-Ö.";
        }

        public override bool IsValid(object value)
        {
            string name = value as string;

            bool result = name.Any(x => !char.IsLetter(x));

            return result;
        }
    }



    [Route("api/values")]
    public class ValuesController : Controller
    {
        private DatabaseContext databaseContext;
        private CustomerRepository cr;

        public ValuesController(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
            cr = new CustomerRepository();
        }


        // GET api/values
        [HttpGet, Route("getUsingId")]
        public IActionResult GetUsingId(int id, bool brief)
        {
            var result = cr.GetCustomerById(id, databaseContext);
            var returnMessage = "";
            if (result != null)
            {
                if (brief)
                {
                    returnMessage = $"{result.FirstName} {result.LastName}";
                }
                else
                {
                    returnMessage = $"{result.Id}. {result.FirstName} {result.LastName} - {result.Gender} - {result.Email} - {result.Age} years old";
                }
                return Ok(returnMessage);
            }
            else
            {
                return NotFound($"A customer with the id {id} was not found.");
            }
        }

        [HttpGet, Route("getallcustomers")]
        public List<Customer> GetAllCustomers()
        {
            databaseContext.Database.EnsureCreated();
            var listOfCUstomers = GetCustomerList();

            return listOfCUstomers;
        }

        public List<Customer> GetCustomerList()
        {
            //string dataLocation = "C:/Users/jspan/Documents/visual studio 2017/Projects/dayThreeRegister/dayThreeRegister/wwwroot/data.txt";
            //var dataSet = System.IO.File.ReadAllLines(dataLocation);

            //var customers = new List<Customer2>();
            //var databaseCustomers = new List<Customer>();



            //foreach (var customer in dataSet)
            //{
            //    string[] splitString = customer.Split(",");
            //    customers.Add(new Customer2
            //    {
            //        Id = int.Parse(splitString[0]),
            //        FirstName = splitString[1],
            //        LastName = splitString[2],
            //        Gender = splitString[3],
            //        Email = splitString[4],
            //        Age = int.Parse(splitString[5])
            //    });
            //}

            return databaseContext.GetAllCustomers();
        }

        //public void SaveList(Customer2 newCustomer)
        //{
        //    string dataLocation = "C:/Users/jspan/Documents/visual studio 2017/Projects/dayThreeRegister/dayThreeRegister/wwwroot/data.txt";

        //    string newCustomerConvertedToString = newCustomer.Id.ToString() + "," + newCustomer.FirstName + "," + newCustomer.LastName + "," + newCustomer.Gender + "," + newCustomer.Email + "," + newCustomer.Age.ToString() + "\r\n";

        //    System.IO.File.AppendAllText(dataLocation, newCustomerConvertedToString);
        //}

        [HttpPost, Route("createcustomer")]
        public IActionResult CreateCustomer(AddCustomer newCustomer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                string concatinatedForValidation = newCustomer.FirstName + newCustomer.LastName + newCustomer.Gender + newCustomer.Email + newCustomer.Age;

                if (concatinatedForValidation.Contains(","))
                {
                    return BadRequest($"Your data contains an illegal character (,)");
                }
                else
                {
                    var currentCustomers = GetCustomerList();
                    int newId = (currentCustomers.Count() + 1);

                    var customerToAdd = new Customer { FirstName = CapitalizeFirstLetterAccordingToWritingRules(newCustomer.FirstName), LastName = CapitalizeFirstLetterAccordingToWritingRules(newCustomer.LastName), Gender = CapitalizeFirstLetterAccordingToWritingRules(newCustomer.Gender), Email = newCustomer.Email, Age = newCustomer.Age, DateCreated = DateTime.Now };

                    cr.AddCustomer(customerToAdd, databaseContext);
                    //databaseContext.Add(customerToAdd);
                    //databaseContext.SaveChanges();
                    //SaveList(customerToAdd);

                    return Ok($"Added {customerToAdd.FirstName} {customerToAdd.LastName}.");
                }
            }
        }

        [HttpPost, Route("createaddress")]
        public IActionResult CreateAddress(Address newAddress, string custId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var addressToAdd = new Address { StreetName = newAddress.StreetName, Number = newAddress.Number, PostalCode = newAddress.PostalCode, Area = newAddress.Area };

                cr.AddAddress(addressToAdd, int.Parse(custId), databaseContext);
                //databaseContext.Add(customerToAdd);
                //databaseContext.SaveChanges();
                //SaveList(customerToAdd);

                return Ok($"Added {addressToAdd.StreetName} {addressToAdd.Number}.");
            }
        }

        public string CapitalizeFirstLetterAccordingToWritingRules(string input)
        {
            var output = new CultureInfo("en-US").TextInfo.ToTitleCase(input);
            return output;
        }
        public string CapitalizeFirstLetterWithoutTouchingTheRest(string input)
        {
            var output = char.ToUpper(input[0]) + input.Substring(1);
            return output;
        }

        [HttpPost, Route("editcustomer")]
        public IActionResult EditCustomer(string name, string pk, string value)
        {
            int selectedId = int.Parse(pk);
            var returnMessage = "";
            bool somethingFails = false;
            string capitalizedPropertyName = CapitalizeFirstLetterWithoutTouchingTheRest(name);

            foreach (var customerToEdit in cr.GetAllCustomers(databaseContext).Where(c => c.Id == selectedId ))
            {
                cr.UpdateCustomer(customerToEdit, capitalizedPropertyName, value, databaseContext);
                returnMessage = $"Updated {capitalizedPropertyName} to {value}.";
            }
            if (somethingFails == false)
            {
                
                //string dataLocation = "C:/Users/jspan/Documents/visual studio 2017/Projects/dayThreeRegister/dayThreeRegister/wwwroot/data.txt";
                //System.IO.File.WriteAllBytes(dataLocation, new byte[0]);
                //foreach (var customer in listOfCustomers)
                //{
                //    SaveList(customer);
                //}
                return Ok(returnMessage);
            }
            else
            {
                return BadRequest("I have failed you.");
            }
        }

        [HttpPost, Route("deleteacustomer")]
        public IActionResult DeleteACustomer(int id)
        {
            var customerToRemove = cr.GetCustomerById(id, databaseContext);
            cr.RemoveCustomer(customerToRemove, databaseContext);

            return Ok("Customer removed");
        }

        [HttpPost, Route("deleteaddress")]
        public IActionResult DeleteAddress(string addressId, string custId)
        {
            cr.RemoveCustomer(int.Parse(addressId), int.Parse(custId), databaseContext);

            return Ok($"Address removed");
        }

        [HttpGet, Route("countcustomers")]
        public IActionResult CountCustomers()
        {
            var customers = databaseContext.GetAllCustomers();
            var amountOfCustomers = customers.Count();

            return Ok($"Amount of customers is: {amountOfCustomers}");
        }

        [HttpGet, Route("seedcustomers")]
        public IActionResult SeedCustomers()
        {
            var location = Assembly.GetEntryAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(location);
            string dataLocation = directory + "/data.txt";
            
            cr.SeedCustomers(dataLocation, databaseContext);

            return Ok("Removed and re-seeded the database.");
        }

        [HttpGet, Route("showcustomeradresses")]
        public List<Address> ShowCustomerAdresses(int id)
        {
            return databaseContext.GetAllAddressesRelatedToACustomer(id);
        }

        [HttpPost, Route("editaddress")]
        public IActionResult EditAddress(string name, string pk, string value)
        {
            var addressId = int.Parse(pk);
            var capitalizedPropertyName = CapitalizeFirstLetterWithoutTouchingTheRest(name);
            var returnMessage = "";

            foreach (var addressToEdit in databaseContext.GetAllAddresses().Where(address => address.Id == addressId))
            {
                cr.UpdateAddress(addressToEdit, capitalizedPropertyName, value, databaseContext);
                returnMessage = $"Updated {capitalizedPropertyName} to {value}.";
            }

            return Ok(returnMessage);
        }
    }
}
