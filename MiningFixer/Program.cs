using Autofac;
using Divergic.Configuration.Autofac;
using System;
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
            containerBuilder.RegisterType<VoltageFixRunner>().As<IVoltageFixRunner>().InstancePerLifetimeScope();
            containerBuilder.Register<Func<FileStream, ILogParser>>(context =>
            {
                var newContext = context.Resolve<IComponentContext>();
                return stream =>
                {
                    return new LogParser(stream, newContext.Resolve<AppSettings>(), newContext.Resolve<IVoltageFixRunner>());
                };
            });

            var container = containerBuilder.Build();

            ServiceBase.Run(container.Resolve<MiningFixerService>());
        }
    }
}
