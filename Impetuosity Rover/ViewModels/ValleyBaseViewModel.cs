using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Leds;
using System;
using System.Collections.Generic;
using System.Text;

namespace Impetuosity_Rover.ViewModels
{
    public class ValleyBaseViewModel
    {
        public bool enableDebugMessages = true;

        protected MeadowApp _appRoot => MeadowApp.Current;
        public F7MicroV2 _device => MeadowApp.Device;

        public ValleyBaseViewModel()
        {
        }

        public void ShowDebugMessage(string messageToShow, bool force = false)
        {
            if (enableDebugMessages || force)
            {
                Console.WriteLine(messageToShow);
            }
        }
    }
}
