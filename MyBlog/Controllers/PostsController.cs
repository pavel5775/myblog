using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyBlog.Data;
using MyBlog.Models;
using MyBlog.ViewModels;

namespace MyBlog.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        //annex
        private IWebHostEnvironment _env;
        //annex2 - расширение для аплоуда, но можно и без него
        private readonly string[] permittedExtensios = new string[]
        {
            ".jpg", ".jpeg", ".png", ".bmp", ".gif"
        };

        public PostsController(ApplicationDbContext context, /*annext*/IWebHostEnvironment env)
        {
            _context = context;

            //annex
            _env = env;
        }

        // GET: Posts
        public async Task<IActionResult> Index(/*annex*/ int pageNumber = 1)
        {
            //annex
            var posts = _context.Posts;
            int pageSize = 3;
            int count = await posts.CountAsync();
            var items = await posts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            PageViewModel paginator = new PageViewModel(count, pageNumber, pageSize);

            PostsViewModel viewModel = new PostsViewModel()
            {
                Posts = items,
                Paginator = paginator
            };

            return View(viewModel);            

            //original
            //return View(await _context.Posts.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Title,Description,Content,PublishDate,PublishTime,ImagePath")] Post post,
            /*annex*/ IFormFile uploadFile)
        {
            if (ModelState.IsValid)
            {
                if (uploadFile !=null)
                {
                    string name = uploadFile.FileName;
                    var ext = Path.GetExtension(name);
                    if (permittedExtensios.Contains(ext))
                    {
                        string path = $"/files/{name}";
                        string serverPath = _env.WebRootPath + path;
                        using (FileStream fs = new FileStream(serverPath, FileMode.Create, FileAccess.Write))
                        {
                            await uploadFile.CopyToAsync(fs);
                        }
                        post.ImagePath = path;

                        _context.Posts.Add(post);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return RedirectToAction("ExtensionsError", "Errors");
                    }
                }   
                else
                {
                    return RedirectToAction("Upload", "Errors");
                }
            }
            return View(post);


            //original
            //if (ModelState.IsValid)
            //{
            //    _context.Add(post);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Title,Description,Content,PublishDate,PublishTime,ImagePath")] Post post)
        {
            if (id != post.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.id))
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
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.id == id);
        }
    }
}
