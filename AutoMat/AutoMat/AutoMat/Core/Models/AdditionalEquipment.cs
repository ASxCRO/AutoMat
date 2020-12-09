using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMat.Core.Models
{
    [System.Serializable]
    public class AdditionalEquipment
    {
        public List<string> AirConditioner { get; set; }
        public List<string> AirBags { get; set; }
        public List<string> General { get; set; }
        public List<string> Radio { get; set; }
        public List<string> Safety { get; set; }
        public List<string> AntiSteal { get; set; }
        public List<string> Comfort { get; set; }
    }
}
