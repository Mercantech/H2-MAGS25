using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        // API Healthcheck
        [HttpGet("healthcheck")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "OK", message = "API'en er kørende!" });
        }

        // Database Healthcheck
        [HttpGet("dbhealthcheck")]
        public IActionResult DBHealthCheck()
        {
            // Indtil vi har opsat EFCore, returnerer vi bare en besked

            try {
                // using (var context = new ApplicationDbContext())
                // {
                //     context.Database.CanConnect();
                // }
                throw new Exception("I har endnu ikke lært at opsætte EFCore! Det kommer senere!");
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Fejl ved forbindelse til database: " + ex.Message });
            }
            return Ok(new { status = "OK", message = "Database er kørende!" });
        }

        // Ping 
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { status = "OK", message = "Pong 🏓" });
        }
    }
}
