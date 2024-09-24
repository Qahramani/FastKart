namespace FastKartProject.Extentions;

public static class FileExtentionMethods
{
    public static bool IsImage(this IFormFile file)
    {
        return file.ContentType.Contains("image");
    }
    public static bool IsValidSize(this IFormFile file, int MB)
    {
        return file.Length <= MB * 1024 * 1024;
    }
    public static async Task<string> GenerateFileAsync(this IFormFile file,string path)
    {
        var imageName = $"{Guid.NewGuid()}-{file.FileName}";
        path = Path.Combine(path, imageName);

        using(var fs = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(fs);
        }
        return imageName;

    }
}
