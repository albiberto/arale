namespace SlackAlertOwner.Notifier.Services
{
    using Abstract;
    using Microsoft.Extensions.Options;
    using Model;
    using System.Collections.Generic;
    using System.Linq;

    public class ShiftService : IShiftService
    {
        readonly ILocalDateService _dateService;
        readonly MyOptions _options;

        public ShiftService(ILocalDateService dateService, IOptions<MyOptions> options)
        {
            _dateService = dateService;
            _options = options.Value;
        }

        public Shift Parse(IEnumerable<object> origin)
        {
            var value = origin.ToList();

            return new Shift
            {
                Schedule = _dateService.Parse(value),
                TeamMate = new TeamMate
                {
                    Name = $"{value.FirstOrDefault()}"
                }
            };
        }
    }
}