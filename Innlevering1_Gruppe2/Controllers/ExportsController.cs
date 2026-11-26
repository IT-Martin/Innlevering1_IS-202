using System.Text;
using Reachout.DataAccess;
using Reachout.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Reachout.Controllers;
[Authorize(Roles = "Public")]
public class ExportsController : Controller
{
    private readonly ApplicationDbContext context;
    public ExportsController(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<IActionResult> Index(string? region)
    {
        if (!string.IsNullOrWhiteSpace(region) && !ResourceExportViewModel.Regions.Contains(region, StringComparer.Ordinal))
        {
            return BadRequest("Ugyldig region.");
        }

        var query = context.Resources.Include(resource => resource.ResourceType).Include(resource => resource.Location).AsNoTracking().Where(resource => !resource.archived);
        if (!string.IsNullOrWhiteSpace(region))
        {
            query = query.Where(resource => resource.Location.address_or_area == region);
        }

        var rows = await query.Select(resource => new { resource.resource_name, resource.ResourceType.category_name, resource.Location.address_or_area, resource.Location.latitude, resource.Location.longitude, resource.availability_status }).ToListAsync();
        var csv = new StringBuilder("Navn;Kategori;Sted;Breddegrad;Lengdegrad;Tilgjengelighet\r\n");
        foreach (var row in rows)
        {
            csv.AppendLine(string.Join(';', Cell(row.resource_name), Cell(row.category_name), Cell(row.address_or_area), row.latitude.ToString(System.Globalization.CultureInfo.InvariantCulture), row.longitude.ToString(System.Globalization.CultureInfo.InvariantCulture), Cell(row.availability_status)));
        }

        var filename = string.IsNullOrWhiteSpace(region) ? "ressurser.csv" : $"ressurser-{region.ToLowerInvariant()}.csv";
        return File(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray(), "text/csv; charset=utf-8", filename);
    }

    [HttpGet]
    public async Task<IActionResult> Print(string? region)
    {
        if (!string.IsNullOrWhiteSpace(region) && !ResourceExportViewModel.Regions.Contains(region, StringComparer.Ordinal))
        {
            return BadRequest("Ugyldig region.");
        }

        var query = context.Resources.Include(resource => resource.ResourceType).Include(resource => resource.Location).AsNoTracking().Where(resource => !resource.archived);
        if (!string.IsNullOrWhiteSpace(region))
        {
            query = query.Where(resource => resource.Location.address_or_area == region);
        }

        var resources = await query.OrderBy(resource => resource.Location.address_or_area).ThenBy(resource => resource.resource_name).Select(resource => new ResourceExportRow
        {
            Name = resource.resource_name,
            Category = resource.ResourceType.category_name,
            Region = resource.Location.address_or_area,
            Latitude = resource.Location.latitude,
            Longitude = resource.Location.longitude,
            Availability = resource.availability_status
        }).ToListAsync();

        return View(new ResourceExportViewModel { Region = region, Resources = resources });
    }

    private static string Cell(string value)
    {
        if (value.TrimStart().StartsWith('=') || value.TrimStart().StartsWith('+') || value.TrimStart().StartsWith('-') || value.TrimStart().StartsWith('@'))
        {
            value = "'" + value;
        }

        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }
}
