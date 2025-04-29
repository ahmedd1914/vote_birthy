using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteBirthy.Models
{
    public class Vote : BaseEntity
    {
        [NotMapped]
        public Guid VoteId { get => Id; set => Id = value; }
        
        public Guid BirthdayEmpId { get; set; }
        public Employee BirthdayEmp { get; set; }
        
        public Guid StartedById { get; set; }
        public Employee StartedBy { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsClosed { get; set; }
        
        public ICollection<VoteOption> Options { get; set; }
    }
} 