using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Layton.Common.Controls
{
    public class ControlFlasher
    {
        // Properties
        public Control ControlToFlash;
        public int NumberOfFlashes;
        public Color FlashColor;
        public string Msg;
        public int FlashCycleDuration;

        // Internal Data
        private Timer _tmrFlasher;

        // State internal data - needs to be reset every time the flasher is started
        int _numberOfFlashesFinished;
        Color _initialControlBackColor;
        string _strInitialControlText;

        // Factory
        public static ControlFlasher Start(Control controlToFlash, int numberOfFlashes, int flashCycleDuration, Color flashColor, string strMessage)
        {
            ControlFlasher flasher = new ControlFlasher(controlToFlash, numberOfFlashes, flashCycleDuration, flashColor, strMessage);
            flasher.Start();
            return flasher;
        }

        // Constructor
        public ControlFlasher(Control controlToFlash, int numberOfFlashes, int flashCycleDuration, Color flashColor, string strMessage)
        {
            // Initialize the flasher
            _tmrFlasher = new Timer();
            _tmrFlasher.Tick += new EventHandler(tmrFlasher_Tick);
            _tmrFlasher.Interval = 10;

            // Initialize the flasher data
            ControlToFlash = controlToFlash;
            NumberOfFlashes = numberOfFlashes;
            FlashCycleDuration = flashCycleDuration;
            FlashColor = flashColor;
            Msg = strMessage;
        }
        
        public void Start()
        {
            // reset the flasher
            _numberOfFlashesFinished = 0;
            _initialControlBackColor = ControlToFlash.BackColor;
            _strInitialControlText = ControlToFlash.Text;

            // Start the flash timer
            _tmrFlasher.Start();
        }

        void tmrFlasher_Tick(object sender, EventArgs e)
        {
            // Set the timer interval (timer fine tuning)
            _tmrFlasher.Interval = FlashCycleDuration;

            // Check if we are finished
            if (++_numberOfFlashesFinished > NumberOfFlashes)
            {
                // Our work is finished
                _tmrFlasher.Stop();

                // Set the control back color back to the way it was before
                ControlToFlash.BackColor = _initialControlBackColor;
                ControlToFlash.Text = _strInitialControlText;

                // No more flashing
                return;
            }

            Flash();
        }

        void Flash()
        {
            // Flash! Alternate the control back color
            ControlToFlash.BackColor = ((ControlToFlash.BackColor == _initialControlBackColor) ? FlashColor : _initialControlBackColor);
            ControlToFlash.Text = Msg;
        }
    }
}
