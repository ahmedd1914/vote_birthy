using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Diagnostics;
using VoteBirthy.Data;
using VoteBirthy.Models;
using VoteBirthy.Services;
using VoteBirthy.ViewModels;

namespace VoteBirthy.Controllers
{
    [Authorize]
    public class VoteController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVoteService _voteService;
        private readonly ILogger<VoteController> _logger;

        public VoteController(
            IUnitOfWork unitOfWork,
            IVoteService voteService,
            ILogger<VoteController> logger = null)
        {
            _unitOfWork = unitOfWork;
            _voteService = voteService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                
                // Check if the user can view this vote
                if (!await _voteService.CanViewVoteAsync(id, currentUserId))
                {
                    TempData["Error"] = "You cannot view votes for your own birthday";
                    return RedirectToAction("Index", "Home");
                }
                
                // Get the details view model
                var model = await _voteService.GetVoteDetailsViewModelAsync(id, currentUserId);
                
                return View(model);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var model = await _voteService.PrepareVoteCreateViewModelAsync(currentUserId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VoteCreateViewModel model)
        {
            _logger?.LogInformation("VoteController.Create POST method started");
            
            if (!ModelState.IsValid)
            {
                _logger?.LogInformation("Model validation errors");
                var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                model = await _voteService.PrepareVoteCreateViewModelAsync(currentUserId);
                return View(model);
            }
            
            try
            {
                _logger?.LogInformation("Attempting to create vote");
                var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                
                // Create the vote with end date included
                var voteId = await _voteService.StartVoteAsync(
                    model.BirthdayEmpId, 
                    currentUserId, 
                    model.SelectedGiftIds,
                    model.EndDate);
                
                _logger?.LogInformation($"Vote created successfully with ID: {voteId}");
                return RedirectToAction("Details", new { id = voteId });
            }
            catch (ArgumentException ex)
            {
                _logger?.LogInformation($"ArgumentException: {ex.Message}");
                ModelState.AddModelError("", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger?.LogInformation($"InvalidOperationException: {ex.Message}");
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                _logger?.LogInformation($"Unexpected exception: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while creating the vote.");
            }

            _logger?.LogInformation("Exception occurred, returning to Create view");
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            model = await _voteService.PrepareVoteCreateViewModelAsync(userId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CastVote(Guid voteId, Guid optionId)
        {
            _logger?.LogInformation($"CastVote called with voteId={voteId}, optionId={optionId}");
            
            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            try
            {
                var result = await _voteService.CastVoteAsync(voteId, currentUserId, optionId);
                
                if (result)
                {
                    _logger?.LogInformation("Vote cast successfully");
                    TempData["Success"] = "Your vote has been recorded";
                }
                else
                {
                    _logger?.LogInformation("Failed to cast vote");
                    TempData["Error"] = "Failed to record your vote";
                }
            }
            catch (ArgumentException)
            {
                _logger?.LogInformation("Vote or option not found");
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                _logger?.LogInformation($"Invalid operation: {ex.Message}");
                TempData["Error"] = ex.Message;
            }
            
            return RedirectToAction("Details", new { id = voteId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseVote(Guid id)
        {
            _logger?.LogInformation($"CloseVote called with id={id}");
            
            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            try
            {
                var result = await _voteService.EndVoteAsync(id, currentUserId);
                
                if (result)
                {
                    _logger?.LogInformation("Vote closed successfully");
                    TempData["Success"] = "Vote closed successfully";
                }
                else
                {
                    _logger?.LogInformation("Failed to close vote");
                    TempData["Error"] = "Failed to close the vote";
                }
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                _logger?.LogInformation($"Invalid operation: {ex.Message}");
                TempData["Error"] = ex.Message;
            }
            
            return RedirectToAction("Details", new { id });
        }

        [HttpGet]
        public async Task<IActionResult> TestDbConnection()
        {
            try
            {
                _logger?.LogInformation("Testing database connection and employee records...");
                
                // Test if we can retrieve employees
                var allEmployees = await _unitOfWork.Employees.GetAllAsync();
                var employeeCount = allEmployees?.Count() ?? 0;
                _logger?.LogInformation($"Retrieved {employeeCount} employees");
                
                if (employeeCount > 0)
                {
                    _logger?.LogInformation("Employee details:");
                    foreach (var emp in allEmployees.Take(5)) // List first 5 only to avoid too much output
                    {
                        _logger?.LogInformation($"- ID: {emp.EmployeeId}, Name: {emp.FullName}, DOB: {emp.DateOfBirth}");
                    }
                    
                    // Test if we can retrieve gifts
                    var allGifts = await _unitOfWork.Gifts.GetAllAsync();
                    var giftCount = allGifts?.Count() ?? 0;
                    _logger?.LogInformation($"Retrieved {giftCount} gifts");
                    
                    if (giftCount > 0)
                    {
                        _logger?.LogInformation("Gift details:");
                        foreach (var gift in allGifts.Take(5))
                        {
                            _logger?.LogInformation($"- ID: {gift.GiftId}, Name: {gift.Name}");
                        }
                    }
                    else
                    {
                        _logger?.LogInformation("ERROR: No gifts found in database!");
                    }
                    
                    // Check existing votes
                    var allVotes = await _unitOfWork.Votes.GetAllAsync();
                    var voteCount = allVotes?.Count() ?? 0;
                    _logger?.LogInformation($"Retrieved {voteCount} votes");
                    
                    if (voteCount > 0)
                    {
                        _logger?.LogInformation("Vote details:");
                        foreach (var vote in allVotes.Take(5))
                        {
                            _logger?.LogInformation($"- ID: {vote.Id}, BirthdayEmp: {vote.BirthdayEmpId}, StartedBy: {vote.StartedById}, Date: {vote.StartDate}");
                        }
                    }
                    
                    // Test data retrieval complete successfully
                    return Content("Database connection successful! See console for details.");
                }
                else
                {
                    _logger?.LogInformation("ERROR: No employees found in database!");
                    return Content("Database connection successful, but no employees found!");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogInformation($"Database test exception: {ex.GetType().Name}: {ex.Message}");
                _logger?.LogInformation($"Stack trace: {ex.StackTrace}");
                return Content($"Database test failed: {ex.Message}");
            }
        }
    }
} 