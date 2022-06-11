using System;
using System.Collections.Generic;
using System.Text;

namespace Impetuosity_Rover.Enumerations
{
    public static class Enumerations
    {
        //public enum ErrorCodes { UnknownError, ParseError, NoMessageBodyError, IncorrectParametersError  }

        public static int valleyMapleError_UnknownError = 100;
        public static int valleyMapleError_ParseError = 101;
        public static int valleyMapleError_NoMessageBodyError = 102;
        public static int valleyMapleError_IncorrectParametersError = 103;
        public static int valleyMapleError_ActionError = 104;

        public enum MessageStatus 
        { 
            error, 
            pendingData, 
            pendingSend, 
            sent, 
            receivedPendingAction, 
            completedPendingConfirmation, 
            confirmed}
        //public enum MessageType { movement };

        public enum SteeringRequestType
        {
            AdjustBy,
            SetTo,
            Centre
        }

        public enum TestMethodology
        {
            none,
            simple,
            thorough,
            iAmDeathIncarnate
        }

        public enum ErrorLoggingThreshold
        {
            none,
            exception,
            important,
            debug
        }
    }
}
