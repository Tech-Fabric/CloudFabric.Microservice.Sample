using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudFabric.SampleService.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CloudFabric.SampleService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        [HttpPost("")]
        public async Task<ActionResult<ProductDTO>> AddNewProduct([FromBody] ProductDTO newProduct)
        {
            await Task.Delay(1000);
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }
            return new ObjectResult(newProduct);
        }


        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }


        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}