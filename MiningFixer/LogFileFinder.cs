using System.IO;
using System.Linq;

namespace MiningFixer
{
    public class LogFileFinder : ILogFileFinder
    {
        private readonly AppSettings settings;

        public LogFileFinder(AppSettings settings)
        {
            this.settings = settings;
        }

        public string LastLogFile
        {
            get
            {
                var directory = new DirectoryInfo(Path.GetFullPath(settings.LogDirectory));
                var mostRecentFileQuery = from file in directory.GetFiles()
                                          orderby file.LastWriteTime descending
                                          select file.FullName;
                return mostRecentFileQuery.First();
            }
        }
    }
}
