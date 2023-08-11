using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;
using IOSAS.Infrastructure.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace IOSAS.Infrastructure.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EOIController : ControllerBase
    {
        private readonly ID365WebAPIService _d365webapiservice;
        public EOIController(ID365WebAPIService d365webapiservice)
        {
            _d365webapiservice = d365webapiservice ?? throw new ArgumentNullException(nameof(d365webapiservice));
        }

        [HttpGet("GetById")]
        public ActionResult<string> Get(string id, string userId)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Invalid Request - Id is required");

            if (string.IsNullOrEmpty(userId))
                return BadRequest("Invalid Request - userId is userId");

            var fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' no-lock='false' distinct='true'>
                                <entity name='iosas_expressionofinterest'>
                                    <attribute name='iosas_name' />   
                                    <attribute name='iosas_existingauthority' />  
                                    <attribute name='iosas_existinghead' />  
                                    <attribute name='iosas_authorityhead' />  
                                    <attribute name='iosas_designatedcontactsameasauthorityhead' />  
                                    <attribute name='iosas_existingcontact' />  
                                    <attribute name='iosas_edu_year' />  
                                    <attribute name='iosas_proposedschoolname' />  
                                    <attribute name='iosas_schooladdressline1' />  
                                    <attribute name='iosas_schooladdressline2' />  
                                    <attribute name='iosas_schoolcity' />  
                                    <attribute name='iosas_schoolprovince' />  
                                    <attribute name='iosas_schoolpostalcode' />  
                                    <attribute name='iosas_schoolcountry' />  
                                    <attribute name='iosas_website' />  
                                    <attribute name='iosas_groupclassification' />  
                                    <attribute name='iosas_startgrade' />  
                                    <attribute name='iosas_endgrade' />  
                                    <attribute name='iosas_schoolauthorityname' />  
                                    <attribute name='iosas_authorityaddressline1' />  
                                    <attribute name='iosas_authorityaddressline2' />
                                    <attribute name='iosas_authoritycity' /> 
                                    <attribute name='iosas_authoritypostalcode' />  
                                    <attribute name='iosas_authorityprovince' />  
                                    <attribute name='iosas_authoritycountry' />
                                    <attribute name='iosas_authorityheadfirstname' />
                                    <attribute name='iosas_schoolauthorityheadname' />
                                    <attribute name='iosas_schoolauthorityheademail' />
                                    <attribute name='iosas_schoolauthorityheadphone' />
                                    <attribute name='iosas_designatedcontactfirstname' />
                                    <attribute name='iosas_schoolauthoritycontactname' />
                                    <attribute name='iosas_schoolauthoritycontactemail' />
                                    <attribute name='iosas_schoolauthoritycontactphone' />
                                    <attribute name='iosas_reviewstatus' />
                                    <attribute name='iosas_submissiondate' />
                                    <attribute name='iosas_approvaldate' />
                                    <attribute name='createdon' />
                                    <attribute name='modifiedon' />
                                    <attribute name='statecode' />
                                    <attribute name='statuscode' />
                                    <attribute name='iosas_eoinumber' />
                                    <attribute name='iosas_expressionofinterestid' />
                                    <attribute name='iosas_seekgrouponeclassification' />  
                                    <attribute name='iosas_incorporationcertificateissuedate' />  
                                    <attribute name='iosas_certificateofgoodstandingissuedate' /> 
                                    <attribute name='iosas_notes' /> 
                                    <filter type='and'>
                                        <condition attribute='iosas_expressionofinterestid' operator='eq' value='{id}' />
                                        <filter type='or'>
                                           <condition attribute='iosas_authorityhead' operator='eq' value='{userId}' />
                                           <condition attribute='iosas_authortiycontact' operator='eq' value='{userId}' />
                                        </filter>
                                    </filter>
                                </entity>
                            </fetch>";

            var message = $"iosas_expressionofinterests?fetchXml=" + WebUtility.UrlEncode(fetchXml);

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
                    return NotFound($"No Data: {id}");
                }
            }
            else
                return StatusCode((int)response.StatusCode,
                    $"Failed to Retrieve records: {response.ReasonPhrase}");
        }

        [HttpGet("GetAllByUser")]
        public ActionResult<string> GetAllByUser(string userId)
        {

            if (string.IsNullOrEmpty(userId))
                return BadRequest("Invalid Request - userId is userId");

            var fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' no-lock='false' distinct='true'>
                                <entity name='iosas_expressionofinterest'>
                                    <attribute name='iosas_name' />   
                                    <attribute name='iosas_existingauthority' />  
                                    <attribute name='iosas_existinghead' />  
                                    <attribute name='iosas_authorityhead' />  
                                    <attribute name='iosas_designatedcontactsameasauthorityhead' />  
                                    <attribute name='iosas_existingcontact' />  
                                    <attribute name='iosas_edu_year' />  
                                    <attribute name='iosas_proposedschoolname' />  
                                    <attribute name='iosas_schooladdressline1' />  
                                    <attribute name='iosas_schooladdressline2' />  
                                    <attribute name='iosas_schoolcity' />  
                                    <attribute name='iosas_schoolprovince' />  
                                    <attribute name='iosas_schoolpostalcode' />  
                                    <attribute name='iosas_schoolcountry' />  
                                    <attribute name='iosas_website' />  
                                    <attribute name='iosas_groupclassification' />  
                                    <attribute name='iosas_startgrade' />  
                                    <attribute name='iosas_endgrade' />  
                                    <attribute name='iosas_schoolauthorityname' />  
                                    <attribute name='iosas_authorityaddressline1' />  
                                    <attribute name='iosas_authorityaddressline2' />
                                    <attribute name='iosas_authoritycity' /> 
                                    <attribute name='iosas_authoritypostalcode' />  
                                    <attribute name='iosas_authorityprovince' />  
                                    <attribute name='iosas_authoritycountry' />
                                    <attribute name='iosas_authorityheadfirstname' />
                                    <attribute name='iosas_schoolauthorityheadname' />
                                    <attribute name='iosas_schoolauthorityheademail' />
                                    <attribute name='iosas_schoolauthorityheadphone' />
                                    <attribute name='iosas_designatedcontactfirstname' />
                                    <attribute name='iosas_schoolauthoritycontactname' />
                                    <attribute name='iosas_schoolauthoritycontactemail' />
                                    <attribute name='iosas_schoolauthoritycontactphone' />
                                    <attribute name='iosas_reviewstatus' />
                                    <attribute name='iosas_submissiondate' />
                                    <attribute name='iosas_approvaldate' />
                                    <attribute name='createdon' />
                                    <attribute name='modifiedon' />
                                    <attribute name='statecode' />
                                    <attribute name='statuscode' />
                                    <attribute name='iosas_eoinumber' />
                                    <attribute name='iosas_expressionofinterestid' />
                                    <attribute name='iosas_seekgrouponeclassification' />  
                                    <filter type='and'>
                                       <condition attribute='statecode' operator='eq' value='0'/>
                                       <condition attribute='iosas_reviewstatus' operator='ne' value='100000005' />
                                       <condition attribute='iosas_reviewstatus' operator='ne' value='100000004' />
                                       <filter type='or'>
                                           <condition attribute='iosas_authorityhead' operator='eq' value='{userId}' />
                                           <condition attribute='iosas_authortiycontact' operator='eq' value='{userId}' />
                                        </filter>
                                    </filter>
                                </entity>
                            </fetch>";

            var message = $"iosas_expressionofinterests?fetchXml=" + WebUtility.UrlEncode(fetchXml);

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
                    return Ok($"[]");
                }
            }
            else
                return StatusCode((int)response.StatusCode,
                    $"Failed to Retrieve records: {response.ReasonPhrase}");
        }

        [HttpPatch("Cancel")]
        public ActionResult<string> Cancel(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Invalid Request - Id is required");

            var value = new JObject
                        { 
                            { "iosas_reviewstatus", 100000005}, 
                            { "iosas_reviewnotes", "Abandoned by user" }
                        };

            var statement = $"iosas_expressionofinterests({id})";
            HttpResponseMessage response = _d365webapiservice.SendUpdateRequestAsync(statement, value.ToString());
            if (response.IsSuccessStatusCode)
                return Ok($"EPO {id} Cancelled.");
            else
                return StatusCode((int)response.StatusCode,
                    $"Failed to Update record: {response.ReasonPhrase}");
        }

        [HttpPatch("Update")]
        public ActionResult<string> Update([FromBody] dynamic value, bool submitted, string id, string? userId = null)
        {

            if (string.IsNullOrEmpty(id))
                return BadRequest("Invalid Request - Id is required");

            if (string.IsNullOrEmpty(userId))
                return BadRequest("Invalid Request - UserId is required");


            string statement = $"iosas_expressionofinterests({id})";
            var response = _d365webapiservice.SendRetrieveRequestAsync($"{statement}?$select=iosas_reviewstatus,iosas_name", true);
            JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            if (body != null)
            {
                var  reviewId = body.GetValue("iosas_reviewstatus");
                if (int.Parse((string)reviewId) != 100000006)
                {
                    return BadRequest($"EOI {value.iosas_name} not in draft mode.");
                }
            }

            var eoi = PrepareEOI(value,submitted, userId);

            response = _d365webapiservice.SendUpdateRequestAsync(statement, eoi.ToString());

            if (response.IsSuccessStatusCode)
            {
               return Ok($"EOI {value.iosas_expressionofinterestid} updated successfully");
            }
            else
                return StatusCode((int)response.StatusCode,
                    $"Failed to Update record: {response.ReasonPhrase}");
        }

        [HttpPost("Create")]
        public ActionResult<string> Create([FromBody] dynamic value, bool submitted, string? userId = null)
        {
            string statement = "iosas_expressionofinterests";


            //Default status is Draft no need to set it at Creation Time
            //For Authority Head and Designated Contact: If logged in with user then use it designated Contact, otherwise set fields in EOI
            //If SA is the same as DC then use then id supplied otherwise jisut use EOI fields for SA Head
            //https://learn.microsoft.com/en-us/power-apps/developer/data-platform/webapi/create-entity-web-api

            var eoi = PrepareEOI(value,submitted, userId);

            var response = _d365webapiservice.SendCreateRequestAsync(statement, eoi.ToString());

            if (response.IsSuccessStatusCode)
            {
                var entityUri = response.Headers.GetValues("OData-EntityId")[0];

                string pattern = @"(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}";
                Match m = Regex.Match(entityUri, pattern, RegexOptions.IgnoreCase);
                var newRecordId = string.Empty;
                if (m.Success)
                {
                    newRecordId = m.Value;
                    return Ok($"{newRecordId}");
                }
                else
                    return StatusCode((int)HttpStatusCode.InternalServerError, "Unable to create record at this time");
            }
            else
                return StatusCode((int)response.StatusCode,
                    $"Failed to Create record: {response.ReasonPhrase}");
        }
    
    
        private JObject PrepareEOI(dynamic value, bool submitted, string? userId = null)
        {
            var eoi = new JObject
                        {
                            { "iosas_name", "TBA"},
                            { "iosas_schooladdressline1", value.iosas_schooladdressline1},
                            { "iosas_schooladdressline2", value.iosas_schooladdressline2},
                            { "iosas_authoritycountry", value.iosas_authoritycountry},
                            { "iosas_proposedschoolname", value.iosas_proposedschoolname},
                            { "iosas_schoolcity", value.iosas_schoolcity},
                            { "iosas_schoolpostalcode", value.iosas_schoolpostalcode},
                            { "iosas_schoolprovince", value.iosas_schoolprovince},
                            { "iosas_endgrade", value.iosas_endgrade},
                            { "iosas_startgrade", value.iosas_startgrade},
                            { "iosas_schoolcountry", value.iosas_schoolcountry},
                            { "iosas_groupclassification", value.iosas_groupclassification},
                            { "iosas_website", value.iosas_website},
                            { "iosas_seekgrouponeclassification", value.iosas_seekgrouponeclassification},
                            { "iosas_edu_Year@odata.bind", $"/edu_years({value._iosas_edu_year_value})" },
                            { "iosas_designatedcontactsameasauthorityhead",value.iosas_designatedcontactsameasauthorityhead },
                            { "iosas_existingauthority",value.iosas_existingauthority },
                            { "iosas_submissionmethod",100000001 },
                            { "iosas_incorporationcertificateissuedate",value.iosas_incorporationcertificateissuedate },
                            { "iosas_certificateofgoodstandingissuedate",value.iosas_certificateofgoodstandingissuedate },
                            { "iosas_notes",value.iosas_notes }
                        };

            if (submitted)
            {
                eoi["iosas_reviewstatus"] = 100000003; //New (sbumitted)
                eoi["iosas_submissiondate"] = DateTime.UtcNow;
            }
            else
            {
                eoi["iosas_reviewstatus"] = 100000006; //Draft
            }

            if (value.iosas_existingauthority == true)
            {
                eoi["iosas_edu_SchoolAuthority@odata.bind"] = $"/edu_schoolauthorities({value._iosas_edu_schoolauthority_value})";

            }
            else
            {
                eoi["iosas_schoolauthorityname"] = value.iosas_schoolauthorityname;
                eoi["iosas_authorityaddressline1"] = value.iosas_authorityaddressline1;
                eoi["iosas_authorityaddressline2"] = value.iosas_authorityaddressline2;
                eoi["iosas_authoritycity"] = value.iosas_authoritycity;
                eoi["iosas_authorityprovince"] = value.iosas_authorityprovince;
                eoi["iosas_authoritypostalcode"] = value.iosas_authoritypostalcode;
                eoi["iosas_authoritycountry"] = value.iosas_authoritycountry;
            }

            if (string.IsNullOrEmpty(userId)) //unathenticated
            {
                eoi["iosas_authorityheadfirstname"] = value.iosas_authorityheadfirstname;
                eoi["iosas_schoolauthorityheadname"] = value.iosas_schoolauthorityheadname;
                eoi["iosas_schoolauthorityheademail"] = value.iosas_schoolauthorityheademail;
                eoi["iosas_schoolauthorityheadphone"] = value.iosas_schoolauthorityheadphone;

                if (value.iosas_designatedcontactsameasauthorityhead == true)
                {
                    eoi["iosas_designatedcontactfirstname"] = value.iosas_authorityheadfirstname;
                    eoi["iosas_schoolauthoritycontactname"] = value.iosas_schoolauthorityheadname;
                    eoi["iosas_schoolauthoritycontactemail"] = value.iosas_schoolauthorityheademail;
                    eoi["iosas_schoolauthoritycontactphone"] = value.iosas_schoolauthorityheadphone;
                }
                else
                {
                    eoi["iosas_designatedcontactfirstname"] = value.iosas_designatedcontactfirstname;
                    eoi["iosas_schoolauthoritycontactname"] = value.iosas_schoolauthoritycontactname;
                    eoi["iosas_schoolauthoritycontactemail"] = value.iosas_schoolauthoritycontactemail;
                    eoi["iosas_schoolauthoritycontactphone"] = value.iosas_schoolauthoritycontactphone;
                }
            }
            else
            {
                //Designated contact
                eoi["iosas_existingcontact"] = true;
                eoi["iosas_AuthortiyContact@odata.bind"] = $"/contacts({userId})";
                if (value.iosas_designatedcontactsameasauthorityhead == true)
                {
                    eoi["iosas_AuthorityHead@odata.bind"] = $"/contacts({userId})";
                    eoi["iosas_existinghead"] = true;
                }
                else
                {
                    eoi["iosas_authorityheadfirstname"] = value.iosas_authorityheadfirstname;
                    eoi["iosas_schoolauthorityheadname"] = value.iosas_schoolauthorityheadname;
                    eoi["iosas_schoolauthorityheademail"] = value.iosas_schoolauthorityheademail;
                    eoi["iosas_schoolauthorityheadphone"] = value.iosas_schoolauthorityheadphone;
                }
            }

            return eoi;
        }
    }
}