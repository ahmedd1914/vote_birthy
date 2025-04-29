using System;
using System.Collections.Generic;
using VoteBirthy.Models;

namespace VoteBirthy.ViewModels
{
    public class VoteDetailsViewModel
    {
        public Guid VoteId { get; set; }
        public string BirthdayEmployeeName { get; set; }
        public string StartedByName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsClosed { get; set; }
        public List<VoteOptionViewModel> Options { get; set; }
        public VoteOptionViewModel WinningOption { get; set; }
        public bool HasUserVoted { get; set; }
        public Guid VotedOptionId { get; set; }
        public bool CanCloseVote { get; set; }
        public List<string> NonVoterNames { get; set; }
    }

    public class VoteOptionViewModel
    {
        public Guid VoteOptionId { get; set; }
        public string GiftName { get; set; }
        public string GiftDescription { get; set; }
        public int VoteCount { get; set; }
        public List<string> VoterNames { get; set; }
    }
} 