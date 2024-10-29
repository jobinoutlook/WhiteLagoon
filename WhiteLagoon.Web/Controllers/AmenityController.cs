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
using WhiteLagoon.Application.Common.Utility;

namespace WhiteLagoon.Web.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class AmenityController : Controller
    {
        // private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;

        
        public AmenityController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
           
        }
        public IActionResult Index()
        {
            var amenities = _unitOfWork.Amenity.GetAll(includeProperties: "Villa").ToList();
            return View(amenities);
        }

        public IActionResult Create()
        {
            AmenityVM amenityVM = new()
            {

                VillaList = _unitOfWork.Villa.GetAll().ToList().Select(u => new SelectListItem
                {

                    Text = u.Name,
                    Value = u.Id.ToString()


                }),
                 
            };


            return View(amenityVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AmenityVM obj)
        {

            

            
            if (ModelState.IsValid)
            {


                _unitOfWork.Amenity.Add(obj.Amenity);
                _unitOfWork.Save();
                TempData["success"] = "Amenity created successfully";
                return RedirectToAction(nameof(Index), nameof(Amenity));
            }

            

            obj.VillaList = _unitOfWork.Villa.GetAll().ToList().Select(u => new SelectListItem
            {

                Text = u.Name,
                Value = u.Id.ToString()

            });

            return View(obj);
        }

        


        public IActionResult Update(int amenityId)
        {
            AmenityVM amenityVM = new()
            {

                VillaList = _unitOfWork.Villa.GetAll().ToList().Select(u => new SelectListItem
                {

                    Text = u.Name,
                    Value = u.Id.ToString()


                }),
                Amenity = _unitOfWork.Amenity.Get(x => x.Id == amenityId)


            };



            if (amenityVM.Amenity == null)
            {

                return RedirectToAction("Error", "Home");
            }

            return View(amenityVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(AmenityVM amenityVM)
        {


            


            if (ModelState.IsValid)
            {


                _unitOfWork.Amenity.Update(amenityVM.Amenity);
                _unitOfWork.Save();
                TempData["success"] = "Amenity updated successfully";
                return RedirectToAction("Index", "Amenity");
            }

           
           TempData["error"] = "Model state is invalid";
            

            amenityVM.VillaList = _unitOfWork.Villa.GetAll().ToList().Select(u => new SelectListItem
            {

                Text = u.Name,
                Value = u.Id.ToString()

            });

            return View(amenityVM);



        }



        public IActionResult Delete(int amenityId)
        {
            AmenityVM amenityVM = new()
            {

                VillaList = _unitOfWork.Villa.GetAll().ToList().Select(u => new SelectListItem
                {

                    Text = u.Name,
                    Value = u.Id.ToString()


                }),
                Amenity = _unitOfWork.Amenity.Get(x => x.Id == amenityId)


            };

            //var amenity = _db.Amenitys.Find(amenityId);

            return View(amenityVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(AmenityVM objAmenity)
        {
            var objAmenityDb = _unitOfWork.Amenity.Get(u => u.Id == objAmenity.Amenity.Id);
            if (objAmenityDb != null)
            {

                _unitOfWork.Amenity.Remove(objAmenityDb);
                _unitOfWork.Save();
                TempData["success"] = "The Amenity has deleted Successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "The Amenity could not be deleted";
            return View(objAmenity);
        }
    }
}
