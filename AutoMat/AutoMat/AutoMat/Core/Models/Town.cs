using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMat.Core.Models
{
    public class Town
    {
        public string Id { get; set; }
        public string CountyId { get; set; }
        public string CountyName { get; set; }
        public string EntityType { get; set; }
        public string Name { get; set; }
    }
}
