using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    //[ApiController]
    //[Route("api/[controller]")]
    public class UsersController : BaseApiController //ControllerBase
    {
        //private readonly DataContext _context ;
        // public UsersController(DataContext context)
        // {
        //     _context = context;
        // }
        private readonly IUserRepository _userRepository ;
        private readonly IMapper _mapper ;
        //public UsersController(IUserRepository userRepository)
        public UsersController(IUserRepository userRepository,IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }
        
        //[AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            //return await _context.Users.ToListAsync();
            
            // var users = await _userRepository.GetUsersAsync();
            // //return Ok(users);
            // var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users); 
            // return Ok(usersToReturn);

            var users = await _userRepository.GetMembersAsync();
            return Ok(users);
        }

        //[Authorize]
        // [HttpGet("{id}")]
        // public async Task<ActionResult<AppUser>> GetUser(int id)
        // {
        //     return await _context.Users.FindAsync(id);
        // }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            //return await _userRepository.GetUserByUsernameAsync(username);
            
            //var user = await _userRepository.GetUserByUsernameAsync(username);
            //return _mapper.Map<MemberDto>(user);

            return await _userRepository.GetMemberAsync(username);
            //
            

        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //It will give us username for the token Api usered to autenticate this user
            var user = await _userRepository.GetUserByUsernameAsync(username);
            //user.City = memberUpdateDto.City; //we do not need => we use AutoMaper
            // ...
            // or
            _mapper.Map(memberUpdateDto, user);
            _userRepository.Update(user);
            if (await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update user");

            // var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            // _mapper.Map(memberUpdateDto, user);

            // _unitOfWork.UserRepository.Update(user);

            // if (await _unitOfWork.Complete()) return NoContent();

            // return BadRequest("Failed to update user");
        }
    }
}