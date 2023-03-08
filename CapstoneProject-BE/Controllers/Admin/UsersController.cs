﻿using AutoMapper;
using CapstoneProject_BE.AutoMapper;
using CapstoneProject_BE.DTO;
using CapstoneProject_BE.Helper;
using CapstoneProject_BE.Models;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Math;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProject_BE.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly InventoryManagementContext _context;
        public IMapper mapper;
        public UsersController(InventoryManagementContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            mapper = config.CreateMapper();
        }
        [HttpPost("Deactivate")]
        public async Task<IActionResult> Deactivate(int userid)
        {
            try
            {
                //var storageid = Int32.Parse(User.Claims.SingleOrDefault(x => x.Type == "StorageId").Value);
                var result = await _context.Users.SingleOrDefaultAsync(x => x.UserId == userid);
                if (result != null && result.Status)
                {
                    result.Status = false;
                    await _context.SaveChangesAsync();
                    return Ok("Thành công");
                }
                else
                {
                    return BadRequest("Không có dữ liệu");
                }
            }
            catch
            {
                return StatusCode(500);
            }
        }
        [HttpPost("Activate")]
        public async Task<IActionResult> Activate(int userid)
        {
            try
            {
                //var storageid = Int32.Parse(User.Claims.SingleOrDefault(x => x.Type == "StorageId").Value);
                var result = await _context.Users.SingleOrDefaultAsync(x => x.UserId == userid);
                if (result != null && !result.Status)
                {
                    result.Status = true;
                    await _context.SaveChangesAsync();
                    return Ok("Thành công");
                }
                else
                {
                    return BadRequest("Không có dữ liệu");
                }
            }
            catch
            {
                return StatusCode(500);
            }
        }
        [HttpGet("GetUserDetail")]
        public async Task<IActionResult> GetDetail(int userid)
        {
            try
            {
                //var storageid = Int32.Parse(User.Claims.SingleOrDefault(x => x.Type == "StorageId").Value);
                var result = await _context.Users.SingleOrDefaultAsync(x => x.UserId == userid);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Không có dữ liệu");
                }
            }
            catch
            {
                return StatusCode(500);
            }
        }
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers(int offset, int limit, int? roleid = 0, bool? status = false, string? search = "")
        {
            try
            {
                //var storageid = Int32.Parse(User.Claims.SingleOrDefault(x => x.Type == "StorageId").Value);
                var result = await _context.Users
                    .Where(x => (x.UserCode.Contains(search)||x.UserName.Contains(search)||x.Phone.Contains(search) || search == "")
                    && (x.Status == status || status == false)&&(x.RoleId==roleid||roleid==0)
                 ).OrderBy(x => x.UserName).ToListAsync();
                if (limit > result.Count() && offset >= 0)
                {
                    return Ok(new ResponseData<User>
                    {
                        Data = result.Skip(offset).Take(result.Count()).ToList(),
                        Offset = offset,
                        Limit = limit,
                        Total = result.Count()
                    });
                }
                else if (offset >= 0)
                {
                    return Ok(new ResponseData<User>
                    {
                        Data = result.Skip(offset).Take(limit).ToList(),
                        Offset = offset,
                        Limit = limit,
                        Total = result.Count()
                    });
                }
                else
                {
                    return NotFound("Không kết quả");
                }
            }
            catch
            {
                return StatusCode(500);
            }

        }
        [HttpPost("CreateStaff")]
        public async Task<IActionResult> CreateStaff(UserDTO u)
        {
            try
            {
                //var storageid = Int32.Parse(User.Claims.SingleOrDefault(x => x.Type == "StorageId").Value);
                var user = mapper.Map<User>(u);
                user.StorageId = 1;
                user.Password = "123456789aA@";
                _context.Add(user);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }

        }
        [HttpPut("UpdateStaff")]
        public async Task<IActionResult> UpdateStaff(UserDTO u)
        {
            try
            {
                //var storageid = Int32.Parse(User.Claims.SingleOrDefault(x => x.Type == "StorageId").Value);
                var user = mapper.Map<User>(u);
                user.StorageId = 1;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }

        }
    }
}
