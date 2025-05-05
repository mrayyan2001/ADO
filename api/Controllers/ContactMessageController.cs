using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.ContactMessage;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactMessageController : ControllerBase
    {
        private readonly SqlConnection _conn;
        public ContactMessageController(IConfiguration config)
        {
            _conn = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var selectCommand = @$"
                        SELECT Id, Name, CreatedAt
                        FROM ContactMessages
                        ORDER BY CreatedAt ASC
                        OFFSET {(pageNumber - 1) * pageSize} ROWS fetch next {pageSize} rows only;
                        ";

            using (var adapter = new SqlDataAdapter(selectCommand, _conn))
            {
                await _conn.OpenAsync();

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var cmdText = @$"
                SELECT Id, Name,Email, Message, CreatedAt
                FROM ContactMessages
                where Id = {id};
            ";
                using (var cmd = new SqlCommand(cmdText, _conn))
                {
                    await _conn.OpenAsync();
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       

    }
}