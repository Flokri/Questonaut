using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Questonaut.config.configreader
{
    class SettingsReader : ISettingsReader
    {
        #region instances
        private readonly string _ressourceId;
        private readonly string _sectionNameSuffix;
        #endregion

        #region staticFunctions
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new SettingsReaderContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        #endregion

        #region constructor
        public SettingsReader(string configurationFilePath, string sectionName = "Settings")
        {
            _ressourceId = configurationFilePath;
            _sectionNameSuffix = sectionName;
        }
        #endregion

        #region privateFunctions
        private static string ToCamelCase(string text)
            => string.IsNullOrWhiteSpace(text)
                ? string.Empty
                : $"{text[0].ToString().ToLowerInvariant()}{text.Substring(1)}";

        private static string ReadFile(string ressourceId)
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;
            Stream stream = assembly.GetManifestResourceStream(ressourceId);
            string text = "";
            using (var reader = new System.IO.StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            return text;
        }
        #endregion

        #region nestedClass
        private class SettingsReaderContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Select(p => CreateProperty(p, memberSerialization))
                    .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Select(f => CreateProperty(f, memberSerialization)))
                    .ToList();

                props.ForEach(p =>
                {
                    p.Writable = true;
                    p.Readable = true;
                });

                return props;
            }
        }
        #endregion

        #region publicFunctions
        public T Load<T>() where T : class, new() => Load(typeof(T)) as T;

        public object Load(Type type)
        {
            var jsonFile = ReadFile(_ressourceId);

            if (jsonFile == null)
            {
                return Activator.CreateInstance(type);
            }

            return JsonConvert.DeserializeObject(jsonFile, type, JsonSerializerSettings);
        }

        public object LoadSelection(Type type)
        {
            var jsonFile = ReadFile(_ressourceId);

            if (jsonFile == null)
            {
                return Activator.CreateInstance(type);
            }

            var section = ToCamelCase(type.Name.Replace(_sectionNameSuffix, string.Empty));
            var settingsData = JsonConvert.DeserializeObject<dynamic>(jsonFile, JsonSerializerSettings);
            var settingsSection = settingsData[section];

            return settingsSection == null
                ? Activator.CreateInstance(type)
                : JsonConvert.DeserializeObject(JsonConvert.SerializeObject(settingsSection), type,
                    JsonSerializerSettings);
        }

        public T LoadSettings<T>() where T : class, new() => LoadSelection(typeof(T)) as T;
        #endregion
    }
}
