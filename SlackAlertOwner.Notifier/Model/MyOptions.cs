namespace SlackAlertOwner.Notifier.Model
{
    using System.Collections.Generic;

    public class MyOptions
    {
        public string BaseUrl { get; set; }
        public string EndPoint { get; set; }
        public string CronExpression { get; set; }
        public string SpreadsheetId { get; set; }
        public string SpreadsheetRange { get; set; }
        public string Pattern { get; set; }
        public string ApplicationName { get; set; }
        public string CredPath { get; set; }

        public IEnumerable<TeamMate> TeamMates { get; set; }
    }
}