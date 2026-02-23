using System;
using System.IO;

namespace BitcoinClub.Infrastructure.Files
{
    public sealed class UploadPathValidator : IUploadPathValidator
    {
        private static readonly string[] AllowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

        public bool IsAllowedFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            // Reject backslash explicitly (Path.GetFileName treats \ as literal on Linux)
            if (fileName.Contains('\\'))
            {
                return false;
            }

            var name = Path.GetFileName(fileName);
            return string.Equals(fileName, name, StringComparison.Ordinal);
        }

        public bool IsAllowedExtension(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                return false;
            }

            extension = extension.StartsWith('.') ? extension : $".{extension}";
            foreach (var allowed in AllowedExtensions)
            {
                if (string.Equals(allowed, extension, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
