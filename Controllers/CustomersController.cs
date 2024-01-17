using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace jwtAuthDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Roy", "Kelly" };
        }
    }
}
