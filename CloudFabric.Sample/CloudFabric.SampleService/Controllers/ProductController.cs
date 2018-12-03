using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using CloudFabric.SampleService.DTOs;

namespace CloudFabric.SampleService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public ILogger<ProductController> _logger { get; set; }
        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var returnObject = new string[] { "value1", "value2" };
            return Ok(returnObject);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(ProductDTO))]
        [ProducesResponseType(404)]
        public IActionResult GetById(int id)
        {
            try
            {
                var product = new ProductDTO(); // This needs to be fetched using an instance of IProductRepository. For ex, _repository.GetProduct(id)

                if (product == null)
                {
                    return NotFound();
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return BadRequest("Message");
            }
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(ProductDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateAsync([FromBody] ProductDTO product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newProduct = new ProductDTO(); //await _repository.AddProductAsync(product);

                return CreatedAtAction(nameof(GetById), new { id = newProduct.Id }, newProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return BadRequest("Message");
            }
        }


        [HttpPut("{id}")]
        public IActionResult Update(long id, ProductDTO product)
        {
            try
            {
                var newProduct = new ProductDTO(); //await _repository.AddProductAsync(product);
                if (newProduct == null)
                {
                    return NotFound();
                }

                newProduct.Name = "newName";
                newProduct.Category = "newCategory";

                // Save Product

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return BadRequest("Message");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            try
            {
                var newProduct = new ProductDTO(); //await _repository.AddProductAsync(product);
                if (newProduct == null)
                {
                    return NotFound();
                }

                // Delete Object and Save Changes.
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return BadRequest("Message");
            }
        }
    }
}