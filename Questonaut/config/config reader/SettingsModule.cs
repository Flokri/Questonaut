using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Questonaut.config.configtypes;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;

namespace Questonaut.config.configreader
{
    public class SettingsModule : Autofac.Module
    {
        private readonly string _ressourceId;
        private readonly string _sectionNameSuffix;

        public SettingsModule(string configurationFilePath, string sectionNameSuffix = "Settings")
        {
            _ressourceId = configurationFilePath;
            _sectionNameSuffix = sectionNameSuffix;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new SettingsReader(_ressourceId, _sectionNameSuffix))
                .As<ISettingsReader>()
                .SingleInstance();

            var settings = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Name.EndsWith(_sectionNameSuffix, StringComparison.InvariantCulture))
                .ToList();

            settings.ForEach(type =>
            {
                builder.Register(c => c.Resolve<ISettingsReader>().LoadSelection(type))
                    .As(type)
                    .SingleInstance();
            });
        }
    }

    public static class Container
    {
        public static IContainer Initialize()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new SettingsModule("Questonaut.config.config.json"));
            builder.RegisterType<AppCenterConfig>().As<IAppCenterConfig>();

            return builder.Build();
        }
    }
}
