using System;
using System.Collections.Generic;
using System.Text;

namespace Impetuous.Models
{
    public class LightMessageModel : MessageBaseModel
    {
        public Enumerations.Enumerations.LightSelect Light { get; set; }
        public bool NewState { get; set; }

    }
}
