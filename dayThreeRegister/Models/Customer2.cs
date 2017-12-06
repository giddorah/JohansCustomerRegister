using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace dayThreeRegister.Models
{
    public class Customer2
    {
        [Required(ErrorMessage = "An Id is required")]
        public int Id { get; set; }

        [StringLength(20, ErrorMessage = "A first name is required", MinimumLength = 10)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "A last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "An Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A gender is required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "An Age is required")]
        public int Age { get; set; }
    }
}
