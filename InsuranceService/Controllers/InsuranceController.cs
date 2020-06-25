using InsuranceDataAccess;
using InsuranceService.Models;
using InsuranceService.Utilitaire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;

namespace InsuranceService.Controllers
{
    public class InsuranceController : Controller
    {


        // GET: Insurance
        public ActionResult Index()
        {
            List<string> list_sexe = new List<string>() { "Femme", "Homme" };
            List<string> list_civilite = new List<string>() { "Célibataire", "Marié(e)", "Divorcé(e)", "Veuf(ve)" };
            List<string> list_policies = new List<string>() {"Auto", "Habitation" };
            ViewBag.Sexe = list_sexe;
            ViewBag.Civilite = list_civilite;
            ViewBag.Policy = list_policies;
            return View();
        }

        public ActionResult Auto()
        {
            List<string> list_years = new List<string>() { "2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017", "2018", "2019", "2020" };
            List<string> list_marque = new List<string>() { "Hyndai" };
            List<string> list_model_hyndai = new List<string>() { "Elantra GL 4DR", "Elantra GL 5DR", "Elantra VE 4DR" };
            List<string> list_nbreVoiture = new List<string>() { "1", "2", "3", "4", "5+" };
            List<string> list_Loue_Finance = new List<string>() { "Louée", "Financée", "Sans objet" };
            List<string> list_Usage = new List<string>() { "Aller au travail", "Promener", "Affaire" };
            List<int> list_nbreInfraction = new List<int>() { 0, 1, 2, 3, 4, 5 };
            List<int> list_nbreAccident = new List<int>() { 0, 1, 2, 3, 4, 5 };
            List<string> list_type_accident = new List<string>() { "Résponsable", "Accident partiellement résponsable", "Accident non résponsable", "Sans objet" };
            ViewBag.Years = list_years;
            ViewBag.Marque = list_marque;
            ViewBag.Model_Hyndai = list_model_hyndai;
            ViewBag.NbreVoiture = list_nbreVoiture;
            ViewBag.Loue_Finance = list_Loue_Finance;
            ViewBag.Usage = list_Usage;
            ViewBag.Nre_Infraction = list_nbreInfraction;
            ViewBag.Nbre_Accident = list_nbreAccident;
            ViewBag.Accident_Type = list_type_accident;

            return View();
        }

        public ActionResult Property()
        {

            return View();
        }

        public ActionResult Contact()
        {
            List<string> list_contact = new List<string>() { "Téléphone maison", "Téléphone mobile", "Adresse éléctronique" };
            ViewBag.List_contact = list_contact;
            return View();
        }

        public ActionResult Confirme()
        {
            ViewBag.Price = TempData["Price"];
            return View();
        }

        // ------------------------------------------------------------------------------------------------

        public ActionResult GetCustomer(Customers customer)
        {
            Session["Customers"] = customer;

            if (customer.Type_Of_insurance.Equals("Auto"))
            {
                return RedirectToAction("Auto");
            }
            else
            {
                return RedirectToAction("Property");
            }
        }

        public ActionResult GetAuto(Autos auto)
        {
            Customers customers = (Customers) Session["Customers"];
            customers.Autos = auto;
            Session["Customers"] = customers;
            return RedirectToAction("Contact");
        }

        // Post client avec la police auto
        public ActionResult SaveAutoPolicy(ContactMapping contactMapping)
        {
            //Récuperer customer + auto via la variable session
            Customers customer = (Customers)Session["Customers"];

            //Calculer le prix de la soumission
            double soumission_price = BusinessAuto.CalculAutoSoumission(customer.Autos);
            customer.Autos.Estimation_Per_Month = soumission_price;

            //Récuperer contact
            Contacts contacts = BusinessAuto.ContactMapping(contactMapping);
            //Récuperer adresse
            Addresses addresses = BusinessAuto.AddressesMapping(contactMapping);

            //Customer + auto + Adresse + Contact
            customer.Addresses = addresses;
            customer.Contacts = contacts;

            TempData["Price"] = soumission_price;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44365/api/");
                var responseTask = client.PostAsJsonAsync<Customers>("customers", customer);
                responseTask.Wait();
                var results = responseTask.Result;
                if (results.IsSuccessStatusCode)
                {
                    var readJob = results.Content.ReadAsAsync<Customers>();
                    readJob.Wait();
                    return RedirectToAction("Confirme");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error is occured during the processing");
                    return RedirectToAction("SaveAutoPolicy");
                }
            }
         }

