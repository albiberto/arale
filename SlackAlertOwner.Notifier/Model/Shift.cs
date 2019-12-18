namespace SlackAlertOwner.Notifier.Model
{
    using NodaTime;

    public class Shift
    {
        public Shift(TeamMate teamMate, LocalDate schedule)
        {
            TeamMate = teamMate;
            Schedule = schedule;
        }

        public TeamMate TeamMate { get;  }
        public LocalDate Schedule { get;  }
    }
}