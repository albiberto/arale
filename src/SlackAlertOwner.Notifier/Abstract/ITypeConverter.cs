namespace SlackAlertOwner.Notifier.Abstract
{
    public interface ITypeConverter<T>
    {
        string Pattern { get; set; }
        string FormatValueAsString(T value);
        T ParseValueFromString(string value);
    }
}