        public ActionResult GetAutoPolicy()
        {
            IEnumerable<Customers> customers = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44365/api/customers/");
                var responseTask = client.GetAsync("autos");
                responseTask.Wait();
                var results = responseTask.Result;
                if (results.IsSuccessStatusCode)
                {
                    var readJob = results.Content.ReadAsAsync<List<Customers>>();
                    readJob.Wait();
                    customers = readJob.Result;

                }
                else
                {
                    customers = Enumerable.Empty<Customers>();
                    ModelState.AddModelError(string.Empty, "An error is occured during the processing");
                }
            }
            return View(customers);
        }

        public ActionResult EditAutoPolicy(int id)
        {
            //Auto
            List<string> list_years = new List<string>() { "2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017", "2018", "2019", "2020" };
            List<string> list_marque = new List<string>() { "Hyndai" };
            List<string> list_model_hyndai = new List<string>() { "Elantra GL 4DR", "Elantra GL 5DR", "Elantra VE 4DR" };
            List<string> list_nbreVoiture = new List<string>() { "1", "2", "3", "4", "5+" };
            List<string> list_Loue_Finance = new List<string>() { "Louée", "Financée", "Sans objet" };
            List<string> list_Usage = new List<string>() { "Aller au travail", "Promener", "Affaire" };
            List<int> list_nbreInfraction = new List<int>() { 0, 1, 2, 3, 4, 5 };
            List<int> list_nbreAccident = new List<int>() { 0, 1, 2, 3, 4, 5 };
            List<string> list_type_accident = new List<string>() { "Résponsable", "Accident partiellement résponsable", "Accident non résponsable", "Sans objet" };
            ViewBag.Years = list_years;
            ViewBag.Marque = list_marque;
            ViewBag.Model_Hyndai = list_model_hyndai;
            ViewBag.NbreVoiture = list_nbreVoiture;
            ViewBag.Loue_Finance = list_Loue_Finance;
            ViewBag.Usage = list_Usage;
            ViewBag.Nre_Infraction = list_nbreInfraction;
            ViewBag.Nbre_Accident = list_nbreAccident;
            ViewBag.Accident_Type = list_type_accident;

            //Contact :
            List<string> list_contact = new List<string>() { "Téléphone maison", "Téléphone mobile", "Adresse éléctronique" };
            ViewBag.List_contact = list_contact;

            Customers entity = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44365/api/customers/");
                var responseTask = client.GetAsync(id.ToString());
                responseTask.Wait();
                var results = responseTask.Result;
                if (results.IsSuccessStatusCode)
                {
                    var readJob = results.Content.ReadAsAsync<Customers>();
                    readJob.Wait();
                    entity = readJob.Result;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error is occured during the processing");
                }
            }
            return View(entity);
        }

        [HttpPost]
        public ActionResult EditAutoPolicy(Customers entity)
        {
            //Auto
            List<string> list_years = new List<string>() { "2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017", "2018", "2019", "2020" };
            List<string> list_marque = new List<string>() { "Hyndai" };
            List<string> list_model_hyndai = new List<string>() { "Elantra GL 4DR", "Elantra GL 5DR", "Elantra VE 4DR" };
            List<string> list_nbreVoiture = new List<string>() { "1", "2", "3", "4", "5+" };
            List<string> list_Loue_Finance = new List<string>() { "Louée", "Financée", "Sans objet" };
            List<string> list_Usage = new List<string>() { "Aller au travail", "Promener", "Affaire" };
            List<int> list_nbreInfraction = new List<int>() { 0, 1, 2, 3, 4, 5 };
            List<int> list_nbreAccident = new List<int>() { 0, 1, 2, 3, 4, 5 };
            List<string> list_type_accident = new List<string>() { "Résponsable", "Accident partiellement résponsable", "Accident non résponsable", "Sans objet" };
            ViewBag.Years = list_years;
            ViewBag.Marque = list_marque;
            ViewBag.Model_Hyndai = list_model_hyndai;
            ViewBag.NbreVoiture = list_nbreVoiture;
            ViewBag.Loue_Finance = list_Loue_Finance;
            ViewBag.Usage = list_Usage;
            ViewBag.Nre_Infraction = list_nbreInfraction;
            ViewBag.Nbre_Accident = list_nbreAccident;
            ViewBag.Accident_Type = list_type_accident;

            //Contact :
            List<string> list_contact = new List<string>() { "Téléphone maison", "Téléphone mobile", "Adresse éléctronique" };
            ViewBag.List_contact = list_contact;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44365/api/");
                var responseTask = client.PutAsJsonAsync<Customers>("customers", entity);
                responseTask.Wait();
                var results = responseTask.Result;
                if (results.IsSuccessStatusCode)
                {
                    var readJob = results.Content.ReadAsAsync<Customers>();
                    readJob.Wait();
                    return RedirectToAction("GetAutoPolicy");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error is occured during the processing");
                    return View("EditAutoPolicy");
                }
            }
        }

        public ActionResult DeleteAutoPolicy(int? id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44365/api/customers/");
                var responseTask = client.DeleteAsync(id.ToString());
                responseTask.Wait();
                var results = responseTask.Result;
                if (results.IsSuccessStatusCode)
                {
                    var readJob = results.Content.ReadAsAsync<Customers>();
                    readJob.Wait();
                    return RedirectToAction("GetAutoPolicy");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error is occured during the processing");
                    return View("EditAutoPolicy");
                }
            }
        }
    }
}




