using Backend.Domain.Modules.Locations.Contracts;
using Backend.Domain.Modules.Locations.Models;
using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.EFC.Repositories;

public class LocationRepository(CoursesOnlineDbContext context) : ILocationRepository
{
    private readonly CoursesOnlineDbContext _context = context;

    private static Location ToModel(LocationEntity entity)
        => new(entity.Id, entity.StreetName, entity.PostalCode, entity.City);

    public async Task<Location> CreateLocationAsync(Location location, CancellationToken cancellationToken)
    {
        var entity = new LocationEntity
        {
            StreetName = location.StreetName,
            PostalCode = location.PostalCode,
            City = location.City
        };

        _context.Locations.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return ToModel(entity);
    }

    public async Task<bool> DeleteLocationAsync(int locationId, CancellationToken cancellationToken)
    {
        var entity = await _context.Locations.SingleOrDefaultAsync(l => l.Id == locationId, cancellationToken);

        if (entity == null)
            throw new KeyNotFoundException($"Location '{locationId}' not found.");

        _context.Locations.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IReadOnlyList<Location>> GetAllLocationsAsync(CancellationToken cancellationToken)
    {
        var entities = await _context.Locations
            .AsNoTracking()
            .OrderBy(l => l.City)
            .ThenBy(l => l.StreetName)
            .ToListAsync(cancellationToken);

        return [.. entities.Select(ToModel)];
    }

    public async Task<Location?> GetLocationByIdAsync(int locationId, CancellationToken cancellationToken)
    {
        var entity = await _context.Locations
            .AsNoTracking()
            .SingleOrDefaultAsync(l => l.Id == locationId, cancellationToken);

        return entity == null ? null : ToModel(entity);
    }

    public async Task<Location?> UpdateLocationAsync(Location location, CancellationToken cancellationToken)
    {
        var entity = await _context.Locations.SingleOrDefaultAsync(l => l.Id == location.Id, cancellationToken);

        if (entity == null)
            throw new KeyNotFoundException($"Location '{location.Id}' not found.");

        entity.StreetName = location.StreetName;
        entity.PostalCode = location.PostalCode;
        entity.City = location.City;

        await _context.SaveChangesAsync(cancellationToken);

        return ToModel(entity);
    }

    public async Task<bool> HasInPlaceLocationsAsync(int locationId, CancellationToken cancellationToken)
    {
        return await _context.InPlaceLocations
            .AsNoTracking()
            .AnyAsync(ipl => ipl.LocationId == locationId, cancellationToken);
    }
}
