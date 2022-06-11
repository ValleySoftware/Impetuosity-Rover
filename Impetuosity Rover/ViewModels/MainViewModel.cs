using Impetuosity_Rover.Models;
using Meadow.Foundation;
using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Leds;
using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Impetuosity_Rover.Enumerations.Enumerations;

namespace Impetuosity_Rover.ViewModels
{
    public class MainViewModel : ValleyBaseViewModel
    {

        public RgbPwmLed onboardLed;

        private Timer debugLogTimer;
        private bool debugQueueScanActive = false;

        private readonly TestMethodology startupTestingMethod = TestMethodology.simple;

        private List<string> debugQueue = new List<string>();

        public MovementViewModel Movement;
        //private OnboardButonControlsViewModel Buttons;
        private CommunicationsViewModel comms;

        private Pca9685 pca9685;
        private II2cBus i2CBus;

        public readonly int i2cFrequency = 60;

        public List<MessageBaseModel> messages;

        public MainViewModel(string name) : base(name)
        {
            ShowDebugMessage(this, "Initialize LED for debugging.");

            if (onboardLed == null)
            {
                onboardLed = new RgbPwmLed(
                    device: _device,
                    redPwmPin: _device.Pins.OnboardLedRed,
                    greenPwmPin: _device.Pins.OnboardLedGreen,
                    bluePwmPin: _device.Pins.OnboardLedBlue,
                    new Voltage(3.3f, Voltage.UnitType.Volts),
                    new Voltage(3.3f, Voltage.UnitType.Volts),
                    new Voltage(3.3f, Voltage.UnitType.Volts),
                    Meadow.Peripherals.Leds.IRgbLed.CommonType.CommonAnode);
            }

            AutoResetEvent autoResetEvent = new AutoResetEvent(true);
            debugLogTimer = new Timer(
                new TimerCallback(ScanDebugQueueForNewMessages), 
                autoResetEvent,
                TimeSpan.FromSeconds(0), 
                TimeSpan.FromSeconds(1));
            
        }

        public bool Init()
        {
            try
            {
                onboardLed.SetColor(Color.Blue);

                messages = new List<MessageBaseModel>();

                ShowDebugMessage(this, "Initialize I2C");

                i2CBus = _device.CreateI2cBus(I2cBusSpeed.Standard);

                ShowDebugMessage(this, "Create PCA9685");
                pca9685 = new Pca9685(i2CBus, 0x40, i2cFrequency);
               
                ShowDebugMessage(this, "Initialize PCA9685");
                pca9685.Initialize();

                ShowDebugMessage(this, "Initialize Master Movement Controller");
                Movement = new MovementViewModel("MovementViewModel");
                Movement.Init(ref pca9685, startupTestingMethod);
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

                try
                {
                    comms = new CommunicationsViewModel("comms");
                    comms.Init();
                }
                catch (Exception ex) 
                { 
                    ShowDebugMessage(this, 
                        ex.ToString(), 
                        ErrorLoggingThreshold.exception);
                    onboardLed.SetColor(Color.Red);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowDebugMessage(
                    this, 
                    "Initialize error: " + ex.Message, 
                    ErrorLoggingThreshold.exception);
                onboardLed.SetColor(Color.Red);
                return false;
            }

        }

        public void ShowDebugMessage(ValleyBaseViewModel sender, string messageToShow, ErrorLoggingThreshold messageCategory = ErrorLoggingThreshold.debug)
        {
            if (messageCategory <= debugThreshhold)
            {
                debugQueue.Add(sender.Name + " - " + messageToShow + " - " + DateTimeOffset.Now.TimeOfDay);
            }
        }

        public void ScanDebugQueueForNewMessages(object state)
        {
            if (debugQueueScanActive)
            {
                return;
            }

            debugQueueScanActive = true;

            try
            {

                int lastIndex = debugQueue.Count - 1;
                while (lastIndex >= 0)
                {
                    Console.WriteLine(debugQueue[lastIndex]);
                    debugQueue.RemoveAt(lastIndex);
                    lastIndex--;
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                debugQueueScanActive = false;
            }
        }
    }
}
