using System.Text;
using Reachout.DataAccess;
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

    public async Task<IActionResult> Index()
    {
        var rows = await context.Resources.Include(resource => resource.ResourceType).Include(resource => resource.Location).AsNoTracking().Where(resource => !resource.archived).Select(resource => new { resource.resource_name, resource.ResourceType.category_name, resource.Location.address_or_area, resource.Location.latitude, resource.Location.longitude, resource.availability_status }).ToListAsync();
        var csv = new StringBuilder("Navn;Kategori;Sted;Breddegrad;Lengdegrad;Tilgjengelighet\r\n");
        foreach (var row in rows)
        {
            csv.AppendLine(string.Join(';', Cell(row.resource_name), Cell(row.category_name), Cell(row.address_or_area), row.latitude.ToString(System.Globalization.CultureInfo.InvariantCulture), row.longitude.ToString(System.Globalization.CultureInfo.InvariantCulture), Cell(row.availability_status)));
        }

        return File(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray(), "text/csv; charset=utf-8", "ressurser.csv");
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
