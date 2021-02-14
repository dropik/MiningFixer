using System;
using System.Globalization;
using System.IO;

namespace MiningFixer
{
    public class LogParser : ILogParser
    {
        private readonly StreamReader logReader;
        private readonly AppSettings settings;
        private readonly IVoltageFixRunner voltageFixRunner;

        private int suspiciousCount = 0;

        public LogParser(StreamReader logReader, AppSettings settings, IVoltageFixRunner voltageFixRunner)
        {
            this.logReader = logReader;
            this.settings = settings;
            this.voltageFixRunner = voltageFixRunner;
        }

        public void Parse()
        {
            var line = logReader.ReadLine();
            while (line != null)
            {
                if (line.Contains("GPUs power:"))
                {
                    var split = line.Split(' ');
                    try
                    {
                        var amount = float.Parse(split[2], NumberStyles.Float);
                        if (amount >= settings.MaxAllowedPower)
                        {
                            suspiciousCount++;
                        }

                        if (suspiciousCount >= settings.Threshold)
                        {
                            suspiciousCount = 0;
                            voltageFixRunner.Run();
                        }
                    }
                    catch (Exception) { }
                }
                line = logReader.ReadLine();
            }
        }
    }
}
