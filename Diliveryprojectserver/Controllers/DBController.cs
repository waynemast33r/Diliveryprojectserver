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

        #region User Methods

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

        [HttpPut]
        [Route("/updateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
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

                _logger.LogWarning("Модель не прошла валидацию при обновлении пользователя.");
                return BadRequest(errorsDetailed);
            }

            try
            {
                var existingUser = await _db.Users.FindAsync(user.UserId);
                if (existingUser == null)
                {
                    _logger.LogWarning($"Пользователь с ID {user.UserId} не найден.");
                    return NotFound($"Пользователь с ID {user.UserId} не найден.");
                }

                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.Phone = user.Phone;
                existingUser.RoleId = user.RoleId;

                if (!string.IsNullOrEmpty(user.PasswordHash) && user.PasswordHash != "unchanged")
                {
                    existingUser.PasswordHash = user.PasswordHash;
                }

                _db.Users.Update(existingUser);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"Пользователь {existingUser.Email} успешно обновлен.");
                return Ok(existingUser);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Ошибка базы данных при обновлении пользователя.");
                return StatusCode(500, "Ошибка при обновлении пользователя в базе данных.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неожиданная ошибка при обновлении пользователя.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        #endregion

        #region Order Methods

        // Получение всех заказов с именами пользователей
        [HttpGet]
        [Route("/getOrders")]
        public async Task<ActionResult<IEnumerable<object>>> GetOrders()
        {
            try
            {
                if (_db == null || _db.Orders == null)
                {
                    return NotFound("Сущность 'DeliveryRoutesDbContext.Orders' имеет значение null.");
                }

                var orders = await _db.Orders
                    .Include(o => o.User)
                    .Include(o => o.Driver)
                    .Include(o => o.Status)
                    .Select(o => new
                    {
                        OrderId = o.OrderId,
                        CustomerName = $"{o.User.FirstName} {o.User.LastName}",
                        CustomerEmail = o.User.Email,
                        CustomerPhone = o.User.Phone,
                        DriverName = $"{o.Driver.FirstName} {o.Driver.LastName}",
                        PickupAddress = o.PickupAddress,
                        DeliveryAddress = o.DeliveryAddress,
                        OrderDate = o.OrderDate,
                        DeliveryDate = o.DeliveryDate,
                        StatusName = o.Status.StatusName,
                        StatusId = o.StatusId,
                        UserId = o.UserId,
                        DriverId = o.DriverId
                    })
                    .ToListAsync();

                _logger.LogInformation($"Возвращено {orders.Count} заказов.");
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении заказов.");
                return StatusCode(500, "Ошибка при получении заказов.");
            }
        }

        // Получение заказа по ID
        [HttpGet]
        [Route("/getOrder")]
        public async Task<ActionResult<Order>> GetOrder(int orderId)
        {
            try
            {
                if (_db.Orders == null)
                {
                    return NotFound();
                }

                var order = await _db.Orders
                    .Include(o => o.User)
                    .Include(o => o.Driver)
                    .Include(o => o.Status)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    return NotFound($"Заказ с ID {orderId} не найден.");
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении заказа с ID {orderId}.");
                return StatusCode(500, "Ошибка при получении заказа.");
            }
        }

        // Добавление нового заказа
        [HttpPost]
        [Route("/addOrder")]
        public async Task<IActionResult> AddOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                var errorsDetailed = ModelState.Where(x => x.Value.Errors.Any())
                    .Select(x => new
                    {
                        Field = x.Key,
                        Error = x.Value.Errors.FirstOrDefault().ErrorMessage
                    })
                    .ToList();

                _logger.LogWarning("Модель заказа не прошла валидацию.");
                return BadRequest(errorsDetailed);
            }

            try
            {
                // Устанавливаем дату создания заказа
                order.OrderDate = DateTime.Now;

                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"Заказ успешно добавлен. ID: {order.OrderId}");
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении заказа.");
                return StatusCode(500, "Ошибка при добавлении заказа.");
            }
        }

        // Обновление заказа
        [HttpPut]
        [Route("/updateOrder")]
        public async Task<IActionResult> UpdateOrder([FromBody] OrderUpdateDto orderDto)
        {
            try
            {
                var existingOrder = await _db.Orders.FindAsync(orderDto.OrderId);
                if (existingOrder == null)
                {
                    _logger.LogWarning($"Заказ с ID {orderDto.OrderId} не найден.");
                    return NotFound($"Заказ с ID {orderDto.OrderId} не найден.");
                }

                // Проверяем, что пользователь и водитель существуют
                var userExists = await _db.Users.AnyAsync(u => u.UserId == orderDto.UserId);
                if (!userExists)
                {
                    _logger.LogWarning($"Пользователь с ID {orderDto.UserId} не найден.");
                    return BadRequest($"Пользователь с ID {orderDto.UserId} не найден.");
                }

                var driverExists = await _db.Users.AnyAsync(u => u.UserId == orderDto.DriverId && u.RoleId == 3);
                if (!driverExists)
                {
                    _logger.LogWarning($"Водитель с ID {orderDto.DriverId} не найден.");
                    return BadRequest($"Водитель с ID {orderDto.DriverId} не найден.");
                }

                var statusExists = await _db.OrderStatuses.AnyAsync(s => s.StatusId == orderDto.StatusId);
                if (!statusExists)
                {
                    _logger.LogWarning($"Статус с ID {orderDto.StatusId} не найден.");
                    return BadRequest($"Статус с ID {orderDto.StatusId} не найден.");
                }

                // Обновляем только основные поля заказа
                existingOrder.UserId = orderDto.UserId;
                existingOrder.DriverId = orderDto.DriverId;
                existingOrder.PickupAddress = orderDto.PickupAddress;
                existingOrder.DeliveryAddress = orderDto.DeliveryAddress;
                existingOrder.DeliveryDate = orderDto.DeliveryDate;
                existingOrder.StatusId = orderDto.StatusId;

                // Не обновляем навигационные свойства
                await _db.SaveChangesAsync();

                _logger.LogInformation($"Заказ с ID {existingOrder.OrderId} успешно обновлен.");
                return Ok(new { message = "Заказ успешно обновлен", orderId = existingOrder.OrderId });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Ошибка базы данных при обновлении заказа.");
                return StatusCode(500, "Ошибка при обновлении заказа в базе данных.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неожиданная ошибка при обновлении заказа.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Удаление заказа
        [HttpDelete]
        [Route("/deleteOrder")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            try
            {
                if (_db.Orders == null)
                {
                    return NotFound();
                }

                var order = await _db.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return NotFound($"Заказ с ID {orderId} не найден.");
                }

                _db.Orders.Remove(order);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"Заказ с ID {orderId} успешно удален.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при удалении заказа с ID {orderId}.");
                return StatusCode(500, "Ошибка при удалении заказа.");
            }
        }

        // Получение статусов заказов
        [HttpGet]
        [Route("/getOrderStatuses")]
        public async Task<ActionResult<IEnumerable<OrderStatus>>> GetOrderStatuses()
        {
            try
            {
                if (_db == null || _db.OrderStatuses == null)
                {
                    return NotFound("Сущность 'DeliveryRoutesDbContext.OrderStatuses' имеет значение null.");
                }

                var statuses = await _db.OrderStatuses.ToListAsync();
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении статусов заказов.");
                return StatusCode(500, "Ошибка при получении статусов заказов.");
            }
        }

        #endregion
    }
}