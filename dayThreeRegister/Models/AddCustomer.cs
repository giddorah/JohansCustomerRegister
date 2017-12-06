using dayThreeRegister.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace dayThreeRegister.Models
{

    // 
    // 
    public class AddCustomer
    {
        [Required(ErrorMessage = "A first name is required.")]
        [StringLength(20, ErrorMessage = "The first name is too long.")]
        [RegularExpression("^[\\p{L}\\-'. ]+", ErrorMessage = "First name can only contain alphabetic letters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "A last name is required.")]
        [StringLength(20, ErrorMessage = "The last name is too long.")]
        [RegularExpression("^[\\p{L}\\-'. ]+", ErrorMessage = "Last name can only contain alphabetic letters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "An Email is required.")]
        [EmailAddress(ErrorMessage = "Not a valid email-address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A gender is required.")]
        [CustomGender]
        public string Gender { get; set; }

        [Required(ErrorMessage = "An Age is required.")]
        [Range(1, 120, ErrorMessage = "Valid age-range is 1 to 120.")]
        public int Age { get; set; }
    }
}
