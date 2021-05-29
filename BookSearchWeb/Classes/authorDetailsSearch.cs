using BookSearchWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VDS.RDF.Query;

namespace BookSearchWeb.Classes
{
    public class AuthorDetailsSearch
    {
        private readonly BookDBContext _db = new BookDBContext();
        private readonly int _minutesOld = -2;
        public void Search(string authorLink, string authorName)
        {
            // Kiem tra tac gia trong db
            if (_db.AuthorDetailsTable.Any(p => p.AuthorLink.Equals(authorLink)))
            {
                var queryDate = from items in _db.AuthorDetailsTable
                                where items.AuthorLink == authorLink
                                select items;

                var dateTimeNow = DateTime.Now;
                var dateTimeOldest = dateTimeNow.AddMinutes(_minutesOld);
                var dateTimeSearch = queryDate.FirstOrDefault().DataAndTime;

                //Kiem tra ngay
                if (dateTimeSearch <= dateTimeNow && dateTimeSearch >= dateTimeOldest)
                {
                    //Khong can cap nhat thong tin
                }
                else if (dateTimeSearch < dateTimeOldest)
                {
                    //Thông tin lỗi thời, cần cập nhật
                    //Xoá thông tin hiện tại
                    var authors = from b in _db.AuthorDetailsTable select b;
                    authors = authors.Where(p => p.AuthorLink.Equals(authorLink));
                    foreach(BookAuthorDetails i in authors)
                    {
                        _db.AuthorDetailsTable.Remove(_db.AuthorDetailsTable.Find(i.AuthorId));
                    }
                    _db.SaveChanges();

                    //Request thông tin mới
                    SparqlResultSet resultSetAuthorDetails = Utilities.QueryDbpedia(Utilities.QueryAuthorDetails(authorName));
                    SparqlResultSet resultSetAuthorBooks = Utilities.QueryDbpedia(Utilities.QueryAuthorBooks(authorLink));

                    LoopValuesToDatabase(resultSetAuthorDetails, resultSetAuthorBooks);
                }
            }
            else
            {
                SparqlResultSet resultSetAuthorDetails = Utilities.QueryDbpedia(Utilities.QueryAuthorDetails(authorName));
                SparqlResultSet resultSetAuthorBooks = Utilities.QueryDbpedia(Utilities.QueryAuthorBooks(authorLink));

                if (!resultSetAuthorDetails.IsEmpty && !resultSetAuthorBooks.IsEmpty)
                {
                    LoopValuesToDatabase(resultSetAuthorDetails, resultSetAuthorBooks);
                }
            }
        }
        private void LoopValuesToDatabase(SparqlResultSet resultSetAuthorDetails, SparqlResultSet resultSetAuthorBooks)
        {
            if (resultSetAuthorDetails.IsEmpty && resultSetAuthorBooks.IsEmpty)
            {
                return;
            }
            string authorLink = resultSetAuthorDetails.Last()["authorLink"].ToString();
            string authorName = resultSetAuthorDetails.Last()["authorName"].ToString();
            string placeOfBirthLink = resultSetAuthorDetails.Last()["placeOfBirthLink"].ToString();
            string placeOfBirth = resultSetAuthorDetails.Last()["PlaceOfBirth"].ToString();
            string stringLatitude = resultSetAuthorDetails.Last().Value("latitude").ToString();
            string stringLongitude = resultSetAuthorDetails.Last().Value("longitude").ToString();
            //Chuyển toạ độ thành float
            float latitude = float.Parse(Utilities.NumberConverter(stringLatitude));
            float longitude = float.Parse(Utilities.NumberConverter(stringLongitude));

            if (authorName.Length > 3 && placeOfBirth.Length > 3)
            {
                authorName = authorName.Substring(0, authorName.Length - 3);
                placeOfBirth = placeOfBirth.Substring(0, placeOfBirth.Length - 3);
            }

            //List các tác giả
            List<BookDetails> authorBooks = new List<BookDetails>();
            foreach (SparqlResult result in resultSetAuthorBooks)
            {
                string bookLink = result["bookLink"].ToString();
                string name = result["bookName"].ToString();
                string bookAbstract = result["bookAbstract"].ToString();
                string stringNumberOfPages = "";
                if (result["numberOfPages"] != null)
                {
                    stringNumberOfPages = result["numberOfPages"].ToString();
                }
                string comment = result["comment"].ToString();

                //Remove @en from the string
                name = Utilities.RemoveLast3Cahracters(name);
                bookAbstract = Utilities.RemoveLast3Cahracters(bookAbstract);
                comment = Utilities.RemoveLast3Cahracters(comment);

                //Convert numberOfPages to int
                int numberOfPages = 0;
                if (result["numberOfPages"] != null)
                {
                    numberOfPages = int.Parse(Utilities.NumberConverter(stringNumberOfPages));
                }

                //Create a BookDetails object
                BookDetails book = new BookDetails
                {
                    BookLink = bookLink,
                    Name = name,
                    Abstract = bookAbstract,
                    NumberOfPages = numberOfPages,
                    Comment = comment
                };
                authorBooks.Add(book);
            }
            //
            AddToDatabase(authorLink, authorName, placeOfBirthLink, placeOfBirth, latitude, longitude, authorBooks);
        }
        private void AddToDatabase(string authorLink, string authorName, string placeOfBirthLink, string placeOfBirth, float latitude, float longitude, List<BookDetails> authorBooks)
        {
            BookAuthorDetails book = new BookAuthorDetails
            {
                AuthorLink = authorLink,
                AuthorName = authorName,
                PlaceOfBirthLink = placeOfBirthLink,
                PlaceOfBirth = placeOfBirth,
                Latitude = latitude,
                Longitude = longitude,
                AuthorBooks = authorBooks
            };
            _db.AuthorDetailsTable.Add(book);
            try
            {
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("error adding to database");
                Console.WriteLine(e);
            }
        }
        //private SparqlResultSet GetCorrectInfo(SparqlResultSet results, string authorLink)
        //{
        //    SparqlResultSet correct = new SparqlResultSet();
        //    foreach (SparqlResult result in results)
        //    {
        //        if (result["authorLink"].ToString() == authorLink)
        //        {
        //            correct.Append(result);
        //        }
        //    }
        //    Console.WriteLine(authorLink);
        //    return correct;
        //}
    }
}