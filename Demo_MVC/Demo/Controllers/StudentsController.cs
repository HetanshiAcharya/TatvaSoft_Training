using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Entity.DataContext;
using Entity.DataModels;
using Demo.Models;
using System.Collections;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Linq;

namespace Demo.Controllers
{
    public class StudentsController : Controller
    {
        private readonly DemoDbContext _context;

        public StudentsController(DemoDbContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index(PaginatedViewModel<Student> formdata)
        {
            var res = await (from req in _context.Students
                             where (formdata.value == null || req.Name.ToLower().Contains(formdata.value.ToLower()))
                             select new Student()
                             {
                                 Name = req.Name,
                                 Department = req.Department,
                                 Semester = req.Semester,
                                 Age = req.Age,
                                 Fees = req.Fees,
                                 Id = req.Id,
                             }
                               ).ToListAsync();
            switch (formdata.SortColumn)
            {
                case "name":
                    res = formdata.SortOrder == false ? res.OrderByDescending(req => req.Name).ToList() : res.OrderBy(req => req.Name).ToList();
                    break;
                case "department":
                    res = formdata.SortOrder == false ? res.OrderByDescending(req => req.Department).ToList() : res.OrderBy(req => req.Department).ToList();
                    break;
            }
            int totalitemcount = res.Count();
            int totalPages = totalitemcount / formdata.PageSize;
            List<Student> list = res.Skip((formdata.CurrentPage - 1) * formdata.PageSize).Take(formdata.PageSize).ToList();
            PaginatedViewModel<Student> newres = new PaginatedViewModel<Student>()
            {
                list = list,
                CurrentPage = formdata.CurrentPage,
                TotalPages = totalPages,
            };
            return View(newres);

        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student1 req)
        {
            if (ModelState.IsValid)
            {
                var res = new Student()
                {
                    Name = req.Name,
                    Department = req.Department,
                    Semester = req.Semester,
                    Age = req.Age,
                    Fees = req.Fees,
                    Id = req.Id,
                    PhotoId = ""
            };

                if (req.PhotoId != null)
                {
                    string FilePath = "wwwroot\\Upload";
                    string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string fileNameWithPath = Path.Combine(path, req.PhotoId.FileName);

                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        await req.PhotoId.CopyToAsync(stream);
                    }

                    // Set the PhotoPath property with the relative path to the file
                    res.PhotoId = Path.Combine(FilePath, req.PhotoId.FileName);
                }
                    _context.Add(res);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(req);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await (from req in _context.Students
                                where(req.Id==id)
                                select new Student1()
                                {
                                    Name = req.Name,
                                    Department = req.Department,
                                    Semester = req.Semester,
                                    Age = req.Age,
                                    Fees = req.Fees,
                                    Id = req.Id,
                                    

                                }).ToListAsync();
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  Student1 student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }
            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'DemoDbContext.Students'  is null.");
            }
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return (_context.Students?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
