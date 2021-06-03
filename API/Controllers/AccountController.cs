using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using System.Linq;
using AutoMapper;

using Microsoft.AspNetCore.Identity;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        //private readonly DataContext _context ;
        private readonly ITockenService _tockenService ;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        //public AccountController(DataContext context, ITockenService tockenService, IMapper mapper)
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITockenService tockenService, IMapper mapper)
        {
            //_context = context;
            _tockenService = tockenService;
            _mapper = mapper;

            _signInManager = signInManager;
            _userManager = userManager;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDTOs registerDto){
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

            //using var hmac = new HMACSHA512();
            // var user = new AppUser{
            //     UserName = registerDto.Username.ToLower(),
            //     PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            //     PasswordSalt = hmac.Key
            // };
            var user = _mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.Username.ToLower();
            //user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            //user.PasswordSalt = hmac.Key;
            
            // _context.Users.Add(user);
            // await _context.SaveChangesAsync();
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded) return BadRequest(result.Errors);

            //return user;
            return new UserDto{
                Username = user.UserName,
                Token = await _tockenService.CreateTocken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDTOs loginDto){
            //var user = await _context.Users
            var user = await _userManager.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            if (user == null) return Unauthorized("Invalid username");

            // using var hmac = new HMACSHA512(user.PasswordSalt);
            // var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            // //Compare PSW bytes
            // for (int i =0; i < computedHash.Length; i++){
            //     if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password.");
            // }

            var result = await _signInManager
                .CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded) return Unauthorized();

            //return user;
            return new UserDto{
                Username = user.UserName,
                Token = await _tockenService.CreateTocken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };

        }
        public async Task<bool> UserExists(string username){
            //return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

    }
}