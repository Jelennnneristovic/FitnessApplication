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
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ITrainingPlanRepository _planRepository;

        public EnrollmentService(
            IEnrollmentRepository enrollmentRepository,
            ITrainingPlanRepository planRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _planRepository = planRepository;
        }

        // === KLIJENT ===

        public async Task<EnrollmentResponse> RequestEnrollmentAsync(Guid clientId, CreateEnrollmentRequest request)
        {
            var plan = await _planRepository.GetByIdAsync(request.TrainingPlanId)
                ?? throw new KeyNotFoundException("Plan treninga nije pronadjen.");

            if (plan.Status != TrainingPlanStatus.Active)
                throw new InvalidOperationException("Ne mozete se prijaviti na arhivirani plan.");

            // Klijent ne moze da se prijavi na svoj plan (ako je trener)
            if (plan.TrainerId == clientId)
                throw new InvalidOperationException("Ne mozete se prijaviti na sopstveni plan.");

            // Provera da li klijent vec ima Pending ili Approved zahtev za isti plan
            var existing = await _enrollmentRepository.GetByClientAndPlanAsync(clientId, plan.Id);
            if (existing != null)
            {
                if (existing.Status == EnrollmentStatus.Pending)
                    throw new InvalidOperationException("Vec imate aktivan zahtev za ovaj plan.");

                if (existing.Status == EnrollmentStatus.Approved)
                    throw new InvalidOperationException("Vec ste deo ovog plana.");
            }

            // Provera kapaciteta
            var approvedCount = await _enrollmentRepository.CountApprovedByPlanAsync(plan.Id);
            if (approvedCount >= plan.MaxParticipants)
                throw new InvalidOperationException("Plan je popunjen.");

            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = plan.Id,
                ClientId = clientId,
                Status = EnrollmentStatus.Pending,
                RequestedAt = DateTime.UtcNow,
                ClientNote = request.ClientNote?.Trim()
            };

            await _enrollmentRepository.AddAsync(enrollment);

            var created = await _enrollmentRepository.GetByIdAsync(enrollment.Id);
            return MapToResponse(created!);
        }

        public async Task<IEnumerable<EnrollmentResponse>> GetMyEnrollmentsAsync(Guid clientId, EnrollmentStatus? status)
        {
            var enrollments = await _enrollmentRepository.GetByClientIdAsync(clientId, status);
            return enrollments.Select(MapToResponse);
        }

        public async Task CancelMyEnrollmentAsync(Guid enrollmentId, Guid clientId)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId)
                ?? throw new KeyNotFoundException("Zahtev nije pronadjen.");

            if (enrollment.ClientId != clientId)
                throw new UnauthorizedAccessException("Nemate pravo da otkazete tudji zahtev.");

            if (enrollment.Status != EnrollmentStatus.Pending)
                throw new InvalidOperationException("Mozete otkazati samo zahtev koji ceka odgovor.");

            enrollment.Status = EnrollmentStatus.Cancelled;
            enrollment.RespondedAt = DateTime.UtcNow;

            await _enrollmentRepository.UpdateAsync(enrollment);
        }

        // === TRENER ===

        public async Task<IEnumerable<EnrollmentResponse>> GetEnrollmentsForMyPlansAsync(Guid trainerId, EnrollmentStatus? status)
        {
            var enrollments = await _enrollmentRepository.GetByTrainerIdAsync(trainerId, status);
            return enrollments.Select(MapToResponse);
        }

        public async Task<IEnumerable<EnrollmentResponse>> GetEnrollmentsByPlanAsync(Guid planId, Guid trainerId, EnrollmentStatus? status)
        {
            var plan = await _planRepository.GetByIdAsync(planId)
                ?? throw new KeyNotFoundException("Plan treninga nije pronadjen.");

            if (plan.TrainerId != trainerId)
                throw new UnauthorizedAccessException("Nemate pravo da vidite zahteve za tudji plan.");

            var enrollments = await _enrollmentRepository.GetByPlanIdAsync(planId, status);
            return enrollments.Select(MapToResponse);
        }

        public async Task<EnrollmentResponse> ApproveAsync(Guid enrollmentId, Guid trainerId)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId)
                ?? throw new KeyNotFoundException("Zahtev nije pronadjen.");

            if (enrollment.TrainingPlan.TrainerId != trainerId)
                throw new UnauthorizedAccessException("Nemate pravo da odlucujete o ovom zahtevu.");

            if (enrollment.Status != EnrollmentStatus.Pending)
                throw new InvalidOperationException("Mozete odobriti samo zahtev koji ceka odgovor.");

            // Provera kapaciteta (mogla se promeniti od kreiranja zahteva)
            var approvedCount = await _enrollmentRepository.CountApprovedByPlanAsync(enrollment.TrainingPlanId);
            if (approvedCount >= enrollment.TrainingPlan.MaxParticipants)
                throw new InvalidOperationException("Plan je popunjen. Ne mozete odobriti vise zahteva.");

            enrollment.Status = EnrollmentStatus.Approved;
            enrollment.RespondedAt = DateTime.UtcNow;

            await _enrollmentRepository.UpdateAsync(enrollment);
            return MapToResponse(enrollment);
        }

        public async Task<EnrollmentResponse> RejectAsync(Guid enrollmentId, Guid trainerId, RejectEnrollmentRequest request)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId)
                ?? throw new KeyNotFoundException("Zahtev nije pronadjen.");

            if (enrollment.TrainingPlan.TrainerId != trainerId)
                throw new UnauthorizedAccessException("Nemate pravo da odlucujete o ovom zahtevu.");

            if (enrollment.Status != EnrollmentStatus.Pending)
                throw new InvalidOperationException("Mozete odbiti samo zahtev koji ceka odgovor.");

            enrollment.Status = EnrollmentStatus.Rejected;
            enrollment.RespondedAt = DateTime.UtcNow;
            enrollment.RejectionReason = request.RejectionReason?.Trim();

            await _enrollmentRepository.UpdateAsync(enrollment);
            return MapToResponse(enrollment);
        }

        public async Task RemoveClientFromPlanAsync(Guid enrollmentId, Guid trainerId)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId)
                ?? throw new KeyNotFoundException("Zahtev nije pronadjen.");

            if (enrollment.TrainingPlan.TrainerId != trainerId)
                throw new UnauthorizedAccessException("Nemate pravo da menjate ovaj zahtev.");

            if (enrollment.Status != EnrollmentStatus.Approved)
                throw new InvalidOperationException("Mozete izbaciti samo odobrene klijente.");

            enrollment.Status = EnrollmentStatus.Cancelled;
            enrollment.RespondedAt = DateTime.UtcNow;

            await _enrollmentRepository.UpdateAsync(enrollment);
        }

        // === HELPER ===

        private static EnrollmentResponse MapToResponse(Enrollment e)
        {
            return new EnrollmentResponse
            {
                Id = e.Id,
                TrainingPlanId = e.TrainingPlanId,
                TrainingPlanTitle = e.TrainingPlan?.Title ?? string.Empty,
                TrainerId = e.TrainingPlan?.TrainerId ?? Guid.Empty,
                ClientId = e.ClientId,
                Status = e.Status,
                RequestedAt = e.RequestedAt,
                RespondedAt = e.RespondedAt,
                RejectionReason = e.RejectionReason,
                ClientNote = e.ClientNote
            };
        }
    }
}
