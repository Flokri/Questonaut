using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using Prism.Commands;
using Unity.ResolverPolicy;
using Xamarin.Forms;

namespace Questonaut.Model
{
    /// <summary>
    /// Represents a questonaut study.
    /// </summary>
    public class QStudy
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
        public List<string> Container { get; set; }

        /// <summary>
        /// Elements that represents the study controls
        /// </summary>
        [Ignored]
        public List<QElement> Elements { get; set; }

        /// <summary>
        /// The end date of this study.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// This command will guide the user to the study detail screen. If he clicks the add button he get a selection of all studies to choose one to participate.
        /// </summary>
        [JsonIgnore]
        public DelegateCommand Command { get; set; }
    }
}
