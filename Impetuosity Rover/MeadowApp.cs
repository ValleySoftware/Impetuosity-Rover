using Impetuosity_Rover.ViewModels.Primary;
using Meadow;
using Meadow.Devices;
using System;

namespace Impetuosity_Rover
{
    // Change F7MicroV2 to F7Micro for V1.x boards
    public class MeadowApp : App<F7FeatherV2, MeadowApp>
    {
        public MainViewModel mainViewModel;

        public MeadowApp()
        {
            //Console.WriteLine("meadow app started");
            if (Init())
            {

            }   
        }

        private bool Init()
        {
            bool result = false;

            try
            {
                //Console.WriteLine("meadow app init started");

                mainViewModel = new MainViewModel("MainViewModel");
                //Console.WriteLine("mainViewModel constructor completed, about to start MainViewModel Init");
                mainViewModel.Init();

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Console.WriteLine("MeadowApp Init error: " + ex.Message);
            }

            return result;
        }

        
    }
}
