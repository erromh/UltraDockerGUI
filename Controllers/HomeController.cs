// DockerController.cs
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace UltraDockerGUI.Controllers
{
public class DockerController : Controller
{
    public IActionResult Index()
    {
        var output = GetDockerContainers();
        ViewBag.Containers = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        return View();
    }

    private string GetDockerContainers()
    {
        var process = new Process() { StartInfo = new ProcessStartInfo {
            FileName = "docker",
            Arguments = "ps --format \"{{.ID}}\t{{.Image}}\t{{.Status}}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        } };
        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return output;
    }
}
}
