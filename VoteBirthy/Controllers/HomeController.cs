using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using VoteBirthy.Data;
using VoteBirthy.Models;
using VoteBirthy.Services;
using VoteBirthy.ViewModels;

namespace VoteBirthy.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVoteService _voteService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IUnitOfWork unitOfWork, 
            IVoteService voteService,
            ILogger<HomeController> logger = null)
        {
            _unitOfWork = unitOfWork;
            _voteService = voteService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var activeVotes = await _voteService.GetActiveVotesAsync();
            var completedVotes = await _voteService.GetCompletedVotesAsync();
            
            var model = new VoteListViewModel
            {
                ActiveVotes = new List<VoteSummaryViewModel>(),
                CompletedVotes = new List<VoteSummaryViewModel>()
            };

            Guid currentUserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }

            foreach (var vote in activeVotes)
            {
                // Skip votes for the current user's birthday
                if (User.Identity.IsAuthenticated && vote.BirthdayEmpId == currentUserId)
                {
                    continue;
                }
                
                var hasVoted = false;
                if (User.Identity.IsAuthenticated)
                {
                    hasVoted = await _voteService.HasUserVotedAsync(vote.Id, currentUserId);
                }

                model.ActiveVotes.Add(new VoteSummaryViewModel
                {
                    VoteId = vote.Id,
                    BirthdayEmployeeName = vote.BirthdayEmpName ?? "Unknown",
                    StartedByName = vote.StartedByName ?? "Unknown",
                    StartDate = vote.StartDate,
                    EndDate = vote.EndDate,
                    IsClosed = vote.IsClosed,
                    TotalVotes = vote.Options?.Count ?? 0,
                    HasUserVoted = hasVoted
                });
            }

            foreach (var vote in completedVotes)
            {
                // Skip votes for the current user's birthday
                if (User.Identity.IsAuthenticated && vote.BirthdayEmpId == currentUserId)
                {
                    continue;
                }
                
                var hasVoted = false;
                if (User.Identity.IsAuthenticated)
                {
                    hasVoted = await _voteService.HasUserVotedAsync(vote.Id, currentUserId);
                }

                model.CompletedVotes.Add(new VoteSummaryViewModel
                {
                    VoteId = vote.Id,
                    BirthdayEmployeeName = vote.BirthdayEmpName ?? "Unknown",
                    StartedByName = vote.StartedByName ?? "Unknown",
                    StartDate = vote.StartDate,
                    EndDate = vote.EndDate,
                    IsClosed = vote.IsClosed,
                    TotalVotes = vote.Options?.Count ?? 0,
                    HasUserVoted = hasVoted
                });
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
