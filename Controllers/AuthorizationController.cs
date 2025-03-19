using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AuthorizationSystem.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace AuthorizationSystem.Controllers
{
    [Authorize] // Ensures only authenticated users can access
    public class AuthorizationController : Controller
    {
        private readonly ILogger<AuthorizationController> _logger;
        private static List<Role> _roles = new List<Role>(); // Temporary in-memory storage

        public AuthorizationController(ILogger<AuthorizationController> logger)
        {
            _logger = logger;
            
            // Initialize with default roles if empty
            if (!_roles.Any())
            {
                InitializeDefaultRoles();
            }
        }

        private void InitializeDefaultRoles()
        {
            // CEO Role
            var ceoRole = new Role { Id = 1, Name = "CEO", Description = "Chief Executive Officer" };
            
            // COO Role
            var cooRole = new Role { Id = 2, Name = "COO", ParentRoleId = 1, Description = "Chief Operating Officer" };
            
            // Country Manager Role
            var countryManagerRole = new Role { Id = 3, Name = "Country Manager", ParentRoleId = 2, Description = "Country Level Management" };
            
            // Regional Manager Role
            var regionalManagerRole = new Role { Id = 4, Name = "Regional Manager", ParentRoleId = 3, Description = "Regional Level Management" };
            
            // Finance Office Role
            var financeOfficeRole = new Role { Id = 5, Name = "Finance Office", ParentRoleId = 3, Description = "Financial Operations" };
            
            // HR Manager Role
            var hrManagerRole = new Role { Id = 6, Name = "HR Manager", ParentRoleId = 3, Description = "Human Resources Management" };
            
            // HR Sub-roles
            var leaveSanctionRole = new Role { Id = 7, Name = "Leave Sanction", ParentRoleId = 6, Description = "Leave Management" };
            var hiringRole = new Role { Id = 8, Name = "Hiring Section", ParentRoleId = 6, Description = "Recruitment Management" };
            var equipmentRole = new Role { Id = 9, Name = "Equipment Management Section", ParentRoleId = 6, Description = "Equipment Management" };

            _roles.AddRange(new[] { 
                ceoRole, cooRole, countryManagerRole, regionalManagerRole, 
                financeOfficeRole, hrManagerRole, leaveSanctionRole, 
                hiringRole, equipmentRole 
            });
        }

        public IActionResult Index()
        {
            try
            {
                var rootRoles = _roles.Where(r => !r.ParentRoleId.HasValue).ToList();
                foreach (var role in rootRoles)
                {
                    BuildRoleHierarchy(role);
                }
                return View(rootRoles);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Index: {ex.Message}");
                return View("Error");
            }
        }

        private void BuildRoleHierarchy(Role role)
        {
            role.Children = _roles.Where(r => r.ParentRoleId == role.Id).ToList();
            foreach (var child in role.Children)
            {
                BuildRoleHierarchy(child);
            }
        }

        public IActionResult Create()
        {
            ViewBag.ParentRoles = _roles.Select(r => new { r.Id, r.Name }).ToList();
            return View(new Role());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Role role)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    role.Id = _roles.Max(r => r.Id) + 1;
                    _roles.Add(role);
                    _logger.LogInformation($"Role created: {role.Name}");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating role: {ex.Message}");
                ModelState.AddModelError("", "Error creating role");
            }

            ViewBag.ParentRoles = _roles.Select(r => new { r.Id, r.Name }).ToList();
            return View(role);
        }

        public IActionResult Edit(int id)
        {
            var role = _roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            ViewBag.ParentRoles = _roles.Where(r => r.Id != id).Select(r => new { r.Id, r.Name }).ToList();
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Role role)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingRole = _roles.FirstOrDefault(r => r.Id == role.Id);
                    if (existingRole != null)
                    {
                        existingRole.Name = role.Name;
                        existingRole.Description = role.Description;
                        existingRole.ParentRoleId = role.ParentRoleId;
                        existingRole.LastModified = DateTime.UtcNow;
                        _logger.LogInformation($"Role updated: {role.Name}");
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating role: {ex.Message}");
                ModelState.AddModelError("", "Error updating role");
            }

            ViewBag.ParentRoles = _roles.Where(r => r.Id != role.Id).Select(r => new { r.Id, r.Name }).ToList();
            return View(role);
        }

        public IActionResult Details(int id)
        {
            var role = _roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            BuildRoleHierarchy(role);
            return View(role);
        }

        public IActionResult Delete(int id)
        {
            var role = _roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var role = _roles.FirstOrDefault(r => r.Id == id);
                if (role != null)
                {
                    // Check if role has children
                    if (_roles.Any(r => r.ParentRoleId == id))
                    {
                        ModelState.AddModelError("", "Cannot delete role with subordinate roles");
                        return View(role);
                    }

                    _roles.Remove(role);
                    _logger.LogInformation($"Role deleted: {role.Name}");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting role: {ex.Message}");
                ModelState.AddModelError("", "Error deleting role");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}