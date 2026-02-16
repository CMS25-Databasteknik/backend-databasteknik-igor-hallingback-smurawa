using Backend.Domain.Modules.InPlaceLocations.Contracts;
using Backend.Domain.Modules.InPlaceLocations.Models;
using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.EFC.Repositories;

public class InPlaceLocationRepository(CoursesOnlineDbContext context) : IInPlaceLocationRepository
{
    private readonly CoursesOnlineDbContext _context = context;

    private static InPlaceLocation ToModel(InPlaceLocationEntity entity)
        => new(entity.Id, entity.LocationId, entity.RoomNumber, entity.Seats);

    public async Task<InPlaceLocation> CreateInPlaceLocationAsync(InPlaceLocation inPlaceLocation, CancellationToken cancellationToken)
    {
        var entity = new InPlaceLocationEntity
        {
            LocationId = inPlaceLocation.LocationId,
            RoomNumber = inPlaceLocation.RoomNumber,
            Seats = inPlaceLocation.Seats
        };

        _context.InPlaceLocations.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return ToModel(entity);
    }

    public async Task<bool> DeleteInPlaceLocationAsync(int inPlaceLocationId, CancellationToken cancellationToken)
    {
        var entity = await _context.InPlaceLocations.SingleOrDefaultAsync(ipl => ipl.Id == inPlaceLocationId, cancellationToken);

        if (entity == null)
            throw new KeyNotFoundException($"In-place location '{inPlaceLocationId}' not found.");

        _context.InPlaceLocations.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IReadOnlyList<InPlaceLocation>> GetAllInPlaceLocationsAsync(CancellationToken cancellationToken)
    {
        var entities = await _context.InPlaceLocations
            .AsNoTracking()
            .OrderBy(ipl => ipl.LocationId)
            .ThenBy(ipl => ipl.RoomNumber)
            .ToListAsync(cancellationToken);

        return [.. entities.Select(ToModel)];
    }

    public async Task<InPlaceLocation?> GetInPlaceLocationByIdAsync(int inPlaceLocationId, CancellationToken cancellationToken)
    {
        var entity = await _context.InPlaceLocations
            .AsNoTracking()
            .SingleOrDefaultAsync(ipl => ipl.Id == inPlaceLocationId, cancellationToken);

        return entity == null ? null : ToModel(entity);
    }

    public async Task<IReadOnlyList<InPlaceLocation>> GetInPlaceLocationsByLocationIdAsync(int locationId, CancellationToken cancellationToken)
    {
        var entities = await _context.InPlaceLocations
            .AsNoTracking()
            .Where(ipl => ipl.LocationId == locationId)
            .OrderBy(ipl => ipl.RoomNumber)
            .ToListAsync(cancellationToken);

        return [.. entities.Select(ToModel)];
    }

    public async Task<InPlaceLocation?> UpdateInPlaceLocationAsync(InPlaceLocation inPlaceLocation, CancellationToken cancellationToken)
    {
        var entity = await _context.InPlaceLocations.SingleOrDefaultAsync(ipl => ipl.Id == inPlaceLocation.Id, cancellationToken);

        if (entity == null)
            throw new KeyNotFoundException($"In-place location '{inPlaceLocation.Id}' not found.");

        entity.LocationId = inPlaceLocation.LocationId;
        entity.RoomNumber = inPlaceLocation.RoomNumber;
        entity.Seats = inPlaceLocation.Seats;

        await _context.SaveChangesAsync(cancellationToken);

        return ToModel(entity);
    }

    public async Task<bool> HasCourseEventsAsync(int inPlaceLocationId, CancellationToken cancellationToken)
    {
        return await _context.InPlaceLocations
            .AsNoTracking()
            .Where(ipl => ipl.Id == inPlaceLocationId)
            .SelectMany(ipl => ipl.CourseEvents)
            .AnyAsync(cancellationToken);
    }
}
