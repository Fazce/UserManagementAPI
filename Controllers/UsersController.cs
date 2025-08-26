using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private static List<User> Users = new List<User>();
        private static int nextId = 1;

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAll()
        {
            try
            {
                // Return a copy to avoid exposing internal list
                return Ok(Users.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetById(int id)
        {
            try
            {
                var user = Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound(new { error = "User not found." });
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult<User> Create(User user)
        {
            try
            {
                var validationResult = ValidateUser(user, isNew:true);
                if (!string.IsNullOrEmpty(validationResult))
                    return BadRequest(new { error = validationResult });

                if (Users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                    return BadRequest(new { error = "A user with this email already exists." });
                    
                user.Id = nextId++;
                Users.Add(user);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, User updatedUser)
        {
            try
            {
                var user = Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound(new { error = "User not found." });

                var validationResult = ValidateUser(updatedUser, isNew:false);
                if (!string.IsNullOrEmpty(validationResult))
                    return BadRequest(new { error = validationResult });

                if (Users.Any(u => u.Email.Equals(updatedUser.Email, StringComparison.OrdinalIgnoreCase) && u.Id != id))
                    return BadRequest(new { error = "A user with this email already exists." });

                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;
                user.Email = updatedUser.Email;
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var user = Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound(new { error = "User not found." });
                Users.Remove(user);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }

        private string? ValidateUser(User user, bool isNew)
        {
            if (string.IsNullOrWhiteSpace(user.FirstName))
                return "First name is required.";
            if (string.IsNullOrWhiteSpace(user.LastName))
                return "Last name is required.";
            if (string.IsNullOrWhiteSpace(user.Email))
                return "Email is required.";
            if (!IsValidEmail(user.Email))
                return "Email format is invalid.";
            return null;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
