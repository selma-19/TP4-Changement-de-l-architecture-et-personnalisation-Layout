using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tp4.Models;
using tp4.Services.ServiceContracts;
using tp4.Services.Services;

namespace tp4.Controllers
{

        public class MovieController : Controller
        {
            private readonly IMovieService _movieService;

            public MovieController(IMovieService movieService)
            {
                _movieService = movieService;
            }

            public IActionResult Index()
            {
                return View(_movieService.GetAllMovies());
            }

            public IActionResult Create()
            {
                return View();
            }

            [HttpPost]
            public ActionResult CreateMovie(Movie movie)
            {
                _movieService.CreateMovie(movie);
                return RedirectToAction("Index");
            }

            public IActionResult Edit(int id)
            {
                var movie = _movieService.GetMovieById(id);

                if (movie == null)
                {
                    return NotFound();
                }

                return View(movie);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult Edit(Movie movie)
            {
                if (ModelState.IsValid)
                {
                    if (movie.ImageFile != null && movie.ImageFile.Length > 0)
                    {
                        var imagePath = Path.Combine("wwwroot/images", movie.ImageFile.FileName);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            movie.ImageFile.CopyTo(stream);
                        }

                        movie.Photo = $"/images/{movie.ImageFile.FileName}";
                    }
                    _movieService.Edit(movie);
                    return RedirectToAction("Index");


                }

                return View(movie);
            }

            public IActionResult Delete(int id)
            {
                var movie = _movieService.GetMovieById(id);

                if (movie == null)
                {
                    return NotFound();
                }

                return View(movie);
            }

            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public IActionResult DeleteConfirmed(int id)
            {
                var movie = _movieService.GetMovieById(id);

                if (movie == null)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(movie.Photo))
                {
                    var imagePath = Path.Combine("wwwroot", movie.Photo.TrimStart('/'));

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _movieService.Delete(id);
                return RedirectToAction("Index");
            }

            public IActionResult Details(int? id)
            {
                if (id == null) return Content("unable to find Id");
                var c = _movieService.GetMovieById(id.Value);
                return View(c);
            }



            public IActionResult MoviesByGenre(int id)
            {
                var movies = _movieService.GetMoviesByGenre(id);
                return View("MoviesByGenre", movies);
            }


            public IActionResult MoviesOrderedAscending()
            {
                var movies = _movieService.GetAllMoviesOrderedAscending();
                return View("MoviesOrderedAscending", movies);
            }

            public IActionResult MoviesByUserDefinedGenre(string name)
            {
                var movies = _movieService.GetMoviesByUserDefinedGenre(name);
                return View("MoviesByUserDefinedGenre", movies);
            }
        }}
    
