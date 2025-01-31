using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Movies.wkktoria.Data;
using Movies.wkktoria.Models;

namespace Movies.wkktoria.Controllers;

public class MoviesController : Controller
{
    private readonly AppDbContext _context;

    public MoviesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Movies
    public async Task<IActionResult> Index(string searchString, string movieGenre)
    {
        var genreQuery = from m in _context.Movies orderby m.Genre select m.Genre;
        var movies = from m in _context.Movies select m;

        if (!string.IsNullOrEmpty(searchString)) movies = movies.Where(m => m.Title!.Contains(searchString));
        if (!string.IsNullOrEmpty(movieGenre)) movies = movies.Where(m => m.Genre == movieGenre);

        var movieGenreVm = new MovieGenreViewModel
        {
            Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
            Movies = await movies.ToListAsync()
        };

        return View(movieGenreVm);
    }

    // GET: Movies/Details/5
    public async Task<IActionResult> Details(long? id)
    {
        if (id == null || _context.Movies == null) return NotFound();

        var movie = await _context.Movies
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie == null) return NotFound();

        return View(movie);
    }

    // GET: Movies/Edit/5
    public async Task<IActionResult> Edit(long? id)
    {
        if (id == null || _context.Movies == null) return NotFound();

        var movie = await _context.Movies.FindAsync(id);

        if (movie == null) return NotFound();

        return View(movie);
    }

    // POST: Movies/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, [Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
    {
        if (id != movie.Id) return NotFound();

        if (!ModelState.IsValid) return View(movie);

        try
        {
            _context.Update(movie);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MovieExists(movie.Id)) return NotFound();
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Movies/Delete/5
    public async Task<IActionResult> Delete(long? id)
    {
        if (id == null || _context.Movies == null) return NotFound();

        var movie = await _context.Movies.FirstOrDefaultAsync(movie => movie.Id == id);

        if (movie == null) return NotFound();

        return View(movie);
    }

    // POST: Movies/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        if (_context.Movies == null) return NotFound();

        var movie = await _context.Movies.FindAsync(id);

        if (movie != null) _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: Movies/Add
    public IActionResult Add()
    {
        return View();
    }

    // POST: Movies/Add
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add([Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
    {
        if (!ModelState.IsValid) return View(movie);

        _context.Add(movie);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool MovieExists(long id)
    {
        return (_context.Movies?.Any(movie => movie.Id == id)).GetValueOrDefault();
    }
}