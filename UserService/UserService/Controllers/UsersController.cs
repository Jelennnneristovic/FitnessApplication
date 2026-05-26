using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UserServiceApplication.DTOs.Requests;
using UserServiceApplication.Interfaces;
using UserServiceInfrastructure.Data;


namespace UserServiceAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        private readonly AppDbContext _context; 
        private readonly IFileStorageService _fileStorageService;

        private readonly ITrainingServiceClient _trainingServiceClient;

        public UsersController(IUserService userService, AppDbContext context,
        IFileStorageService fileStorageService, ITrainingServiceClient trainingServiceClient)  
        {
            _userService = userService;
            _context = context;
            _fileStorageService = fileStorageService;
            _trainingServiceClient = trainingServiceClient;  

        }
        

        [HttpGet("clients")]
        [Authorize(Roles = "Admin")]
    
        public async Task<IActionResult> GetClients([FromQuery] UserFilterRequest filter)
        {
            var clients = await _userService.GetAllClientsAsync(filter);
            return Ok(clients);
        }

        [HttpGet("trainers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTrainers([FromQuery] UserFilterRequest filter)
        {
            var trainers = await _userService.GetAllTrainersAsync(filter);
            return Ok(trainers);
        }

        [HttpPatch("{id:guid}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Activate(Guid id)
        {
            try
            {
                var user = await _userService.ActivateAsync(id);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:guid}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            try
            {
                var user = await _userService.DeactivateAsync(id);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        

        [HttpPost("me/image")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            // 1. Validacija ekstenzije
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Nedozvoljen format slike. Koristite JPG, PNG ili WEBP.");

            // 2. Validacija veličine (5 MB = 5 * 1024 * 1024 bajtova)
            if (file.Length > 5 * 1024 * 1024)
                return BadRequest("Slika je prevelika. Maksimalna veličina je 5MB.");

            // 3. Uzmi ID ulogovanog korisnika (pretpostavka da imaš User.Identity)
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized();
            }
            // 4. Sačuvaj na disk preko servisa
            var imagePath = await _fileStorageService.SaveUserImage(userId, file);

            // 5. Ažuriraj putanju u bazi podataka
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound("Korisnik nije pronađen.");

            user.ProfileImageUrl = imagePath;
            await _context.SaveChangesAsync();

            return Ok(new { path = imagePath });
        }

        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetById(Guid userId)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (currentUserId == null)
                    return Unauthorized();

                var isAdmin = currentUserRole == "Admin";
                var isOwner = currentUserId == userId.ToString();

                if (!isAdmin && !isOwner)
                    return Forbid();

                var user = await _userService.GetByIdAsync(userId);

                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("{userId:guid}")]
        public async Task<IActionResult> UpdateProfile(Guid userId, [FromBody] UpdateUserRequest request)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (currentUserId == null)
                    return Unauthorized();

                var isAdmin = currentUserRole == "Admin";
                var isOwner = currentUserId == userId.ToString();

                if (!isAdmin && !isOwner)
                    return Forbid();

                var updatedUser = await _userService.UpdateAsync(userId, request);
                return Ok(updatedUser);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Pregled trenerskog profila (svako ulogovan moze da vidi)
        [HttpGet("{userId:guid}/trainer-profile")]
        public async Task<IActionResult> GetTrainerProfile(Guid userId)
        {
            try
            {
                var profile = await _userService.GetTrainerProfileAsync(userId);
                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Trener dopunjava SVOJ profil
        [HttpPut("me/trainer-profile")]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> UpdateMyTrainerProfile([FromBody] UpdateTrainerProfileRequest request)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdString, out var userId))
                    return Unauthorized();

                var profile = await _userService.UpdateTrainerProfileAsync(userId, request);
                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

