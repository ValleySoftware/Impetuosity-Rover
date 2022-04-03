using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Servos;
using Meadow.Hardware;
using System;
using System.Threading;

namespace Impetuosity_Rover
{
    // Change F7MicroV2 to F7Micro for V1.x boards
    public class MeadowApp : App<F7MicroV2, MeadowApp>
    {
        RgbPwmLed onboardLed;

        private Pca9685 pca9685;
        private II2cBus i2CBus;

        IPwmPort LeftFrontSteeringPort;
        IPwmPort LeftRearSteeringPort;
        IPwmPort RightFrontSteeringPort;
        IPwmPort RightRearSteeringPort;


        Servo LeftFrontServo;
        Servo LeftRearServo;
        Servo RightFrontServo;
        Servo RightRearServo;

        public readonly int PWMFrequency = 60;

        public MeadowApp()
        {
            if (Initialize())
            {
                Test();
            }   
        }

        private bool Initialize()
        {
            try
            {
                Console.WriteLine("Initialize LED for debugging.");
                onboardLed = new RgbPwmLed(device: Device,
                    redPwmPin: Device.Pins.OnboardLedRed,
                    greenPwmPin: Device.Pins.OnboardLedGreen,
                    bluePwmPin: Device.Pins.OnboardLedBlue,
                    3.3f, 3.3f, 3.3f,
                    Meadow.Peripherals.Leds.IRgbLed.CommonType.CommonAnode);

                onboardLed.SetColor(Color.Blue);

                Console.WriteLine("Initialize I2C");

                i2CBus = Device.CreateI2cBus(I2cBusSpeed.Standard);

                Console.WriteLine("Initialize PCA9685");
                pca9685 = new Pca9685(i2CBus, 0x40, PWMFrequency);
                pca9685.Initialize();

                Console.WriteLine("Initialize PWM Ports for Servos");
                LeftFrontSteeringPort =  pca9685.CreatePwmPort(Convert.ToByte(15));
                LeftRearSteeringPort = pca9685.CreatePwmPort(Convert.ToByte(14));
                RightFrontSteeringPort = pca9685.CreatePwmPort(Convert.ToByte(0));
                RightRearSteeringPort = pca9685.CreatePwmPort(Convert.ToByte(1));

                Console.WriteLine("Initialize Servos");

                ServoConfig SG51 = new ServoConfig
                (
                    new Meadow.Units.Angle(0, Meadow.Units.Angle.UnitType.Degrees), 
                    new Meadow.Units.Angle(180, Meadow.Units.Angle.UnitType.Degrees), 
                    1000, 
                    2750, 
                    60
                ); //Some experimenting done here to get rotation kinda close...
                
                LeftFrontServo = new Servo(LeftFrontSteeringPort, SG51);
                LeftRearServo = new Servo(LeftRearSteeringPort, SG51);
                RightFrontServo = new Servo(RightFrontSteeringPort, SG51);
                RightRearServo = new Servo(RightRearSteeringPort, SG51);

                onboardLed.SetColor(Color.Green);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Initialize error: " + ex.Message);
                onboardLed.SetColor(Color.Red);
                return false;
            }
        }

        void Test()
        {
            bool success = true;
            Console.WriteLine("Dubstep, baby. ");
            while (success)
            {
                try
                {
                    int outer = 5;

                    while (outer > 0)
                    {
                        GoToEngle(new Meadow.Units.Angle(0, Meadow.Units.Angle.UnitType.Degrees));
                        GoToEngle(new Meadow.Units.Angle(90, Meadow.Units.Angle.UnitType.Degrees));
                        GoToEngle(new Meadow.Units.Angle(0, Meadow.Units.Angle.UnitType.Degrees));
                        GoToEngle(new Meadow.Units.Angle(90, Meadow.Units.Angle.UnitType.Degrees));

                        GoToEngle(new Meadow.Units.Angle(180, Meadow.Units.Angle.UnitType.Degrees));
                        GoToEngle(new Meadow.Units.Angle(90, Meadow.Units.Angle.UnitType.Degrees));
                        GoToEngle(new Meadow.Units.Angle(0, Meadow.Units.Angle.UnitType.Degrees));
                        GoToEngle(new Meadow.Units.Angle(180, Meadow.Units.Angle.UnitType.Degrees));

                        outer--;
                    }

                    int outerwiggle = 4;
                    while (outerwiggle >= 0)
                    {
                        int wiggle = 10;
                        while (wiggle > 0)
                        {
                            GoToEngle(new Meadow.Units.Angle(70, Meadow.Units.Angle.UnitType.Degrees), TimeSpan.FromTicks(250));
                            GoToEngle(new Meadow.Units.Angle(110, Meadow.Units.Angle.UnitType.Degrees), TimeSpan.FromTicks(250));
                            wiggle--;
                        }

                        Thread.Sleep(1000);
                        outerwiggle--;
                    }

                    
                }
                catch (Exception ex)
                {
                    success = false;
                    Console.WriteLine("Initialize error: " + ex.Message);
                    onboardLed.SetColor(Color.Red);
                }
            }
        }


        private void GoToEngle(Meadow.Units.Angle desiredAngle)
        {
            GoToEngle(desiredAngle, TimeSpan.FromMilliseconds(500));
        }

        private void GoToEngle(Meadow.Units.Angle desiredAngle, TimeSpan PauseAfterMovement)
        { 

            onboardLed.SetColor(Color.Yellow);
            Console.WriteLine("Go to " + desiredAngle.Degrees + " degrees");

            LeftFrontServo.RotateTo(desiredAngle);
            LeftRearServo.RotateTo(desiredAngle);
            RightFrontServo.RotateTo(desiredAngle);
            RightRearServo.RotateTo(desiredAngle);

            onboardLed.SetColor(Color.Blue);
            Console.WriteLine("Sleep " + PauseAfterMovement.Ticks + " ticks");
            Thread.Sleep(PauseAfterMovement);

        }
    }
}
