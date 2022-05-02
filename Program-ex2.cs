using System;
using System.Collections.Generic;

namespace LibraryApp2
{
    public class Book
    {
        public int ID { get; }
        public string Title { get; set; }
        public string Author_Fname { get; set; }
        public string Author_Lname { get; set; }
        public string Genre { get; set; }

        public Book(int id, string title, string author_first_name, string author_last_name, string genre = "")
        {
            ID = id;
            Title = title;
            Author_Fname = author_first_name;
            Author_Lname = author_last_name;
            Genre = genre;
        }
        public override bool Equals(object obj)
        {
            bool result = (this.ID == ((Book)obj).ID);
            return result;
        }
    }

    public class PaperBook : Book
    {
        public int Copy_Num { get; set; }

        public PaperBook(int id, string title, string author_first_name, string author_last_name, string genre) : base(id, title, author_first_name, author_last_name, genre)
        {
            Copy_Num = 1;
        }

        public void AddCopy()
        {
            Copy_Num++;
        }
    }


    public class DigitalBook : Book
    {
        public DigitalBook(int id, string title, string author_first_name, string author_last_name, string genre) : base(id, title, author_first_name, author_last_name, genre)
        {

        }
    }

    public class Subscriber
    {
        public string ID { get; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public int NumofBooksLoaned { get; set; }
        public List<Book>SubBooksList;

        public Subscriber(string id, string first_name, string last_name)
        {
            ID = id;
            Fname = first_name;
            Lname = last_name;
            NumofBooksLoaned = 0;
            SubBooksList = new List<Book>();
        }

        public override bool Equals(object obj)
        {
            bool result = (this.ID == ((Subscriber)obj).ID);
            return result;
        }

        public void loanBook(Book book)
        {
            NumofBooksLoaned++;
            SubBooksList.Add(book);
        }

        public int returnBook(Book book)
        {
            if (SubBooksList.Contains(book))
            {
                SubBooksList.Remove(book);
                if (NumofBooksLoaned > 0)
                {
                    NumofBooksLoaned--;
                }
                return 1;
            }
            else
            {
                Console.WriteLine("Book doesn't exist in subscriber loaned books list!");
                return -1;
            }

        }
    }

    public class Library
    {
        public Dictionary<int, Book> All_Books_Dict;
        public Dictionary<string, Subscriber> All_Subscribers_Dict;

        private int Loan_Limit;
        private int last_book_location;
        public int BookCounter { get; set; }

        public Library(int loan_limit)
        {
            All_Books_Dict = new Dictionary<int, Book>();
            All_Subscribers_Dict = new Dictionary<string, Subscriber>();

            Loan_Limit = loan_limit;
            last_book_location = -1;
            BookCounter = 0;
        }


        public void AddSubscriber(string strID, Subscriber subscriber)
        {
            if (All_Subscribers_Dict.ContainsKey(strID) == false)
            {
                All_Subscribers_Dict.Add(strID, subscriber);
                Console.WriteLine("Subscriber added successfully");
            }
            else
            {
                Console.WriteLine("Subscriber already exists");
                All_Subscribers_Dict[strID] = subscriber;  //update value
            }
            Console.WriteLine("Current subscribers Count = " + All_Subscribers_Dict.Count);
        }



        public void AddBook(int key, Book book)
        {
            if (All_Books_Dict.ContainsKey(key) == true)
            {
                if (book is PaperBook)
                {
                    ((PaperBook)All_Books_Dict[key]).AddCopy();
                    Console.WriteLine("updated number of copies, current copies number:" + ((PaperBook)All_Books_Dict[key]).Copy_Num);
                }
                else
                {
                    Console.WriteLine("book already exists");
                }
            }
            else
            {
                All_Books_Dict.Add(key, book);
                Console.WriteLine("Success");
                BookCounter++;
            }
        }



