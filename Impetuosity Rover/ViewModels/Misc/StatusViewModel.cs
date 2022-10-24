using Impetuosity_Rover.ViewModels.Primary;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static Impetuous.Enumerations.Enumerations;

namespace Impetuosity_Rover.ViewModels.Misc
{
    public class DebugLogObject
    {
        public DebugLogObject(string message, string senderName, DateTimeOffset stamp)
        {
            Message = message;
            SenderName = senderName;
            Stamp = stamp;
        }

        public string Message { get; set; }
        public string SenderName { get; set; }
        public DateTimeOffset Stamp { get; set; }
    }

    public class StatusViewModel : ValleyBaseViewModel
    {    
        public RgbPwmLed onboardLed;

        private Timer debugLogTimer;
        private bool debugQueueScanActive = false;
        private List<DebugLogObject> debugQueue = new List<DebugLogObject>();

        private List<ComponentStatus> statusObjects;
        private ComponentStatus _commsStatus = ComponentStatus.Uninitialised;
        private ComponentStatus _movementStatus = ComponentStatus.Uninitialised;
        private ComponentStatus _lightsStatus = ComponentStatus.Uninitialised;
        private ComponentStatus _displayStatus = ComponentStatus.Uninitialised;
        private ComponentStatus _sensorsStatus = ComponentStatus.Uninitialised;
        private ComponentStatus _coreComponentsStatus = ComponentStatus.Uninitialised;//i2c channel, pca, etc.
        private ComponentStatus _globalStatus = ComponentStatus.Uninitialised;

        public StatusViewModel(string name) : base(name)
        {
            statusObjects = new List<ComponentStatus>();
            statusObjects.Add(_commsStatus);
            statusObjects.Add(_movementStatus);
            statusObjects.Add(_lightsStatus);
            statusObjects.Add(_displayStatus);
            statusObjects.Add(_sensorsStatus);
            statusObjects.Add(_coreComponentsStatus);
        }

        public void Init()
        {
            try
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

                ShowDebugMessage(this, "LED ready.");
                
                AutoResetEvent autoResetEvent = new AutoResetEvent(true);
                debugLogTimer = new Timer(
                    new TimerCallback(ScanDebugQueueForNewMessages),
                    autoResetEvent,
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(1));

                ShowDebugMessage(this, "debug log timer ready.");

                onboardLed.SetColor(Color.Blue);
            }
            catch (Exception e)
            {
                onboardLed.SetColor(Color.Red);
                Console.WriteLine("Status Init error: " + e.Message);
            }
        }

        public void RefreshGlobalStatus(string senderMessage)
        {

            _globalStatus = ComponentStatus.Active;

            if (!string.IsNullOrEmpty(senderMessage))
            {
                ShowDebugMessage(this, "statusChange : " + senderMessage, ErrorLoggingThreshold.important);
            }

            foreach (var element in statusObjects)
            {
                if (element > _globalStatus )
                {
                    GlobalStatus = element;
                }
            }

            if (GlobalStatus == ComponentStatus.Ready)
            {
                onboardLed.SetColor(Color.Green);
            }
            else
            {
                if (GlobalStatus >= ComponentStatus.Warning)
                {
                    onboardLed.SetColor(Color.Red);
                }
                else
                {
                    onboardLed.SetColor(Color.Blue);
                }
            }

            //ShowDebugMessage(this, "commStatus: " +_commsStatus.ToString(), ErrorLoggingThreshold.important);
            //ShowDebugMessage(this, "movementStatus: " + _movementStatus.ToString(), ErrorLoggingThreshold.important);
            //ShowDebugMessage(this, "lightsStatus: " + _lightsStatus.ToString(), ErrorLoggingThreshold.important);
            //ShowDebugMessage(this, "displayStatus: " + _displayStatus.ToString(), ErrorLoggingThreshold.important);
            //ShowDebugMessage(this, "sensorsStatus: " + _sensorsStatus.ToString(), ErrorLoggingThreshold.important);
            //ShowDebugMessage(this, "coreStatus: " + _coreComponentsStatus.ToString(), ErrorLoggingThreshold.important);
        }

        public ComponentStatus CommsStatus { get => _commsStatus; set { _commsStatus = value; RefreshGlobalStatus("comms " + value); } }
        public ComponentStatus MovementStatus { get => _movementStatus; set { _movementStatus = value; RefreshGlobalStatus("movement " + value); } }
        public ComponentStatus LightsStatus { get => _lightsStatus; set { _lightsStatus = value; RefreshGlobalStatus("lights " + value); } }
        public ComponentStatus DisplayStatus { get => _displayStatus; set { _displayStatus = value; RefreshGlobalStatus("display " + value); } }
        public ComponentStatus SensorsStatus { get => _sensorsStatus; set { _sensorsStatus = value; RefreshGlobalStatus("sensor " + value); } }
        public ComponentStatus CoreComponentsStatus { get => _coreComponentsStatus; set { _coreComponentsStatus = value; RefreshGlobalStatus("core " + value); } }//i2c channel, pca, etc.
        public ComponentStatus GlobalStatus { get => _globalStatus; set => _globalStatus = value; }
        

        public void ShowDebugMessage(ValleyBaseViewModel sender, string messageToShow, ErrorLoggingThreshold messageCategory = ErrorLoggingThreshold.debug)
        {
            if (messageCategory <= debugThreshhold)
            {
                Console.WriteLine("SDM: " + messageToShow);

                debugQueue.Add(new DebugLogObject(messageToShow, sender.Name, DateTimeOffset.Now));
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

                if (debugQueue.Count > 0)
                {
                    var transientArray = new DebugLogObject[debugQueue.Count];
                    debugQueue.CopyTo(transientArray);
                    debugQueue.Clear();
                    var transientList = transientArray.OrderBy(o => o.Stamp);
                        
                    DebugLogObject lastObjectInList = null;

                    foreach (var element in transientList)
                    {
                        Console.WriteLine(element.Message);
                        lastObjectInList = element;
                    }

                    if (mainViewModel.Display != null &&
                        mainViewModel.Display.IsReady)
                    {
                        mainViewModel.Display.ShowMessage(
                            new List<string>() { lastObjectInList.Message });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("statusViewModelScanError: " + e.Message);
            }
            finally
            {                
                debugQueueScanActive = false;
            }
        }

    }
}
