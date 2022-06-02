using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Models;

namespace PatientPortal.Controllers
{
    public class PatientsController : Controller
    {
        private readonly PatientPortalContext _context;

        public PatientsController(PatientPortalContext context)
        {
            _context = context;
        }

        // GET: Patients
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Index()
        {
            var user = await _context.Userinfos.FirstOrDefaultAsync(m => m.UserName == User.Identity.Name);
            var patientPortalContext = _context.Patients.Include(d => d.PatientUser);
            if (user.Role == "User")
                return View(await patientPortalContext.Where(m => m.PatientUserId == user.UserId).ToListAsync());
            return View(await patientPortalContext.ToListAsync());
        }

        // GET: Patients/Details/5
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Details(ulong? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);

            var user = await _context.Userinfos.FirstOrDefaultAsync(m => m.UserName == User.Identity.Name);
            if (patient == null || user == null)
                return NotFound();

            if (patient.PatientUserId != user.UserId)
                return StatusCode(StatusCodes.Status403Forbidden);

            return View(patient);
        }


        // GET: Patients/Create
        [Authorize(Roles ="Admin, User")]
        public async Task<IActionResult> Create()
        {
            if (TempData["UserId"] != null)
            {
                if (_context.Userinfos == null)
                {
                    return NotFound();
                }

                var user = await _context.Userinfos.FirstOrDefaultAsync(m => m.UserName == User.Identity.Name);
                if (user == null)
                {
                    return NotFound();
                }
               
            }

            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,Name,Sex,Age,BloodType,PastHistory,City,State,Reports")] Patient patient, bool IsFamilyDonorAvailable = false)
        { 
            var user = await _context.Userinfos.FirstOrDefaultAsync(m => m.UserName == User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            patient.PatientUser = user;
            patient.PatientUserId = user.UserId;

            ModelState.Remove("PatientUser");
            ModelState.Remove("PatientUserId");

            if (ModelState.IsValid)
            {
                _context.Add(patient);

                await _context.SaveChangesAsync();

                if (IsFamilyDonorAvailable)
                    return RedirectToAction("Create", "Donors", new { familyPatientId = patient.PatientId });

                return RedirectToAction("Details", "Patients", new {id = patient.PatientId});
            }
            return View(patient);
        }


        // GET: Patients/Edit/5
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Edit(ulong? id)
        {
            if (id == null || _context.Patients == null)
                return NotFound();
            var patient = await _context.Patients.FindAsync(id);
            
            var user = await _context.Userinfos.FirstOrDefaultAsync(m => m.UserName == User.Identity.Name);
            if (patient == null || user == null)
                return NotFound();

            if (patient.PatientUserId != user.UserId)
                return StatusCode(StatusCodes.Status403Forbidden);
            
            return View(patient);

        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ulong id, [Bind("PatientId,Name,Sex,Age,BloodType,PastHistory,City,State,Reports")] Patient patient)
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.PatientId))
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
            return View(patient);
        }

        // GET: Patients/Delete/5
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Delete(ulong? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.PatientId == id);
            var user = await _context.Userinfos.FirstOrDefaultAsync(m => m.UserName == User.Identity.Name);
            
            if (patient == null || user == null)
                return NotFound();

            if (patient.PatientUserId != user.UserId)
                return StatusCode(StatusCodes.Status403Forbidden);

            return View(patient);
        }

        // POST: Patients/Delete/5
        [Authorize(Roles = "Admin, User")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ulong id)
        {
            if (_context.Patients == null)
            {
                return Problem("Entity set 'PatientPortalContext.Patients'  is null.");
            }
            var patient = await _context.Patients.FindAsync(id);
            var user = await _context.Userinfos.FirstOrDefaultAsync(m => m.UserName == User.Identity.Name);
            if (patient == null || user == null)
                return NotFound();

            if (patient.PatientUserId != user.UserId)
                return StatusCode(StatusCodes.Status403Forbidden);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(ulong id)
        {
            return (_context.Patients?.Any(e => e.PatientId == id)).GetValueOrDefault();
        }
    }
}
