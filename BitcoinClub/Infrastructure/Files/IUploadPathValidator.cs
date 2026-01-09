namespace BitcoinClub.Infrastructure.Files
{
    public interface IUploadPathValidator
    {
        bool IsAllowedFileName(string fileName);

        bool IsAllowedExtension(string extension);
    }
}
