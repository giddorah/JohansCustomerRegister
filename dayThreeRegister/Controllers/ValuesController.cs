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
using Microsoft.AspNetCore.Hosting;

namespace dayThreeRegister.Controllers
{
    [Route("api/values")]
    public class ValuesController : Controller
    {
        private DatabaseContext databaseContext;
        private CustomerRepository customerRepository;
        private readonly ILogger<ValuesController> _eventLogger;
        private readonly IHostingEnvironment env;
        private readonly MailConfiguration mailConfiguration;

        public ValuesController(DatabaseContext databaseContext, ILogger<ValuesController> eventLogger, IHostingEnvironment env, MailConfiguration mailConfiguration)
        {
            this.databaseContext = databaseContext;
            customerRepository = new CustomerRepository();
            _eventLogger = eventLogger;
            this.env = env;
            this.mailConfiguration = mailConfiguration;
        }
        [HttpGet, Route("env")]
        public IActionResult Env()
        {
            return Ok( new object[] {
                env.IsDevelopment(),
                env.IsProduction(),
                env.ContentRootPath,
                env.ApplicationName,
                env.EnvironmentName,
                env.WebRootPath,
                mailConfiguration
            });
        }

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
            return databaseContext.GetAllCustomers();
        }

        [HttpPost, Route("createcustomer")]
        public IActionResult CreateCustomer(AddCustomer newCustomer)
        {
            if (!ModelState.IsValid)
            {
                _eventLogger.LogInformation("Failed: Failed to add customer");
                return BadRequest(ModelState);
            }
            else
            {
                string concatinatedForValidation = newCustomer.FirstName + newCustomer.LastName + newCustomer.Gender + newCustomer.Email + newCustomer.Age;

                if (concatinatedForValidation.Contains(","))
                {
                    _eventLogger.LogInformation("Failed: Failed to add customer due to illegal character");
                    return BadRequest($"Your data contains an illegal character (,)");
                }
                else
                {
                    var currentCustomers = GetCustomerList();
                    int newId = (currentCustomers.Count() + 1);

                    var customerToAdd = new Customer { FirstName = CapitalizeFirstLetterAccordingToWritingRules(newCustomer.FirstName), LastName = CapitalizeFirstLetterAccordingToWritingRules(newCustomer.LastName), Gender = CapitalizeFirstLetterAccordingToWritingRules(newCustomer.Gender), Email = newCustomer.Email, Age = newCustomer.Age, DateCreated = DateTime.Now };

                    customerRepository.AddCustomer(customerToAdd, databaseContext);

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
                _eventLogger.LogInformation("Failed: Failed to add address");
                return BadRequest(ModelState);
            }
            else
            {
                var addressToAdd = new Address { StreetName = newAddress.StreetName, Number = newAddress.Number, PostalCode = newAddress.PostalCode, Area = newAddress.Area };

                customerRepository.AddAddress(addressToAdd, int.Parse(custId), databaseContext);

                _eventLogger.LogInformation("Command: Added address");
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
                _eventLogger.LogInformation("Failed: Failed to edit customer");
                return BadRequest(returnMessage);
            }
            else
            {
                _eventLogger.LogInformation("Command: Edited customer");
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
            _eventLogger.LogInformation("Command: Deleted customer");
            return Ok("Customer removed");
        }

        [HttpPost, Route("deleteaddress")]
        public IActionResult DeleteAddress(string addressId, string custId)
        {
            customerRepository.RemoveCustomer(int.Parse(addressId), int.Parse(custId), databaseContext);
            _eventLogger.LogInformation("Command: Deleted address");
            return Ok($"Address removed");
        }

        [HttpGet, Route("countcustomers")]
        public IActionResult CountCustomers()
        {
            var customers = databaseContext.GetAllCustomers();
            var amountOfCustomers = customers.Count();
            _eventLogger.LogInformation("Command: Counted customers");
            return Ok($"Amount of customers is: {amountOfCustomers}");
        }

        [HttpGet, Route("seedcustomers")]
        public IActionResult SeedCustomers()
        {
            var location = Assembly.GetEntryAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(location);
            string dataLocation = directory + "/data.txt";
            
            customerRepository.SeedCustomers(dataLocation, databaseContext);
            _eventLogger.LogInformation("Command: Re-seeded the database");
            return Ok("Removed and re-seeded the database.");
        }

        [HttpGet, Route("showcustomeradresses")]
        public List<Address> ShowCustomerAdresses(int id)
        {
            _eventLogger.LogInformation("Command: Caught addresses of a customer");
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

            _eventLogger.LogInformation("Command: Edited an address");
            return Ok(returnMessage);
        }
    }
}
