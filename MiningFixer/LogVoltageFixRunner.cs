using System.Diagnostics;

namespace MiningFixer
{
    public class LogVoltageFixRunner : IVoltageFixRunner
    {
        private readonly IVoltageFixRunner wrappee;
        private readonly EventLog eventLog;

        public LogVoltageFixRunner(IVoltageFixRunner wrappee, EventLog eventLog)
        {
            this.wrappee = wrappee;
            this.eventLog = eventLog;
        }

        public void Run()
        {
            wrappee.Run();
            eventLog.WriteEntry("Executed voltage fix bat");
        }
    }
}
