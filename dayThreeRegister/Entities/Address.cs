using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dayThreeRegister.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string StreetName { get; set; }
        public int Number { get; set; }
        public int PostalCode { get; set; }
        public string Area { get; set; }
    }
}
