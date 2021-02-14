using Autofac;
using Divergic.Configuration.Autofac;
using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;

namespace MiningFixer
{
    static class Program
    {
        static void Main()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule<ConfigurationModule<JsonResolver<AppSettings>>>();

            containerBuilder.RegisterType<MiningFixerService>().AsSelf().InstancePerLifetimeScope();
            containerBuilder.RegisterType<LogFileFinder>().As<ILogFileFinder>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<LogStreamProvider>().As<ILogStreamProvider>().InstancePerLifetimeScope();
            containerBuilder.Register<Func<FileStream, ILogParser>>(context =>
            {
                var newContext = context.Resolve<IComponentContext>();
                return stream =>
                {
                    return new LogParser(stream, newContext.Resolve<AppSettings>(), newContext.Resolve<IVoltageFixRunner>());
                };
            });
            containerBuilder.Register(context =>
            {
                var eventLog = new EventLog();
                if (!EventLog.SourceExists("MiningFixer"))
                {
                    EventLog.CreateEventSource("MiningFixer", "Application");
                }
                eventLog.Source = "MiningFixer";
                eventLog.Log = "Application";
                return eventLog;
            });
            containerBuilder.Register<IVoltageFixRunner>(context =>
            {
                return new LogVoltageFixRunner(new VoltageFixRunner(context.Resolve<AppSettings>()), context.Resolve<EventLog>());
            });

            var container = containerBuilder.Build();

            ServiceBase.Run(container.Resolve<MiningFixerService>());
        }
    }
}
