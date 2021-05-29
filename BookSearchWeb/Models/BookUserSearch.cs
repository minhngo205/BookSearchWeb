using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSearchWeb.Models
{
    public class BookUserSearch
    {
        public int Id { get; set; }
        public string SearchedFor { set; get; }
        public string Name { get; set; }
        public string AuthorLink { get; set; }
        public string Author { get; set; }
        public string BookLink { get; set; }
        public DateTime DataAndTime { get; set; } = DateTime.Now;
    }
}