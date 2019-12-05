namespace SlackAlertOwner.Notifier.Model
{
    using System.Collections.Generic;

    public class MyOptions
    {
        public string BaseUrl { get; set; }
        public string EndPoint { get; set; }
        public string CronExpression { get; set; }
        public IEnumerable<TeamMate> TeamMates { get; set; }
    }

    public class TeamMate
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }
}