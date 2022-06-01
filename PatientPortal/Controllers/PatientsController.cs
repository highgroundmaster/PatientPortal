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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var patientPortalContext = _context.Patients.Include(d => d.PatientUser);
            return View(await patientPortalContext.ToListAsync());
        }

        // GET: Patients/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(ulong? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

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
        public async Task<IActionResult> Create(string IsFamilyDonorAvailable, [Bind("PatientId,Name,Sex,Age,BloodType,PastHistory,City,State,Reports")] Patient patient)
        {
            Console.WriteLine("Reached Patient Create POST");
            
            var user = await _context.Userinfos.FirstOrDefaultAsync(m => m.UserName == User.Identity.Name.ToString());
            if (user == null)
            {
                return NotFound();
            }

            patient.PatientUser = user;
            patient.PatientUserId = user.UserId;

            if (ModelState.IsValid)
            {
                Console.WriteLine($"Model State Valid - user - {user.UserId}");
                _context.Add(patient);

                await _context.SaveChangesAsync();

                if (Convert.ToBoolean(IsFamilyDonorAvailable))
                {
                    return RedirectToAction("Create", "Donors", new { familyPatientId = patient.PatientId });
                }
                return RedirectToAction("Index", "Home");
            }
            return View(patient);
        }


        // GET: Patients/Edit/5
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Edit(ulong? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
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
            if (patient == null)
            {
                return NotFound();
            }

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
