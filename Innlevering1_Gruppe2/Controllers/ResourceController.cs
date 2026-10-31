
using Innlevering1_Gruppe2.Models.Entities;
using Innlevering1_Gruppe2.Models.ViewModels.Resource;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Innlevering1_Gruppe2.Controllers
{
    public class ResourceController : Controller
    {
        private readonly Innlevering1_Gruppe2.DataAccess.Repositories.IResourceRepository _repo;
        
        public ResourceController(Innlevering1_Gruppe2.DataAccess.Repositories.IResourceRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult ResourceForm()
        {
            var vm = new ResourceViewModel();
            return View(vm);
        }


        [Authorize]
        [HttpPost]
        public IActionResult ResourceOverview(ResourceViewModel resViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("ResourceForm", resViewModel);
            }

            var accountIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(accountIdValue, out var accountId))
            {
                return Challenge();
            }

            var resource = new Resource
            {
                resource_name = resViewModel.Name ?? string.Empty,
                description = resViewModel.Description ?? string.Empty,
                contact_details = resViewModel.ContactDetails ?? string.Empty,
                verification_status = "Pending",
                account_id = accountId,
                resource_type_id = resViewModel.ResourceTypeId,
                location_id = resViewModel.LocationId
            };


            _repo.AddAsync(resource).GetAwaiter().GetResult();

            return View(resViewModel);
        }




    }
}
