using System;
using System.Collections.Generic;
using System.Text;

namespace Impetuosity_Rover.ViewModels
{
    public class DrivePowerViewModel : ValleyBaseViewModel
    {
        public DrivePowerViewModel() : base()
        {

        }

        public bool Init()
        {
            bool result = false;

            try
            {

                result = true;
            }
            catch (Exception ex)
            {
                ShowDebugMessage("Error: " + ex.Message, true);
                result = false;
            }

            return result;
        }

        
    }
}
