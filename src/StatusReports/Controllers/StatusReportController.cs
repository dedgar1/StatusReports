using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using StatusReports.Models;
using System.Threading.Tasks;

namespace StatusReports.Controllers
{
    public class StatusReportController : Controller
    {
        private StatusReportsDbContext _context;

        public StatusReportController(StatusReportsDbContext context)
        {
            _context = context;    
        }

        // GET: IndividualStatus
        public async Task<IActionResult> Index()
        {
            var draftStatusReports = await _context.IndividualStatusReports.Include(i => i.Person)
                                                                         .Include(i => i.Project).ThenInclude(c=> c.Client)
                                                                         .Include(i => i.Week)
                                                                         .ToListAsync();
            return View(draftStatusReports);
        }
       
        public IActionResult Submitted()
        {
            var statusReportsDbContext = _context.IndividualStatusReports.Include(i => i.Person).Include(i => i.Project).Include(i => i.Week).Where(i => i.Status == StatusCode.Submitted);
            return View(statusReportsDbContext.ToList());
        }
        public IActionResult Approved()
        {
            var statusReportsDbContext = _context.IndividualStatusReports.Include(i => i.Person).Include(i => i.Project).Include(i => i.Week).Where(i => i.Status == StatusCode.Approved);
            return View(statusReportsDbContext.ToList());
        }


        // GET: IndividualStatus/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var details = _context.IndividualStatusItems.Where(i => i.IndividualStatusReportId == id);
            //IndividualStatusReport individualStatusReport = _context.IndividualStatusReports.Single(m => m.Id == id);
            if (details == null)
            {
                return HttpNotFound();
            }

            return View(details.ToList());
        }

        // GET: IndividualStatus/Create
        public IActionResult Create()
        {
            ViewData["PersonId"] = new SelectList(_context.People.ToList(), "PersonId", "FullName");
            ViewData["ProjectId"] = new SelectList(_context.Projects.ToList(), "Id", "Name");
            ViewData["WeekId"] = new SelectList(_context.Weeks.ToList(), "Id", "EndingDate");
            return View();
        }

        // POST: IndividualStatus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IndividualStatusReport individualStatusReport)
        {
            if (ModelState.IsValid)
            {
                //Add the draft status to the report because all new reports should be drafts.
                individualStatusReport.Status = StatusCode.Draft;
                _context.IndividualStatusReports.Add(individualStatusReport);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewData["PersonId"] = new SelectList(_context.People, "PersonId", "FullName", individualStatusReport.PersonId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", individualStatusReport.ProjectId);
            ViewData["WeekId"] = new SelectList(_context.Weeks, "Id", "EndingDate", individualStatusReport.WeekId);
            return View(individualStatusReport);
        }

        // GET: IndividualStatus/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            IndividualStatusReport individualStatusReport = _context.IndividualStatusReports.Single(m => m.Id == id);
            if (individualStatusReport == null)
            {
                return HttpNotFound();
            }
            ViewData["PersonId"] = new SelectList(_context.People, "PersonId", "Person", individualStatusReport.PersonId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Project", individualStatusReport.ProjectId);
            ViewData["WeekId"] = new SelectList(_context.Weeks, "Id", "Week", individualStatusReport.WeekId);
            return View(individualStatusReport);
        }

        // POST: IndividualStatus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(IndividualStatusReport individualStatusReport)
        {
            if (ModelState.IsValid)
            {
                _context.Update(individualStatusReport);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewData["PersonId"] = new SelectList(_context.People, "PersonId", "Person", individualStatusReport.PersonId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Project", individualStatusReport.ProjectId);
            ViewData["WeekId"] = new SelectList(_context.Weeks, "Id", "Week", individualStatusReport.WeekId);
            return View(individualStatusReport);
        }

        // GET: IndividualStatus/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            IndividualStatusReport individualStatusReport = _context.IndividualStatusReports.Single(m => m.Id == id);
            if (individualStatusReport == null)
            {
                return HttpNotFound();
            }

            return View(individualStatusReport);
        }

        // POST: IndividualStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            IndividualStatusReport individualStatusReport = _context.IndividualStatusReports.Single(m => m.Id == id);
            _context.IndividualStatusReports.Remove(individualStatusReport);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
