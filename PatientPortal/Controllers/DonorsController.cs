using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Models;

namespace PatientPortal.Controllers
{
    public class DonorsController : Controller
    {
        private readonly PatientPortalContext _context;

        public DonorsController(PatientPortalContext context)
        {
            _context = context;
        }

        // GET: Donors
        public async Task<IActionResult> Index()
        {
            var patientPortalContext = _context.Donors.Include(d => d.FamilyPatient);
            return View(await patientPortalContext.ToListAsync());
        }

        // GET: Donors/Details/5
        public async Task<IActionResult> Details(ulong? id)
        {
            if (id == null || _context.Donors == null)
            {
                return NotFound();
            }

            var donor = await _context.Donors
                .Include(d => d.FamilyPatient)
                .FirstOrDefaultAsync(m => m.DonorId == id);
            if (donor == null)
            {
                return NotFound();
            }

            return View(donor);
        }

        // GET: Donors/Create
        public IActionResult Create()
        {
            ViewData["FamilyPatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId");
            return View();
        }

        // POST: Donors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DonorId,Name,Sex,Age,BloodType,PastHistory,City,State,PatientRelation,FamilyPatientId")] Donor donor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(donor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FamilyPatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", donor.FamilyPatientId);
            return View(donor);
        }

        // GET: Donors/Edit/5
        public async Task<IActionResult> Edit(ulong? id)
        {
            if (id == null || _context.Donors == null)
            {
                return NotFound();
            }

            var donor = await _context.Donors.FindAsync(id);
            if (donor == null)
            {
                return NotFound();
            }
            ViewData["FamilyPatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", donor.FamilyPatientId);
            return View(donor);
        }

        // POST: Donors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ulong id, [Bind("DonorId,Name,Sex,Age,BloodType,PastHistory,City,State,PatientRelation,FamilyPatientId")] Donor donor)
        {
            if (id != donor.DonorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(donor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DonorExists(donor.DonorId))
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
            ViewData["FamilyPatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", donor.FamilyPatientId);
            return View(donor);
        }

        // GET: Donors/Delete/5
        public async Task<IActionResult> Delete(ulong? id)
        {
            if (id == null || _context.Donors == null)
            {
                return NotFound();
            }

            var donor = await _context.Donors
                .Include(d => d.FamilyPatient)
                .FirstOrDefaultAsync(m => m.DonorId == id);
            if (donor == null)
            {
                return NotFound();
            }

            return View(donor);
        }

        // POST: Donors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ulong id)
        {
            if (_context.Donors == null)
            {
                return Problem("Entity set 'PatientPortalContext.Donors'  is null.");
            }
            var donor = await _context.Donors.FindAsync(id);
            if (donor != null)
            {
                _context.Donors.Remove(donor);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DonorExists(ulong id)
        {
          return (_context.Donors?.Any(e => e.DonorId == id)).GetValueOrDefault();
        }
    }
}
