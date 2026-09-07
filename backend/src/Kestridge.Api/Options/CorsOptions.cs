namespace Kestridge.Api.Options;

public sealed class CorsOptions
{
    public const string Section = "Kestridge:Cors";

    public bool AllowVercelPreviews { get; set; }
}
