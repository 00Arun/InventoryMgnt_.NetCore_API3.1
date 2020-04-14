using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "OldPassword")]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "NewPassword")]
        [DataType(DataType.Password)]       
        public string NewPassword { get; set; }

        [Required]       
        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        [CompareAttribute("NewPassword",ErrorMessage ="Password doesn't match.")]
        public string ConfirmPassword { get; set; }
    }
}
