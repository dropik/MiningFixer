using System.IO;

namespace MiningFixer
{
    public interface ILogStreamProvider
    {
        StreamReader GetLogStream(string logFilePath);
    }
}
