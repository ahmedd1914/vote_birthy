using System;
using System.Collections.Generic;

namespace VoteBirthy.DTOs
{
    public class VoteOptionDto
    {
        public Guid Id { get; set; }
        public Guid VoteId { get; set; }
        public Guid GiftId { get; set; }
        public string GiftName { get; set; }
        public string GiftDescription { get; set; }
        public int VoteCount { get; set; }
        public ICollection<VoterDto> Voters { get; set; }
    }
} 