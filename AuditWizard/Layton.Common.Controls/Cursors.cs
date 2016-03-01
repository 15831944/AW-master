using System;
using System.Windows.Forms;

namespace Layton.Common.Controls
{
    public class CurrentCursor : IDisposable
    {
        private Cursor saved = null;

        public CurrentCursor(Cursor newCursor)
        {
            saved = Cursor.Current;

            Cursor.Current = newCursor;
        }

        public void Dispose()
        {
            Cursor.Current = saved;
        }
    }

    public class WaitCursor : CurrentCursor
    {
        public WaitCursor() : base(Cursors.WaitCursor) { }
    }
}
