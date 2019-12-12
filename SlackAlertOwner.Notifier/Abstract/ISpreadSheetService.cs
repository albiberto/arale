namespace SlackAlertOwner.Notifier.Abstract
{
    using Model;
    using System.Collections.Generic;

    public interface ISpreadSheetService
    {
        Shift Read();
    }
}