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
using WhiteLagoon.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace WhiteLagoon.Web.Controllers
{
    [Authorize]
    public class VillaNumberController : Controller
    {
        // private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;

        
        public VillaNumberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
           
        }
        public IActionResult Index()
        {
            var villaNumbers = _unitOfWork.VillaNumber.GetAll(includeProperties: "Villa").ToList();
            return View(villaNumbers);
        }

        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new()
            {

                VillaList = _unitOfWork.Villa.GetAll().ToList().Select(u => new SelectListItem
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

            bool roomNumberExists = _unitOfWork.VillaNumber.Any(u => u.Villa_Number == obj.VillaNumber.Villa_Number);

            
            if (ModelState.IsValid && !roomNumberExists)
            {


                _unitOfWork.VillaNumber.Add(obj.VillaNumber);
                _unitOfWork.Save();
                TempData["success"] = "VillaNumber created successfully";
                return RedirectToAction(nameof(Index), nameof(VillaNumber));
            }

            if(!roomNumberExists)
            {
                TempData["error"] = "Villa Number already exists";
            }
            

            obj.VillaList = _unitOfWork.Villa.GetAll().ToList().Select(u => new SelectListItem
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

                VillaList = _unitOfWork.Villa.GetAll().ToList().Select(u => new SelectListItem
                {

                    Text = u.Name,
                    Value = u.Id.ToString()


                }),
                VillaNumber = _unitOfWork.VillaNumber.Get(x => x.Villa_Number == villaNumberId)


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


                _unitOfWork.VillaNumber.Update(villaNumberVM.VillaNumber);
                _unitOfWork.Save();
                TempData["success"] = "VillaNumber updated successfully";
                return RedirectToAction("Index", "VillaNumber");
            }

           
           TempData["error"] = "Model state is invalid";
            

            villaNumberVM.VillaList = _unitOfWork.Villa.GetAll().ToList().Select(u => new SelectListItem
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

                VillaList = _unitOfWork.Villa.GetAll().ToList().Select(u => new SelectListItem
                {

                    Text = u.Name,
                    Value = u.Id.ToString()


                }),
                VillaNumber = _unitOfWork.VillaNumber.Get(x => x.Villa_Number == villaNumberId)


            };

            //var villaNumber = _db.VillaNumbers.Find(villaNumberId);

            return View(villaNumberVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(VillaNumberVM objVillaNumber)
        {
            var objVillaNumberDb = _unitOfWork.VillaNumber.Get(u => u.Villa_Number == objVillaNumber.VillaNumber.Villa_Number);//objVillaNumber.VillaNumber.Villa_Number);
            if (objVillaNumberDb != null)
            {

                _unitOfWork.VillaNumber.Remove(objVillaNumberDb);
                _unitOfWork.Save();
                TempData["success"] = "The VillaNumber has deleted Successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "The VillaNumber could not be deleted";
            return View(objVillaNumber);
        }
    }
}
