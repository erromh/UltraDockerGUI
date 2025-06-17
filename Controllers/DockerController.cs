using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UltraDockerGUI.Controllers;

namespace UltraDockerGUI.Controllers
{
public class DockerController : ControllerBase
{
    private readonly ILogger<DockerController> _logger;

    [HttpGet("ps")]

    public async Task<IActionResult> GetDockerContainers()
    {
        try
        {
            var result = await RunDockerPs();
            var containers = Ok(ParseDockerPsOutput(result));
            return Ok(containers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении контейнеров Docker");
            return Problem("Не удалось получить список контейнеров. Убедитесь, что Docker установлен и доступен.");
        }
    }

    private async Task<string> RunDockerPs()
    {
        try
        {
            var processStartInfo =
                new ProcessStartInfo { FileName = "docker",
                                       Arguments = "ps --format \"{{.ID}}|{{.Image}}|{{.Names}}|{{.Status}}\"",
                                       RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true };

            using var process = new Process { StartInfo = processStartInfo };
            process.Start();
            string output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            return output;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Не удалось получить список контейнеров Docker", ex);
        }
    }

    private List<Dictionary<string, string>> ParseDockerPsOutput(string output)
    {
        var containers = new List<Dictionary<string, string>>();
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var parts = line.Split('|');
            if (parts.Length >= 4)
            {
                containers.Add(new Dictionary<string, string> {
                    { "ID", parts[0] }, { "Image", parts[1] }, { "Name", parts[2] }, { "Status", parts[3] }
                });
            }
        }

        return containers;
    }
}
}
