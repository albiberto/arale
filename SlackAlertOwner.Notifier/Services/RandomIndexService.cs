namespace SlackAlertOwner.Notifier.Services
{
    using Abstract;
    using System;

    public class RandomIndexService : IRandomIndexService
    {
        public int Random(int last) => new Random().Next(0, last - 1);
    }
}