using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public class EOIController : ControllerBase
    {
        private readonly ID365WebAPIService _d365webapiservice;
        public EOIController(ID365WebAPIService d365webapiservice)
        {
            _d365webapiservice = d365webapiservice ?? throw new ArgumentNullException(nameof(d365webapiservice));
        }



        [HttpGet("GetById")]
        public ActionResult<string> Get(string id)
        {
            if (string.IsNullOrEmpty(id)) 
                return BadRequest("Invalid Request - Id is required");

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
                                    <attribute name='createdon' />
                                    <attribute name='modifiedon' />
                                    <attribute name='statecode' />
                                    <attribute name='statuscode' />
                                    <filter type='and'>
                                        <condition attribute='iosas_expressionofinterestid' operator='eq' value='{id}' />
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
                                    <attribute name='createdon' />
                                    <attribute name='modifiedon' />
                                    <attribute name='statecode' />
                                    <attribute name='statuscode' />
                                    <filter type='and'>
                                       <condition attribute='statecode' operator='eq' value='0'/>
                                       <filter type='and'>
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
                    return NotFound($"No Data");
                }
            }
            else
                return StatusCode((int)response.StatusCode,
                    $"Failed to Retrieve records: {response.ReasonPhrase}");
        }
    }
}