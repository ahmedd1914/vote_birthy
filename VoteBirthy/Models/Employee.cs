using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteBirthy.Models
{
    public class Employee : BaseEntity
    {
        [NotMapped]
        public Guid EmployeeId { get => Id; set => Id = value; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        
        [Required]
        public string PasswordHash { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [InverseProperty("StartedBy")]
        public ICollection<Vote> StartedVotes { get; set; }
        
        [InverseProperty("BirthdayEmp")]
        public ICollection<Vote> BirthdayVotes { get; set; }
        
        public ICollection<VoteCast> Casts { get; set; }
    }
} 