using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlPerformanceKit.Test;

public static class Snapshot
{
    public static void AreEqual<T>(T actual)
    {
        var callerMethod = new StackTrace().GetFrame(1)!.GetMethod()!;
        var callerName = $"{callerMethod.DeclaringType!.Name}.{callerMethod.Name}";

        var expectedPath = @$"..\..\..\{callerName}.expected.txt";
        var actualPath = @$"..\..\..\{callerName}.actual.txt";

        var expectedText = File.ReadAllText(expectedPath);
        var actualText = JsonSerializer.Serialize(actual, new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(actualPath, actualText);

        if (expectedText != actualText)
        {
            new Process
            {
                StartInfo =
                {
                    FileName = "winmerge",
                    UseShellExecute = true,
                    ArgumentList = { expectedPath, actualPath }
                }
            }.Start();
        }

        Assert.AreEqual(expectedText, actualText);
    }
}