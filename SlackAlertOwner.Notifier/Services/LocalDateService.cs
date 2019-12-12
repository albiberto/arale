namespace SlackAlertOwner.Notifier.Services
{
    using Abstract;
    using Microsoft.Extensions.Options;
    using Model;
    using NodaTime;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public class LocalDateService : ILocalDateService
    {
        readonly MyOptions _options;

        public LocalDateService(IOptions<MyOptions> options)
        {
            _options = options.Value;
        }

        public LocalDate Parse(IEnumerable<object> value) => LocalDate.FromDateTime(DateTime.ParseExact(
            value.Skip(1).FirstOrDefault()?.ToString(), _options.Pattern, null,
            DateTimeStyles.None));
    }
}