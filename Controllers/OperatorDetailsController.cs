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

        var query = _context.TblOperatorDetails.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>

                    EF.Functions.Like(x.Name, "%" + search + "%") ||
                    EF.Functions.Like(x.ProcessName, "%" + search + "%")
                );
        }

        var totalRecords = await query.CountAsync();
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.totalOperatorDetails = totalRecords;
        ViewBag.Search = search;
        var operatosDetails = await query
            .OrderBy(r => r.Oid)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return View(operatosDetails);
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
