using System.IO;

namespace MiningFixer
{
    public interface ILogStreamProvider
    {
        FileStream GetLogStream(string logFilePath);
    }
}
