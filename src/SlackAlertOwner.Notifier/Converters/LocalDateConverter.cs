namespace SlackAlertOwner.Notifier.Converters
{
    using Abstract;
    using Microsoft.Extensions.Options;
    using Model;
    using NodaTime;
    using NodaTime.Text;
    using System.Globalization;

    public class LocalDateConverter : ITypeConverter<LocalDate>
    {
        LocalDatePattern _pattern;

        public LocalDateConverter(IOptions<MyOptions> options)
        {
            _pattern = LocalDatePattern.CreateWithInvariantCulture(options.Value.Pattern);
        }

        public string Pattern
        {
            get => _pattern.PatternText;
            set => _pattern = LocalDatePattern.CreateWithInvariantCulture(value);
        }

        public string FormatValueAsString(LocalDate value) =>
            value == new LocalDate()
                ? string.Empty
                : value.ToString(Pattern, CultureInfo.InvariantCulture);

        public LocalDate ParseValueFromString(string value) => _pattern.Parse(value).GetValueOrThrow();
    }
}