namespace SlackAlertOwner.Notifier.Model
{
    using NodaTime;

    public class Shift
    {
        public TeamMate TeamMate { get; set; }
        public LocalDate Schedule { get; }

        public Shift(TeamMate teamMate, LocalDate schedule)
        {
            TeamMate = teamMate;
            Schedule = schedule;
        }
    }
}