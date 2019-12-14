namespace SlackAlertOwner.Notifier.Abstract
{
    using Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISpreadSheetService
    {
        Task<Shift> Read();
    }
}