using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InsuranceService.Models
{
    public class ContactMapping
    {
        //Address
        [DisplayName("Adresse (N° de rue, nom de rue, Appartement,...etc) : ")]
        public string AddressLine { get; set; }
        [DisplayName("Code postal")]
        public string PostalCode { get; set; }
        [DisplayName("Ville ")]
        public string City { get; set; }
        [DisplayName("Pays")]
        public string Country { get; set; }
        public string Laltitude { get; set; }
        public string Longitude { get; set; }
        //Contact 
        [DisplayName("Numéro de téléphone maison")]
        public string Home_Phone { get; set; }
        [DisplayName("Numéro de téléphone mobile")]
        public string Mobile_Phone { get; set; }
        [EmailAddress]
        [DisplayName("Adresse Email")]
        public string Email_Address { get; set; }
        [DisplayName("Choisir le moyen de vous communiquer")]
        public string Contact_Preferences { get; set; }
    }
}