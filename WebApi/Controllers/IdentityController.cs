using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Identity")]
    public class IdentityController : Controller
    {
        [Authorize]
        // GET: api/Identity
        [HttpGet]
        public IActionResult Get()
        {
            string strEmail = "";
            var email = User.Claims.FirstOrDefault(x => x.Type == "email");
            if (null != email)
            {
                strEmail = email.Value;
            }

            string strPhone = "";
            var phone = User.Claims.FirstOrDefault(x => x.Type == "phone");
            if (null != phone)
            {
                strPhone = phone.Value;
            }
            return Ok(new { Mail= strEmail, Phone = strPhone });
        }

        // GET: api/Identity/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/Identity
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/Identity/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
