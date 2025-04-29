using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoteBirthy.Models;

namespace VoteBirthy.Repositories
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<IEnumerable<Employee>> GetUpcomingBirthdaysAsync(int daysAhead = 30);
        Task<IEnumerable<Employee>> GetBirthdaysForMonthAsync(int month);
        Task<Employee> GetByUserIdAsync(string userId);
        Task<Employee> GetByUsernameAsync(string username);
        Task<Employee> GetWithUpcomingBirthdayDetailsAsync(Guid employeeId);
    }
} 