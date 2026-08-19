

namespace PharmacyContracts.Modules.Auth.Application.DTOs
{
    public class CreateReviewerRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
