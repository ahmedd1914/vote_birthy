using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoteBirthy.Models;

namespace VoteBirthy.Repositories
{
    public interface IGiftRepository : IRepository<Gift>
    {
        Task<IEnumerable<Gift>> GetPopularGiftsAsync(int count = 10);
    }
} 