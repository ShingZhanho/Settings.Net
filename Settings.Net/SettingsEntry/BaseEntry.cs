namespace Settings.Net.SettingsEntry {
    /// <summary>
    /// A base class for settings entry. This class is abstract and shall not be used in your code.
    /// </summary>
    public abstract class BaseEntry {
        /// <summary>
        /// The ID of the entry. The ID is unique under an entry group.
        /// </summary>
        public abstract string ID { get; }
        /// <summary>
        /// A descriptive line of this entry. Description is optional.
        /// </summary>
        public abstract string Description { get; set; }

        /// <summary>
        /// Using these chars for ID names is illegal and exception will be thrown.
        /// </summary>
        internal static readonly char[] InvalidIdChars = {'.'};

        /// <summary>
        /// Gets the list of chars that are illegal to use for an ID name.
        /// </summary>
        /// <returns>List of invalid ID chars.</returns>
        public static char[] GetInvalidIdChars() => InvalidIdChars;
    }
}