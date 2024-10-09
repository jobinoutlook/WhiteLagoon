using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using System.IO;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly ApplicationDbContext _db;
        private IWebHostEnvironment _hostnvironment;
        private readonly IMapper _mapper;
        public VillaController(ApplicationDbContext db, IWebHostEnvironment hostenvironment,IMapper mapper)
        {
                _db = db;
            _hostnvironment = hostenvironment;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            var villas = _db.Villas.ToList();
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
                    string filename = file.FileName; //Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, "Images", "Villa");
                    var extention = Path.GetExtension(file.FileName);

                    var fullpath = Path.Combine(wwwRootPath, uploads, filename + extention);

                    using (var fileStreams = new FileStream(fullpath, FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }

                    objVilla.ImageUrl = @"/Images/Villa/" + filename + extention;

                }

                _db.Villas.Add(objVilla);
                _db.SaveChanges();

                return RedirectToAction("Index","Villa");
            }


            return View(objVilla);
        }

        public IActionResult Update(int villaId)
        {
            Villa? villa = _db.Villas.FirstOrDefault(x => x.Id == villaId);
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


            Villa? createObj = _db.Villas?.FirstOrDefault(u => u.Id == objVilla.Id);


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

                string filename = file.FileName;
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

                _db.Villas?.Update(createObj);
                _db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch (Exception)
            {

                return View(objVilla);

            }



         }



        //public IActionResult Delete(int villaId)
        //{
        //}
    }
}
