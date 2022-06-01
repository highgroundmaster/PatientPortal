using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var patientPortalContext = _context.Donors.Include(d => d.FamilyPatient);
            return View(await patientPortalContext.ToListAsync());
        }

        // GET: Donors/Details/5
        [Authorize(Roles = "Admin, User")]
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
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Create(ulong familyPatientId)
        {
            if (_context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FirstOrDefaultAsync(m => m.PatientId == familyPatientId);
            if (patient == null)
            {
                return NotFound();
            }
            ViewData["FamilyPatientName"] = patient.Name;

            TempData["FamilyPatientId"] = familyPatientId.ToString();

            return View();
        }

        // POST: Donors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DonorId,Name,Sex,Age,BloodType,PastHistory,City,State,PatientRelation")] Donor donor)
        {

            if (TempData["FamilyPatientId"] != null)
            {
                donor.FamilyPatientId = Convert.ToUInt64(TempData["FamilyPatientId"]);

                var patient = await _context.Patients.FirstOrDefaultAsync(m => m.PatientId == donor.FamilyPatientId);
                if (patient == null)
                {
                    return NotFound();
                }
                donor.FamilyPatient = patient;

            }
            if (ModelState.IsValid)
            {
                _context.Add(donor);

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(donor);
        }

        // GET: Donors/Edit/5
        [Authorize(Roles = "Admin, User")]
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
        [Authorize(Roles = "Admin, User")]
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
        [Authorize(Roles = "Admin, User")]
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
        [Authorize(Roles = "Admin, User")]
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
