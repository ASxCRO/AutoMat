using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMat.Core.Models
{
    public class Advertisement
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public string CO2 { get; set; }
        public string EcoNorm { get; set; }
        public string ProductionYear { get; set; }
        public string ModelYear { get; set; }
        public string KM { get; set; }
        public string County { get; set; }
        public string Town { get; set; }
        public string MotorVolume { get; set; }
        public string OnRoadSince { get; set; }
        public string Available { get; set; }
        public string ChassisNumber { get; set; }
        public string Shifts { get; set; }
        public string ShiftType { get; set; }
        public string Power { get; set; }
        public string Type { get; set; }
        public string MotorType { get; set; }
        public string Guarantee { get; set; }
        public string ServiceBook { get; set; }
        public string Owner { get; set; }
        public List<string> PayTypes { get; set; }
        public List<string> AntiSteal { get; set; }
        public List<string> PicturesURL { get; set; }
        public List<string> AdditionalEquipment { get; set; }
        public List<string> ComfortEquipment { get; set; }
        public Dictionary<string,string> AdditionalInformations { get; set; }
    }
}
