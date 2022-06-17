using System;
using System.Collections.Generic;
using System.Text;
using Impetuosity_Rover.ViewModels.Primary;
using Meadow.Foundation.Displays.Ssd130x;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;

namespace Impetuosity_Rover.ViewModels.Misc
{
    public class DisplayViewModel : ValleyBaseViewModel
    {
        Ssd1306 display;
        public MicroGraphics graphics;

        public DisplayViewModel(string name) : base(name)
        {

        }

        public void Init(II2cBus i2CBus)
        {
            display = new Ssd1306(i2CBus,
                address: 60,
                displayType: Ssd130xBase.DisplayType.OLED128x32);

            graphics = new MicroGraphics(display);
            graphics.Clear();
            graphics.Show();

        }

        public void ShowMessage(List<string> lines)
        {
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
