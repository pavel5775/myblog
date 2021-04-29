using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyBlog.Data;
using MyBlog.Models;

namespace MyBlog.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Comments
        public async Task<IActionResult> Index(/*annex*/ int? id)
        {
            //annex
            if (id !=null)
            {
                var post = await _context.Posts.Where(p => p.Id == id)
                    .FirstOrDefaultAsync();
                ViewBag.Post = post;

                var comments = await _context.Comments
                    .Include(c => c.ApplicationUser)
                    .Include(c => c.Post)
                    .Where(c => c.PostId == id)
                    .ToListAsync();
                return View(comments);
            }
            
            //original
            var applicationDbContext = _context.Comments.Include(c => c.Post);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        [Authorize]
        public IActionResult Create(/*annex*/int? postId)
        {
            //annex
            var post = _context.Posts.Where(p => p.Id == postId).FirstOrDefault();
            ViewBag.Post = post;

            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]        
        public async Task<IActionResult> Create([Bind("Id,Content,PublishDate,PublishTime,ApplicationUserId,ApplicationUserName,PostId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefaultAsync();
                comment.ApplicationUserId = user.Id;
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id=comment.PostId});
            }
            return View(comment);

            //original
            //if (ModelState.IsValid)
            //{
            //    _context.Add(comment);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Content", comment.PostId);
            //return View(comment);
        }

        // GET: Comments/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);          

            if (comment == null)
            {
                return NotFound();
            }

            //annex Вариант ограничения доступа внутри кода контроллера c редирект на страницу отказа
            if (comment.ApplicationUserName == User.Identity.Name)
            {
                //ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Content", comment.PostId);
                return View(comment);
            }

            return Redirect("Identity/Account/AccessDenied");

            //в оригинале заканчиваллось просто так, там где аннекс начинается.
            //ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Content", comment.PostId);
            //return View(comment);

        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content,PublishDate,PublishTime,ApplicationUserId,PostId")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
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
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Content", comment.PostId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), /*annex*/new { id = comment.PostId } );
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
