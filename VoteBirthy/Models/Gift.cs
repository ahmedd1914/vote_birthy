using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteBirthy.Models
{
    public class Gift : BaseEntity
    {
        public Gift()
        {
            // Initialize the collection to avoid null reference errors
            VoteOptions = new List<VoteOption>();
        }
        
        [NotMapped]
        public Guid GiftId 
        { 
            get => Id; 
            set => Id = value;
        }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Description { get; set; }
        
        public ICollection<VoteOption> VoteOptions { get; set; }
    }
} 