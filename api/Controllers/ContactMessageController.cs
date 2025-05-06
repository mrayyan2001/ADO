using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.DTOs.ContactMessage;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace api.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class ContactMessageController : ControllerBase
    {
        private readonly IConfiguration _config;
        public ContactMessageController(IConfiguration config)
        {
            _config = config;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var selectCommand = @$"
                        SELECT Id, Name, CreatedAt
                        FROM ContactMessages
                        WHERE UserId = {userId}
                        ORDER BY CreatedAt ASC
                        OFFSET {(pageNumber - 1) * pageSize} ROWS fetch next {pageSize} rows only
                        ;
                        ";
                using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    using (var adapter = new SqlDataAdapter(selectCommand, conn))
                    {
                        var data = new DataTable();
                        adapter.Fill(data);
                        return Ok(data.AsEnumerable().Select(d => new ContactMessageDto()
                        {
                            Id = (int)d["Id"],
                            Name = d["Name"]?.ToString() ?? string.Empty,
                            CreatedAt = d["CreatedAt"]?.ToString() ?? string.Empty
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId is null)
                {
                    return BadRequest("User not found");
                }
                var cmdText = @$"
                SELECT Id, Name,Email, Message, CreatedAt
                FROM ContactMessages
                where Id = @Id AND UserId = @UserId;
                ";
                using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    using (var cmd = new SqlCommand(cmdText, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return Ok(new DetailsContactMessageDto()
                                {
                                    Id = (int)reader["Id"],
                                    Name = reader["Name"]?.ToString() ?? string.Empty,
                                    Email = reader["Email"]?.ToString() ?? string.Empty,
                                    Message = reader["Message"]?.ToString() ?? string.Empty,
                                    CreatedAt = reader["CreatedAt"]?.ToString() ?? string.Empty
                                });
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("{contactFormToken}")]
        public async Task<IActionResult> Create([FromRoute] string contactFormToken, [FromBody] CreateContactMessageDto dto)
        {
            try
            {
                var cmdText = @"
                            INSERT INTO [dbo].[ContactMessages]
                                ([Name], [Email], [Message], [CreatedAt], [UserId])
                            VALUES
                                (@Name, @Email, @Message, @CreatedAt, (SELECT Id FROM Users WHERE ContactFormToken = @ContactFormToken));
                            SELECT CAST(SCOPE_IDENTITY() AS INT);
                            ";
                using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    using (var cmd = new SqlCommand(cmdText, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", dto.Name);
                        cmd.Parameters.AddWithValue("@Email", dto.Email);
                        cmd.Parameters.AddWithValue("@Message", dto.Message);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@ContactFormToken", contactFormToken);
                        await conn.OpenAsync();
                        var result = await cmd.ExecuteScalarAsync();
                        if (result is null)
                            return BadRequest();
                        return Ok(new { StatusCode = 200, Message = "Messaged added", Id = (int)result });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}