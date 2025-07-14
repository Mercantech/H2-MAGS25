using Microsoft.AspNetCore.Mvc;
using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StatusController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Tjekker om API'en kører korrekt.
        /// </summary>
        /// <returns>Status og besked om API'ens tilstand.</returns>
        /// <response code="200">API'en er kørende.</response>
        [HttpGet("healthcheck")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "OK", message = "API'en er kørende!" });
        }

        /// <summary>
        /// Tjekker om databasen er tilgængelig.
        /// </summary>
        /// <returns>Status og besked om databaseforbindelse.</returns>
        /// <response code="200">Database er kørende eller fejlbesked gives.</response>
        [HttpGet("dbhealthcheck")]
        public async Task<IActionResult> DBHealthCheck()
        {
            try 
            {
                bool canConnect = await _context.Database.CanConnectAsync();
                if (canConnect)
                {
                    return Ok(new { status = "OK", message = "Database er kørende!" });
                }
                else
                {
                    return Ok(new { status = "Error", message = "Database er ikke tilgængelig" });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Fejl ved forbindelse til database: " + ex.Message });
            }
        }

        /// <summary>
        /// Simpelt ping-endpoint til at teste API'en.
        /// </summary>
        /// <returns>Status og "Pong" besked.</returns>
        /// <response code="200">API'en svarede med Pong.</response>
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { status = "OK", message = "Pong 🏓" });
        }
    }
}
