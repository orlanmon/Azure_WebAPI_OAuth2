using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OAuth2.Web.API.Controllers
{

    [ApiController]
    public class SecuredController : ControllerBase
    {

        public SecuredController()
        {


        }


        //[Authorize]
        [Authorize(Roles = "Web.API.Access.Role")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Web.API.Access.Role.Policy")]
        [ActionName("SecuredZipCodeLookup")]
        [HttpGet("api/[controller]/[action]/{zip_code}")]
        public async Task<ActionResult> SecuredZipCodeLookup(string zip_code)
        {

            string locationJSON;
            HttpClient httpClient = new HttpClient();

            try
            {

                if (zip_code != string.Empty)
                {

                    locationJSON = await httpClient.GetStringAsync($"https://api.zippopotam.us/us/{zip_code}");

                    return Ok(locationJSON);

                }
                else
                {

                    return BadRequest();

                }

            }

            catch (Exception ex)
            {

                return StatusCode(500, new { message = "SecuredZipCodeLookup Error: " + ex.Message });

            }
        }

        [ActionName("UnSecuredZipCodeLookup")]
        [HttpGet("api/[controller]/[action]/{zip_code}")]
        public async Task<ActionResult> UnSecuredZipCodeLookup(string zip_code)
        {

            string locationJSON;
            HttpClient httpClient = new HttpClient();

            try
            {

                if (zip_code != string.Empty)
                {

                    locationJSON = await httpClient.GetStringAsync($"https://api.zippopotam.us/us/{zip_code}");

                    return Ok(locationJSON);

                }
                else
                {

                    return BadRequest();

                }

            }

            catch (Exception ex)
            {

                return StatusCode(500, new { message = "UnSecuredZipCodeLookup Error: " + ex.Message });

            }
        }


        [ActionName("GetTokenClaims")]
        [HttpGet("api/[controller]/[action]")]
        public async Task<ActionResult> GetTokenClaims()
        {

            var allClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();


            return Ok(new { TokenClaims = allClaims });



        }

    }

       
    }
