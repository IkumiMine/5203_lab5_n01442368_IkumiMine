using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using System.Diagnostics;

namespace lab5.Controllers
{
    public class BookController : Controller
    {
        public IActionResult Index()
        {
            IList<Models.Book> BookList = new List<Models.Book>();

            //Load books.xml
            string path = Request.PathBase + "App_Data/books.xml";
            XmlDocument xmldoc = new XmlDocument();

            if (System.IO.File.Exists(path))
            {
                xmldoc.Load(path);
                XmlNodeList Books = xmldoc.GetElementsByTagName("book");

                foreach (XmlElement b in Books)
                {
                    Models.Book book = new Models.Book();
                    book.ID = Int32.Parse(b.GetElementsByTagName("id")[0].InnerText);
                    book.Title = b.GetElementsByTagName("title")[0].InnerText;
                    book.AuthorTitle = b.GetElementsByTagName("author")[0].Attributes["title"].Value;
                    book.FirstName = b.GetElementsByTagName("firstname")[0].InnerText;
                    if(b.GetElementsByTagName("middlename").Count > 0) {
                        book.MiddleName = b.GetElementsByTagName("middlename")[0].InnerText;
                    } else
                    {
                        book.MiddleName = "";
                    }
                    book.LastName = b.GetElementsByTagName("lastname")[0].InnerText;

                    BookList.Add(book);
                }
            }

            return View(BookList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var book = new Models.Book();

            //Load books.xml
            string path = Request.PathBase + "App_Data/books.xml";
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(path);

            //Get book elements
            XmlNode LastBook = xmldoc.DocumentElement;

            //Get last book's ID
            XmlElement LastID = LastBook.LastChild["id"];
            book.ID = Int32.Parse(LastID.InnerText) + 1;
            //Debug.WriteLine(LastBook.LastChild["id"]);
       
            return View(book);
        }

        [HttpPost]
        public IActionResult Create(Models.Book b)
        {
            //Load books.xml
            string path = Request.PathBase + "App_Data/books.xml";
            XmlDocument xmldoc = new XmlDocument();

            if (System.IO.File.Exists(path))
            {
                //If file exists, load it and create a new book
                xmldoc.Load(path);

                //Create a new book
                XmlElement book = CreateBookElement(xmldoc, b);

                //Get the root element and add a new book
                xmldoc.DocumentElement.AppendChild(book);

                //Remove the first book if the number of books exceeds 5
                //If less than 5, just save the file
                if (xmldoc.DocumentElement.GetElementsByTagName("book").Count > 5)
                {
                    xmldoc.DocumentElement.RemoveChild(xmldoc.DocumentElement.FirstChild);
                }
                //Debug.WriteLine(xmldoc.DocumentElement.GetElementsByTagName("book").Count);
            }
            else
            {
                //If file doesn't exist, create it and create a new book
                XmlNode dec = xmldoc.CreateXmlDeclaration("1.0", "utf-8", "");
                xmldoc.AppendChild(xmldoc);
                XmlNode root = xmldoc.CreateElement("books");

                //create a new boook
                XmlElement book = CreateBookElement(xmldoc, b);
                root.AppendChild(book);
                xmldoc.AppendChild(root);

                //Remove the first book if the number of books exceeds 5
                //If less than 5, just save the file
                if (xmldoc.DocumentElement.GetElementsByTagName("book").Count > 5)
                {
                    xmldoc.DocumentElement.RemoveChild(xmldoc.DocumentElement.FirstChild);
                }
            }

            xmldoc.Save(path);
            return RedirectToAction("Index");
        }

        private XmlElement CreateBookElement(XmlDocument xmldoc, Models.Book newBook)
        {
            //Create elements and an attribute
            XmlElement book = xmldoc.CreateElement("book");
            XmlElement id = xmldoc.CreateElement("id");
            id.InnerText = newBook.ID.ToString();
            XmlElement title = xmldoc.CreateElement("title");
            title.InnerText = newBook.Title;

            XmlElement author = xmldoc.CreateElement("author");
            XmlAttribute authortitle = xmldoc.CreateAttribute("title");
            authortitle.Value = newBook.AuthorTitle;
            XmlElement firstname = xmldoc.CreateElement("firstname");
            firstname.InnerText = newBook.FirstName;
            XmlElement middlename = xmldoc.CreateElement("middlename");
            middlename.InnerText = newBook.MiddleName;
            XmlElement lastname = xmldoc.CreateElement("lastname");
            lastname.InnerText = newBook.LastName;

            //Add an attribute and elements to author
            author.Attributes.Append(authortitle);
            author.AppendChild(firstname);
            author.AppendChild(middlename);
            author.AppendChild(lastname);

            //Add elements to book
            book.AppendChild(id);
            book.AppendChild(title);
            book.AppendChild(author);

            return book;
        }
        
    }

}
