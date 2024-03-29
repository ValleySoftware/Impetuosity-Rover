﻿using System;
using System.Collections.Generic;
using System.Text;
using Impetuosity_Rover.ViewModels.Primary;
using Meadow.Foundation.Displays.Ssd130x;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;
using static Impetuous.Enumerations.Enumerations;

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

        public void Init(ref II2cBus i2CBus)
        {
            try
            {
                mainViewModel.MasterStatus.DisplayStatus = ComponentStatus.Initialising;
                display = new Ssd1306(i2CBus,
                    address: 60,
                    displayType: Ssd130xBase.DisplayType.OLED128x64);

                graphics = new MicroGraphics(display);
                ClearDisplay();

                IsReady = true;
                mainViewModel.MasterStatus.DisplayStatus = ComponentStatus.Ready;
                ShowMessage(new List<string>() { "Display Ready" });
            }
            catch (Exception e)
            {
                Console.WriteLine("display Init error: " + e.Message);

            }
        }

        public void ClearDisplay()
        {
            graphics.Clear();
            graphics.Show();
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
