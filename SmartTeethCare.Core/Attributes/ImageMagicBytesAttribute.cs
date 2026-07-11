using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace SmartTeethCare.Core.Attributes
{
    public class ImageMagicBytesAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file == null)
            {
                return ValidationResult.Success;
            }

            if (!IsValidImage(file))
            {
                return new ValidationResult(ErrorMessage ?? "Invalid file type. Only JPG and PNG images are allowed.");
            }

            return ValidationResult.Success;
        }

        private bool IsValidImage(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            if (stream.Length < 8)
                return false;

            byte[] buffer = new byte[8];
            stream.Read(buffer, 0, 8);
            
            // Reset stream position so it can be read later
            stream.Position = 0;

            // Check JPG/JPEG
            // JPEG magic bytes start with FF D8 FF
            if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
            {
                return true;
            }

            // Check PNG
            // PNG magic bytes: 89 50 4E 47 0D 0A 1A 0A
            if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47 &&
                buffer[4] == 0x0D && buffer[5] == 0x0A && buffer[6] == 0x1A && buffer[7] == 0x0A)
            {
                return true;
            }

            return false;
        }
    }
}
