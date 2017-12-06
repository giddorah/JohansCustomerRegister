using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dayThreeRegister.Entities
{
    public partial class DatabaseContext : DbContext
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<CustomerToAddressRelations> Relations { get; set; }

        public List<Address> GetAllAddressesRelatedToACustomer(int customerId)
        {
            var databaseListOfCustomerAddresses = new List<Address>();
            var databaseListOfAddresses = new List<Address>();

            var relationsOfCustomId = Relations.Where(address => address.CustId == customerId);

            foreach (var relation in relationsOfCustomId)
            {
                databaseListOfCustomerAddresses.Add(Addresses.Single(address => relation.AdressId == address.Id));
            }

            return databaseListOfCustomerAddresses;
        }
        public Address GetAddressById(int id)
        {
            var foundAddress = Addresses.SingleOrDefault(address => address.Id == id);

            return foundAddress;
        }

        public List<Address> GetAllAddresses()
        {
            var allAvailableAddresses = new List<Address>();

            foreach (var address in Addresses)
            {
                allAvailableAddresses.Add(address);
            }

            return allAvailableAddresses;
        }

        public void EditAddress(int id, string capitalizedPropertyName)
        {

        }

        public void RemoveAddress(int custId, int addressId)
        { 
            Relations.Remove(Relations.Single(relation => relation.CustId == custId && relation.AdressId == addressId));
            //Addresses.Remove(Addresses.Single(address => address.Id == addressId));
        }
    }
}
