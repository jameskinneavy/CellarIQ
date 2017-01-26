using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CellarIQ.Web.Api.Models;
using Swashbuckle.Swagger.Annotations;

namespace CellarIQ.Web.Api.Controllers
{
    public class CellarItemsController : ApiController
    {
        // GET api/cellaritems
        [SwaggerOperation("GetAll")]
        public IEnumerable<CellarItem> Get()
        {
            return null;
        }

        // GET api/values/5
        [SwaggerOperation("GetById")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public string Get(int id)
        {
            return "value";
        }

    }
}
