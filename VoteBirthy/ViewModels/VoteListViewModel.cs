using System;
using System.Collections.Generic;
using VoteBirthy.Models;

namespace VoteBirthy.ViewModels
{
    public class VoteListViewModel
    {
        public List<VoteSummaryViewModel> ActiveVotes { get; set; }
        public List<VoteSummaryViewModel> CompletedVotes { get; set; }
    }

    public class VoteSummaryViewModel
    {
        public Guid VoteId { get; set; }
        public string BirthdayEmployeeName { get; set; }
        public string StartedByName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsClosed { get; set; }
        public int TotalVotes { get; set; }
        public string WinningGiftName { get; set; }
        public bool HasUserVoted { get; set; }
    }
} 