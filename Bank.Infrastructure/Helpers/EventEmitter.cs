using System;

namespace Bank.Infrastructure.Helpers
{
    public class EventEmitter
    {
        private event EventHandler _handlers;

        public virtual void On(EventHandler handler)
        {
            _handlers += handler;
        }

        public void Emit()
        {
            _handlers?.Invoke(this, EventArgs.Empty);
        }

        public void Off()
        {
            _handlers = null;
        }
    }
}