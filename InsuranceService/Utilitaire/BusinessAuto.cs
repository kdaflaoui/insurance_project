using InsuranceDataAccess;
using InsuranceService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InsuranceService.Utilitaire
{
    public class BusinessAuto
    {
        
        public static double CalculAutoSoumission(Autos auto)
        {
            double base_price = 15;

            if(Int32.Parse(auto.Year) > 2015)
            {
                base_price += 2;
            }
            else
            {
                base_price += 4; 
            }

            if (auto.KilometersPerYear > 75000)
            {
                base_price += 6;
            }
            else
            {
                base_price += 2;
            }

            if (auto.Infraction_Nbre <=2)
            {
                base_price += 2;
            }
            else if(2 < auto.Infraction_Nbre && auto.Infraction_Nbre < 5)
            {
                base_price += 4;
            }
            else
            {
                base_price += 5;
            }

            if (auto.Accident_Nbre <= 2)
            {
                base_price += 2;
            }
            else if (2 < auto.Accident_Nbre && auto.Accident_Nbre < 5)
            {
                base_price += 4;
            }
            else
            {
                base_price += 5;
            }
            return base_price;
        }

        // ------------------------------- Utilitaires ---------------------------
        public static Contacts ContactMapping(ContactMapping contactMapping)
        {
            Contacts contacts = new Contacts();
            contacts.Contact_Preferences = contactMapping.Contact_Preferences;
            contacts.Email_Address = contactMapping.Email_Address;
            contacts.Home_Phone = contactMapping.Home_Phone;
            contacts.Mobile_Phone = contactMapping.Mobile_Phone;
            return contacts;

        }

        public static Addresses AddressesMapping(ContactMapping contactMapping)
        {
            Addresses addresses = new Addresses();
            addresses.AddressLine = contactMapping.AddressLine;
            addresses.City = contactMapping.City;
            addresses.Country = contactMapping.Country;
            addresses.Laltitude = contactMapping.Laltitude;
            addresses.Longitude = contactMapping.Longitude;
            addresses.PostalCode = contactMapping.PostalCode;
            return addresses;
        }
    }
}