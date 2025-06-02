using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace UltraDockerGUI.Controllers
{
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ShowContainers()
    {
        var output = RunDockerPs();
        ViewBag.DockerTable = BuildHtmlTable(output);
        return View("Index"); // Вернёт страницу Index.cshtml (в Layout подгрузится ViewBag)
    }

    private List<string> RunDockerPs()
    {
        var process =
            new Process { StartInfo = new ProcessStartInfo { FileName = "docker", Arguments = "ps -a",
                                                             RedirectStandardOutput = true, UseShellExecute = false,
                                                             CreateNoWindow = true } };

        var output = new List<string>();
        process.Start();
        while (!process.StandardOutput.EndOfStream)
        {
            output.Add(process.StandardOutput.ReadLine() ?? "");
        }
        process.WaitForExit();
        return output;
    }

    private string BuildHtmlTable(List<string> lines)
    {
        if (lines.Count == 0)
            return "<p>No containers found.</p>";

        var headers = Regex.Split(lines[0].Trim(), @"\s{2,}");
        var html = "<table class='table table-striped'><thead><tr>";
        foreach (var h in headers)
            html += $"<th>{h}</th>";
        html += "</tr></thead><tbody>";

        for (int i = 1; i < lines.Count; i++)
        {
            var cols = Regex.Split(lines[i].Trim(), @"\s{2,}");
            html += "<tr>";
            foreach (var c in cols)
                html += $"<td>{c}</td>";
            html += "</tr>";
        }

        html += "</tbody></table>";
        return html;
    }
}
}
