using System;
using System.Collections.Generic;
using System.Text;

namespace Impetuosity_Rover.ViewModels
{
    public class DrivePowerViewModel
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
                result = false;
            }

            return result;
        }

        
    }
}
