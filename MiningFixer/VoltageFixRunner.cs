using System;
using System.Diagnostics;
using System.IO;

namespace MiningFixer
{
    public class VoltageFixRunner : IVoltageFixRunner
    {
        private readonly AppSettings settings;

        public VoltageFixRunner(AppSettings settings)
        {
            this.settings = settings;
        }

        public void Run()
        {
            try
            {
                var batDirectory = Path.GetDirectoryName(settings.VoltageFixerBatPath);
                var process = new Process();
                process.StartInfo.WorkingDirectory = batDirectory;
                process.StartInfo.FileName = Path.GetFileName(settings.VoltageFixerBatPath);
                process.StartInfo.CreateNoWindow = false;
                process.Start();
            }
            catch (Exception) { }
        }
    }
}
