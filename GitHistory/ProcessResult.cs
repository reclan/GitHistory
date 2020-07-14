namespace GitHistory
{
    public class ProcessResult
    {
        public bool Success { get; set; }
        public string StandardOutput { get; set; }
        public string StandardError { get; set; }
        public int ExitCode { get; set; }
    }
}