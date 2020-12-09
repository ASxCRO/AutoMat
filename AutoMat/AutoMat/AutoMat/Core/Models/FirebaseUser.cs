using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMat.Core.Models
{
    public class FirebaseUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Prezime { get; set; }
        public string Ime { get; set; }
    }
}
