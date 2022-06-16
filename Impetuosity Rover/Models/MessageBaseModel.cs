using System;
using System.Collections.Generic;
using System.Text;
using static Impetuosity_Rover.Enumerations.Enumerations;

namespace Impetuosity_Rover.Models
{
    public class MessageBaseModel
    {       
        public DateTimeOffset? RequestSentStamp { get; set; }
        public DateTimeOffset? RequestReceivedStamp { get; set; }
        public DateTimeOffset? RequestPerformedStamp { get; set; }
        public DateTimeOffset? RequestConfirmedStamp { get; set; }
        public string MessageID { get; set; }
        public MessageStatus RequestStatus { get; set; }
        //public MessageType RequestType { get; set; }
    }
}
