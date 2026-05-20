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
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public AttendanceService(
            IAttendanceRepository attendanceRepository,
            ISessionRepository sessionRepository,
            IEnrollmentRepository enrollmentRepository)
        {
            _attendanceRepository = attendanceRepository;
            _sessionRepository = sessionRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<AttendanceResponse> MarkAsync(Guid sessionId, Guid clientId, Guid markedByUserId, MarkAttendanceRequest request)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId)
                ?? throw new KeyNotFoundException("Sesija nije pronadjena.");

            // Provera da je klijent Approved na ovom planu
            var enrollment = await _enrollmentRepository.GetByClientAndPlanAsync(clientId, session.TrainingPlanId);
            if (enrollment == null || enrollment.Status != EnrollmentStatus.Approved)
                throw new InvalidOperationException("Klijent nije deo ovog plana.");

            // Provera ko oznacava: ili sam klijent ili trener tog plana
            var isClient = markedByUserId == clientId;
            var isTrainer = markedByUserId == session.TrainingPlan.TrainerId;

            if (!isClient && !isTrainer)
                throw new UnauthorizedAccessException("Nemate pravo da oznacite dolazak.");

            // Sesija ne sme biti otkazana
            if (session.Status == TrainingSessionStatus.Cancelled)
                throw new InvalidOperationException("Sesija je otkazana.");

            // Ne moze unapred (osim ako trener to zeli, sad necemo zabraniti)
            // if (session.StartTime > DateTime.UtcNow)
            //     throw new InvalidOperationException("Sesija jos nije bila.");

            // Da li vec postoji Attendance za ovu sesiju i klijenta?
            var existing = await _attendanceRepository.GetBySessionAndClientAsync(sessionId, clientId);

            if (existing != null)
            {
                // Update postojeci
                existing.Attended = request.Attended;
                existing.MarkedAt = DateTime.UtcNow;
                existing.MarkedByUserId = markedByUserId;
                existing.Notes = request.Notes?.Trim();

                await _attendanceRepository.UpdateAsync(existing);
                return MapToResponse(existing);
            }

            // Kreiraj novi
            var attendance = new Attendance
            {
                Id = Guid.NewGuid(),
                TrainingSessionId = sessionId,
                ClientId = clientId,
                Attended = request.Attended,
                MarkedAt = DateTime.UtcNow,
                MarkedByUserId = markedByUserId,
                Notes = request.Notes?.Trim()
            };

            await _attendanceRepository.AddAsync(attendance);

            var created = await _attendanceRepository.GetByIdAsync(attendance.Id);
            return MapToResponse(created!);
        }

        public async Task<IEnumerable<AttendanceResponse>> GetMyHistoryAsync(Guid clientId)
        {
            var attendances = await _attendanceRepository.GetByClientIdAsync(clientId);
            return attendances.Select(MapToResponse);
        }

        public async Task<IEnumerable<AttendanceResponse>> GetBySessionAsync(Guid sessionId, Guid trainerId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId)
                ?? throw new KeyNotFoundException("Sesija nije pronadjena.");

            if (session.TrainingPlan.TrainerId != trainerId)
                throw new UnauthorizedAccessException("Nemate pravo da vidite listu dolazaka.");

            var attendances = await _attendanceRepository.GetBySessionIdAsync(sessionId);
            return attendances.Select(MapToResponse);
        }

        private static AttendanceResponse MapToResponse(Attendance a)
        {
            return new AttendanceResponse
            {
                Id = a.Id,
                TrainingSessionId = a.TrainingSessionId,
                SessionStartTime = a.TrainingSession?.StartTime ?? default,
                TrainingPlanTitle = a.TrainingSession?.TrainingPlan?.Title ?? string.Empty,
                ClientId = a.ClientId,
                Attended = a.Attended,
                MarkedAt = a.MarkedAt,
                MarkedByUserId = a.MarkedByUserId,
                Notes = a.Notes
            };
        }
    }
}
