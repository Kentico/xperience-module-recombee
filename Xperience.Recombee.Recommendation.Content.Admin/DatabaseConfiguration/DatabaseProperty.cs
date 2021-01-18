namespace Xperience.Recombee.Recommendation.Content.Admin
{
    /// <summary>
    /// Represents an item property in the Recombee database.
    /// </summary>
    public class DatabaseProperty
    {
        /// <summary>
        /// Name of the property.
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Type of the property.
        /// </summary>
        public string Type { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseProperty"/> class.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        public DatabaseProperty(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}
