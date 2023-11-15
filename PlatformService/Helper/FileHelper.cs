using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace PlatformService.Helper{
    public static class FileHelper
{
    public static string ConvertImageToBase64(IFormFile image)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            image.CopyTo(ms);
            byte[] imageBytes = ms.ToArray();
            return Convert.ToBase64String(imageBytes);
        }
    }
}
}