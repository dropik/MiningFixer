using System;
using System.Diagnostics;
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
        private readonly EventLog eventLog;

        private string prevLastLogFile = null;
        private ILogParser currentParser;

        public MiningFixerService(
            ILogFileFinder logFileFinder,
            ILogStreamProvider logStreamProvider,
            Func<FileStream, ILogParser> logParserProvider,
            EventLog eventLog)
        {
            InitializeComponent();
            timer = new Timer();
            this.logFileFinder = logFileFinder;
            this.logStreamProvider = logStreamProvider;
            this.logParserProvider = logParserProvider;
            this.eventLog = eventLog;
        }

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new ElapsedEventHandler(OnTimerTick);
            timer.Interval = 10000;
            timer.Enabled = true;

            AutoLog = false;
            eventLog.WriteEntry("Mining Fixer Service started");
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
            eventLog.WriteEntry("Mining Fixer Service stopped");
        }
    }
}
