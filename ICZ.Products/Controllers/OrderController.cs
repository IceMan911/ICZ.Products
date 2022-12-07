using ICZ.Products.DB;
using ICZ.Products.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ICZ.Products.Helpers;

namespace ICZ.Products.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        IConfiguration _config;

        public OrderController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> getOrderDetail(Guid id)
        {
            DBaseOrder lData = new DBaseOrder(_config);
            var orderFull = await lData.getOrderDetail(id, _config.GetConnectionString("OrderDatabase"));

            var orderDetail = Utils.getOrderDetail(orderFull);

            if (orderDetail == null)
            {
                return NotFound();
            }

            return orderDetail;
        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> createOrder(OrderCreate orderCreate)
        {
            bool lReturn = false;

            DBaseOrder lData = new DBaseOrder(_config);
            lReturn = await lData.createOrder(orderCreate, _config.GetConnectionString("OrderDatabase"));

            if (lReturn)
                return Ok(lReturn);
            else
                return Problem("DB does not know any data from the order", "createOrder", 500, "DBaseOrder", "Error");

        }

        //[HttpPut("Update")]
        //public async Task<IActionResult> updateProduct(Product product)
        //{
        //    bool lReturn = false;

        //    DBaseProduct lData = new DBaseProduct(_config);
        //    lReturn = await lData.updateProduct(product, _config.GetConnectionString("OrderDatabase"));

        //    if (lReturn)
        //    {
        //        return NoContent();
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }

        //}

        //[HttpDelete("{id}")]
        //public async Task<ActionResult<bool>> DeleteStack(Guid id)
        //{
        //    bool lReturn = false;

        //    DBaseProduct lData = new DBaseProduct(_config);
        //    lReturn = await lData.deleteProduct(id, _config.GetConnectionString("OrderDatabase"));

        //    if (lReturn)
        //    {
        //        return Ok();
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }

        //}

        //[HttpGet()]
        //[Route("Products")]
        //public async Task<ActionResult<List<Product>>> getProducts()
        //{
        //    DBaseProduct lData = new DBaseProduct(_config);
        //    var lreturn = await lData.getProducts(_config.GetConnectionString("OrderDatabase"));

        //    if (lreturn == null && lreturn == null && lreturn.Count == 0)
        //    {
        //        return NotFound();
        //    }

        //    return lreturn;
        //}

    }
}

