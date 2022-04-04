using Meadow.Foundation;
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

        MovementViewModel Movement;

        private Pca9685 pca9685;
        private II2cBus i2CBus;

        public readonly int PWMFrequency = 60;

        public MainViewModel() 
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
                pca9685 = new Pca9685(i2CBus, 0x40, PWMFrequency);
                ShowDebugMessage("Initialize PCA9685");
                pca9685.Initialize();

                ShowDebugMessage("Initialize Master Movement Controller");
                Movement = new MovementViewModel();
                Movement.Init(pca9685);

                onboardLed.SetColor(Color.Green);

                var t = new Task(() =>
                {
                    Movement.Test();
                });

                t.Start();

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
