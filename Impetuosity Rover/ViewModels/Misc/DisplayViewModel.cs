using System;
using System.Collections.Generic;
using System.Text;
using Impetuosity_Rover.ViewModels.Primary;
using Meadow.Foundation.Displays.Ssd130x;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;
using static Impetuosity_Rover.Enumerations.Enumerations;

namespace Impetuosity_Rover.ViewModels.Misc
{
    public class DisplayViewModel : ValleyBaseViewModel
    {
        Ssd1306 display;
        public MicroGraphics graphics;


        public DisplayViewModel(string name) : base(name)
        {
            mainViewModel.MasterStatus.DisplayStatus = ComponentStatus.Uninitialised;
        }

        public void Init(II2cBus i2CBus)
        {
            mainViewModel.MasterStatus.DisplayStatus = ComponentStatus.Initialising;
            display = new Ssd1306(i2CBus,
                address: 60,
                displayType: Ssd130xBase.DisplayType.OLED128x32);

            graphics = new MicroGraphics(display);
            graphics.Clear();
            graphics.Show();

            IsReady = true;
            mainViewModel.MasterStatus.DisplayStatus = ComponentStatus.Ready;
        }

        public void ShowMessage(List<string> lines)
        {
            if (!IsReady)
            {
                return;
            }

            graphics.Clear();
            int i = 0;

            foreach (var element in lines)
            {
                graphics.CurrentFont = new Font8x12();
                graphics.DrawText(0, 12 * i, element.Trim(), ScaleFactor.X1);
                i++;
            }

            graphics.Show();
        }
    }
}
