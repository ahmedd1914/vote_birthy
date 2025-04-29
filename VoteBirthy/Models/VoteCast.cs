using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteBirthy.Models
{
    public class VoteCast : BaseEntity
    {
        [NotMapped]
        public Guid VoteCastId { get => Id; set => Id = value; }
        
        public Guid VoteOptionId { get; set; }
        public VoteOption VoteOption { get; set; }
        
        public Guid VoterId { get; set; }
        public Employee Voter { get; set; }
        
        public DateTime CastDate { get; set; }
    }
} 