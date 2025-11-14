using Fall2025_Project3_jdpeacock.Data;
using Fall2025_Project3_jdpeacock.Models;
using Fall2025_Project3_jdpeacock.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using VaderSharp2;

namespace Fall2025_Project3_jdpeacock.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AIService _ai;
        public MoviesController(ApplicationDbContext context, AIService ai)
        {
            _context = context;
            _ai = ai;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movie.ToListAsync();
            var actorMovies = await _context.ActorMovie
                .Include(am => am.Actor)
                .Include(am => am.Movie)
                .ToListAsync();

            var vm = movies.Select(m => new MovieInfoViewModel
            {
                Movie = m,
                Actors = actorMovies
                    .Where(am => am.MovieId == m.id)
                    .Select(am => am.Actor!)
                    .ToList()
            }).ToList();

            return View(vm);
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.id == id);

            var actors = await _context.ActorMovie
                .Include(a => a.Actor)
                .Where(a => a.MovieId == id)
                .Select(a => a.Actor!)
                .ToListAsync();

            string[] personalities = ["is harsh", "loves romance", "loves comedy", "loves thrillers", "loves fantasy"];

            var reviews = new List<KeyValuePair<string, string>>();
            var prompt = $"You are a movie reviewer. You are given a list of personalities: ${string.Join(", ", personalities)}. When you receive a prompt, make sure that every answer is given in one of the personalities. Do not include the personality in your response. Separate each answer with ONLY a |";
            var response = await _ai.GetChatResponseAsync($"Generate 10 reviews including a score (1-10/10) for the movie {movie.title}", prompt);

            string[] reviewsText = response.Split("|").Select(s => s.Trim()).ToArray();

            var analyzer = new SentimentIntensityAnalyzer();

            double totalSentiment = 0;

            for (var i = 0; i < reviewsText.Length; i++)
            {
                var sentiment = analyzer.PolarityScores(reviewsText[i]);
                totalSentiment += sentiment.Compound;
                reviews.Add(new KeyValuePair<string, string>(reviewsText[i], sentiment.ToString()));
            }

            double avgSentiment = Math.Round(totalSentiment / reviewsText.Length, 2);

            var vm = new MovieInfoViewModel()
            {
                Actors = actors,
                Movie = movie,
                Reviews = reviews,
                AvgSentiment = avgSentiment
            };

            if (movie == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        // GET: Movies/Poster/5
        public async Task<IActionResult> Poster(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null || movie.poster == null)
            {
                return NotFound();
            }

            return File(movie.poster, "image/jpg"); // MediaTypeNames.Image.Jpeg
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,title,imdbLink,genre,releaseYear,poster")] Movie movie, IFormFile poster)
        {
            if (ModelState.IsValid)
            {
                if (poster != null && poster.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await poster.CopyToAsync(memoryStream);
                    movie.poster = memoryStream.ToArray();
                }
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,title,imdbLink,genre,releaseYear,poster")] Movie movie, IFormFile poster)
        {
            if (id != movie.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (poster != null && poster.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await poster.CopyToAsync(memoryStream);
                        movie.poster = memoryStream.ToArray();
                    }
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.id == id);
        }
    }
}