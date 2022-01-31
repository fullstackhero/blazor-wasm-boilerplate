namespace FSH.BlazorWebAssembly.Client.Infrastructure.Common;

public static class ApplicationConstants
{
    public static readonly List<string> SupportedImageFormats = new()
    {
        ".jpeg",
        ".jpg",
        ".png"
    };
    public static readonly string StandardImageFormat = "image/jpeg";
    public static readonly int MaxImageWidth = 1500;
    public static readonly int MaxImageHeight = 1500;
    public static readonly long MaxAllowedSize = 1000000; // Allows Max File Size of 1 Mb.
}