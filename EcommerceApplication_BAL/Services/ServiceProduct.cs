using EcommerceApplication_DAL.Contracts;
using EcommerceApplication_DAL.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceApplication_BAL.Services
{
    public class ServiceProduct
    {
        private readonly IRepository<Product> _repository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IMemoryCache _cache;


        public ServiceProduct(IRepository<Product> repository, IRepository<Category> categoryRepository, IMemoryCache cache)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<Product> AddProduct(Product product)
        {
            // Check if the provided category ID exists
            var category = _categoryRepository.GetById(product.CategoryId);
            if (category == null)
            {
                throw new ArgumentException("Category with the provided ID does not exist.");
            }

            return await _repository.Create(product);
        }

        public void UpdateProduct(int id, Product product)
        {
            // Check if the provided category ID exists
            var category = _categoryRepository.GetById(product.CategoryId);
            if (category == null)
            {
                throw new ArgumentException("Category with the provided ID does not exist.");
            }

            var existingProduct = _repository.GetById(id);
            if (existingProduct != null)
            {
                product.ProductId = id;
                _repository.Update(product);
            }
        }


        public IEnumerable<Product> GetAllProducts()
        {
            const string cacheKey = "AllProducts";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Product> products))
            {
                // Data not in cache, retrieve from repository
                products = _repository.GetAll();

                // Store data in cache
                _cache.Set(cacheKey, products, TimeSpan.FromMinutes(10)); // Cache for 10 minutes
            }

            return products;
        }

        public Product GetProductById(int id)
        {
            return _repository.GetById(id);
        }

        public void DeleteProduct(int id)
        {
            var product = _repository.GetById(id);
            if (product != null)
            {
                _repository.Delete(product);
            }
        }
    }
}
