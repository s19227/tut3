using System;
using System.Collections.Generic;

using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using tut3.Models;
using tut3.Services;

namespace tut3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentController : ControllerBase
    {
        IConfiguration _configuration;

        public StudentController(IStudentDbService studentsDbService, IConfiguration configuration)
        {
            _studentsDbService = studentsDbService;
            _configuration = configuration;
        }

        private readonly IStudentDbService _studentsDbService;

        [HttpGet][Authorize]
        public IActionResult GetStudents()
        {
            var students = _studentsDbService.GetStudents();
            return Ok(students);
        }

        [HttpGet("{@index}")][Route("enrollment")][Authorize]
        public IActionResult GetEnrollmentDataByStudentID(string index)
        {
            var enrollments = _studentsDbService.GetEnrollmentDataByStudentID(index);             

            if (enrollments.Count > 0) return Ok(enrollments);
            else return NotFound();
        }

        [HttpPost][Route("login")]
        public IActionResult Login(LoginRequest loginRequest)
        {
            var result = _studentsDbService.CheckLogin(loginRequest);

            if (result == null) return Unauthorized();

            Claim nameClaim = new Claim(ClaimTypes.Actor, loginRequest.Login);

            IEnumerable<Claim> claimsList = result.Roles.Select(role => new Claim(ClaimTypes.Role, role));
            var list = claimsList.ToList();
            list.Add(nameClaim);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(GenerateJWT(list.ToArray())),
                refreshToken = result.RefreshToken
            });

        }
        [HttpPost][Route("refresh")]
        public IActionResult Refresh(RefreshRequest request)
        {
            JwtSecurityToken oldJWT = new JwtSecurityToken(request.oldJWT);

            string rawIndex = oldJWT.Claims
                .ToList()
                .Find(e => e.Type.Equals(ClaimTypes.Actor))
                .ToString();

            var indexData = rawIndex.Split(" ");
            string index = indexData[^1][..^0];

            var rtr = new RefreshTokenRequest(request.refreshToken, index);

            var result = _studentsDbService.CheckRefreshToken(rtr);

            if (result == null) return Unauthorized();

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(GenerateJWT(oldJWT.Claims.ToArray())),
                refreshToken = result
            });
        }

        private JwtSecurityToken GenerateJWT(Claim[] claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
            );
        }
    }
}