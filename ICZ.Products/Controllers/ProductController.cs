using ICZ.Products.DB;
using ICZ.Products.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICZ.Products.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        IConfiguration _config;

        public ProductController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> getProductDetail(Guid id)
        {
            DBaseProduct lData = new DBaseProduct(_config );
            var productDetail = await lData.getProductDetail(id, _config.GetConnectionString("OrderDatabase"));

            if (productDetail == null)
            {
                return NotFound();
            }

            return productDetail;
        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> createProduct(Product product)
        {
            bool lReturn = false;

            DBaseProduct lData = new DBaseProduct(_config);
            lReturn = await lData.createProduct(product, _config.GetConnectionString("OrderDatabase"));

            if (lReturn)
                return Ok(lReturn);
            else
                return Problem("DB doesnt consume data", "createProduct", 500, "DBaseProduct", "Error");

        }

        [HttpPut("Update")]
        public async Task<IActionResult> updateProduct(Product product)
        {
            bool lReturn = false;

            DBaseProduct lData = new DBaseProduct(_config);
            lReturn = await lData.updateProduct(product, _config.GetConnectionString("OrderDatabase"));

            if (lReturn)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteStack(Guid id)
        {
            bool lReturn = false;

            DBaseProduct lData = new DBaseProduct(_config);
            lReturn = await lData.deleteProduct(id, _config.GetConnectionString("OrderDatabase"));

            if (lReturn)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }

        [HttpGet()]
        [Route("Products")]
        public async Task<ActionResult<List<Product>>> getProducts()
        {
            DBaseProduct lData = new DBaseProduct(_config);
            var lreturn = await lData.getProducts( _config.GetConnectionString("OrderDatabase"));

            if (lreturn == null && lreturn == null && lreturn.Count == 0)
            {
                return NotFound();
            }

            return lreturn;
        }


        /*
         * TODO import a export seznamu produku do csv (xml)
         * se samotnou implementaci importu a exportu nemam zadne zkusenosti a je urcite skoda, ze jsem si neudelal na ni cas
         * nicmene z projektu skladu modulu vim, ze implementace pozadovanch funkcionalit neni vubec jednoducha a jeji odladeni zabere pomerne dlouhou dobu
         * to znamena ze bych sel cestou pouziti nejake placeneho balicku/knihovny, ktera je odladena a hlavne 100% funkcni
         *
         * */

    }
}

