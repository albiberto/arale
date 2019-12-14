namespace SlackAlertOwner.Notifier.Abstract
{
    using Model;
    using System.Threading.Tasks;

    public interface ISpreadSheetService
    {
        Task<Shift> GetShift();
        Task WriteCalendar();
    }
}