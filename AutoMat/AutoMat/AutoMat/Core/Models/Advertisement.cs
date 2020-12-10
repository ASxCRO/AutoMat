using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AutoMat.Core.Models
{
    public class Advertisement
    {
        public Advertisement()
        {
            PayTypes = new List<string>();
             AntiSteal = new List<string>();
             PicturesURL = new List<string>();
             AdditionalEquipment = new List<string>();
             SafetyEquipment = new List<string>();
             ACEquipment = new List<string>();
             RadioEquipment = new List<string>();
             ComfortEquipment = new List<string>();
        }

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

        //new
        public string AverageFuel { get; set; }
        //new
        public string Drive { get; set; }
        //new
        public string Doors { get; set; }
        //new
        public string Body { get; set; }

        //new
        public string Suspension { get; set; }
        //new
        public string Color { get; set; }
        //new
        public string Price { get; set; }
        //new
        public string Availability { get; set; }

        //new
        public string PhoneNumber { get; set; }

        //new 
        public string AirBags { get; set; }
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
        public List<string> SafetyEquipment { get; set; }
        public List<string> ACEquipment { get; set; }
        public List<string> RadioEquipment { get; set; }
        public List<string> ComfortEquipment { get; set; }

        //delete
        public Dictionary<string,string> AdditionalInformations { get; set; }
    }
}
