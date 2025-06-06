using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace UltraDockerGUI
{
    [ApiController]
    public class DockerController : ControllerBase
    {
        [HttpGet("ps")]
        public async Task<IActionResult> GetDockerContainers()
        {
            var result = await RunDockerPs();
            return Ok(ParseDockerPsOutput(result));
        }

        private async Task<string> RunDockerPs()
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "ps --format \"{{.ID}}|{{.Image}}|{{.Names}}|{{.Status}}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processStartInfo };
            process.Start();
            string output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            return output;
        }

        private List<Dictionary<string, string>> ParseDockerPsOutput(string output)
        {
            var containers = new List<Dictionary<string, string>>();
            var lines = output.Split('\n', System.StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length >= 4)
                {
                    containers.Add(new Dictionary<string, string>
                    {
                        { "ID", parts[0] },
                        { "Image", parts[1] },
                        { "Name", parts[2] },
                        { "Status", parts[3] }
                    });
                }
            }

            return containers;
        }
    }
}
