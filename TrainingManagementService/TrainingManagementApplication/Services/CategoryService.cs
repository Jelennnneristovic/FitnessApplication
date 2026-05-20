using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementApplication.DTOs.Requests;
using TrainingManagementApplication.DTOs.Responses;
using TrainingManagementApplication.Interfaces;
using TrainingManagementDomain.Entities;

namespace TrainingManagementApplication.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllAsync(bool includeInactive)
        {
            var categories = await _categoryRepository.GetAllAsync(includeInactive);
            return categories.Select(MapToResponse);
        }

        public async Task<CategoryResponse> GetByIdAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Kategorija nije pronadjena.");

            return MapToResponse(category);
        }

        public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Naziv kategorije je obavezan.");

            var name = request.Name.Trim();

            var existing = await _categoryRepository.GetByNameAsync(name);
            if (existing != null)
                throw new InvalidOperationException("Kategorija sa tim nazivom vec postoji.");

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = request.Description?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _categoryRepository.AddAsync(category);
            return MapToResponse(category);
        }

        public async Task<CategoryResponse> UpdateAsync(Guid id, UpdateCategoryRequest request)
        {
            var category = await _categoryRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Kategorija nije pronadjena.");

            if (request.Name != null)
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    throw new ArgumentException("Naziv kategorije ne moze biti prazan.");

                var newName = request.Name.Trim();

                // Provera da li drugi entitet već koristi to ime
                var existing = await _categoryRepository.GetByNameAsync(newName);
                if (existing != null && existing.Id != id)
                    throw new InvalidOperationException("Kategorija sa tim nazivom vec postoji.");

                category.Name = newName;
            }

            if (request.Description != null)
            {
                category.Description = request.Description.Trim();
            }

            if (request.IsActive.HasValue)
            {
                category.IsActive = request.IsActive.Value;
            }

            category.UpdatedAt = DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(category);
            return MapToResponse(category);
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Kategorija nije pronadjena.");

            await _categoryRepository.DeleteAsync(category);
        }

        private static CategoryResponse MapToResponse(Category category)
        {
            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }
    }
}
