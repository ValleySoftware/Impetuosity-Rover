﻿using Meadow.Foundation;
using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Leds;
using Meadow.Hardware;
using System;
using System.Threading.Tasks;

namespace Impetuosity_Rover.ViewModels
{
    public class MainViewModel : ValleyBaseViewModel
    {
        public RgbPwmLed onboardLed;

        public MovementViewModel Movement;
        //private OnboardButonControlsViewModel Buttons;

        private Pca9685 pca9685;
        private II2cBus i2CBus;

        public readonly int i2cFrequency = 60;

        public MainViewModel(string name) : base(name)
        {
            ShowDebugMessage("Initialize LED for debugging.");

            if (onboardLed == null)
            {
                onboardLed = new RgbPwmLed(
                    device: _device,
                    redPwmPin: _device.Pins.OnboardLedRed,
                    greenPwmPin: _device.Pins.OnboardLedGreen,
                    bluePwmPin: _device.Pins.OnboardLedBlue,
                    3.3f, 3.3f, 3.3f,
                    Meadow.Peripherals.Leds.IRgbLed.CommonType.CommonAnode);
            }
        }

        public bool Init()
        {
            try
            {
                onboardLed.SetColor(Color.Blue);

                ShowDebugMessage("Initialize I2C");

                i2CBus = _device.CreateI2cBus(I2cBusSpeed.Standard);

                ShowDebugMessage("Create PCA9685");
                pca9685 = new Pca9685(i2CBus, 0x40, i2cFrequency);
               
                ShowDebugMessage("Initialize PCA9685");
                pca9685.Initialize();

                ShowDebugMessage("Initialize Master Movement Controller");
                Movement = new MovementViewModel("MovementViewModel");
                Movement.Init(pca9685);
/*
                try
                {
                    ShowDebugMessage("Initialize Onboard Buttons");
                    Buttons = new OnboardButonControlsViewModel("ButtonsViewModel");
                    Buttons.Init();
                    onboardLed.SetColor(Color.Green);
                }
                catch (Exception instantiateButtonEx)
                {
                    ShowDebugMessage("Initialize Buttons error: " + instantiateButtonEx.Message, true);
                    onboardLed.SetColor(Color.Coral);
                }
*/
                //Movement.TestBogies(false);


                return true;
            }
            catch (Exception ex)
            {
                ShowDebugMessage("Initialize error: " + ex.Message, true);
                onboardLed.SetColor(Color.Red);
                return false;
            }

        }
    }
}
