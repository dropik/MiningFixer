using System.IO;

namespace MiningFixer
{
    public class LogStreamProvider : ILogStreamProvider
    {
        public FileStream GetLogStream(string logFilePath)
        {
            return new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }
}
