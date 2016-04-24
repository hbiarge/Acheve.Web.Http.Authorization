using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sample.Api.Models;

namespace Sample.Api.Services
{
    public class InMemoryProductsStore : IProductsStore
    {
        private readonly HashSet<Product> _products = new HashSet<Product>(new StoreProductComparer());
        private int _idAcumulator;

        public InMemoryProductsStore()
        {
            _idAcumulator = 1;

            _products.Add(new Product
            {
                Id = _idAcumulator++,
                Name = "Microsoft Surface 4 Pro",
                Category = "Computers",
                Price = 550M,
                ProductType = ProductType.Standard
            });

            _products.Add(new Product
            {
                Id = _idAcumulator++,
                Name = "Microsoft Surface Book",
                Category = "Computers",
                Price = 1200M,
                ProductType = ProductType.Special
            });
        }

        public InMemoryProductsStore(int idAcumulator)
        {
            _idAcumulator = idAcumulator;
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            return Task.FromResult(_products.AsEnumerable());
        }

        public Task<Product> GetByIdAsync(int id)
        {
            var product = _products.SingleOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }

        public Task AddAsync(Product product)
        {
            product.Id = _idAcumulator++;
            _products.Add(product);
            return Task.FromResult(0);
        }

        public Task UpdateAsync(Product product)
        {
            _products.RemoveWhere(p => p.Id == product.Id);
            _products.Add(product);
            return Task.FromResult(0);
        }

        public Task RemoveAsync(Product product)
        {
            _products.RemoveWhere(p => p.Id == product.Id);
            return Task.FromResult(0);
        }

        private class StoreProductComparer : IEqualityComparer<Product>
        {
            public bool Equals(Product x, Product y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(Product obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}