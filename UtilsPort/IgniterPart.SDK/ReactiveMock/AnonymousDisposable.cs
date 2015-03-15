using System;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace IgniterPart.SDK.ReactiveMock
{
    internal sealed class AnonymousDisposable : IDisposable
    {
        private volatile Action _dispose;

        /// <summary>
        /// Gets a value that indicates whether the object is disposed.
        /// 
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return this._dispose == null;
            }
        }

        /// <summary>
        /// Constructs a new disposable with the given action used for disposal.
        /// 
        /// </summary>
        /// <param name="dispose">Disposal action which will be run upon calling Dispose.</param>
        public AnonymousDisposable(Action dispose)
        {
            this._dispose = dispose;
        }

        /// <summary>
        /// Creates a disposable object that invokes the specified action when disposed.
        /// 
        /// </summary>
        /// <param name="dispose">Action to run during the first call to <see cref="M:System.IDisposable.Dispose"/>. The action is guaranteed to be run at most once.</param>
        /// <returns>
        /// The disposable object that runs the given action upon disposal.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="dispose"/> is null.</exception>
        public static IDisposable Create(Action dispose)
        {
            if (dispose == null)
                throw new ArgumentNullException("dispose");
            return (IDisposable)new AnonymousDisposable(dispose);
        }        

        /// <summary>
        /// Calls the disposal action if and only if the current instance hasn't been disposed yet.
        /// 
        /// </summary>
        public void Dispose()
        {
            Action action = Interlocked.Exchange<Action>(ref this._dispose, (Action)null);
            if (action == null)
                return;
            action();
        }
    }
}
