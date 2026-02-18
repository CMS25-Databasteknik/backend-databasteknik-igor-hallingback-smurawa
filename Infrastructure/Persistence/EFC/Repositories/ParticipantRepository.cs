using Backend.Domain.Modules.ParticipantContactTypes.Models;
using Backend.Domain.Modules.Participants.Contracts;
using Backend.Domain.Modules.Participants.Models;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.EFC.Repositories;

public class ParticipantRepository(CoursesOnlineDbContext context) : IParticipantRepository
{
    private readonly CoursesOnlineDbContext _context = context;

    private static Participant ToModel(ParticipantEntity entity)
        => new(entity.Id, entity.FirstName, entity.LastName, entity.Email, entity.PhoneNumber, (ParticipantContactType)entity.ContactTypeId);

    public async Task<Participant> CreateParticipantAsync(Participant participant, CancellationToken cancellationToken)
    {
        var entity = new ParticipantEntity
        {
            Id = participant.Id,
            FirstName = participant.FirstName,
            LastName = participant.LastName,
            Email = participant.Email,
            PhoneNumber = participant.PhoneNumber,
            ContactTypeId = (int)participant.ContactType
        };

        _context.Participants.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return ToModel(entity);
    }

    public async Task<bool> DeleteParticipantAsync(Guid participantId, CancellationToken cancellationToken)
    {
        using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var exists = await _context.Participants
                .AnyAsync(p => p.Id == participantId, cancellationToken);

            if (!exists)
                throw new KeyNotFoundException($"Participant '{participantId}' not found.");

            await _context.Database.ExecuteSqlAsync(
                $"DELETE FROM CourseRegistrations WHERE ParticipantId = {participantId}",
                cancellationToken);

            await _context.Database.ExecuteSqlAsync(
                $"DELETE FROM Participants WHERE Id = {participantId}",
                cancellationToken);

            await tx.CommitAsync(cancellationToken);
            return true;
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<IReadOnlyList<Participant>> GetAllParticipantsAsync(CancellationToken cancellationToken)
    {
        var entities = await _context.Participants
            .AsNoTracking()
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync(cancellationToken);

        return [.. entities.Select(ToModel)];
    }

    public async Task<Participant?> GetParticipantByIdAsync(Guid participantId, CancellationToken cancellationToken)
    {
        var entity = await _context.Participants
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == participantId, cancellationToken);

        return entity == null ? null : ToModel(entity);
    }

    public async Task<Participant?> UpdateParticipantAsync(Participant participant, CancellationToken cancellationToken)
    {
        var entity = await _context.Participants.SingleOrDefaultAsync(p => p.Id == participant.Id, cancellationToken);

        if (entity == null)
            throw new KeyNotFoundException($"Participant '{participant.Id}' not found.");

        entity.FirstName = participant.FirstName;
        entity.LastName = participant.LastName;
        entity.Email = participant.Email;
        entity.PhoneNumber = participant.PhoneNumber;
        entity.ContactTypeId = (int)participant.ContactType;
        entity.ModifiedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return ToModel(entity);
    }

    public async Task<bool> HasRegistrationsAsync(Guid participantId, CancellationToken cancellationToken)
    {
        return await _context.CourseRegistrations
            .AsNoTracking()
            .AnyAsync(cr => cr.ParticipantId == participantId, cancellationToken);
    }
}




