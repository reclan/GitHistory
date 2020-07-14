using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GitHistory
{
    class Program
    {
        const string SEPARATOR      = "===END===";
        const string COMMIT_PATTERN = @"^([^)]*)(?:\(([^)]*?)\)|):(.*?(?:\[([^\]]+?)\]|))\s*$";
        const string FORMAT         = "%H%n%s%n%b%n" + SEPARATOR;

        static async Task Main(string[] args)
        {

            var path = @"c:\repos\GitHistory";
            string value = await GetLastTag( path );

            Console.WriteLine($"A tag is '{value}'");
        }

        private async static Task<string> GetLastTag( string path )
        {
            using var p = new Process();
            p.StartInfo.FileName = "git";
            p.StartInfo.WorkingDirectory = path;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.Arguments = "describe --tags --abbrev=0";
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;

            p.Start();

            using( var errorReader = p.StandardError ){
                var error = await errorReader.ReadToEndAsync();
                // Console.Error.WriteLine(error);
            }

            using( var reader = p.StandardOutput ){
                var output = await reader.ReadToEndAsync();
                return Regex.Replace( output, @"\s", string.Empty);
            }
        }   
    }
}
