using System;
using Plugin.CloudFirestore.Attributes;

namespace Questonaut.Model
{
    /// <summary>
    /// Represents a Questonaut user.s
    /// </summary>
    public class QUser
    {
        public static string CollectionPath = "users";

        /// <summary>
        /// The uid from the user. This uid comes from firebase auth.
        /// </summary>
        [Id]
        public string Id { get; set; }

        /// <summary>
        /// The username.
        /// </summary>
        public string Name { get; set; }
    }
}
