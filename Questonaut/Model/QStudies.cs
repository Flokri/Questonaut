using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Plugin.CloudFirestore.Attributes;
using Prism.Commands;

namespace Questonaut.Model
{
    /// <summary>
    /// Represents a questonaut study.
    /// </summary>
    public class QStudies
    {
        public static string CollectionPath = "Studies";

        /// <summary>
        /// The uid from the study. This uid comes from firebase auth.
        /// </summary>
        [Id]
        public string Id { get; set; }

        /// <summary>
        /// The name of the study.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The researcher team which leads the study.
        /// </summary>
        public string Team { get; set; }

        /// <summary>
        /// Alle the user elements that contains in this study.
        /// </summary>
        [JsonIgnore]
        public List<Plugin.CloudFirestore.IDocumentReference> Container { get; set; }

        /// <summary>
        /// This command will guide the user to the study detail screen. If he clicks the add button he get a selection of all studies to choose one to participate.
        /// </summary>
        [JsonIgnore]
        public DelegateCommand Command { get; set; }
    }
}
