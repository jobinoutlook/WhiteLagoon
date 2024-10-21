using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly ApplicationDbContext _db;
        private IWebHostEnvironment _hostnvironment;
        private readonly IMapper _mapper;

        
        public VillaNumberController(ApplicationDbContext db, IWebHostEnvironment hostenvironment, IMapper mapper)
        {
            _db = db;
            _hostnvironment = hostenvironment;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            var villaNumbers = _db.VillaNumbers.Include(u=>u.Villa).ToList();
            return View(villaNumbers);
        }

        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new()
            {

                VillaList = _db.Villas.ToList().Select(u => new SelectListItem
                {

                    Text = u.Name,
                    Value = u.Id.ToString()


                })
            };


            return View(villaNumberVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(VillaNumberVM obj)
        {

            

            
            if (ModelState.IsValid)
            {


                _db.VillaNumbers.Add(obj.VillaNumber);
                _db.SaveChanges();
                TempData["success"] = "VillaNumber created successfully";
                return RedirectToAction(nameof(Index), nameof(VillaNumber));
            }

            

            obj.VillaList = _db.Villas.ToList().Select(u => new SelectListItem
            {

                Text = u.Name,
                Value = u.Id.ToString()

            });

            return View(obj);
        }

        


        public IActionResult Update(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new()
            {

                VillaList = _db.Villas.ToList().Select(u => new SelectListItem
                {

                    Text = u.Name,
                    Value = u.Id.ToString()


                }),
                VillaNumber = _db.VillaNumbers.FirstOrDefault(x => x.Villa_Number == villaNumberId)


            };



            if (villaNumberVM.VillaNumber == null)
            {

                return RedirectToAction("Error", "Home");
            }

            return View(villaNumberVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(VillaNumberVM villaNumberVM)
        {


            


            if (ModelState.IsValid)
            {


                _db.VillaNumbers.Update(villaNumberVM.VillaNumber);
                _db.SaveChanges();
                TempData["success"] = "VillaNumber updated successfully";
                return RedirectToAction("Index", "VillaNumber");
            }

           
           TempData["error"] = "Model state is invalid";
            

            villaNumberVM.VillaList = _db.Villas.ToList().Select(u => new SelectListItem
            {

                Text = u.Name,
                Value = u.Id.ToString()

            });

            return View(villaNumberVM);



        }



        public IActionResult Delete(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new()
            {

                VillaList = _db.Villas.ToList().Select(u => new SelectListItem
                {

                    Text = u.Name,
                    Value = u.Id.ToString()


                }),
                VillaNumber = _db.VillaNumbers.FirstOrDefault(x => x.Villa_Number == villaNumberId)


            };

            //var villaNumber = _db.VillaNumbers.Find(villaNumberId);

            return View(villaNumberVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(VillaNumberVM objVillaNumber)
        {
            var objVillaNumberDb = _db.VillaNumbers.Find(objVillaNumber.VillaNumber.Villa_Number);
            if (objVillaNumberDb != null)
            {
                
                _db.VillaNumbers.Remove(objVillaNumberDb);
                _db.SaveChanges();
                TempData["success"] = "The VillaNumber has deleted Successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "The VillaNumber could not be deleted";
            return View(objVillaNumber);
        }
    }
}