        public void LoanBook(int inptOpt, string inptBook, string subscriberID)
        {
            if (All_Subscribers_Dict.ContainsKey(subscriberID) == false)
            {
                Console.WriteLine("Subscriber doesn't exist!");
            }
            else
            {
                Subscriber subscriber = All_Subscribers_Dict[subscriberID];
                Book req_book;
                
                int bookID;
                if (inptOpt == 1)
                    //lookup book by key
                {
                    try
                    {
                        bookID = int.Parse(inptBook);   

                        if (All_Books_Dict.ContainsKey(bookID) == true)
                        {
                            req_book = All_Books_Dict[bookID];
                        }
                        else
                        {
                            Console.WriteLine("Book does not exist!");
                            return;
                        }
                    }
                    catch (Exception e)    // error in case of user input string instead of int
                    {
                        Console.WriteLine("Error! Please type book ID in numbers only!");
                        return;
                    }
                    
                }
                else if (inptOpt == 2)
                {
                    //lookup book by name
                    string bookTitle = inptBook;
                    bool book_exist = false;
                    foreach (var item in All_Books_Dict)
                    {
                        if (item.Value.Title == bookTitle)
                        {
                            book_exist = true;
                            Console.WriteLine("Book ID: " + item.Key + ", Book Info: " + item.Value.Title, item.Value.Author_Fname, item.Value.Author_Lname);
                        }
                    }
                    if (!book_exist)
                    {
                        Console.WriteLine("No Books found with this name!");
                        return;
                    }

                    Console.WriteLine("type Book ID..");
                    bookID = Convert.ToInt32(Console.ReadLine().Trim());
                    req_book = All_Books_Dict[bookID];
                }
                else
                {
                    return;
                }


                if (subscriber.NumofBooksLoaned < Loan_Limit)
                {
                    if (req_book is PaperBook)
                    {
                        if ( ((PaperBook)All_Books_Dict[bookID]).Copy_Num > 0)
                        {
                            Console.WriteLine("Paper Book successfully loaned");
                            subscriber.loanBook(req_book);
                            ((PaperBook)All_Books_Dict[bookID]).Copy_Num--;
                        }
                        else
                        {
                            Console.WriteLine("All copies of the book are already taken");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Digital Book successfully loaned");
                        subscriber.loanBook(req_book);
                    }
                }
                else
                {
                    Console.WriteLine("Subscriber reached loan limit");
                }
            }
        }

        public void ReturnBook(string sub_id, int book_key)
        {
            Subscriber subscriber;
            if (All_Subscribers_Dict.ContainsKey(sub_id))
            {
                subscriber = All_Subscribers_Dict[sub_id];
            }
            else
            {
                Console.WriteLine("Subscriber doesn't exist");
                return;
            }

            if (All_Books_Dict.ContainsKey(book_key))
            {
                Book book = All_Books_Dict[book_key];

                //check if subscriber has this book. if yes then update subscriber books list
                int result = subscriber.returnBook(book);
                if (result == -1) {  // (-1) if subscriber doesn't own the book
                    return;
                }

                if (book is PaperBook)
                {
                    ((PaperBook)All_Books_Dict[book_key]).Copy_Num++;
                }
                Console.WriteLine("Book returned successfully");
            }
            else
            {
                Console.WriteLine("Book doesn't exist");
                return;
            }
        }



        
        public void PrintBookInfo(string bookName, string authorFirstName, string authorLastName)
        {
            Book book;
            int book_key;
            bool isFound = false;

            foreach (var item in All_Books_Dict)
            {
                if (item.Value.Title == bookName && item.Value.Author_Fname == authorFirstName && item.Value.Author_Lname == authorLastName)
                {
                    book_key = item.Key;
                    book = All_Books_Dict[book_key];
                    isFound = true;
                    if (book is PaperBook)
                    {
                        int numOfCopies = ((PaperBook)All_Books_Dict[book_key]).Copy_Num;
                        Console.WriteLine(item.Value.Title + ", paper-book, " + item.Value.Genre + ", num of available copies: " + numOfCopies);
                        return;
                    }
                    Console.WriteLine(item.Value.Title + ", digital-book, " + item.Value.Genre);
                    return;
                }
            }
            
            if (!isFound)
            {
                Console.WriteLine("Book doesn't exist!");
            }
        }
        

        
        public void PrintBooksByGenre(string genre)
        {
            bool isFound = false;
            foreach (var item in All_Books_Dict)
            {
                if (item.Value.Genre == genre)
                {
                    isFound = true;
                    Console.WriteLine(item.Value.Title);
                }
            }
            if (!isFound)
            {
                Console.WriteLine("No books found with this genre");
            }
        }
        


        public void ShowSubBooks (string subscriber_id)
        {
            Subscriber subscriber;
            if (All_Subscribers_Dict.ContainsKey(subscriber_id))
            {
                subscriber = All_Subscribers_Dict[subscriber_id];
                if (subscriber.SubBooksList.Count == 0)
                {
                    Console.WriteLine("Subscriber doesn't own any books");
                    return;
                }
                foreach (var item in subscriber.SubBooksList)
                {
                    Console.WriteLine("Book ID: "+ item.ID + ", Title: " + item.Title + ", Author: " + item.Author_Fname + " " + item.Author_Lname);
                }
            }
            else
            {
                Console.WriteLine("Subscriber doesn't exist!");
            }
        }




    }




