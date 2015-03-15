using System;

namespace Igniter.Core
{
    interface IWatcher : IDisposable
    {
        event EventHandler Changed;

        void SubscribeToCurrentNotifier();
    }
}