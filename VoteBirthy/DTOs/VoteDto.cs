using System;
using System.Collections.Generic;

namespace VoteBirthy.DTOs
{
    public class VoteDto
    {
        public Guid Id { get; set; }
        public Guid BirthdayEmpId { get; set; }
        public string BirthdayEmpName { get; set; }
        public Guid StartedById { get; set; }
        public string StartedByName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsClosed { get; set; }
        public ICollection<VoteOptionDto> Options { get; set; }
    }
} 