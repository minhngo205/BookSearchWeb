using PagedList;
using BookSearchWeb.Classes;
using BookSearchWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BookSearchWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly BookDBContext _db = new BookDBContext();
        private readonly UserSearch _userSearch = new UserSearch();
        private readonly AuthorDetailsSearch _authorSearch = new AuthorDetailsSearch();
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> SearchResults(string id = null, int page = 1, int pageSize = 10)
        {
            if (!String.IsNullOrEmpty(id))
            {
                if (!String.IsNullOrEmpty(id.Trim()))
                {
                    _userSearch.Search(id);
                }
            }
            var books = from b in _db.BookNameTable select b;
            if (!String.IsNullOrEmpty(id))
            {
                if (!String.IsNullOrEmpty(id.Trim()))
                {
                    books = books.Where(s => s.SearchedFor.Equals(id));
                }
            }
            IEnumerable<BookUserSearch> bookList = await books.OrderByDescending(s => s.DataAndTime).ToListAsync();
            PagedList<BookUserSearch> model = new PagedList<BookUserSearch>(bookList, page, pageSize);
            return View(model);
        }
        public ActionResult AuthorDetails(string authorLink, string authorName)
        {
            //Checks if the user link has the proper parameters, if not the user will be redirected to the home page
            if (!String.IsNullOrEmpty(authorLink)||!String.IsNullOrEmpty(authorName))
            {
                if (!String.IsNullOrEmpty(authorLink.Trim())||!String.IsNullOrEmpty(authorName.Trim()))
                {
                    _authorSearch.Search(authorLink, authorName);
                }
                else
                {
                    Response.Redirect("Index");
                }
            }
            else
            {
                Response.Redirect("Index");
            }

            //Creates a list of the author details
            var details = from b in _db.AuthorDetailsTable select b;
            details = details.Where(s => s.AuthorLink.Equals(authorLink));

            return View(details);
        }
        public ActionResult BookDetails(string bookLink, string authorLink, string authorName)
        {
            //Checks if the user link has the proper parameters, if not the user will be redirected to the home page
            if (!String.IsNullOrEmpty(authorLink)|| !String.IsNullOrEmpty(authorName))
            {
                if (!String.IsNullOrEmpty(authorLink.Trim())|| !String.IsNullOrEmpty(authorName.Trim()))
                {
                    _authorSearch.Search(authorLink, authorName);
                }
                else
                {
                    Response.Redirect("Index");
                }
            }
            else
            {
                Response.Redirect("Index");
            }

            //Creates a list of the book details
            var details = from b in _db.BookDetailsTable select b;
            details = details.Where(s => s.BookLink.Equals(bookLink));

            return View(details);
        }
        ~HomeController()
        {
            _db.Dispose();
        }

        public new void Dispose()
        {
            _db.Dispose();
        }
    }
}