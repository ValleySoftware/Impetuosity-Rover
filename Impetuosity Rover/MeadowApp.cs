using Impetuosity_Rover.ViewModels;
using Meadow;
using Meadow.Devices;
using System;

namespace Impetuosity_Rover
{
    // Change F7MicroV2 to F7Micro for V1.x boards
    public class MeadowApp : App<F7MicroV2, MeadowApp>
    {
        public MainViewModel mainViewModel;

        public MeadowApp()
        {
            if (Init())
            {

            }   
        }

        private bool Init()
        {
            bool result = false;

            try
            {
                mainViewModel = new MainViewModel();
                mainViewModel.Init();

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Console.WriteLine(ex.Data);
            }

            return result;
        }

        
    }
}
