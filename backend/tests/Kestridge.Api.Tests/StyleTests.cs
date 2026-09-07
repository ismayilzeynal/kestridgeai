using System.Globalization;
using System.Text;

namespace Kestridge.Api.Tests;

// The repo forbids em dash, en dash, figure dash and horizontal bar in every
// file, and the site copy rules forbid them in user-visible strings.
public class StyleTests
{
    private static readonly char[] Dashes = ['\u2012', '\u2013', '\u2014', '\u2015'];

    private static readonly string[] Extensions =
        [".cs", ".sql", ".ps1", ".json", ".md", ".props", ".cnf", ".sln", ".editorconfig", ".sh"];

    private static readonly string[] SkipDirectories = ["bin", "obj", "Migrations", ".git", "TestResults"];

    [Fact]
    public void NoDashCharacters_InAnyBackendSourceFile()
    {
        var offences = new List<string>();

        foreach (var path in BackendFiles())
        {
            var lines = File.ReadAllLines(path, Encoding.UTF8);
            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i].IndexOfAny(Dashes) >= 0)
                {
                    offences.Add($"{path}:{(i + 1).ToString(CultureInfo.InvariantCulture)}");
                }
            }
        }

        Assert.True(offences.Count == 0, "Dash characters found in:\n" + string.Join('\n', offences));
    }

    [Fact]
    public void NoEmoji_InAnyBackendSourceFile()
    {
        var offences = new List<string>();

        foreach (var path in BackendFiles())
        {
            var lines = File.ReadAllLines(path, Encoding.UTF8);
            for (var i = 0; i < lines.Length; i++)
            {
                foreach (var rune in lines[i].EnumerateRunes())
                {
                    if (IsEmoji(rune.Value))
                    {
                        offences.Add($"{path}:{(i + 1).ToString(CultureInfo.InvariantCulture)}");
                        break;
                    }
                }
            }
        }

        Assert.True(offences.Count == 0, "Emoji found in:\n" + string.Join('\n', offences));
    }

    private static bool IsEmoji(int value) =>
        (value >= 0x1F300 && value <= 0x1FAFF)
        || (value >= 0x2600 && value <= 0x27BF)
        || value == 0xFE0F;

    private static IEnumerable<string> BackendFiles()
    {
        var root = BackendRoot();

        foreach (var path in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(root, path);
            var segments = relative.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (segments.Any(s => SkipDirectories.Contains(s, StringComparer.OrdinalIgnoreCase)))
            {
                continue;
            }

            var name = Path.GetFileName(path);
            if (Extensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase)
                || Extensions.Contains(name, StringComparer.OrdinalIgnoreCase))
            {
                yield return path;
            }
        }
    }

    private static string BackendRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Kestridge.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not locate the backend root from " + AppContext.BaseDirectory);
    }
}
