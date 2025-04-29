using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VoteBirthy.ViewModels
{
    public class VoteCreateViewModel
    {
        public VoteCreateViewModel()
        {
            // Initialize collections to avoid null reference exceptions
            EmployeeList = new List<SelectListItem>();
            GiftList = new List<SelectListItem>();
            SelectedGiftIds = new List<Guid>();
        }

        [Required(ErrorMessage = "Please select the birthday employee")]
        [Display(Name = "Birthday Employee")]
        public Guid BirthdayEmpId { get; set; }

        [Required(ErrorMessage = "The Birthday Employee field is required")]
        [Display(Name = "Birthday Employee")]
        public string BirthdayEmpName { get; set; }

        public List<SelectListItem> EmployeeList { get; set; }

        [Required(ErrorMessage = "Please select at least 2 gifts")]
        [Display(Name = "Gift Options")]
        public List<Guid> SelectedGiftIds { get; set; }

        public List<SelectListItem> GiftList { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
    }
} 