using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IOSAS.Infrastructure.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace IOSAS.Infrastructure.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ID365WebAPIService _d365webapiservice;
        public ContactController(ID365WebAPIService d365webapiservice)
        {
            _d365webapiservice = d365webapiservice ?? throw new ArgumentNullException(nameof(d365webapiservice));
        }

        // GET: api/contact
        [HttpGet("GetbyExternalId")]
        public ActionResult<string> Get(string externalId)
        {
            if (string.IsNullOrEmpty(externalId)) 
                return BadRequest("Invalid Request - BCeID or Invite Code is required.");

            var fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' no-lock='false' distinct='true'>
                                    <entity name='contact'>
                                        <attribute name='entityimage_url' />
                                        <attribute name='fullname' />
                                        <attribute name='emailaddress1' />
                                        <attribute name='contactid' />
                                        <attribute name='firstname' />
                                        <attribute name='lastname' />
                                        <attribute name='telephone1' />
                                        <attribute name='iosas_loginenabled' />
                                        <attribute name='iosas_externaluserid' />
                                        <attribute name='iosas_invitecode' />
                                        <filter type='and'>
                                            <condition attribute='statecode' operator='eq' value='0' />
                                            <filter type='or'>
                                                <condition attribute='iosas_invitecode' operator='eq' value='{externalId}' />
                                                <condition attribute='iosas_externaluserid' operator='eq' value='{externalId}' />
                                            </filter>
                                        </filter>
                                        <order attribute='fullname' descending='false' />
                                    </entity>
                                </fetch>";

            var message = $"contacts?fetchXml=" + WebUtility.UrlEncode(fetchXml);

            var response = _d365webapiservice.SendMessageAsync(HttpMethod.Get, message);
            if (response.IsSuccessStatusCode)
            {
                var root = JToken.Parse(response.Content.ReadAsStringAsync().Result);
               
                if (root.Last().First().HasValues)
                {     
                    return Ok(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    return NotFound($"No Data: {externalId}");
                }
            }
            else
                return StatusCode((int)response.StatusCode,
                    $"Failed to Retrieve records: {response.ReasonPhrase}");


        }


        [HttpGet("GetBySchoolAuthority")]
        public ActionResult<string> GetBySchoolAuthority(string schoolAuthorityId)
        {
            if (string.IsNullOrEmpty(schoolAuthorityId)) return BadRequest("Invalid Request");

            var fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' no-lock='false' distinct='true'>
    <entity name='contact'>
        <attribute name='entityimage_url' />
        <attribute name='fullname' />
        <order attribute='fullname' descending='false' />
        <attribute name='emailaddress1' />
        <attribute name='contactid' />
        <attribute name='firstname' />
        <attribute name='lastname' />
        <attribute name='telephone1' />
        <attribute name='iosas_loginenabled' />
        <attribute name='iosas_externalauthid' />
        <attribute name='iosas_invitecode' />
        <filter type='and'>
            <condition attribute='statecode' operator='eq' value='0' />
            <condition attribute='iosas_edu_SchoolAuthority' operator='eq' value='{schoolAuthorityId}' />
        </filter>
    </entity>
</fetch>";

            var message = $"contact?fetchXml=" + WebUtility.UrlEncode(fetchXml);

            var response = _d365webapiservice.SendMessageAsync(HttpMethod.Get, message);
            if (response.IsSuccessStatusCode)
            {
                var root = JToken.Parse(response.Content.ReadAsStringAsync().Result);

                if (root.Last().First().HasValues)
                {
                    return Ok(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    return NotFound($"No Data: {schoolAuthorityId}");
                }
            }
            else
                return StatusCode((int)response.StatusCode,
                    $"Failed to Retrieve records: {response.ReasonPhrase}");

        }


        [HttpPatch("UpdateLogin")]
        public ActionResult<string> UpdateLogin(string id, string externalId)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Invalid Request - Contact unique identfier is required.");

            var value = new JObject
                        {
                            { "iosas_externaluserid", externalId},
                            { "iosas_invitecode", null}
                        };

            var statement = $"contacts({id})";

            HttpResponseMessage response = _d365webapiservice.SendUpdateRequestAsync(statement, value.ToString());

            if (response.IsSuccessStatusCode)
                return Ok($"Contact {externalId} login updated.");
            else
                return StatusCode((int)response.StatusCode,
                    $"Failed to Update record: {response.ReasonPhrase}");
        }


        [HttpPost("Login")]
        public ActionResult<string> Login([FromBody] dynamic value)
        {

            //Validate fields from body and check if user is existing or new
            //Create if does not exist and send external I dback to user.
            //Returning crm contactid.
            //throw new NotImplementedException();

            if (value.iosas_externaluserid == null)
                return BadRequest("iosas_externaluserid is not provided");

            if (value.emailaddress1 == null)
                return BadRequest("emailaddress1 is not provided");

            if (value.firstname == null)
                return BadRequest("firstname is not provided");

            if (value.lastname == null)
                return BadRequest("lastname is not provided");

            if (value.telephone1 == null)
                return BadRequest("telephone1 is not provided");


            var fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' no-lock='false' distinct='true'>
                                <entity name='contact'>
                                    <attribute name='fullname' />
                                    <attribute name='emailaddress1' />
                                    <attribute name='contactid' />
                                    <attribute name='firstname' />
                                    <attribute name='lastname' />
                                    <attribute name='telephone1' />
                                    <attribute name='iosas_loginenabled' />
                                    <attribute name='iosas_externaluserid' />     
                                    <filter type='and'>
                                        <condition attribute='statecode' operator='eq' value='0' />
                                        <filter type='or'>
                                            <condition attribute='iosas_externaluserid' operator='eq' value='{value.iosas_externaluserid}' />
                                        </filter>
                                    </filter>
                                    <order attribute='fullname' descending='false' />
                                </entity>
                            </fetch>";

            var message = $"contacts?fetchXml=" + WebUtility.UrlEncode(fetchXml);
            var exists = _d365webapiservice.SendMessageAsync(HttpMethod.Get, message);

            if (exists.IsSuccessStatusCode)
            {
                var root = JToken.Parse(exists.Content.ReadAsStringAsync().Result);

                if (root.Last().First().HasValues)
                {
                    return Ok(exists.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    string statement = "contacts?$select=fullname,emailaddress1,contactid,firstname,lastname,telephone1,iosas_loginenabled,iosas_externaluserid";
                    var createResponse = _d365webapiservice.SendCreateRequestAsyncRtn(statement, value.ToString());

                    if (createResponse.IsSuccessStatusCode)
                    {
                        return Ok(createResponse.Content.ReadAsStringAsync().Result);
                    }
                    else
                        return StatusCode((int)exists.StatusCode, $"Failed to create user: {exists.ReasonPhrase}");
                }
            }
            else
                return StatusCode((int)exists.StatusCode,
                    $"Failed to retrieve user details: {exists.ReasonPhrase}");
        }


    }
}