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
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly ITrainingPlanRepository _planRepository;

        public SessionService(
            ISessionRepository sessionRepository,
            ITrainingPlanRepository planRepository)
        {
            _sessionRepository = sessionRepository;
            _planRepository = planRepository;
        }

        public async Task<SessionResponse> CreateAsync(Guid trainerId, CreateSessionRequest request)
        {
            var plan = await _planRepository.GetByIdAsync(request.TrainingPlanId)
                ?? throw new KeyNotFoundException("Plan treninga nije pronadjen.");

            if (plan.TrainerId != trainerId)
                throw new UnauthorizedAccessException("Nemate pravo da kreirate sesije za tudji plan.");

            if (plan.Status != TrainingPlanStatus.Active)
                throw new InvalidOperationException("Ne mozete kreirati sesije za arhivirani plan.");

            ValidateTimes(request.StartTime, request.EndTime);

            var session = new TrainingSession
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = plan.Id,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Status = TrainingSessionStatus.Scheduled,
                Notes = request.Notes?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _sessionRepository.AddAsync(session);

            var created = await _sessionRepository.GetByIdAsync(session.Id);
            return MapToResponse(created!);
        }

        public async Task<SessionResponse> UpdateAsync(Guid sessionId, Guid trainerId, UpdateSessionRequest request)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId)
                ?? throw new KeyNotFoundException("Sesija nije pronadjena.");

            if (session.TrainingPlan.TrainerId != trainerId)
                throw new UnauthorizedAccessException("Nemate pravo da menjate ovu sesiju.");

            var newStart = request.StartTime ?? session.StartTime;
            var newEnd = request.EndTime ?? session.EndTime;

            if (request.StartTime.HasValue || request.EndTime.HasValue)
                ValidateTimes(newStart, newEnd);

            session.StartTime = newStart;
            session.EndTime = newEnd;

            if (request.Status.HasValue)
                session.Status = request.Status.Value;

            if (request.Notes != null)
                session.Notes = request.Notes.Trim();

            session.UpdatedAt = DateTime.UtcNow;

            await _sessionRepository.UpdateAsync(session);
            return MapToResponse(session);
        }

        public async Task DeleteAsync(Guid sessionId, Guid trainerId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId)
                ?? throw new KeyNotFoundException("Sesija nije pronadjena.");

            if (session.TrainingPlan.TrainerId != trainerId)
                throw new UnauthorizedAccessException("Nemate pravo da brisete ovu sesiju.");

            await _sessionRepository.DeleteAsync(session);
        }

        public async Task<IEnumerable<SessionResponse>> GetByPlanAsync(Guid planId)
        {
            var sessions = await _sessionRepository.GetByPlanIdAsync(planId);
            return sessions.Select(MapToResponse);
        }

        public async Task<IEnumerable<SessionResponse>> GetMyTrainerScheduleAsync(Guid trainerId, DateTime? from, DateTime? to)
        {
            var sessions = await _sessionRepository.GetByTrainerIdAsync(trainerId, from, to);
            return sessions.Select(MapToResponse);
        }

        public async Task<IEnumerable<SessionResponse>> GetMyClientScheduleAsync(Guid clientId, DateTime? from, DateTime? to)
        {
            var sessions = await _sessionRepository.GetByClientIdAsync(clientId, from, to);
            return sessions.Select(MapToResponse);
        }

        public async Task<SessionResponse> GetByIdAsync(Guid sessionId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId)
                ?? throw new KeyNotFoundException("Sesija nije pronadjena.");

            return MapToResponse(session);
        }

        // === HELPERS ===

        private static void ValidateTimes(DateTime start, DateTime end)
        {
            if (end <= start)
                throw new ArgumentException("Kraj sesije mora biti posle pocetka.");

            var duration = end - start;
            if (duration.TotalMinutes < 15)
                throw new ArgumentException("Sesija mora trajati najmanje 15 minuta.");

            if (duration.TotalHours > 8)
                throw new ArgumentException("Sesija ne moze trajati duze od 8 sati.");
        }

        private static SessionResponse MapToResponse(TrainingSession s)
        {
            return new SessionResponse
            {
                Id = s.Id,
                TrainingPlanId = s.TrainingPlanId,
                TrainingPlanTitle = s.TrainingPlan?.Title ?? string.Empty,
                TrainerId = s.TrainingPlan?.TrainerId ?? Guid.Empty,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Status = s.Status,
                Notes = s.Notes
            };
        }
    }
}
