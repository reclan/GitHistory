using System.Diagnostics;
using System.Threading.Tasks;

namespace GitHistory
{
    public class GitWrapper
    {
        public static async Task<ProcessResult> RunAsync(string workingDirectory, string arguments)
        {
            var result = new ProcessResult();

            using var p = new Process
            {
                StartInfo =
                {
                    FileName = "git",
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = false,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            if (p.Start())
            {
                using (var errorReader = p.StandardError)
                {
                    result.StandardError= await errorReader.ReadToEndAsync();
                }

                using (var outputReader = p.StandardOutput)
                {
                    result.StandardOutput = await outputReader.ReadToEndAsync();
                }

                p.WaitForExit(1000);

                result.ExitCode = p.ExitCode;
                result.Success = true;
            }

            return result;
        }
    }
}