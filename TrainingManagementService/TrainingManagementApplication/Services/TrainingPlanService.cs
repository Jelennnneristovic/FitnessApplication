using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementApplication.DTOs.Requests;
using TrainingManagementApplication.DTOs.Responses;
using TrainingManagementApplication.Interfaces;
using TrainingManagementDomain.Entities;
using TrainingManagementDomain.Enums;

namespace TrainingManagementApplication.Services
{
    public class TrainingPlanService : ITrainingPlanService
    {
        private readonly ITrainingPlanRepository _planRepository;
        private readonly ICategoryRepository _categoryRepository;

        public TrainingPlanService(
            ITrainingPlanRepository planRepository,
            ICategoryRepository categoryRepository)
        {
            _planRepository = planRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<TrainingPlanResponse>> GetAllAsync(TrainingPlanFilterRequest filter)
        {
            var plans = await _planRepository.GetAllAsync(filter);
            return plans.Select(MapToResponse);
        }

        public async Task<TrainingPlanResponse> GetByIdAsync(Guid id)
        {
            var plan = await _planRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Plan treninga nije pronadjen.");

            return MapToResponse(plan);
        }

        public async Task<IEnumerable<TrainingPlanResponse>> GetMyPlansAsync(Guid trainerId)
        {
            var plans = await _planRepository.GetByTrainerIdAsync(trainerId);
            return plans.Select(MapToResponse);
        }

        public async Task<TrainingPlanResponse> CreateAsync(Guid trainerId, CreateTrainingPlanRequest request)
        {
            ValidateRequest(request.Title, request.Description, request.Price,
                request.MaxParticipants, request.DurationMinutes, request.Type);

            // Provera da kategorija postoji i da je aktivna
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId)
                ?? throw new ArgumentException("Kategorija nije pronadjena.");

            if (!category.IsActive)
                throw new ArgumentException("Kategorija nije aktivna.");

            var plan = new TrainingPlan
            {
                Id = Guid.NewGuid(),
                TrainerId = trainerId,
                CategoryId = request.CategoryId,
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                Type = request.Type,
                Price = request.Price,
                MaxParticipants = request.MaxParticipants,
                DurationMinutes = request.DurationMinutes,
                Location = request.Location?.Trim(),
                ImageUrl = request.ImageUrl?.Trim(),
                Status = TrainingPlanStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            await _planRepository.AddAsync(plan);

            // Reload sa Category za response
            var created = await _planRepository.GetByIdAsync(plan.Id);
            return MapToResponse(created!);
        }

        public async Task<TrainingPlanResponse> UpdateAsync(Guid id, Guid trainerId, UpdateTrainingPlanRequest request)
        {
            var plan = await _planRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Plan treninga nije pronadjen.");

            // Provera vlasništva
            if (plan.TrainerId != trainerId)
                throw new UnauthorizedAccessException("Nemate pravo da menjate ovaj plan.");

            // Selektivne izmene
            if (request.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(request.CategoryId.Value)
                    ?? throw new ArgumentException("Kategorija nije pronadjena.");

                if (!category.IsActive)
                    throw new ArgumentException("Kategorija nije aktivna.");

                plan.CategoryId = request.CategoryId.Value;
            }

            if (request.Title != null)
            {
                if (string.IsNullOrWhiteSpace(request.Title))
                    throw new ArgumentException("Naziv plana ne moze biti prazan.");
                plan.Title = request.Title.Trim();
            }

            if (request.Description != null)
            {
                plan.Description = request.Description.Trim();
            }

            if (request.Type.HasValue)
                plan.Type = request.Type.Value;

            if (request.Price.HasValue)
            {
                if (request.Price.Value < 0)
                    throw new ArgumentException("Cena ne moze biti negativna.");
                plan.Price = request.Price.Value;
            }

            if (request.MaxParticipants.HasValue)
            {
                if (request.MaxParticipants.Value < 1)
                    throw new ArgumentException("Maksimalan broj ucesnika mora biti najmanje 1.");
                plan.MaxParticipants = request.MaxParticipants.Value;
            }

            if (request.DurationMinutes.HasValue)
            {
                if (request.DurationMinutes.Value < 15)
                    throw new ArgumentException("Trajanje treninga mora biti najmanje 15 minuta.");
                plan.DurationMinutes = request.DurationMinutes.Value;
            }

            if (request.Location != null)
                plan.Location = request.Location.Trim();

            if (request.ImageUrl != null)
                plan.ImageUrl = request.ImageUrl.Trim();

            if (request.Status.HasValue)
                plan.Status = request.Status.Value;

            
            if (plan.Type == TrainingType.Individual && plan.MaxParticipants != 1)
                throw new ArgumentException("Individualni plan mora imati maksimalno 1 ucesnika.");

            if (plan.Type == TrainingType.Group && plan.MaxParticipants < 2)
                throw new ArgumentException("Grupni plan mora imati najmanje 2 ucesnika.");

            plan.UpdatedAt = DateTime.UtcNow;

            await _planRepository.UpdateAsync(plan);

            var updated = await _planRepository.GetByIdAsync(plan.Id);
            return MapToResponse(updated!);
        }

        public async Task DeleteAsync(Guid id, Guid trainerId)
        {
            var plan = await _planRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Plan treninga nije pronadjen.");

            if (plan.TrainerId != trainerId)
                throw new UnauthorizedAccessException("Nemate pravo da brisete ovaj plan.");

            await _planRepository.DeleteAsync(plan);
        }

        private static void ValidateRequest(
            string title, string description, decimal price,
            int maxParticipants, int durationMinutes, TrainingType type)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Naziv plana je obavezan.");

            if (price < 0)
                throw new ArgumentException("Cena ne moze biti negativna.");

            if (maxParticipants < 1)
                throw new ArgumentException("Maksimalan broj ucesnika mora biti najmanje 1.");

            if (type == TrainingType.Individual && maxParticipants != 1)
                throw new ArgumentException("Individualni plan mora imati maksimalno 1 ucesnika.");
           
            if (type == TrainingType.Group && maxParticipants < 2)
                throw new ArgumentException("Grupni plan mora imati najmanje 2 ucesnika.");

            if (durationMinutes < 15)
                throw new ArgumentException("Trajanje treninga mora biti najmanje 15 minuta.");
        }

        private static TrainingPlanResponse MapToResponse(TrainingPlan plan)
        {
            return new TrainingPlanResponse
            {
                Id = plan.Id,
                TrainerId = plan.TrainerId,
                CategoryId = plan.CategoryId,
                CategoryName = plan.Category?.Name ?? string.Empty,
                Title = plan.Title,
                Description = plan.Description,
                Type = plan.Type,
                Price = plan.Price,
                MaxParticipants = plan.MaxParticipants,
                DurationMinutes = plan.DurationMinutes,
                Location = plan.Location,
                ImageUrl = plan.ImageUrl,
                Status = plan.Status,
                CreatedAt = plan.CreatedAt,
                UpdatedAt = plan.UpdatedAt
            };
        }
    }
    }
