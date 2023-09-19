using CS322_eTickets.Data.Services;
using CS322_eTickets.Data.Static;
using CS322_eTickets.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS322_eTickets.Controllers
{
	[Authorize(Roles = UserRoles.Admin)]
	public class MoviesController : Controller
	{
		private readonly IMoviesService _service;

		public MoviesController(IMoviesService service)
		{
			_service = service;
		}

		[AllowAnonymous]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 6)
        {
            // Calculate the number of items to skip based on the page number and page size
            int skipAmount = (page - 1) * pageSize;

            // Retrieve movies with pagination
            var movies = await _service.GetAllAsync(n => n.Cinema);

            // Apply pagination
            var paginatedMovies = movies.Skip(skipAmount).Take(pageSize).ToList();

            // Create a view model to pass both the movies and pagination information to the view
            var viewModel = new MoviePagination
            {
                Movies = paginatedMovies,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = movies.Count() // Total number of movies (you may need to adjust this)
            };

            return View(viewModel);
        }


        [AllowAnonymous]
		public async Task<IActionResult> Filter(string searchString)
		{
			var allMovies = await _service.GetAllAsync(n => n.Cinema);

			if (!string.IsNullOrEmpty(searchString))
			{
				var filteredResultNew = allMovies.Where(n => string.Equals(n.Name, searchString, StringComparison.CurrentCultureIgnoreCase) || string.Equals(n.Description, searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();

				return View("Index", filteredResultNew);
			}

			return View("Index", allMovies);
		}

		//GET: Movies/Details/1
		[AllowAnonymous]
		public async Task<IActionResult> Details(int id)
		{
			var movieDetail = await _service.GetMovieByIdAsync(id);
			return View(movieDetail);
		}

		//GET: Movies/Create
		public async Task<IActionResult> Create()
		{
			var movieDropdownsData = await _service.GetNewMovieDropdownsValues();

			ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
			ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
			ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(NewMovieVM movie)
		{
			if (!ModelState.IsValid)
			{
				var movieDropdownsData = await _service.GetNewMovieDropdownsValues();

				ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
				ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
				ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");

				return View(movie);
			}

			await _service.AddNewMovieAsync(movie);
			return RedirectToAction(nameof(Index));
		}


		//GET: Movies/Edit/1
		public async Task<IActionResult> Edit(int id)
		{
			var movieDetails = await _service.GetMovieByIdAsync(id);
			if (movieDetails == null) return View("NotFound");

			var response = new NewMovieVM()
			{
					Id = movieDetails.Id,
					Name = movieDetails.Name,
					Description = movieDetails.Description,
					Price = movieDetails.Price,
					StartDate = movieDetails.StartDate,
					EndDate = movieDetails.EndDate,
					ImageURL = movieDetails.ImageURL,
					MovieCategory = movieDetails.MovieCategory,
					CinemaId = movieDetails.CinemaId,
					ProducerId = movieDetails.ProducerId,
					ActorIds = movieDetails.Actors_Movies.Select(n => n.ActorId).ToList(),
			};

			var movieDropdownsData = await _service.GetNewMovieDropdownsValues();
			ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
			ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
			ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");

			return View(response);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(int id, NewMovieVM movie)
		{
			if (id != movie.Id) return View("NotFound");

			if (!ModelState.IsValid)
			{
				var movieDropdownsData = await _service.GetNewMovieDropdownsValues();

				ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
				ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
				ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");

				return View(movie);
			}

			await _service.UpdateMovieAsync(movie);
			return RedirectToAction(nameof(Index));
		}
	}
}
