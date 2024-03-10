using EcommerceApplication_DAL.Contracts;
using EcommerceApplication_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceApplication_BAL.Services
{
    public class ServiceCategory
    {
        private readonly IRepository<Category> _repository;

        public ServiceCategory(IRepository<Category> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Category> AddCategory(Category category)
        {
            return await _repository.Create(category);
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _repository.GetAll();
        }

        public Category GetCategoryById(int id)
        {
            return _repository.GetById(id);
        }

        public void DeleteCategory(int id)
        {
            var category = _repository.GetById(id);
            if (category != null)
            {
                _repository.Delete(category);
            }
        }

        public void UpdateCategory(int id, Category category)
        {
            var existingCategory = _repository.GetById(id);
            if (existingCategory != null)
            {
                category.CategoryId = id;
                _repository.Update(category);
            }
        }
    }
}
