namespace Kestridge.Api.Tests;

// GetUtcNow is called in ContactEndpoint after validation, outside every try
// block, so this produces a genuinely unhandled exception and exercises
// UseExceptionHandler and CorsHeaderSafetyNet for real.
public sealed class ThrowingTimeProvider : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => throw new BadImageFormatException("forced by test");
}
