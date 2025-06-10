using Diliveryprojectserver.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Diliveryprojectserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DBController : Controller
    {
        private DeliveryRoutesDbContext? _db;
        private readonly string _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        private readonly ILogger<DBController> _logger;
        public DBController(ILogger<DBController> logger, DeliveryRoutesDbContext appDbContext)
        {
            _db = appDbContext;
            _logger = logger;
        }

        [HttpGet]
        [Route("/getAdminByLogPass")]
        public async Task<ActionResult<User>> GetAdmin(string login, string pass)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == login && u.PasswordHash == pass && u.RoleId == 1);
            if (user == null)
                return NotFound("Администратор не найден");
            return user;
        }

        [HttpGet]
        [Route("/getUserByLogPass")]
        public async Task<ActionResult<User>> GetUser(string login, string pass)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == login && u.PasswordHash == pass);
        }

        [HttpGet]
        [Route("/getDriverByLogPass")]
        public async Task<ActionResult<User>> GetDriver(string login, string pass)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == login && u.PasswordHash == pass);
        }

        [HttpGet]
        [Route("/getManagerByLogPass")]
        public async Task<ActionResult<User>> GetManager(string login, string pass)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == login && u.PasswordHash == pass);
        }

        [HttpGet]
        [Route("/getUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            if (_db == null || _db.Users == null)
            {
                return NotFound("Сущность 'DeliveryRoutesDbContext.Users' имеет значение null.");
            }
            return await _db.Users.ToListAsync();
        }

        [HttpPost]
        [Route("/addUser")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    if (errors.Any())
                    {
                        foreach (var error in errors)
                        {
                            _logger.LogError($"Ошибка валидации для поля {key}: {error.ErrorMessage}");
                        }
                    }
                }

              
                var errorsDetailed = ModelState.Where(x => x.Value.Errors.Any())
                    .Select(x => new
                    {
                        Field = x.Key,
                        Error = x.Value.Errors.FirstOrDefault().ErrorMessage
                    })
                    .ToList(); 

                _logger.LogWarning("Модель не прошла валидацию. Возвращаем BadRequest с детальной информацией.");
                return BadRequest(errorsDetailed); 
            }

            try
            {
                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"Пользователь {user.Email} успешно добавлен.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении пользователя.");
                return StatusCode(500, "Internal Server Error"); 
            }
        }
    

    [HttpGet]
        [Route("/getUser")]
        public async Task<ActionResult<User>> GetUser(int UserID) 
        {
            if (_db.Users == null)
            {
                return NotFound();
            }

            var user = await _db.Users.FindAsync(UserID); 

            if (user == null)
            {
                return NotFound();
            }

            return user;
      
        }

        [HttpDelete]
        [Route("/deleteUser")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_db.Users == null)
            {
                return NotFound();
            }
            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return NoContent();
        }


    }

}

