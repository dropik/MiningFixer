using System;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace MiningFixer
{
    public partial class MainService : ServiceBase
    {
        private readonly Timer timer;
        private readonly ILogFileFinder logFileFinder;
        private readonly ILogStreamProvider logStreamProvider;
        private readonly Func<StreamReader, ILogParser> logParserProvider;

        private string prevLastLogFile = null;
        private ILogParser currentParser;

        public MainService(
            ILogFileFinder logFileFinder,
            ILogStreamProvider logStreamProvider,
            Func<StreamReader, ILogParser> logParserProvider)
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
        }

        private void OnTimerTick(object sender, ElapsedEventArgs args)
        {
            var lastLogFile = logFileFinder.LastLogFile;
            if (lastLogFile != prevLastLogFile)
            {
                prevLastLogFile = lastLogFile;
                var reader = logStreamProvider.GetLogStream(lastLogFile);
                currentParser = logParserProvider.Invoke(reader);
            }
            currentParser.Parse();
        }

        protected override void OnStop()
        {
        }
    }
}
