using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2025_Project3_jdpeacock.Data;
using Fall2025_Project3_jdpeacock.Models;
using Microsoft.Data.SqlClient;

namespace Fall2025_Project3_jdpeacock.Controllers
{
    public class ActorMoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActorMoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ActorMovies
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ActorMovie.Include(a => a.Actor).Include(a => a.Movie);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ActorMovies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actorMovie = await _context.ActorMovie
                .Include(a => a.Actor)
                .Include(a => a.Movie)
                .FirstOrDefaultAsync(m => m.id == id);
            if (actorMovie == null)
            {
                return NotFound();
            }

            return View(actorMovie);
        }

        // GET: ActorMovies/Create
        public IActionResult Create()
        {
            ViewData["ActorId"] = new SelectList(_context.Actor, "id", "name");
            ViewData["MovieId"] = new SelectList(_context.Movie, "id", "title");
            return View();
        }

        // POST: ActorMovies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,ActorId,MovieId")] ActorMovie actorMovie)
        {
            if (ModelState.IsValid)
            {
                // Prevent duplicates before adding
                if (_context.ActorMovie.Any(a => a.ActorId == actorMovie.ActorId && a.MovieId == actorMovie.MovieId))
                {
                    ModelState.AddModelError(string.Empty, "This actor/movie relationship already exists.");
                    ViewData["ActorId"] = new SelectList(_context.Actor, "id", "name", actorMovie.ActorId);
                    ViewData["MovieId"] = new SelectList(_context.Movie, "id", "title", actorMovie.MovieId);
                    return View(actorMovie);
                }

                _context.Add(actorMovie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActorId"] = new SelectList(_context.Actor, "id", "name", actorMovie.ActorId);
            ViewData["MovieId"] = new SelectList(_context.Movie, "id", "title", actorMovie.MovieId);
            return View(actorMovie);
        }

        // GET: ActorMovies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actorMovie = await _context.ActorMovie.FindAsync(id);
            if (actorMovie == null)
            {
                return NotFound();
            }
            ViewData["ActorId"] = new SelectList(_context.Actor, "id", "name", actorMovie.ActorId);
            ViewData["MovieId"] = new SelectList(_context.Movie, "id", "title", actorMovie.MovieId);
            return View(actorMovie);
        }

        // POST: ActorMovies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,ActorId,MovieId")] ActorMovie actorMovie)
        {
            if (id != actorMovie.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Exclude the current record when checking for duplicates
                if (_context.ActorMovie.Any(a => a.ActorId == actorMovie.ActorId
                                             && a.MovieId == actorMovie.MovieId
                                             && a.id != actorMovie.id))
                {
                    ModelState.AddModelError(string.Empty, "Another record with the same actor and movie already exists.");
                    ViewData["ActorId"] = new SelectList(_context.Actor, "id", "name", actorMovie.ActorId);
                    ViewData["MovieId"] = new SelectList(_context.Movie, "id", "title", actorMovie.MovieId);
                    return View(actorMovie);
                }

                try
                {
                    _context.Update(actorMovie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorMovieExists(actorMovie.id))
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
            ViewData["ActorId"] = new SelectList(_context.Actor, "id", "name", actorMovie.ActorId);
            ViewData["MovieId"] = new SelectList(_context.Movie, "id", "title", actorMovie.MovieId);
            return View(actorMovie);
        }

        // GET: ActorMovies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actorMovie = await _context.ActorMovie
                .Include(a => a.Actor)
                .Include(a => a.Movie)
                .FirstOrDefaultAsync(m => m.id == id);
            if (actorMovie == null)
            {
                return NotFound();
            }

            return View(actorMovie);
        }

        // POST: ActorMovies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actorMovie = await _context.ActorMovie.FindAsync(id);
            if (actorMovie != null)
            {
                _context.ActorMovie.Remove(actorMovie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorMovieExists(int id)
        {
            return _context.ActorMovie.Any(e => e.id == id);
        }
    }
}
