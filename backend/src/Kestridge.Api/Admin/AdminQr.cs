using QRCoder;

namespace Kestridge.Api.Admin;

// Drawn on the server and shipped as a data: URI, so the panel shows it with a
// plain <img> and no script from anywhere else runs on the origin that holds
// the token. img-src already allows data:, and an image src is not a Trusted
// Types sink.
//
// PngByteQRCode only. It writes the PNG itself; the other QRCoder renderers go
// through System.Drawing.Common, which throws on Linux.
public static class AdminQr
{
    public static string DataUri(string text)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.M);
        using var png = new PngByteQRCode(data);

        return "data:image/png;base64," + Convert.ToBase64String(png.GetGraphic(5));
    }
}
