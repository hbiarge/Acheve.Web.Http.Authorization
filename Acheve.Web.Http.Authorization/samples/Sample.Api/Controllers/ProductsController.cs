using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Acheve.Web.Http.Authorization;
using Sample.Api.Infrastructure.Authorization;
using Sample.Api.Models;
using Sample.Api.Services;

namespace Sample.Api.Controllers
{
    [AuthorizePolicy(Policies.Sales)]
    [RoutePrefix("products")]
    public class ProductsController : ApiController
    {
        private readonly IProductsStore _store;
        private readonly IAuthorizationService _authz;

        public ProductsController(
            IProductsStore productsStore,
            IAuthorizationService authorizationService)
        {
            _store = productsStore;
            _authz = authorizationService;
        }

        // GET: Products
        [HttpGet]
        [Route]
        public async Task<IHttpActionResult> Index()
        {
            return Ok(await _store.GetAllAsync());
        }

        // GET: Products/5
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Details(int? id)
        {
            if (id.HasValue == false)
            {
                return NotFound();
            }

            Product product = await _store.GetByIdAsync(id.Value);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // POST: Products/Discount/5
        [HttpPost]
        [Route("discount/{id:int}/{discount:decimal}")]
        public async Task<IHttpActionResult> Discount(int id, decimal discount)
        {
            if (discount < 1 || discount > 100)
            {
                return BadRequest(ModelState);
            }

            Product product = await _store.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var operation = ProductOperations.GiveDiscount(discount);

            if (await _authz.AuthorizeAsync(User, product, operation))
            {
                product.Price -= discount;
                await _store.UpdateAsync(product);
                return Ok(product);
            }

            return StatusCode(HttpStatusCode.Forbidden);
        }

        // POST: Products/Create
        [HttpPost]
        [Route]
        public async Task<IHttpActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                await _store.AddAsync(product);
                return Ok(product);
            }
            return BadRequest(ModelState);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [Route("edit")]
        public async Task<IHttpActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                if (await _authz.AuthorizeAsync(User, product, ProductOperations.Edit))
                {
                    await _store.UpdateAsync(product);
                    return Ok(product);
                }
                else
                {
                    return StatusCode(HttpStatusCode.Forbidden);
                }
            }
            return BadRequest(ModelState);
        }
    }
}
