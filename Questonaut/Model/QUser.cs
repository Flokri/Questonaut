using System.Collections.Generic;
using Plugin.CloudFirestore.Attributes;
using System.Collections.ObjectModel;

namespace Questonaut.Model
{
    /// <summary>
    /// Represents a Questonaut user.s
    /// </summary>
    public class QUser
    {
        public static string CollectionPath = "Users";

        /// <summary>
        /// The uid from the user. This uid comes from firebase auth.
        /// </summary>
        [Id]
        public string Id { get; set; }

        /// <summary>
        /// The username.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The email address of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The birthday of the user
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// The image of the user.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// This image is just a local copy to save ressources.
        /// </summary>
        [Ignored]
        public byte[] LocalImage { get; set; } = new byte[] { };

        /// <summary>
        /// The gender of the user.
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Alle the user elements that contains in this study.
        /// </summary>
        public List<string> ActiveStudies { get; set; } = new List<string>() { "AddStudy" };

        /// <summary>
        /// The real Study objects
        /// </summary>
        [Ignored]
        public ObservableCollection<QStudy> ActiveStudiesObjects { get; set; } = new ObservableCollection<QStudy>();
    }
}
