namespace Cards.Data.Models
{
    /// <summary>
    /// Datamodel for project setup and configuration
    /// </summary>
    public class CardsConfig
    {
        public string DbConnection { get; set; }
        public string SentryIOToken { get; set; }
    }
}
