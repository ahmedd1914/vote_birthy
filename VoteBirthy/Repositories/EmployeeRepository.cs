using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoteBirthy.Data;
using VoteBirthy.Models;

namespace VoteBirthy.Repositories
{
    public class EmployeeRepository : EfRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Employee>> GetUpcomingBirthdaysAsync(int daysAhead = 30)
        {
            var today = DateTime.Today;
            var endDate = today.AddDays(daysAhead);
            
            return await _dbContext.Employees
                .Where(e => 
                    (e.DateOfBirth.Month > today.Month || 
                    (e.DateOfBirth.Month == today.Month && e.DateOfBirth.Day >= today.Day)) &&
                    (e.DateOfBirth.Month < endDate.Month || 
                    (e.DateOfBirth.Month == endDate.Month && e.DateOfBirth.Day <= endDate.Day)))
                .OrderBy(e => e.DateOfBirth.Month)
                .ThenBy(e => e.DateOfBirth.Day)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetBirthdaysForMonthAsync(int month)
        {
            return await _dbContext.Employees
                .Where(e => e.DateOfBirth.Month == month)
                .OrderBy(e => e.DateOfBirth.Day)
                .ToListAsync();
        }

        public async Task<Employee> GetByUserIdAsync(string userId)
        {
            return await _dbContext.Employees
                .FirstOrDefaultAsync(e => e.Username == userId);
        }

        public async Task<Employee> GetByUsernameAsync(string username)
        {
            return await _dbContext.Employees
                .FirstOrDefaultAsync(e => e.Username == username);
        }

        public async Task<Employee> GetWithUpcomingBirthdayDetailsAsync(Guid employeeId)
        {
            return await _dbContext.Employees
                .Include(e => e.BirthdayVotes)
                    .ThenInclude(v => v.Options)
                        .ThenInclude(o => o.Gift)
                .Include(e => e.BirthdayVotes)
                    .ThenInclude(v => v.Options)
                        .ThenInclude(o => o.Casts)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }
    }
} 