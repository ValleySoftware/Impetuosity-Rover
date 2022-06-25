using Impetuosity_Rover.ViewModels.Primary;
using System;
using System.Collections.Generic;
using System.Text;
using static Impetuosity_Rover.Enumerations.Enumerations;

namespace Impetuosity_Rover.ViewModels.Misc
{
    public class LightsViewModel : ValleyBaseViewModel
    {
        public LightsViewModel(string name) : base(name)
        {
            mainViewModel.MasterStatus.LightsStatus = ComponentStatus.Uninitialised;

        }

        public void Init()
        {
            mainViewModel.MasterStatus.LightsStatus = ComponentStatus.Initialising;

            IsReady = true;
            mainViewModel.MasterStatus.LightsStatus = ComponentStatus.Ready;
        }
    }
}
