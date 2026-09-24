using LinePerformanceDashboard.Data;
using LinePerformanceDashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class OperatorDetailsController : Controller
{
    private readonly ApplicationDbContext _context;
    public OperatorDetailsController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> OperatorDetailsList(int page = 1,int pageSize = 15,string search = "")
    {
        // Safety checks
        if (page < 1)
            page = 1;

        if (pageSize <= 0)
            pageSize = 15;

        // Get data
        var query = _context.TblOperatorDetails.AsQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                EF.Functions.Like(x.Name, "%" + search + "%") ||
                EF.Functions.Like(x.ProcessName, "%" + search + "%")
            );
        }

        // Total records
        var totalRecords = await query.CountAsync();

        // Total pages
        var totalPages = (int)Math.Ceiling(
            totalRecords / (double)pageSize
        );

        // Prevent invalid page
        if (totalPages > 0 && page > totalPages)
            page = totalPages;

        // Get paginated data
        var operatorDetails = await query
            .OrderBy(x => x.Oid)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Send values to View
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.totalOperatorDetails = totalRecords;
        ViewBag.Search = search;

        return View(operatorDetails);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var operatorDetail = new TblOperatorDetail
        {
            ProdDate = DateTime.Today
        };
        return View(operatorDetail);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblOperatorDetail operatorDetail)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields.";
                return View(operatorDetail);
            }

            // ✅ CHECK DUPLICATE SL NO
            var isExist = await _context.TblOperatorDetails
                .AnyAsync(x => x.Oid == operatorDetail.Oid);

            if (isExist)
            {
                ModelState.AddModelError("SlNo", "This SL No already exists!");
                return View(operatorDetail);
            }

            _context.TblOperatorDetails.Add(operatorDetail);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Equipment created successfully.";

            return RedirectToAction(nameof(OperatorDetailsList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to create equipment.";
            return View(operatorDetail);
        }
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var data = _context.TblOperatorDetails.FirstOrDefault(x => x.Oid == id);
        return View(data);
    }

    [HttpPost]
    public IActionResult Edit(TblOperatorDetail model)
    {
        _context.TblOperatorDetails.Update(model);
        _context.SaveChanges();
        return RedirectToAction("OperatorDetailsList");
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var data = _context.TblOperatorDetails.FirstOrDefault(x => x.Oid == id);

        if (data == null)
        {
            return NotFound();
        }

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var data = _context.TblOperatorDetails.FirstOrDefault(x => x.Oid == id);

        if (data != null)
        {
            _context.TblOperatorDetails.Remove(data);
            _context.SaveChanges();
        }

        return RedirectToAction("OperatorDetailsList");
    }

}
