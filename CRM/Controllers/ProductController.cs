using CRM.Models.Crm;
using CRM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CRM.Controllers
{
    public class ProductController : Controller
    {
        private readonly admin_NDCrMContext _context;
        private readonly ICrmrpo _ICrmrpo;

        public ProductController(ICrmrpo _ICrmrpo, admin_NDCrMContext _context)
        {
            this._context = _context;
            this._ICrmrpo = _ICrmrpo;
        }
        [HttpGet, Route("Product/Product")]
        public async Task<IActionResult> Product(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                ViewBag.UserName = AddedBy;

                ViewBag.Gst = await _context.GstMasters.Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.GstPercentagen + "%"
                }).ToListAsync();
                ViewBag.Category = await _context.Categories.Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.CategoryName
                }).ToListAsync(); 
                ViewBag.id = 0;
                ViewBag.ProductName = "";
                ViewBag.Categories = "";
                ViewBag.Gstdr = "";
                ViewBag.HsnSacCode = "";
                ViewBag.Price = "";
                ViewBag.Heading = "Add Product";
                ViewBag.btnText = "SAVE";

                if (id != 0)
                {
                    var data = await _context.ProductMasters.Where(x => x.Id == id).FirstOrDefaultAsync();
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.ProductName = data.ProductName;
                        ViewBag.Categories = data.Category; 
                        ViewBag.Gstdr = data.Gst;
                        ViewBag.HsnSacCode = data.HsnSacCode;
                        ViewBag.Price = data.Price;
                        ViewBag.Heading = "Update Product";
                        ViewBag.btnText = "Update";
                    }
                }

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Product(ProductMaster model)
        {
            try
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                ViewBag.UserName = AddedBy;

                if (model == null)
                {
                    ModelState.Clear();
                    return View();
                }
                if (model.Id != 0)
                {
                    var response = await _ICrmrpo.updateproduct(model);
                    if (response != null)
                    {
                        TempData["Message"] = "Data Update Successfully.";
                        return RedirectToAction("Product", "Product");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Record not found for update.");
                        return View(model);
                    }
                }
                else
                {
                    var product = await _ICrmrpo.Product(model);
                    TempData["Message"] = "Data Added Successfully.";
                    return RedirectToAction("Product", "Product");
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }
        public async Task<IActionResult> ProductList()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {             
                string AddedBy = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                var response = await _ICrmrpo.ProductList();
                ViewBag.UserName = AddedBy;
                return View(response);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var data = _context.ProductMasters.Find(id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    _context.SaveChanges();

                }
                return RedirectToAction("ProductList", "Product");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        [HttpGet, Route("Product/VendorProduct")]
        public async Task<IActionResult> VendorProduct(int? id = 0)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                ViewBag.UserName = AddedBy;

                ViewBag.Gst = await _context.GstMasters.Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.GstPercentagen
                }).ToListAsync();
                ViewBag.Category = await _context.VendorCategoryMasters.Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.CategoryName
                }).ToListAsync();
                ViewBag.id = 0;
                ViewBag.ProductName = "";
                ViewBag.Categories = "";
                ViewBag.Gstdr = "";
                ViewBag.HsnSacCode = "";
                ViewBag.Price = "";
                ViewBag.Heading = "Add Product";
                ViewBag.btnText = "SAVE";

                if (id != 0)
                {
                    var data = await _context.VendorProductMasters.Where(x => x.Id == id).FirstOrDefaultAsync();
                    if (data != null)
                    {
                        ViewBag.id = data.Id;
                        ViewBag.ProductName = data.ProductName;
                        ViewBag.Categories = data.CategoryId;
                        ViewBag.Gstdr = data.Gst;
                        ViewBag.HsnSacCode = data.Hsncode;
                        ViewBag.Price = data.ProductPrice;
                        ViewBag.Heading = "Update Product";
                        ViewBag.btnText = "Update";
                    }
                }

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        [HttpPost]
        public async Task<IActionResult> VendorProduct(VendorProductMaster model)
        {
            try
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
                ViewBag.UserName = AddedBy;
                int vendorid = (int)adminlogin.Vendorid;
                if (model == null)
                {
                    ModelState.Clear();
                    return View();
                }
                if (model.Id != 0)
                {
                    var response = await _ICrmrpo.AddVendorProduct(model, vendorid);
                    if (response != null)
                    {
                        TempData["Message"] = "Product Updated Successfully.";
                        return RedirectToAction("VendorProduct", "Product");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Record not found for update.");
                        return View(model);
                    }
                }
                else
                {
                    var product = await _ICrmrpo.AddVendorProduct(model, vendorid);
                    TempData["Message"] = "Product Added Successfully.";
                    return RedirectToAction("VendorProduct", "Product");
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Error:" + Ex.Message);
            }
        }
        public async Task<IActionResult> VendorProductList()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string AddedBy = HttpContext.Session.GetString("UserName");
                int Userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var adminlogin = await _context.AdminLogins.Where(x => x.Id == Userid).FirstOrDefaultAsync();
               
                var response = await _ICrmrpo.GetVendorProductList((int)adminlogin.Vendorid);
                ViewBag.UserName = AddedBy;
                return View(response);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        public async Task<IActionResult> DeleteVendorProduct(int id)
        {
            try
            {
                var data = _context.VendorProductMasters.Find(id);
                if (data != null)
                {
                    data.IsActive = false;
                    _context.SaveChanges();

                }
                return RedirectToAction("VendorProductList", "Product");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

    }
}
