using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.User;
using api.Helpers;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ITokenService _tokenService;

        public UserController(IConfiguration config, ITokenService tokenService)
        {
            _config = config;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var cmdText = @$"
            INSERT INTO [users]
                (FirstName, LastName, Email, PasswordHash, ContactFormToken, CreatedAt)
            VALUES
                (@FirstName, @LastName, @Email, @PasswordHash, @ContactFormToken, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS INT) AS UserId;
            ";
                using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    using (var cmd = new SqlCommand(cmdText, conn))
                    {
                        var contactFormToken = Guid.NewGuid();
                        cmd.Parameters.AddWithValue("@FirstName", dto.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", dto.LastName);
                        cmd.Parameters.AddWithValue("@Email", dto.Email);
                        cmd.Parameters.AddWithValue("@PasswordHash", HashHelper.ComputeSha512(dto.Password));
                        cmd.Parameters.AddWithValue("@ContactFormToken", contactFormToken);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                        await conn.OpenAsync();
                        var result = await cmd.ExecuteScalarAsync();
                        if (result is null)
                            return BadRequest();

                        var token = _tokenService.CreateToken((int)result, dto.Email, "User");

                        return Ok(new
                        {
                            Message = "Account is created",
                            User = new UserDto()
                            {
                                FirstName = dto.FirstName,
                                LastName = dto.LastName,
                                Email = dto.Email,
                                ContactFormToken = contactFormToken.ToString()
                            },
                            Token = token
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var cmdText = @"
                SELECT Id, FirstName, LastName, ContactFormToken
                FROM Users
                WHERE Email = @Email AND PasswordHash = @PasswordHash;
                ";
                await using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await using var command = new SqlCommand(cmdText, conn);
                command.Parameters.AddWithValue("@Email", dto.Email);
                command.Parameters.AddWithValue("@PasswordHash", HashHelper.ComputeSha512(dto.Password));
                await conn.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return Ok(new
                        {
                            Message = "Login Successfully",
                            User = new UserDto()
                            {
                                FirstName = reader["FirstName"]?.ToString() ?? string.Empty,
                                LastName = reader["LastName"]?.ToString() ?? string.Empty,
                                Email = dto.Email,
                                ContactFormToken = reader["ContactFormToken"]?.ToString() ?? string.Empty
                            },
                            Token = _tokenService.CreateToken(Convert.ToInt32(reader["Id"]), dto.Email, "User")
                        });
                    }
                    return Unauthorized("Email or Password is invalid.");
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}