using System.Web.Http;
using Newtonsoft.Json;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace ZendeskRestApi.Controllers.Api.Crm
{
    public class ContactController : ApiController
    {
        // GET api/<controller>
        public ContactModel Get(string email = "", string crmUrl = "", string userName = "", string password = "", string debug = "")
        {
            if (debug == "1578")
            {
                email = "bob@bob.com";
                crmUrl = "https://eone.crm.dynamics.com";
                userName = "bob.christianson@eonesolutions.com";
                password = "Rchris78";
            }

            var model = new ContactModel();
            var conUrl = string.Format(@"Url={0}; Username={1}; Password={2};", crmUrl, userName, password);
            var connection = CrmConnection.Parse(conUrl);
            var service = new OrganizationService(connection);
            var query = new QueryExpression("contact");
            var condition = new ConditionExpression
            {
                AttributeName = "emailaddress1",
                EntityName = "contact",
                Operator = ConditionOperator.Equal,
            };
            condition.Values.Add(email);
            query.Criteria.AddCondition(condition);
            query.ColumnSet.AddColumn("fullname");
            query.ColumnSet.AddColumn("emailaddress1");
            query.ColumnSet.AddColumn("address1_line1");
            query.ColumnSet.AddColumn("address1_line2");
            query.ColumnSet.AddColumn("address1_line3");
            query.ColumnSet.AddColumn("telephone1");
            query.ColumnSet.AddColumn("address1_city");
            //query.ColumnSet.AddColumn("parentcustomerid");

            try
            {
                var result = service.RetrieveMultiple(query);

                foreach (var entity in result.Entities)
                {
                    foreach (var att in entity.Attributes)
                    {
                        var attrib = new Attribute
                        {
                            Name = att.Key,
                            Value = att.Value.ToString()
                        };

                        model.Attributes.Add(attrib);
                        //if (att.Key == "firstname") model.FirstName = att.Value.ToString();
                        //if (att.Key == "lastname") model.LastName = att.Value.ToString();
                        //if (att.Key == "emailaddress1") model.Email = att.Value.ToString();
                        //if (att.Key == "contactid") model.ContactId = att.Value.ToString();
                    }
                }

                //model.HtmlString = "<p>" + model.FirstName + " " + model.LastName + "</p>";
                //model.HtmlString += "<p>" + model.Email + "</p>";
                //model.HtmlString += "<p>" + model.ContactId + "</p>";
            }
            catch (Exception e)
            {
                model.Error = e.Message;
            }

            return model;
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return JsonConvert.SerializeObject(id);
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }

    public class ContactModel
    {
        public List<Attribute> Attributes { get; set; }
        public string Error { get; set; }

        public ContactModel()
        {
            Error = "no error";
            Attributes = new List<Attribute>();
        }
    }

    public class Attribute
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

}