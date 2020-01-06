namespace SlackAlertOwner.Notifier.Model
{
    using System.Collections.Generic;

    public class MyOptions
    {
        public string BaseUrl { get; set; }
        public string EndPoint { get; set; }
        public string CalendarJobCronExpression { get; set; }
        public string NotifyJobCronExpression { get; set; }
        public string SpreadsheetId { get; set; }
        public string CalendarRange { get; set; }
        public string PatronDaysRange { get; set; }
        public string TeamMatesRange { get; set; }
        public string Pattern { get; set; }
        public string ApplicationName { get; set; }
        public string Certificate { get; set; }
        public string Password { get; set; }
        public string ServiceAccountEmail { get; set; }
        
        public IEnumerable<TeamMate> TeamMates { get; set; }
    }
}