using EcommerceApplication_DAL.Contracts;
using EcommerceApplication_DAL.Data;
using EcommerceApplication_DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceApplication_DAL.Repositories
{
    public class RepositoryProduct : IRepository<Product>
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger _logger;

        public RepositoryProduct(AppDbContext appDbContext, ILogger<RepositoryProduct> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<Product> Create(Product product)
        {
            try
            {
                if (product != null)
                {
                    var obj = _appDbContext.Add(product);
                    await _appDbContext.SaveChangesAsync();
                    return obj.Entity;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Delete(Product product)
        {
            try
            {
                var existingProduct = _appDbContext.Products.FirstOrDefault(p => p.ProductId == product.ProductId);
                if (existingProduct != null)
                {
                    _appDbContext.Products.Remove(existingProduct);
                    _appDbContext.SaveChanges();
                }
                else
                {
                    throw new ArgumentException("Product not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IEnumerable<Product> GetAll()
        {
            try
            {
                var obj = _appDbContext.Products.ToList();
                if (obj != null) return obj;
                else return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Product GetById(int id)
        {
            try
            {
                var obj = _appDbContext.Products.FirstOrDefault(x => x.ProductId == id);
                return obj;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(Product product)
        {
            try
            {
                var existingProduct = _appDbContext.Products.FirstOrDefault(p => p.ProductId == product.ProductId);
                if (existingProduct != null)
                {
                    // Detach the existing entity
                    _appDbContext.Entry(existingProduct).State = EntityState.Detached;

                    // Attach and mark as modified
                    _appDbContext.Entry(product).State = EntityState.Modified;
                    _appDbContext.SaveChanges();
                }
                else
                {
                    throw new ArgumentException("Product not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
