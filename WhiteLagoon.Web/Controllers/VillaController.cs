using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using System.IO;
using WhiteLagoon.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace WhiteLagoon.Web.Controllers
{
    [Authorize]
    public class VillaController : Controller
    {
        //private readonly ApplicationDbContext _db;
        private IWebHostEnvironment _hostnvironment;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        private static readonly Random random = new Random();
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public VillaController(IWebHostEnvironment hostenvironment, IMapper mapper,IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
            _hostnvironment = hostenvironment;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            var villas = _unitOfWork.Villa.GetAll();
            return View(villas);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Villa objVilla, IFormFile? file)
        {
            //if (objVilla.Name == objVilla.Description)
            //{
            //    ModelState.AddModelError("", "Error, Name and Description cannot be same");
            //}
            if (ModelState.IsValid)
            {

                if (file != null)
                {
                    string wwwRootPath = _hostnvironment.WebRootPath;
                    string filename = GetRandomText(5) +"_"+ file.FileName; //Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, "Images", "Villa");
                    var extention = Path.GetExtension(file.FileName);

                    var fullpath = Path.Combine(wwwRootPath, uploads, filename);

                    using (var fileStreams = new FileStream(fullpath, FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }

                    objVilla.ImageUrl = @"/Images/Villa/" + filename;

                }

                _unitOfWork.Villa.Add(objVilla);
                _unitOfWork.Save();
                TempData["success"] = "Villa created successfully";
                return RedirectToAction("Index", "Villa");
            }


            return View(objVilla);
        }

        private string GetRandomText(int length)
        {

            return new string(Enumerable.Repeat(chars, length)
                                              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public IActionResult Update(int villaId)
        {
            Villa? villa = _unitOfWork.Villa.Get(x => x.Id == villaId);
            if (villa == null)
            {

                return NotFound();
            }

            return View(villa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Villa objVilla, IFormFile file)
        {


            Villa? createObj = _unitOfWork.Villa.Get(u => u.Id == objVilla.Id);


            //createObj.Author = objVilla.Author;
            //createObj.Description = obj.Description;
            //createObj.Title = obj.Title;
            //createObj.ISBN = obj.ISBN;
            //createObj.CoverTypeId = obj.CoverTypeId;
            //createObj.CategoryId = obj.CategoryId;
            //createObj.Price = obj.Price;

            //createObj = _mapper.Map<Villa>(objVilla);



            string wwwRootPath = _hostnvironment.WebRootPath;
            if (file != null)
            {

                if (!string.IsNullOrEmpty(createObj.ImageUrl))
                {
                    var prevFile = Path.Combine(wwwRootPath, "Images", "Villa", Path.GetFileName(createObj.ImageUrl));
                    if (Path.Exists(prevFile))
                    {
                        // var prevFile = Path.Combine(wwwRootPath, "Images", "Product", Path.GetFileName(createObj.ImageUrl));
                        System.IO.File.Delete(prevFile);
                    }
                }

                string filename = GetRandomText(5) + "_" + file.FileName;
                var uploads = Path.Combine(wwwRootPath, "Images", "Villa");
                //var extention = Path.GetExtension(file.FileName);

                var fullpath = Path.Combine(wwwRootPath, uploads, filename);

                using (var fileStreams = new FileStream(fullpath, FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }

                objVilla.ImageUrl = @"/Images/Villa/" + filename;

            }

            _mapper.Map<Villa, Villa>(objVilla, createObj);



            try
            {

                _unitOfWork.Villa.Update(createObj);
                _unitOfWork.Save();
                return RedirectToAction("Index");

            }
            catch (Exception)
            {

                return View(objVilla);

            }



        }



        public IActionResult Delete(int villaId)
        {
            var villa = _unitOfWork.Villa.Get(u=>u.Id == villaId);

            return View(villa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Villa objVilla)
        {
            var objVillaDb = _unitOfWork.Villa.Get(u=>u.Id==objVilla.Id);
            if (objVillaDb != null)
            {
                var prevFile = Path.Combine(_hostnvironment.WebRootPath, "Images", "Villa", Path.GetFileName(objVillaDb.ImageUrl));
                System.IO.File.Delete(prevFile);

                _unitOfWork.Villa.Remove(objVillaDb);
                _unitOfWork.Save();
                TempData["success"] = "The Villa has deleted Successfully.";
                return RedirectToAction("Index");
            }
            TempData["error"] = "The Villa could not be deleted";
            return View(objVilla);
        }
    }
}
