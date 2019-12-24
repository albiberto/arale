namespace SlackAlertOwner.Notifier.Model
{
    using System;

    public class TeamMate : ICloneable
    {
        public string Name { get; }
        public string Id { get; }
        public string CountryCode { get; }

        public TeamMate(string id, string name, string countryCode)
        {
            Id = id;
            Name = name;
            CountryCode = countryCode;
        }

        public object Clone() => new TeamMate(Id, Name, CountryCode);
    }
}