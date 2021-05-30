using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly IPhotoService _photoService;
        //public UsersController(IUserRepository userRepository)
        public UsersController(IUserRepository userRepository,IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;
        }
        
        //[AllowAnonymous]
        [HttpGet]
        // public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        // {
        //     //return await _context.Users.ToListAsync();
            
        //     // var users = await _userRepository.GetUsersAsync();
        //     // //return Ok(users);
        //     // var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users); 
        //     // return Ok(usersToReturn);

        //     var users = await _userRepository.GetMembersAsync();
        //     return Ok(users);
        // }
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            userParams.CurrentUsername = user.UserName;
            if(string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = user.Gender== "male" ? "female" : "male";

            var users = await _userRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }

        //[Authorize]
        // [HttpGet("{id}")]
        // public async Task<ActionResult<AppUser>> GetUser(int id)
        // {
        //     return await _context.Users.FindAsync(id);
        // }

        [HttpGet("{username}",Name = "GetUser")]
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
            //var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //It will give us username for the token Api used to autenticate this user
            var username = User.GetUsername();
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

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            //var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if (await _userRepository.SaveAllAsync())
            //if (await _unitOfWork.Complete())
            {
                //return _mapper.Map<PhotoDto>(photo);
                return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
            }


            return BadRequest("Problem addding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            //var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _userRepository.SaveAllAsync()) return NoContent();
            //if (await _unitOfWork.Complete()) return NoContent();

            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            //var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("You cannot delete your main photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await _userRepository.SaveAllAsync()) return Ok();
            //if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Failed to delete the photo");
        }









    }
}