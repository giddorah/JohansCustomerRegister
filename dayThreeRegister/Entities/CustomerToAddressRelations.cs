using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dayThreeRegister.Entities
{
    public class CustomerToAddressRelations
    {
        public int Id { get; set; }
        public int CustId { get; set; }
        public int AdressId { get; set; }
    }
}
