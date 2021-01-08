using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMat.Core.Models
{
    public class FirebaseUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Year { get; set; }
        public string PicturePath { get; set; }
        public string PhoneNumber { get; set; }
    }
}
