using System;
using System.Globalization;
using System.IO;

namespace MiningFixer
{
    public class LogParser : ILogParser
    {
        private readonly FileStream logStream;
        private readonly AppSettings settings;
        private readonly IVoltageFixRunner voltageFixRunner;

        private int suspiciousCount = 0;

        public LogParser(FileStream logStream, AppSettings settings, IVoltageFixRunner voltageFixRunner)
        {
            this.logStream = logStream;
            this.settings = settings;
            this.voltageFixRunner = voltageFixRunner;
        }

        public void Dispose()
        {
            logStream.Close();
        }

        public void Parse()
        {
            logStream.Seek(-1, SeekOrigin.Current);
            var reader = new StreamReader(logStream);
            var line = reader.ReadLine();
            while (line != null)
            {
                if (line.Contains("GPUs power:"))
                {
                    var split = line.Split(' ');
                    try
                    {
                        var amount = float.Parse(split[2], CultureInfo.InvariantCulture);
                        if (amount >= settings.MaxAllowedPower)
                        {
                            suspiciousCount++;
                        }

                        if (amount < settings.MaxAllowedPower)
                        {
                            suspiciousCount = 0;
                        }

                        if (suspiciousCount >= settings.Threshold)
                        {
                            suspiciousCount = 0;
                            voltageFixRunner.Run();
                        }
                    }
                    catch (Exception) { }
                }
                line = reader.ReadLine();
            }
        }
    }
}
