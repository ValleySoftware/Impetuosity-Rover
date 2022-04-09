using System;
using System.Collections.Generic;
using System.Text;
using Meadow.Foundation.Sensors.Buttons;

namespace Impetuosity_Rover.ViewModels
{
    public class OnboardButonControlsViewModel : ValleyBaseViewModel
    {
        PushButton testDriveMotorsButton;
        PushButton testBogiesButton;
        PushButton stopDriveMotorsButton;

        public OnboardButonControlsViewModel(string name) : base(name)
        {

        }

        public void Init()
        {
            testDriveMotorsButton = new PushButton(_device, _device.Pins.D00, Meadow.Hardware.ResistorMode.InternalPullUp);
            testDriveMotorsButton.Clicked += TestDriveMotorsButton_Clicked;

            stopDriveMotorsButton = new PushButton(_device, _device.Pins.D01, Meadow.Hardware.ResistorMode.InternalPullUp);
            stopDriveMotorsButton.Clicked += StopDriveMotorsButton_Clicked;

            testBogiesButton = new PushButton(_device, _device.Pins.D02, Meadow.Hardware.ResistorMode.InternalPullUp);
            testBogiesButton.Clicked += TestBogiesButton_Clicked;
        }

        private void TestDriveMotorsButton_Clicked(object sender, EventArgs e)
        {
            _appRoot.mainViewModel.Movement.TestPower();
        }

        private void StopDriveMotorsButton_Clicked(object sender, EventArgs e)
        {
            _appRoot.mainViewModel.Movement.Stop();
        }

        private void TestBogiesButton_Clicked(object sender, EventArgs e)
        {
            _appRoot.mainViewModel.Movement.TestBogies(false);
        }
    }
}
