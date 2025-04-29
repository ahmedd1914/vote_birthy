using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteBirthy.Models
{
    public class VoteOption : BaseEntity
    {
        [NotMapped]
        public Guid VoteOptionId { get => Id; set => Id = value; }
        
        public Guid VoteId { get; set; }
        public Vote Vote { get; set; }
        
        public Guid GiftId { get; set; }
        public Gift Gift { get; set; }
        
        public ICollection<VoteCast> Casts { get; set; }
    }
} 