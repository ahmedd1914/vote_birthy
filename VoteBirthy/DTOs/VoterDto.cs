using System;

namespace VoteBirthy.DTOs
{
    public class VoterDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public DateTime VoteDate { get; set; }
    }
} 