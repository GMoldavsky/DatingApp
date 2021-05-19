using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        //public UserRepository(DataContext context)
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        // public async Task<MemberDto> GetMemberAsync(string username)
        // {
        //     return await _context.Users
        //         .Where(x => x.UserName == username)
        //         .Select(user => new MamberDto{
        //             //Manually map 'AppUser' to 'MemberDto'  instaed of useing AutoMapper 
        //             Id = user.Id,
        //             username = user.UserName
        //             //...
        //         }).SingleOrDefaultAsync();
        // }
        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        // {
        //     var query = _context.Users.AsQueryable();

        //     query = query.Where(u => u.UserName != userParams.CurrentUsername);
        //     query = query.Where(u => u.Gender == userParams.Gender);

        //     var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
        //     var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

        //     query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

        //     query = userParams.OrderBy switch
        //     {
        //         "created" => query.OrderByDescending(u => u.Created),
        //         _ => query.OrderByDescending(u => u.LastActive)
        //     };

        //     return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper
        //         .ConfigurationProvider).AsNoTracking(), 
        //             userParams.PageNumber, userParams.PageSize);
        // }

        ///////////////////////////////
        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            //return await _context.Users.SingleOrDefaultAsync(x => x.UserName == username);
            return await _context.Users
                .Include(p => p.Photos) //Will get error in browser 500, because class Photos return AppUser and User return AppUser: cercular reference exception. To solve this problem we will add another DTO
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

        // public async Task<string> GetUserGender(string username)
        // {
        //     return await _context.Users
        //         .Where(x => x.UserName == username)
        //         .Select(x => x.Gender).FirstOrDefaultAsync();
        // }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            //return await _context.Users.ToListAsync();
            return await _context.Users
                .Include(p => p.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync(){
            return await _context.SaveChangesAsync()> 0; //To return boolean. If something changed will return value > 0
        }
        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified; //We not do changes to Database, just mark this entity as modified
        }
    }
}
