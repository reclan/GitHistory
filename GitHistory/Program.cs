using System;

namespace GitHistory
{
    class Program
    {
        const string SEPARATOR      = "===END===";
        const string COMMIT_PATTERN = @"^([^)]*)(?:\(([^)]*?)\)|):(.*?(?:\[([^\]]+?)\]|))\s*$";
        const string FORMAT         = "%H%n%s%n%b%n" + SEPARATOR;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        private static string GetLastTag()
        {
            return null;
        }   
    }
}
