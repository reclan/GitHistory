using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommandLine;

namespace GitHistory
{
    class Program
    {
        const string Separator      = "===END===";
        const string CommitPattern = @"^([^)]*)(?:\(([^)]*?)\)|):(.*?(?:\[([^\]]+?)\]|))\s*$";

        static async Task Main(string[] args)
        {
            await (await Parser.Default.ParseArguments<Options>(args)
                    .WithParsedAsync(RunOptionsAsync))
                    .WithNotParsedAsync(HandleParseErrorAsync);

        }

        private static async Task RunOptionsAsync(Options opts)
        {
            var path = opts.WorkingDirectory;
            string value = await GetLastTagAsync( path );

            var history = await GetHistoryAsync(path, value);
            if (history != null)
            {
                foreach (var commit in history)
                {
                    Console.WriteLine($"{commit.Hash}\n{commit.Subject}\n{commit.Body}\n");
                }
            }
            else
            {
                Environment.ExitCode = 128;
            }
        }

        private static async Task HandleParseErrorAsync(IEnumerable<Error> errs)
        {
            await Console.Out.WriteAsync("An error occured");
        }

        private static async Task<IEnumerable<CommitMeta>> GetHistoryAsync( string path, string tag)
        {
            IEnumerable<CommitMeta> GetCommits(string output)
            {
                var rawCommits = output.Split($"\n{Separator}");
                var metas = new List<CommitMeta>();

                foreach (var raw in rawCommits)
                {
                    if (!string.IsNullOrWhiteSpace(raw))
                    {
                        var lines = raw.TrimStart().Split("\n");
                        var meta = new CommitMeta
                        {
                            Hash = lines[0].Trim(),
                            Subject = lines[1].Trim(),
                            Body = string.Join("\n", lines[2..]).Trim()
                        };

                        metas.Add(meta);
                    }
                }

                return metas;
            }

            const string Format = "%H%n%s%n%b%n" + Separator;

            var revisions = string.IsNullOrWhiteSpace(tag) ? "" : (tag.Contains("..") ? tag : $"{tag}..HEAD");
            var result = await GitWrapper.RunAsync( path, $"log -E --format={Format} {revisions}" );
            
            if (result.Success && result.ExitCode == 0)
            {
                var output = result.StandardOutput.Trim();
                if (!string.IsNullOrWhiteSpace(output)) 
                    return GetCommits(output);
            }
            else if( !string.IsNullOrWhiteSpace( result.StandardError ))
            {
                await Console.Error.WriteAsync(result.StandardError);
            }

            return null;
        }

        private static async Task<string> GetLastTagAsync( string path )
        {
            var result = await GitWrapper.RunAsync(path, "describe --tags --abbrev=0");
            if (result.Success)
            {
                var output = result.StandardOutput.Trim();
                if (string.IsNullOrWhiteSpace(output))
                    return null;
                return output;
            }

            return null;
        }   
    }

    public class Options
    {
        [Option('w', "work-dir", Required = false, Default = ".", HelpText = "Set working directory")]
        public string WorkingDirectory { get; set; }   
    }
}
