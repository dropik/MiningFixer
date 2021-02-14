using System;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace MiningFixer
{
    public partial class MiningFixerService : ServiceBase
    {
        private readonly Timer timer;
        private readonly ILogFileFinder logFileFinder;
        private readonly ILogStreamProvider logStreamProvider;
        private readonly Func<FileStream, ILogParser> logParserProvider;

        private string prevLastLogFile = null;
        private ILogParser currentParser;

        public MiningFixerService(
            ILogFileFinder logFileFinder,
            ILogStreamProvider logStreamProvider,
            Func<FileStream, ILogParser> logParserProvider)
        {
            InitializeComponent();
            timer = new Timer();
            this.logFileFinder = logFileFinder;
            this.logStreamProvider = logStreamProvider;
            this.logParserProvider = logParserProvider;
        }

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new ElapsedEventHandler(OnTimerTick);
            timer.Interval = 10000;
            timer.Enabled = true;

            EventLog.Source = "Services";
            EventLog.Log = "MiningFixerService";
        }

        private void OnTimerTick(object sender, ElapsedEventArgs args)
        {
            var lastLogFile = logFileFinder.LastLogFile;
            if (lastLogFile != prevLastLogFile)
            {
                if (currentParser != null)
                {
                    currentParser.Dispose();
                }

                prevLastLogFile = lastLogFile;
                var stream = logStreamProvider.GetLogStream(lastLogFile);
                stream.Seek(0, SeekOrigin.End);
                currentParser = logParserProvider.Invoke(stream);
            }
            currentParser?.Parse();
        }

        protected override void OnStop()
        {
        }
    }
}
