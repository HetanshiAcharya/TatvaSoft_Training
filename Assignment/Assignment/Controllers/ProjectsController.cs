using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssignmentEntity.DataContext;
using AssignmentEntity.DataModels;
using AssignmentEntity.ViewModels;


namespace Assignment.Controllers
{

    public class ProjectsController : Controller

    {
        private readonly AssignmentDbContext _context;

        public ProjectsController(AssignmentDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(ProjectDetails p)
        {
            ViewBag.domains = _context.Domains.ToList();
            var projects = _context.Projects
                            .Select(p => new ProjectDetails
                            {
                                ProjectName = p.ProjectName,
                                Assignee = p.Assignee,
                                Description = p.Description,
                                DueDate = p.DueDate,
                                Domain = p.Domain,
                                City = p.City
                            }).ToList();

            return View(projects);
        }
        public IActionResult CreateProject()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateProjectPost(ProjectDetails p)
        {
            var dname = _context.Domains.Where(req => req.Id == Convert.ToInt32(p.Domain)).Select(req=>req.Name).ToString();

            Project res = new Project
            {
                ProjectName = p.ProjectName,
                Assignee = p.Assignee,
                Description = p.Description,
                DueDate = p.DueDate,
                Domain = dname,
                City = p.City,
                DomainId=Convert.ToInt32(p.Domain),
            };
            _context.Projects.Add(res);
            _context.SaveChanges();
            return View("Index");
        }
        public IActionResult DeleteProject(int Id)
        {
            var res = _context.Projects.SingleOrDefault(req => req.Id == Id);
            _context.Projects.Remove(res);
            _context.SaveChanges();
                       
            return View("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var student = await (from p in _context.Projects
                                 where (p.Id == id)
                                 select new ProjectDetails()
                                 {
                                     ProjectName = p.ProjectName,
                                     Assignee = p.Assignee,
                                     Description = p.Description,
                                     DueDate = p.DueDate,
                                     City = p.City,
                                     DomainId = Convert.ToInt32(p.Domain),

                                 }).ToListAsync();
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }
    }
}
