using Impetuosity_Rover.Models;
using Impetuosity_Rover.ViewModels.Comms;
using Impetuosity_Rover.ViewModels.Misc;
using Impetuosity_Rover.ViewModels.Movement;
using Meadow.Foundation;
using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Leds;
using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Impetuous.Enumerations.Enumerations;

namespace Impetuosity_Rover.ViewModels.Primary
{
    public class MainViewModel : ValleyBaseViewModel
    { 

        private readonly TestMethodology startupTestingMethod = TestMethodology.none;

        public MovementViewModel Movement;
        private CommunicationsViewModel comms;

        public DisplayViewModel Display;
        private Pca9685 pca9685;
        private II2cBus i2CBus;

        public readonly int i2cFrequency = 60;

        public List<MessageBaseModel> messages;

        private LightsViewModel _lights;
        private SensorsViewModel _sensors;

        private StatusViewModel _masterStatus;


        public MainViewModel(string name) : base(name)
        {
            Console.WriteLine("mainViewModel constructor started");

            _masterStatus = new StatusViewModel("Master Status View Model");
            _masterStatus.Init();
        }

        public void Init()
        {
            Console.WriteLine("mainViewModel Init started");

            List<Task> startupTasks = new List<Task>();

            MasterStatus.CoreComponentsStatus = ComponentStatus.Initialising;

            try
            {
                //Create all the viewmodels, but not initialise yet.                

                messages = new List<MessageBaseModel>();

                Display = new DisplayViewModel("Main Display");
                Movement = new MovementViewModel("MovementViewModel");
                comms = new CommunicationsViewModel("comms");
                _sensors = new SensorsViewModel("Sensors Controller");
                _lights = new LightsViewModel("Lights Controller");

                MasterStatus.ShowDebugMessage(this, "Initialize I2C");
                i2CBus = _device.CreateI2cBus(I2cBusSpeed.Standard);

                Display.Init(ref i2CBus);

                MasterStatus.ShowDebugMessage(this, "Create PCA9685");
                pca9685 = new Pca9685(i2CBus, 0x40, i2cFrequency);
                MasterStatus.ShowDebugMessage(this, "Initialize PCA9685");
                pca9685.Initialize();
                
                Task commsStartupTask = new Task(async () =>
                {
                    MasterStatus.ShowDebugMessage(this, "Init Comms");

                    await comms.Init();
                });

                startupTasks.Add(commsStartupTask);
                
                Task lightsStartupTask = new Task(() =>
                {
                    _lights.Init();
                });
                startupTasks.Add(lightsStartupTask);

                Task sensorsStartupTask = new Task(() =>
                {
                    MasterStatus.ShowDebugMessage(this, "Init Sensors");

                    _sensors.Init(ref i2CBus);
                });
                startupTasks.Add(sensorsStartupTask);
                


                foreach (var element in startupTasks)
                {
                    element.Start();
                }

                Task.WaitAll(startupTasks.ToArray());


                Task movementStartupTask = new Task(() =>
                {
                    MasterStatus.ShowDebugMessage(this, "Initialize Master Movement Controller");
                    Movement.Init(ref pca9685, startupTestingMethod);
                });

                movementStartupTask.Start();
                MasterStatus.CoreComponentsStatus = ComponentStatus.Ready;
                MasterStatus.RefreshGlobalStatus("MainViewModel init complete");

            }
            catch (Exception ex)
            {
                MasterStatus.ShowDebugMessage(
                    this,
                    "Initialize error: " + ex.Message,
                    ErrorLoggingThreshold.exception);

                MasterStatus.RefreshGlobalStatus("MainViewModel init error");
            }
        }

        public StatusViewModel MasterStatus
        {
            get => _masterStatus;
        }
    }
}
