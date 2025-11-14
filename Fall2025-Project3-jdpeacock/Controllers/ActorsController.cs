using Fall2025_Project3_jdpeacock.Data;
using Fall2025_Project3_jdpeacock.Models;
using Fall2025_Project3_jdpeacock.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaderSharp2;

namespace Fall2025_Project3_jdpeacock.Controllers
{
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly AIService _ai;
        public ActorsController(ApplicationDbContext context, AIService ai)
        {
            _context = context;
            _ai = ai;
        }

        // GET: Actors
        public async Task<IActionResult> Index()
        {
            var actors = await _context.Actor.ToListAsync();
            var actorMovies = await _context.ActorMovie
                .Include(am => am.Movie)
                .ToListAsync();

            var vm = actors.Select(a => new ActorInfoViewModel
            {
                Actor = a,
                Movies = actorMovies
                    .Where(am => am.ActorId == a.id)
                    .Select(am => am.Movie!)
                    .ToList()
            }).ToList();

            return View(vm);
        }

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.id == id);

            if (actor == null)
            {
                return NotFound();
            }

            var movies = await _context.ActorMovie
                .Include(m => m.Movie)
                .Where(m => m.ActorId == id)
                .Select(m => m.Movie!)
                .ToListAsync();

            string[] topics = ["opinions on directors they worked with", "day to day activities", "controversial opinions", "teasers for upcoming movies", "family", "their opinion on the new battlefield game"];

            var tweets = new List<KeyValuePair<string, string>>();
            var prompt = $"You are the actor {actor.name}. You are given a list of topics: ${string.Join(", ", topics)}. When you receive a prompt, make sure that every answer relates to one of these topics. Separate each answer with ONLY a |";
            var response = await _ai.GetChatResponseAsync($"Generate 20 tweets, you can include hashtags and emojis.", prompt);

            string[] tweetsText = response.Split("|").Select(s => s.Trim()).ToArray();
            double sentimentTotal = 0;

            var analyzer = new SentimentIntensityAnalyzer();
            for (var i = 0; i < tweetsText.Length; i++)
            {
                if (!String.IsNullOrWhiteSpace(tweetsText[i]))
                {
                    var sentiment = analyzer.PolarityScores(tweetsText[i]);
                    sentimentTotal += sentiment.Compound;
                    tweets.Add(new KeyValuePair<string, string>(tweetsText[i], sentiment.ToString()));
                }
            }

            double avgSentiment = Math.Round(sentimentTotal / tweetsText.Length, 2);

            var vm = new ActorInfoViewModel()
            {
                Actor = actor,
                Movies = movies,
                Tweets = tweets,
                AvgSentiment = avgSentiment
            };

            return View(vm);
        }

        public async Task<IActionResult> Photo(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor.FindAsync(id);
            if (actor == null || actor.photo == null)
            {
                return NotFound();
            }

            return File(actor.photo, "image/jpg"); // MediaTypeNames.Image.Jpeg
        }

        // GET: Actors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,gender,age,imdbLink,photo")] Actor actor, IFormFile photo)
        {
            if (ModelState.IsValid)
            {
                if (photo != null && photo.Length > 0)
                {
                    using var memoryStream = new MemoryStream(); // Dispose() for garbage collection 
                    await photo.CopyToAsync(memoryStream);
                    actor.photo = memoryStream.ToArray();
                }
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }

        // POST: Actors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,gender,age,imdbLink,photo")] Actor actor, IFormFile photo)
        {
            if (id != actor.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (photo != null && photo.Length > 0)
                    {
                        using var memoryStream = new MemoryStream(); // Dispose() for garbage collection 
                        await photo.CopyToAsync(memoryStream);
                        actor.photo = memoryStream.ToArray();
                    }
                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.id))
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
            return View(actor);
        }

        // GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.id == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actor.FindAsync(id);
            if (actor != null)
            {
                _context.Actor.Remove(actor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
            return _context.Actor.Any(e => e.id == id);
        }
    }
}
