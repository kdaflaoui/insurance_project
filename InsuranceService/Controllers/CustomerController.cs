using InsuranceDataAccess;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InsuranceService.Utilitaire;
using System.Data.Entity;

namespace InsuranceService.Controllers
{
    [RoutePrefix("api/customers")]
    public class CustomerController : ApiController
    {

        [Route("")]
        public HttpResponseMessage GetCustomers()
        {
            try
            {
                using (var db = new InsuranceDBEntities())
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, db.Customers.ToList());
                    return response;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }

        }


        [Route("autos")]
        public HttpResponseMessage GetAutoPolicy()
        {
            try
            {
                using (var db = new InsuranceDBEntities())
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK,
                        db.Customers.Where(policy => policy.Type_Of_insurance == "Auto").ToList());
                    return response;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [Route("{id:int}", Name = "GetCustomerByID")]
        public HttpResponseMessage GetCustomers(int? id)
        {
            using (var db = new InsuranceDBEntities())
            {
                var entity = db.Customers.FirstOrDefault(ev => ev.CustomerID == id);
                db.Entry(entity).Reference(policy =>policy.Autos).Load();
                db.Entry(entity).Reference(policy => policy.Addresses).Load();
                db.Entry(entity).Reference(policy => policy.Contacts).Load();
                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The customer with id = " + id + " is not found");
                }
            }
        }

        [HttpPost]
        [Route("")]
        public HttpResponseMessage Post([FromBody] Customers entity)
        {
            try
            {
                using (var db = new InsuranceDBEntities())
                {
                    var customer = db.Customers.FirstOrDefault(cust => cust.CustomerID == entity.CustomerID);
                    if (customer == null)
                    {
                        db.Customers.Add(entity);
                        db.SaveChanges();
                        var response = Request.CreateResponse(HttpStatusCode.Created, entity);
                        response.Headers.Location = new Uri(Url.Link("GetCustomerByID", new { id = entity.CustomerID }));
                        return response;
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.Conflict, "The customer name is already exist");
                    }

                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPut]
        [Route("")]
        public HttpResponseMessage Update([FromBody] Customers entity)
        {
            try
            {
                using (var db = new InsuranceDBEntities())
                {
                    var customer = db.Customers.FirstOrDefault(c => c.CustomerID == entity.CustomerID);
                    var auto = db.Autos.FirstOrDefault(c => c.AutoID == entity.AutoID);
                    var address = db.Addresses.FirstOrDefault(c => c.AddressID == entity.AddressID);
                    var contact = db.Contacts.FirstOrDefault(c => c.ContactID == entity.ContactID);
                    customer.Autos = auto;
                    customer.Addresses = address;
                    customer.Contacts = contact;

                    if (customer != null)
                    {
                        customer.First_Name = entity.First_Name;
                        customer.Last_Name = entity.Last_Name;
                        customer.Civility = entity.Civility;
                        customer.Birthdate = entity.Birthdate;
                        customer.Sexe = entity.Sexe;
                        customer.Autos.Year = entity.Autos.Year;
                        customer.Autos.Marque = entity.Autos.Marque;
                        customer.Autos.Model = entity.Autos.Model;
                        customer.Autos.Vehicle_Nbre = entity.Autos.Vehicle_Nbre;
                        customer.Autos.IsLeqsedOrFinanced = entity.Autos.IsLeqsedOrFinanced;
                        customer.Autos.Vehicle_Use = entity.Autos.Vehicle_Use;
                        customer.Autos.KilometersPerYear = entity.Autos.KilometersPerYear;
                        customer.Autos.Infraction_Nbre = entity.Autos.Infraction_Nbre;
                        customer.Autos.Accident_Nbre = entity.Autos.Accident_Nbre;
                        customer.Autos.Claim_Type = entity.Autos.Claim_Type;
                        customer.Autos.Date_Of_Claim = entity.Autos.Date_Of_Claim;
                        customer.Autos.Estimation_Per_Month = BusinessAuto.CalculAutoSoumission(entity.Autos);
                        //Address
                        customer.Addresses.AddressLine = entity.Addresses.AddressLine;
                        customer.Addresses.PostalCode = entity.Addresses.PostalCode;
                        customer.Addresses.City = entity.Addresses.City;
                        customer.Addresses.Country = entity.Addresses.Country;

                        //Contact 
                        customer.Contacts.Home_Phone = entity.Contacts.Home_Phone;
                        customer.Contacts.Mobile_Phone = entity.Contacts.Mobile_Phone;
                        customer.Contacts.Email_Address = entity.Contacts.Email_Address;
                        customer.Contacts.Contact_Preferences = entity.Contacts.Contact_Preferences;
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, entity);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.Conflict, "The customer name is already exist");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public HttpResponseMessage Delete(int? id)
        {
            using (var db = new InsuranceDBEntities())
            {
                var customer = db.Customers.FirstOrDefault(c => c.CustomerID == id);
                var auto = db.Autos.FirstOrDefault(c => c.AutoID == customer.AutoID);
                var address = db.Addresses.FirstOrDefault(c => c.AddressID == customer.AddressID);
                var contact = db.Contacts.FirstOrDefault(c => c.ContactID == customer.ContactID);
                customer.Autos = auto;
                customer.Addresses = address;
                customer.Contacts = contact;
                if (customer != null)
                {
                    db.Entry(customer).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, customer);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The customer with id = " + id + " is not found");
                }

            }
        }
    }
}
