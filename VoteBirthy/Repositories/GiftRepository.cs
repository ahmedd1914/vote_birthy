using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoteBirthy.Data;
using VoteBirthy.Models;

namespace VoteBirthy.Repositories
{
    public class GiftRepository : EfRepository<Gift>, IGiftRepository
    {
        private readonly AppDbContext _context;

        public GiftRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Gift>> GetPopularGiftsAsync(int count = 10)
        {
            return await _context.Gifts
                .Include(g => g.VoteOptions)
                    .ThenInclude(vo => vo.Casts)
                .OrderByDescending(g => g.VoteOptions.SelectMany(vo => vo.Casts).Count())
                .Take(count)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Gift>> GetAllAsync()
        {
            return await _context.Gifts.ToListAsync();
        }

        public override async Task<Gift> GetByIdAsync(Guid id)
        {
            return await _context.Gifts.FindAsync(id);
        }

        public override async Task<bool> AddAsync(Gift gift)
        {
            if (gift == null)
            {
                throw new ArgumentNullException(nameof(gift));
            }
            
            // Ensure GiftId is set
            if (gift.GiftId == Guid.Empty)
            {
                gift.GiftId = Guid.NewGuid();
            }
            
            // Make sure Id is synced with GiftId
            gift.Id = gift.GiftId;
            
            try
            {
                _context.Gifts.Add(gift);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> UpdateAsync(Gift gift)
        {
            if (gift == null)
            {
                throw new ArgumentNullException(nameof(gift));
            }
            
            // Make sure Id is synced with GiftId
            gift.Id = gift.GiftId;
            
            try
            {
                _context.Gifts.Update(gift);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> DeleteAsync(Guid id)
        {
            var gift = await GetByIdAsync(id);
            if (gift == null)
            {
                return false;
            }

            try
            {
                _context.Gifts.Remove(gift);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
} 