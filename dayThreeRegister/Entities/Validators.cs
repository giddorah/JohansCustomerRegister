using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace dayThreeRegister.Entities
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
}
