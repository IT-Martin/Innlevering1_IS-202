using System.Security.Claims;
using Reachout.DataAccess;
using Reachout.DataAccess.Repositories;
using Reachout.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Reachout.Controllers;
[Authorize(Roles = "Private,Public,Platform")]
public class MapController : Controller
{
    private readonly IResourceRepository resourceRepository;
    private readonly ApplicationDbContext context;

    public MapController(IResourceRepository resourceRepository, ApplicationDbContext context)
    {
        this.resourceRepository = resourceRepository;
        this.context = context;
    }

    [Authorize(Roles = "Private,Public,Platform")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var accountId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? (int?)id : null;
        if (accountId is null)
        {
            return Challenge();
        }

        return View(await GetMapItemsAsync(accountId.Value));
    }

    private async Task<List<MapItemViewModel>> GetMapItemsAsync(int accountId)
    {
        var resources = User.IsInRole("Private") ? await resourceRepository.GetByAccountIdAsync(accountId) : await resourceRepository.GetAllAsync();
        var viewModels = resources.Where(resource => !resource.archived).Select(resource => new MapItemViewModel { Id = resource.resource_id, Name = resource.resource_name, Category = resource.ResourceType.category_name, Place = resource.Location.address_or_area, Status = resource.availability_status, Url = "/Resources/Details/" + resource.resource_id, TypeIcon = ResourceTypeIcon.For(resource.ResourceType.category_name, resource.resource_name), Latitude = (double)resource.Location.latitude, Longitude = (double)resource.Location.longitude }).ToList();
        var needs = await context.ResourceNeeds.Include(need => need.ResourceType).Include(need => need.Location).AsNoTracking().Where(need => !need.archived && need.status != "Løst" && need.required_end > DateTime.Now && (!need.deadline.HasValue || need.deadline >= DateTime.Now)).ToListAsync();
        viewModels.AddRange(needs.Select(need => new MapItemViewModel { Id = need.resource_need_id, Name = need.title, Kind = "Behov", Category = need.ResourceType.category_name, Place = need.Location.address_or_area, Status = need.priority, Url = "/Needs/Details/" + need.resource_need_id, TypeIcon = ResourceTypeIcon.For(need.ResourceType.category_name, need.title), Latitude = (double)need.Location.latitude, Longitude = (double)need.Location.longitude }));
        return viewModels;
    }

}
