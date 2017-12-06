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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace dayThreeRegister.Controllers
{
    [Route("api/values")]
    public class ValuesController : Controller
    {
        private DatabaseContext databaseContext;
        private CustomerRepository customerRepository;
        private readonly ILogger<ValuesController> _eventLogger;

        public ValuesController(DatabaseContext databaseContext, ILogger<ValuesController> logger)
        {
            this.databaseContext = databaseContext;
            customerRepository = new CustomerRepository();
            _eventLogger = logger;
        }


        // GET api/values
        [HttpGet, Route("getUsingId")]
        public IActionResult GetUsingId(int id, bool brief)
        {
            var result = customerRepository.GetCustomerById(id, databaseContext);
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
            _eventLogger.LogInformation("Command: Get all customers");
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

                    customerRepository.AddCustomer(customerToAdd, databaseContext);
                    //databaseContext.Add(customerToAdd);
                    //databaseContext.SaveChanges();
                    //SaveList(customerToAdd);
                    _eventLogger.LogInformation("Command: Created a new customer");

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

                customerRepository.AddAddress(addressToAdd, int.Parse(custId), databaseContext);
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
            string capitalizedPropertyName = CapitalizeFirstLetterWithoutTouchingTheRest(name);

            foreach (var customerToEdit in customerRepository.GetAllCustomers(databaseContext).Where(c => c.Id == selectedId ))
            {
                var message = customerRepository.UpdateCustomer(customerToEdit, this, capitalizedPropertyName, value, databaseContext);
                returnMessage = message;
            }
            if (returnMessage.Contains("Error"))
            {
                
                //string dataLocation = "C:/Users/jspan/Documents/visual studio 2017/Projects/dayThreeRegister/dayThreeRegister/wwwroot/data.txt";
                //System.IO.File.WriteAllBytes(dataLocation, new byte[0]);
                //foreach (var customer in listOfCustomers)
                //{
                //    SaveList(customer);
                //}
                return BadRequest(returnMessage);
            }
            else
            {
                return Ok(returnMessage);
            }
        }

        public ModelStateDictionary CheckIfEditedChangesAreValid(EditCustomer customerToEdit)
        {
            TryValidateModel(customerToEdit);

            if (ModelState.IsValid)
            {
                return ModelState;
            }
            else
            {
                return ModelState;
            }
        }

        [HttpPost, Route("deleteacustomer")]
        public IActionResult DeleteACustomer(int id)
        {
            var customerToRemove = customerRepository.GetCustomerById(id, databaseContext);
            customerRepository.RemoveCustomer(customerToRemove, databaseContext);

            return Ok("Customer removed");
        }

        [HttpPost, Route("deleteaddress")]
        public IActionResult DeleteAddress(string addressId, string custId)
        {
            customerRepository.RemoveCustomer(int.Parse(addressId), int.Parse(custId), databaseContext);

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
            
            customerRepository.SeedCustomers(dataLocation, databaseContext);

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
                customerRepository.UpdateAddress(addressToEdit, capitalizedPropertyName, value, databaseContext);
                returnMessage = $"Updated {capitalizedPropertyName} to {value}.";
            }

            return Ok(returnMessage);
        }
    }
}
