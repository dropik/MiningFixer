using System.IO;

namespace MiningFixer
{
    public class LogStreamProvider : ILogStreamProvider
    {
        public StreamReader GetLogStream(string logFilePath)
        {
            return new StreamReader(new FileStream(logFilePath, FileMode.Open, FileAccess.Read));
        }
    }
}
