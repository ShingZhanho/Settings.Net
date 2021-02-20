namespace Settings.Net.SettingsEntry {
    /// <summary>
    /// Represents an entry with integer value.
    /// </summary>
    public class IntEntry : BaseEntry {
        public override string ID { get; }
        public override string Description { get; set; }
    }
}