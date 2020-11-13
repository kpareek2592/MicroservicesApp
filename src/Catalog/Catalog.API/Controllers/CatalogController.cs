using Catalog.API.Entities;
using Catalog.API.Repositories.Interfaces;
using DnsClient.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _repository.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id}", Name = "GetProduct")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetProductById(string id)
        {
            var product = await _repository.GetProductById(id);
            if (product == null)
            {
                _logger.LogError($"Product with id: {id} not found.");
                return NotFound();
            }
                
            return Ok(product);
        }

        // We can use [action] or method name
        [Route("[action]/{name}")]
        [HttpGet]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByName(string name)
        {
            var product = await _repository.GetProductByName(name);
            if (product == null)
            {
                _logger.LogError($"Product with name: {name} not found.");
                return NotFound();
            }

            return Ok(product);
        }

        [Route("GetProductByCategory/{category}")]
        [HttpGet]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
        {
            var product = await _repository.GetProductByCategory(category);
            if (product == null)
            {
                _logger.LogError($"Product with category name: {category} not found.");
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            await _repository.Create(product);

            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateProduct([FromBody] Product product)
        {
            return Ok(await _repository.Update(product));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteProduct(string id)
        {
            return Ok(await _repository.Delete(id));
        }
    }
}
