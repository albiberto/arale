namespace SlackAlertOwner.Notifier.Abstract
{
    using Model;
    using System.Collections.Generic;

    public interface IShiftService
    {
        Shift Parse(IEnumerable<object> ownerShift);
    }
}