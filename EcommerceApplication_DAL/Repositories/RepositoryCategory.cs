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
    public class RepositoryCategory : IRepository<Category>
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger _logger;

        public RepositoryCategory(AppDbContext appDbContext, ILogger<RepositoryCategory> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<Category> Create(Category category)
        {
            try
            {
                if (category != null)
                {
                    var obj = _appDbContext.Add(category);
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

        public void Delete(Category category)
        {
            try
            {
                var existingCategory = _appDbContext.Categories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
                if (existingCategory != null)
                {
                    // Find all products linked with the category
                    var productsToDelete = _appDbContext.Products.Where(p => p.CategoryId == existingCategory.CategoryId);

                    // Delete these products
                    _appDbContext.Products.RemoveRange(productsToDelete);

                    // Now, delete the category
                    _appDbContext.Categories.Remove(existingCategory);

                    _appDbContext.SaveChanges();
                }
                else
                {
                    throw new ArgumentException("Category not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        public IEnumerable<Category> GetAll()
        {
            try
            {
                var obj = _appDbContext.Categories.ToList();
                if (obj != null) return obj;
                else return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Category GetById(int id)
        {
            try
            {
                var obj = _appDbContext.Categories.FirstOrDefault(x => x.CategoryId == id);
                return obj;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(Category category)
        {
            try
            {
                var existingCategory = _appDbContext.Categories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
                if (existingCategory != null)
                {
                    // Detach the existing entity
                    _appDbContext.Entry(existingCategory).State = EntityState.Detached;
                    
                    // Attach and mark as modified
                    _appDbContext.Entry(category).State = EntityState.Modified;
                    _appDbContext.SaveChanges();
                }
                else
                {
                    throw new ArgumentException("Category not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
