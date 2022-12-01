using ICZ.Products.DB;
using ICZ.Products.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
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

    }
}

