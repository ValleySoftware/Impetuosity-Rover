﻿using System;
using System.Collections.Generic;
using System.Text;
using static Impetuous.Enumerations.Enumerations;

namespace Impetuous.Models
{
    public class MessageBaseModel
    {       
        public DateTimeOffset? RequestSentStamp { get; set; }
        public DateTimeOffset? RequestReceivedStamp { get; set; }
        public DateTimeOffset? RequestPerformedStamp { get; set; }
        public DateTimeOffset? RequestConfirmedStamp { get; set; }
        public string OriginalMessageString { get; set; }
        public string MessageID { get; set; }
        public MessageStatus RequestStatus { get; set; }
        //public MessageType RequestType { get; set; }
    }
}