    class Program
    {
        static void Main(string[] args)
        {
            const int LOAN_LIMIT = 3;    //max num of books a subscriber can loan
            Console.WriteLine("Library Application Started");
            Library Lib = new Library(LOAN_LIMIT);

            bool app_over = false;

            while (!app_over)
            {
                Console.WriteLine("------------ Choose activity 1-7. 8 to Exit ------------");
                Console.WriteLine("1: Add New Book");
                Console.WriteLine("2: Add New Subscriber");
                Console.WriteLine("3: Rent a Book");
                Console.WriteLine("4: Return a Book");
                Console.WriteLine("5: Print Book Details");
                Console.WriteLine("6: Print Book by Genre");
                Console.WriteLine("7: Show Subscriber Books List (*NEW!)");
                Console.WriteLine("8: Exit Library");
                Console.WriteLine("--------------------------------------------------------");

                int option = Convert.ToInt32(Console.ReadLine());
                switch (option)
                {
                    case 1:
                        //Add New Book
                        {
                            Console.WriteLine("Add New Book..");
                            Book book;
                            Console.WriteLine("Please enter book type : paper or digital");
                            string book_type = Console.ReadLine();
                            if (!(book_type == "paper" || book_type == "digital"))
                            {
                                Console.WriteLine("Error!, Book Type must be either 'paper' or 'digital' ");
                                continue;
                            }

                            //enter book key and check validation
                            Console.WriteLine("Please enter book key (up to 7 numbers)");
                            string inpt_key = Console.ReadLine();
                            if (inpt_key.Length > 7)
                            {
                                Console.WriteLine("Key can't be more than 7 numbers");
                                continue;
                            }
                            int book_key;
                            try
                            {
                                book_key = int.Parse(inpt_key);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error: Please use numbers only for book key");
                                continue;
                            }
                            
                            Console.WriteLine("Please enter book title");
                            string book_title = Console.ReadLine();
                            Console.WriteLine("Please enter book author first name");
                            string author_first_name = Console.ReadLine();
                            Console.WriteLine("Please enter book author last name");
                            string author_last_name = Console.ReadLine();
                            Console.WriteLine("Please enter book genre");
                            string book_genre = Console.ReadLine();
                            if (book_type == "paper")
                            {
                                book = new PaperBook(book_key, book_title, author_first_name, author_last_name, book_genre);
                            }
                            else
                            {
                                book = new DigitalBook(book_key, book_title, author_first_name, author_last_name, book_genre);
                            }
                            Lib.AddBook(book_key, book);
                            break;
                        }

                    case 2:
                        //Add New Subscriber
                        {
                            Subscriber sub;
                            Console.WriteLine("Add New Subscriber..");
                            Console.WriteLine("Input Subsriber ID");
                            string subID = Console.ReadLine().Trim();
                            try
                            {
                                int.Parse(subID);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error: Please type only numbers for subscriber ID");
                                continue;
                            }
                            Console.WriteLine("Input Subsriber First Name");
                            string subFirstName = Console.ReadLine().Trim();
                            Console.WriteLine("Input Subsriber Last Name");
                            string subLastName = Console.ReadLine().Trim();
                            sub = new Subscriber(subID, subFirstName, subLastName);
                            Lib.AddSubscriber(subID, sub);
                            break;
                        }
                    case 3:
                        //Rent a Book
                        {
                            Console.WriteLine("Rent a Book..");
                            Console.WriteLine("1. Choose Book by ID    2. Choose Book By Title");
                            int inptOpt = Convert.ToInt32(Console.ReadLine().Trim());
                            string inptBook;
                            if (inptOpt == 1)
                            {
                                Console.WriteLine("Input Book ID");
                                inptBook = Console.ReadLine().Trim();
                            }
                            else if (inptOpt == 2)
                            {
                                Console.WriteLine("Input Book Title");
                                inptBook = Console.ReadLine().Trim();
                            }
                            else
                            {
                                Console.WriteLine("Error! Please choose 1 or 2 only!");
                                continue;
                            }

                            Console.WriteLine("Input Subscriber ID");
                            string inptSubID = Console.ReadLine().Trim();

                            Lib.LoanBook(inptOpt, inptBook, inptSubID);
                            break;
                        }
                        
                    case 4:
                        //Return a Book
                        {
                            Console.WriteLine("Return a Book..");

                            Console.WriteLine("Input Subscriber ID");
                            string inptSubID = Console.ReadLine().Trim();

                            Console.WriteLine("Input Book ID");
                            string BookID = Console.ReadLine().Trim();
                            int book_key;
                            try
                            {
                                book_key = int.Parse(BookID);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error: Please use numbers only for book key");
                                continue;
                            }

                            Lib.ReturnBook(inptSubID, book_key);
                            break;
                        }
                        
                        
                    case 5:
                        //Print Book Details
                        {
                            Console.WriteLine("Print Book Details..");

                            Console.WriteLine("Input Book Name");
                            string inptBookName = Console.ReadLine().Trim();
                            Console.WriteLine("Input Author First Name");
                            string inptAuthorFirstName = Console.ReadLine().Trim();
                            Console.WriteLine("Input Author Last Name");
                            string inptAuthorLastName = Console.ReadLine().Trim();

                            Lib.PrintBookInfo(inptBookName, inptAuthorFirstName, inptAuthorLastName);
                            break;
                        }  
                    case 6:
                        //Print Book by Genre
                        {
                            Console.WriteLine("Print Book by Genre..");
                            Console.WriteLine("Input Genre");
                            string inptGenre = Console.ReadLine().Trim();
                            Lib.PrintBooksByGenre(inptGenre);
                            break;
                        }
                    case 7:
                        //Show Subscriber Books List
                        {
                            Console.WriteLine("Show Subscriber Books List..");
                            Console.WriteLine("Enter subscriber ID");
                            string sub_id = Console.ReadLine().Trim();

                            Lib.ShowSubBooks(sub_id);
                            break;
                        }    
                    case 8:
                        //Exit
                        {
                            Console.WriteLine("Good Bye!");
                            app_over = true;
                            break;
                        }
                    default:
                        break;
                }
            }
        }
    }
}
