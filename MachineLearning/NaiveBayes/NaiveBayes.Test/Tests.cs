using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NaiveBayes.Data;

namespace NaiveBayes.Test
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestVocabList()
        {
            var vocabList = VocabBuilder.Vocab(SourceDocuments);

            var documentVocabVector = SourceDocuments.First().VocabListVector(vocabList);

            for (int i = 0; i < SourceDocuments.First().Words.Count;i++)
            {
                // vocab vector is seeded with all 1's, so a value of 2.0 means the word was found once

                Assert.That(documentVocabVector[i], Is.EqualTo(2.0));
            }
        }

        [Test]
        public void TestClassProbability()
        {
            var probs = NaiveBayes.TrainBayes(SourceDocuments);

            var top10Absuve = probs.Probabilities.FirstOrDefault(p => p.Class.Name == "abusive").Top(10);

            Assert.That(top10Absuve.First().Word, Is.EqualTo("stupid"));
        }

        [Test]
        public void TestClassifier()
        {
            var probs = NaiveBayes.TrainBayes(SourceDocuments);

            foreach (var doc in SourceDocuments)
            {
                var @class = NaiveBayes.Classify(doc, probs);

                Assert.That(@class.Name, Is.EqualTo(doc.Class.Name));
            }
        }

        [Test]
        public void TestStackOverflow()
        {
            var stackOverflowDocs = StackExchangeReader.Parse(StackOverflowTrainingJson, "stackOverflow");
            var programmersDocs = StackExchangeReader.Parse(ProgrammersTrainingJson, "programmers");

            var stackOverflowTest = StackExchangeReader.Parse(StackOverflowTestJson, "stackOverflow");
            var programmersTest = StackExchangeReader.Parse(ProgrammersTestJson, "programmers");

            var trainingSet = NaiveBayes.TrainBayes(stackOverflowDocs.Concat(programmersDocs).ToList());

            var top10Programmers = trainingSet.Top(10, "programmers");
            var top10Stack = trainingSet.Top(10, "stackOverflow");

            
            Func<List<Document>, double> verifier = (list) =>
                {
                    var d = 0.0;

                    foreach (var doc in list)
                    {
                        var category = NaiveBayes.Classify(doc, trainingSet);

                        if (category.Name == doc.Class.Name)
                        {
                            d++;
                        }
                    }

                    return d/list.Count();
                };

            var successOfStack = verifier(stackOverflowTest);
            var successOfProgrammers = verifier(programmersTest);

            Console.WriteLine("Stack: {0:0.00}", successOfStack);
            Console.WriteLine("Prog: {0:0.00}", successOfProgrammers);

        }

        private List<Document> SourceDocuments
        {
            get
            {
                var abusive = new Class {Name = "abusive"};
                var nonAbusive = new Class {Name = "nonAbusive"};

                return new List<Document>
                       {
                           new Document
                           {
                               Words = VocabBuilder.Tokenize(@"my dog has fleas problems help please").Words,
                               Class = nonAbusive
                           },
                           new Document
                           {
                               Words = VocabBuilder.Tokenize(@"maybe not take him to dog park stupid").Words,
                               Class = abusive
                           },
                           new Document
                           {
                               Words = VocabBuilder.Tokenize(@"my dalmation is so cute I love him").Words,
                               Class = nonAbusive
                           },
                           new Document
                           {
                               Words = VocabBuilder.Tokenize(@"stop posting stupid worthless garbage").Words,
                               Class = abusive
                           },
                           new Document
                           {
                               Words = VocabBuilder.Tokenize(@"mr licks ate my steak how to stop him").Words,
                               Class = nonAbusive
                           },
                           new Document
                           {
                               Words = VocabBuilder.Tokenize(@"quit buying worthless dog food stupid").Words,
                               Class = abusive
                           }
                       };
            }
        } 

        private string StackOverflowTestJson
        {
            get
            {
                return @"{
  ""items"": [
    {
      ""title"": ""Cannot push to Heroku because key fingerprint""
    },
    {
      ""title"": ""Take (n) characters from a string""
    },
    {
      ""title"": ""Plone - text color and background color not working in TinyMCE editor""
    },
    {
      ""title"": ""WCF and dependency injection""
    },
    {
      ""title"": ""Get Checkbox value from contextmenu (jquery + medialize) with callback - solved""
    },
    {
      ""title"": ""Bottle framework and OOP, using a method instead of a function""
    },
    {
      ""title"": ""Sybase: Incorrect syntax near &#39;go&#39; in a &#39;IF EXISTS&#39; block""
    },
    {
      ""title"": ""Reading HTML content from UIWebView after editing it""
    },
    {
      ""title"": ""Is it possible to page through a view in phpmyadmin?""
    },
    {
      ""title"": ""targeting small devices with mediaelements""
    },
    {
      ""title"": ""Howto Create Recommendations with a Incremental SVD Recommender System""
    },
    {
      ""title"": ""Failed to build json (1.6.3) with native extension after I installed Xcode 4.2""
    },
    {
      ""title"": ""SQL Query pivot approach assistance""
    },
    {
      ""title"": ""FB.login—same app ID with multiple domains""
    },
    {
      ""title"": ""Get last image from Photos.app?""
    },
    {
      ""title"": ""PDF Compression""
    },
    {
      ""title"": ""authenticated users""
    },
    {
      ""title"": ""DefaultHttpClient to AndroidHttpClient""
    },
    {
      ""title"": ""Google Analytics PHP API (GAPI) - Getting number of page views""
    },
    {
      ""title"": ""How to monitor a folder with all subfolders and files inside?""
    },
    {
      ""title"": ""Login to Google with PHP and Curl, Cookie turned off?""
    },
    {
      ""title"": ""Add form values via jQuery, and POST to controller method""
    },
    {
      ""title"": ""My copy of &quot;Processing&quot; does not have the &quot;processing.serial&quot; library""
    },
    {
      ""title"": ""Parallel.For VS For. Why there is this difference?""
    },
    {
      ""title"": ""iLMerge Enterprise Libary 5.0""
    },
    {
      ""title"": ""How can I access Amazon DynamoDB via Python?""
    },
    {
      ""title"": ""Using special characters such as &#39;+&#39; in a DTD entity""
    },
    {
      ""title"": ""Invoke Mac Kernel Panic?""
    },
    {
      ""title"": ""Rails Routing - :action =&gt; &#39;new&#39; returns error &quot;No route matches {:action=&gt;&quot;show&quot;... in the same controller""
    },
    {
      ""title"": ""Horizontally centering fields in a vertical field manager""
    },
    {
      ""title"": ""stream live video with ustream in asp.net""
    },
    {
      ""title"": ""How do I connect to the Google Calendar API without the oAuth authentication?""
    },
    {
      ""title"": ""Failed to find style &#39;null&#39; in current theme in aicharts while drawing barcharts""
    },
    {
      ""title"": ""Android &quot;iPod wheel&quot; seekbar""
    },
    {
      ""title"": ""How can I emulate Modules / Installers / Registries with SimpleInjector""
    },
    {
      ""title"": ""ASP.NET MVC 4 breaks ASP.NET MVC 3 projects""
    },
    {
      ""title"": ""RavenDB query by index and DateTime""
    },
    {
      ""title"": ""Not able to get the required HTTP headers in tcpdump""
    },
    {
      ""title"": ""Good Implementation of Scalable JavaScript Application Architecture (Sandbox by Nicholas Zakas)?""
    },
    {
      ""title"": ""Protobuf-net RPC with byte[]?""
    },
    {
      ""title"": ""GUI XML Comparison tool that parses the XML before diffing for Linux""
    },
    {
      ""title"": ""What is the difference between OSGi Components and Services""
    },
    {
      ""title"": ""Multiple stitching""
    },
    {
      ""title"": ""MySQL difference of two rows between two with PHP mysql_num_rows function?""
    },
    {
      ""title"": ""How to create a table filled with buttons for Android""
    },
    {
      ""title"": ""How to parse xml file on android""
    },
    {
      ""title"": ""forem gem for rails giving issues with custom authentication Rails 3.1""
    },
    {
      ""title"": ""How do I escape EJS template code in node.js to be evaluated on the client side?""
    },
    {
      ""title"": ""unable to run &quot; .jar&quot; file on windows 7""
    },
    {
      ""title"": ""Implementing alpha-numeric TextBox""
    },
    {
      ""title"": ""Failure to run Play Framework project from Eclipse, after &quot;eclipsify&quot;""
    },
    {
      ""title"": ""Error: unmappable character for encoding UTF8 during maven compilation""
    },
    {
      ""title"": ""How can I create a large Metro calendar?""
    },
    {
      ""title"": ""Horizontal scrollbar in Firefox caused by #fb-root""
    },
    {
      ""title"": ""What and How the Java Swing concept related to Cloud?""
    },
    {
      ""title"": ""insert a database value to a string array - not working""
    },
    {
      ""title"": ""How to make twitter bootstrap menu dropdown on hover rather than click""
    },
    {
      ""title"": ""couchdb crasehes on starting very first time""
    },
    {
      ""title"": ""How to resolve &quot;Could not find installable ISAM.&quot; error for OLE DB provider &quot;Microsoft.ACE.OLEDB.12.0&quot;""
    },
    {
      ""title"": ""Send parsed console output in email""
    },
    {
      ""title"": ""How can I close a Twitter Bootstrap popover with a click from anywhere (else) on the page?""
    },
    {
      ""title"": ""Quality Center Modify Status of Automatic Runner via OTA""
    },
    {
      ""title"": ""Should I use Python in stead of VBA?""
    },
    {
      ""title"": ""Standard input/output in Pyclewn(GDB front end for vim)""
    },
    {
      ""title"": ""When using google web font with multiple weights, IE6-8 get confused""
    },
    {
      ""title"": ""&quot;Cannot find interface declaration for NSObject&quot;?""
    },
    {
      ""title"": ""How to specify the public in ruby on rails app?""
    },
    {
      ""title"": ""How can I efficiently &#39;subclass&#39; in external CSS?""
    },
    {
      ""title"": ""How to detect if a paperclip attachment was changed in after_save callback?""
    },
    {
      ""title"": ""Update Rails app to use 3.2.x""
    },
    {
      ""title"": ""Set virtualenv for python in textmate 2""
    },
    {
      ""title"": ""Class not found exception in design time""
    },
    {
      ""title"": ""Show a text on the middle of the page - Vaadin""
    },
    {
      ""title"": ""Implementing Text Watcher for EditText""
    },
    {
      ""title"": ""How can I detect a file&#39;s encoding?""
    },
    {
      ""title"": ""Away3D 4.x alter ProjectionCenter""
    },
    {
      ""title"": ""ASP .NET membership custom error message on creating new user?""
    },
    {
      ""title"": ""Does C struct padding make this use unsafe?""
    },
    {
      ""title"": ""Is const-casting via a union undefined behaviour?""
    },
    {
      ""title"": ""Looking for fast sorted integer array intersection/union algorithms implemented in C""
    },
    {
      ""title"": ""Union of long and char[], byte order""
    },
    {
      ""title"": ""FTP Download Multiple Files using PowerShell""
    },
    {
      ""title"": ""Objective-C associated objects leaking under ARC""
    },
    {
      ""title"": ""what the difference between a class and an ID in Omniture Test &amp; Target?""
    },
    {
      ""title"": ""Add days to a mysql datetime row""
    },
    {
      ""title"": ""MySQL - Select Month difference""
    },
    {
      ""title"": ""SQL report with x and y field the same and z with 1 second difference""
    },
    {
      ""title"": ""Find highest difference between two rows""
    },
    {
      ""title"": ""Getting all friends&#39; pictures using the fields parmeter doesn&#39;t seem to work?""
    },
    {
      ""title"": ""jQuery: live() and delegate()""
    },
    {
      ""title"": ""How to calculate number of days between dates taking in account day saving shift?""
    },
    {
      ""title"": ""Difference between regular gcc and Mac OSX gcc?""
    },
    {
      ""title"": ""Web Audio API on Windows?""
    },
    {
      ""title"": ""How to semantically add heading to a list""
    },
    {
      ""title"": ""How does autolink:map work""
    },
    {
      ""title"": ""fatal error LNK1112: module machine type &#39;X86&#39; conflicts with target machine type &#39;x64&#39;""
    },
    {
      ""title"": ""how to insert null value into datetime column in sql with java""
    },
    {
      ""title"": ""What are the pros and cons of the different Scala middleware choices""
    },
    {
      ""title"": ""What is the difference between .hover and :hover?""
    },
    {
      ""title"": ""IIS Express : Access is denied""
    }
  ]
}";
            }
        }
    
        private string ProgrammersTestJson
        {
            get
            {
                return @"{
  ""items"": [
    {
      ""title"": ""What do you call a pair of function that are complements of each other?""
    },
    {
      ""title"": ""thick client migration to web based application""
    },
    {
      ""title"": ""Optimistic work sharing on sparsely distributed systems""
    },
    {
      ""title"": ""Resources relating to Java EE and Scala""
    },
    {
      ""title"": ""How do I Integrate Production Database Hot Fixes into Shared Database Development model?""
    },
    {
      ""title"": ""Privacy using laptop provided by my company""
    },
    {
      ""title"": ""Are there open source alternatives to Bitbucket, Github, Kiln, and similar DVCS browsing and management tools?""
    },
    {
      ""title"": ""Randomly generate directed graph on a grid""
    },
    {
      ""title"": ""How to Determine # of Programmers needed for a project""
    },
    {
      ""title"": ""Personal Software Process (PSP1)""
    },
    {
      ""title"": ""Rate your programming language expertise by language concepts""
    },
    {
      ""title"": ""Suggested HTTP REST status code for &#39;request limit reached&#39;""
    },
    {
      ""title"": ""Splitting Logic, Data, Layout and &quot;Hacks&quot;""
    },
    {
      ""title"": ""Google&#39;s App Inventor is no longer available for Mobile App development..... any alternatives?""
    },
    {
      ""title"": ""Self-Executing Anonymous Function vs Prototype""
    },
    {
      ""title"": ""What languages are the kids of today actually programming in? Does anyone have real data?""
    },
    {
      ""title"": ""Simple Hierarchical Clustering Implementations for C#?""
    },
    {
      ""title"": ""Can manager classes be a sign of bad architecture?""
    },
    {
      ""title"": ""Empirical Evidence of Popularity of Git and Mercurial""
    },
    {
      ""title"": ""USB software protection dongle for Java with an SDK which is cross-platform “for real”. Does it exist?""
    },
    {
      ""title"": ""Visualizing a CUP grammar""
    },
    {
      ""title"": ""Is this the correct approach to an OOP design structure in php?""
    },
    {
      ""title"": ""MVVM Reporting App Approach ? Data Access Layer?""
    },
    {
      ""title"": ""What are your advice, methods, or practices to take out the most from a day on-site at a customer?""
    },
    {
      ""title"": ""Handling extremely large numbers in a language which can&#39;t?""
    },
    {
      ""title"": ""Is there any site which tells or highlights by zone developer income source? I think i am getting less yearly""
    },
    {
      ""title"": ""Basis of definitions""
    },
    {
      ""title"": ""What advantages does developing applications for smartphones have over developing the same application as a web application?""
    },
    {
      ""title"": ""What are the things that you consider in reviewing an iOS source code?""
    },
    {
      ""title"": ""How to manage github issues for (priority, etc)?""
    },
    {
      ""title"": ""Are Business Majors considered for Programming Jobs?""
    },
    {
      ""title"": ""How does someone with a business degree become a software consultant?""
    },
    {
      ""title"": ""Getting users to write decent and useful bug reports""
    },
    {
      ""title"": ""Source control workflow for managing a software platform""
    },
    {
      ""title"": ""What is a &quot;markdown-formatted&quot; README file on Github?""
    },
    {
      ""title"": ""Are there any podcasts (not lectures) about compiler development?""
    },
    {
      ""title"": ""Linux Programmer moving to Windows""
    },
    {
      ""title"": ""What is the name for the programming paradigm characterized by Go?""
    },
    {
      ""title"": ""Should unit tests be stored in the repository?""
    },
    {
      ""title"": ""Change Management Standard""
    },
    {
      ""title"": ""Testing an IRC Bot""
    },
    {
      ""title"": ""Years experience over unfinished degree?""
    },
    {
      ""title"": ""Best practices for team workflow with RoR/Github for designer + coder?""
    },
    {
      ""title"": ""Mercurial Repository structure with heavyweight corporate comms, configuration management &amp; test requirements""
    },
    {
      ""title"": ""Functional Programming: Are Tuples a viable replacement for Types?""
    },
    {
      ""title"": ""Is it ok to call external services or database inside the entity""
    },
    {
      ""title"": ""Is Learning C++ Through The Qt Framework Really Learning C++""
    },
    {
      ""title"": ""Are C and/or C++ viable/practical options for web development?""
    },
    {
      ""title"": ""Developing a mobile interface for Website""
    },
    {
      ""title"": ""How Do You Estimate Project Hours Using Pivotal Tracker If You Are a Services Firm?""
    },
    {
      ""title"": ""Can you point me to a nontrivial strategy pattern implementation?""
    },
    {
      ""title"": ""Never use Strings in Java?""
    },
    {
      ""title"": ""Is there any material on practical programming in Coq?""
    },
    {
      ""title"": ""WCF and Service Registry""
    },
    {
      ""title"": ""Agile unified process vs. agile lifecycle process""
    },
    {
      ""title"": ""planning the same app for both OSX and iOS""
    },
    {
      ""title"": ""Efficient graph clustering algorithm""
    },
    {
      ""title"": ""Interactive training site for Javascript complete with code challenges""
    },
    {
      ""title"": ""How to find Sub-trees in non-binary tree""
    },
    {
      ""title"": ""Advantage/disadvantage of parameters / return types declaration in languages with type inference""
    },
    {
      ""title"": ""How can I simulate clicks for a mobile browser?""
    },
    {
      ""title"": ""Is DreamWeaver a good tool to write PHP code?""
    },
    {
      ""title"": ""Strategy for building an application to replace a large spreadsheet""
    },
    {
      ""title"": ""Is it a really required skill to program without API documentation?""
    },
    {
      ""title"": ""Is Agile applicable in product development companies as well?""
    },
    {
      ""title"": ""n-layers architecture design, really worth the effort?""
    },
    {
      ""title"": ""Difference between DevOps and Software Configuration Management""
    },
    {
      ""title"": ""Intelligence as a vector quantity""
    },
    {
      ""title"": ""GitHub: Are there external tools for managing issues list vs. project backlog""
    },
    {
      ""title"": ""Mobile Web Security Articles""
    },
    {
      ""title"": ""Where is a good place to start to learn about custom caching in .Net""
    },
    {
      ""title"": ""How to avoid &quot;DO YOU HAZ TEH CODEZ&quot; situations?""
    },
    {
      ""title"": ""How to protect own software from copying""
    },
    {
      ""title"": ""How can I prevent programmers from capturing data entered by users?""
    },
    {
      ""title"": ""Are there any tutorials available like NerdDinner but for Java?""
    },
    {
      ""title"": ""Apps for facilitating live code demos?""
    },
    {
      ""title"": ""Should I intentionally break the build when a bug is found in production?""
    },
    {
      ""title"": ""Right Tools For The Job? One Location vs Multiple?""
    },
    {
      ""title"": ""UserControl inside Placeholder good/bad? + Postback problem""
    },
    {
      ""title"": ""Help me understand how to stream video""
    },
    {
      ""title"": ""How can I transition from database development to programming/software engineering?""
    },
    {
      ""title"": ""Programmers clipboard monitor under Windows""
    },
    {
      ""title"": ""How do you recruit a graduate developer?""
    },
    {
      ""title"": ""How to gauge a person&#39;s ability to write SQL queries?""
    },
    {
      ""title"": ""How do you keep track of large projects?""
    },
    {
      ""title"": ""What is the point of the string.Empty property""
    },
    {
      ""title"": ""Process of developing software?""
    },
    {
      ""title"": ""What&#39;s the best way to learn nature-inspired algorithms?""
    },
    {
      ""title"": ""How should an administrating web application control a WCF service&#39;s entities?""
    },
    {
      ""title"": ""typedefs and #defines""
    },
    {
      ""title"": ""Performance of Scala compared to Java""
    },
    {
      ""title"": ""How do I determine how many numbers are in a floating point number system?""
    },
    {
      ""title"": ""Is it possible to write application-level logic in an XML or plain text format?""
    },
    {
      ""title"": ""What is the difference between routed events and attached events?""
    },
    {
      ""title"": ""How to answer &quot;When will it be done?&quot;""
    },
    {
      ""title"": ""Should SpecFlow be used with BDD as a solo developer?""
    },
    {
      ""title"": ""How to use BPMN and use case and other diagrams together""
    },
    {
      ""title"": ""What is the easiest and cheapest way to distribute apple iOS enterprise app?""
    },
    {
      ""title"": ""How big should OSGi bundles be?""
    },
    {
      ""title"": ""What is the &quot;architecture&quot; that provides functionality to application-level URI interfaces, like chrome:// and Firefox&#39;s about:config, etc?""
    }
  ]
}";
            }
        }
        private string ProgrammersTrainingJson
        {
            get
            {
                return @"{
  ""items"": [
    {
      ""title"": ""Using Bit Torrent for internal release management""
    },
    {
      ""title"": ""Well I was given a &quot;ruby&quot; what do I do with It?""
    },
    {
      ""title"": ""In what language should I name my business classes?""
    },
    {
      ""title"": ""Is there any technique to force a daemon thread to finish some task if User threads already finished their working?""
    },
    {
      ""title"": ""Difference between count(*) and count(1) in mysql?""
    },
    {
      ""title"": ""Is it a must for every programmer to learn regular expressions?""
    },
    {
      ""title"": ""What is the relevance of resumes in the age of GitHub, Stack Exchange, Coursera, Udacity, blogs, etc.?""
    },
    {
      ""title"": ""What&#39;s the difference between the insert methods and why one is faster""
    },
    {
      ""title"": ""Should I take care of race conditions which almost certainly has no chance of occuring?""
    },
    {
      ""title"": ""Design considerations on JSON schema for scalars with a consistent attachment property""
    },
    {
      ""title"": ""Are There Realistic/Useful Solutions for Source Control for Ladder Logic Programs""
    },
    {
      ""title"": ""Can I maintain server attributes in Word outside of sharepoint?""
    },
    {
      ""title"": ""How to generate random numbers without making new Random objects?""
    },
    {
      ""title"": ""best language for making IM like whatsapp or 2go""
    },
    {
      ""title"": ""simple windows SVN""
    },
    {
      ""title"": ""WebForms future""
    },
    {
      ""title"": ""best free downloadable python ebook for beginner""
    },
    {
      ""title"": ""What are the benefits of prefixing function parameter names with p*?""
    },
    {
      ""title"": ""BDD in .NET - Chicken or Egg or..?""
    },
    {
      ""title"": ""Caching&#39;s Effect on Program Performance""
    },
    {
      ""title"": ""If immutable objects are good, why do people keep creating mutable objects?""
    },
    {
      ""title"": ""Is speed a parameter for responding emails of technical tests?""
    },
    {
      ""title"": ""Which CSS attributes should be in HTML and which in BODY?""
    },
    {
      ""title"": ""Work flow when editing files in /var/www. Constantly needing to input &quot;sudo&quot; commands""
    },
    {
      ""title"": ""What is the simplest/easiest way to display line of text only when printing a webpage using PHP?""
    },
    {
      ""title"": ""Using replacement to get possible outcomes to then search through HUGE amount of data""
    },
    {
      ""title"": ""Choosing between Single or multiple projects in a git repository?""
    },
    {
      ""title"": ""iPad app architecture with very large files""
    },
    {
      ""title"": ""What&#39;s it like to program without eyesight?""
    },
    {
      ""title"": ""How do you keep the size of your application down (memory usage and physical storage size)?""
    },
    {
      ""title"": ""Why it is necessary to to test my iPhone app on actual iPhone device""
    },
    {
      ""title"": ""What is your experience regarding iPhone app rejection?""
    },
    {
      ""title"": ""Is it bad practice to use public fields?""
    },
    {
      ""title"": ""Is it better to target iOS 5 and ARC or an earlier version with MRC?""
    },
    {
      ""title"": ""If an app runs well on iPhone 3GS with iOS 5.1, how to know that it runs well on iOS 4.3?""
    },
    {
      ""title"": ""printf - source of bugs?""
    },
    {
      ""title"": ""Is C# more open than Java""
    },
    {
      ""title"": ""Where can I locate business data to use in my application?""
    },
    {
      ""title"": ""Registration: Email hash and verifying only one account per email""
    },
    {
      ""title"": ""Contractor - Acceptable Payment Terms?""
    },
    {
      ""title"": ""Clients with multiple proxy and multithreading callbacks""
    },
    {
      ""title"": ""Why are inheritance and polymorphism so widely used?""
    },
    {
      ""title"": ""Why do we need to separate classes which have different functionality?""
    },
    {
      ""title"": ""What are the problems which I will face if all the classes I use are loosely coupled""
    },
    {
      ""title"": ""Are there deprecated practices for multithread and multiprocessor programming that I should no longer use?""
    },
    {
      ""title"": ""Explanation of the Google Gravity trick""
    },
    {
      ""title"": ""How to use the clients webcam for recording through a website?""
    },
    {
      ""title"": ""How to apply one of the OOP concepts (Closed for Modification and Open for extension)?""
    },
    {
      ""title"": ""What is the best way to INSERT a large dataset into a MySQL database (or any database in general)""
    },
    {
      ""title"": ""Best approach for writing a chess engine?""
    },
    {
      ""title"": ""Could implicit static methods cause problems?""
    },
    {
      ""title"": ""How to architect application or project""
    },
    {
      ""title"": ""SQL Server 2000 vs 2008 Express?""
    },
    {
      ""title"": ""How to add Preference in ICS""
    },
    {
      ""title"": ""How related is coding/information theory to computer networking?""
    },
    {
      ""title"": ""Conditional payment on ecommerce site?""
    },
    {
      ""title"": ""How can I test PHP skills in a interview?""
    },
    {
      ""title"": ""Code refactoring advice""
    },
    {
      ""title"": ""Write Left Outer Join Query with and operator in to LINQ or Entity Data Model""
    },
    {
      ""title"": ""Do large non-IT corporations typically develop their external facing websites in-house?""
    },
    {
      ""title"": ""Why should I declare a class as an abstract class?""
    },
    {
      ""title"": ""Do you feel bad when you have to learn new things?""
    },
    {
      ""title"": ""Will apple approve app which internally uses library developed by Google?""
    },
    {
      ""title"": ""Why is using System.out.println() so bad?""
    },
    {
      ""title"": ""Why payment processors rarely support account outside USA?""
    },
    {
      ""title"": ""Doctoral research and work for a company with similar profile""
    },
    {
      ""title"": ""AVL Trees and the REAL world""
    },
    {
      ""title"": ""Dealing with Upgrade Lists""
    },
    {
      ""title"": ""Boss Asking To Work Overtime But Under the Radar""
    },
    {
      ""title"": ""Can a single simple language such as Clojure replace Html + JavaScript + CSS + Flash + Java Applets ...?""
    },
    {
      ""title"": ""Don&#39;t Use &quot;Static&quot; in C#?""
    },
    {
      ""title"": ""Where can I learn about browser-specific quirks?""
    },
    {
      ""title"": ""Expected behavior when an request for a collection will have zero items""
    },
    {
      ""title"": ""Why null pointer instead of class cast?""
    },
    {
      ""title"": ""Use constructor or setter method?""
    },
    {
      ""title"": ""&quot;Documenting Software Architectures&quot; differences between first and second ed""
    },
    {
      ""title"": ""Pair programming remotely with Visual Studio?""
    },
    {
      ""title"": ""Deploying Symfony2 Project on Cpanel""
    },
    {
      ""title"": ""Can I change operator precedence and associativity in C++?""
    },
    {
      ""title"": ""Macintosh OS10.8 user experience with &#39;identified developer&#39; downloads?""
    },
    {
      ""title"": ""How much control can I expect to have over my work environment?""
    },
    {
      ""title"": ""Don&#39;t Use Static?""
    },
    {
      ""title"": ""Can an open source solution match or surpass Team Foundation Server&#39;s features?""
    },
    {
      ""title"": ""Order of learning sort algorithms""
    },
    {
      ""title"": ""Algorithm Analysis: Frequencies of Execution""
    },
    {
      ""title"": ""Extend functionallity of a class: inheritance or java&#39;s dynamic proxy""
    },
    {
      ""title"": ""Using captured non-local variables in C++ closures""
    },
    {
      ""title"": ""Technical analysis in software development""
    },
    {
      ""title"": ""How to implement Facebook leaderboard game for mobile?""
    },
    {
      ""title"": ""Which PHP framework is best for Facebook app?""
    },
    {
      ""title"": ""Is there a programming language where 1/6 behaves the same as 1.0/6.0?""
    },
    {
      ""title"": ""Why do developers need to keep up to date with technologies and methodologies?""
    },
    {
      ""title"": ""front end development test - html/css/jquery/javascript""
    },
    {
      ""title"": ""Should I prefer properties with or without private fields?""
    },
    {
      ""title"": ""taking project to online from localhost""
    },
    {
      ""title"": ""Programming is easy, Designing is hard""
    },
    {
      ""title"": ""What should I understand before I try to understand functional programming?""
    },
    {
      ""title"": ""How do you organize highly customized software?""
    },
    {
      ""title"": ""Reverse engineering: what is it really good for?""
    },
    {
      ""title"": ""Learning Multiple Languages Simultaneously""
    },
    {
      ""title"": ""How should be a good design in a project with JSF and JPA?""
    },
    {
      ""title"": ""How do I quote a job with PHPUnit?""
    },
    {
      ""title"": ""Using Lucene, Solr or elasticsearch to index Amazon S3, Rackspace Cloud Files or OpenStack SWIFT""
    },
    {
      ""title"": ""What does Symfony Framework offer that Zend Framework does not?""
    },
    {
      ""title"": ""How can I find a good open source project to join?""
    },
    {
      ""title"": ""How to understand Linux kernel source code for a beginner?""
    },
    {
      ""title"": ""OOP - Composition, Components and Composites Example?""
    },
    {
      ""title"": ""Best way to load application settings""
    },
    {
      ""title"": ""Can I legally and ethically take an open-source project with community contributions to closed-source?""
    },
    {
      ""title"": ""Need some thoughts on how to design this project and go about it""
    },
    {
      ""title"": ""How will quantum computing change programming?""
    },
    {
      ""title"": ""Learning the Operating Systems concepts and Programming Languages for elderly""
    },
    {
      ""title"": ""Queue Based Multithreading?""
    },
    {
      ""title"": ""What do you prefer: smaller and higher skilled teams or bigger and less skilled?""
    },
    {
      ""title"": ""How to set up Unit Testing in Visual Studio 2010?""
    },
    {
      ""title"": ""Is it worth taking OCPWCD or OCPBCD exams after clearing up SCJP?""
    },
    {
      ""title"": ""Is it practical to abandon STL in C++ development?""
    },
    {
      ""title"": ""Scenarios Where LINQ is Handy""
    },
    {
      ""title"": ""Objective-C: blocks v NSNotificationCenter""
    },
    {
      ""title"": ""Is there ever a reason to do all an object&#39;s work in a constructor?""
    },
    {
      ""title"": ""Object oriented versus function oriented for backend design in PHP?""
    },
    {
      ""title"": ""What is a good starting point for small scale PHP development and would a framework be overkill?""
    },
    {
      ""title"": ""What is good (neat) architecture in programming?""
    },
    {
      ""title"": ""Object-Oriented Design: What to do when responsibility of the class is big""
    },
    {
      ""title"": ""Where is it possible to find Java code examples of generic data structures?""
    },
    {
      ""title"": ""Which version of Java should I use for learning?""
    },
    {
      ""title"": ""What software development model has worked best for software teams with heavy dependancy on hardware teams?""
    },
    {
      ""title"": ""Which Tech Best Meets the Requirements: Google Earth Compatibility with iOS Safari""
    },
    {
      ""title"": ""Which is most important to learn OOP first or to learn the OOP language you want to learn?""
    },
    {
      ""title"": ""Static functions vs classes""
    },
    {
      ""title"": ""How to integrate PAYPAL in ASP.NET webpages?""
    },
    {
      ""title"": ""What are inexpensive ways to have a dedicated dev server in college?""
    },
    {
      ""title"": ""In a dialog-based program in Visual C++, every time I hit the Enter key, my program exits How can I make Enter not exit the program?""
    },
    {
      ""title"": ""Where can I locate themes for VS2012""
    },
    {
      ""title"": ""Be a great programmer""
    },
    {
      ""title"": ""Choosing the right license for a testing framework""
    },
    {
      ""title"": ""Flash Builder 4.6 bugs""
    },
    {
      ""title"": ""Condition based function declarations""
    },
    {
      ""title"": ""How a software programming company is organized?""
    },
    {
      ""title"": ""Should you make private properties?""
    },
    {
      ""title"": ""OSS - GPL v3 plugins in non-copyleft codebase.""
    },
    {
      ""title"": ""What is a good design pattern / lib for iOS 5 to synchronize with a web service?""
    },
    {
      ""title"": ""Should session variables be avoided?""
    },
    {
      ""title"": ""Where is C++ today, and where is it headed?""
    },
    {
      ""title"": ""How to fix a project with basically no structure?""
    },
    {
      ""title"": ""How to enable user sharing in a multitenant saas application""
    },
    {
      ""title"": ""Developing for Windows 8 coming from Web/Android background""
    },
    {
      ""title"": ""As a programmer, are you required to do timesheets?""
    },
    {
      ""title"": ""Abstracting a zip as a filesystem - C++""
    },
    {
      ""title"": ""How to create a listView in android""
    },
    {
      ""title"": ""What tool sets and applications have affinity with multiprocessor programming?""
    },
    {
      ""title"": ""Name of a class that creates a bunch of classes for a process""
    },
    {
      ""title"": ""Validating best practices, property vs dto, simple type vs object""
    },
    {
      ""title"": ""Overloading Operators - C++""
    },
    {
      ""title"": ""Why Front-End Engineers are using the title engineer?""
    },
    {
      ""title"": ""Is there a downside to installing Visual Studio Ultimate if I don&#39;t need all the features?""
    },
    {
      ""title"": ""Best Practices when extending a project""
    },
    {
      ""title"": ""How to create a cool user interface for an excel file in java or C#?""
    },
    {
      ""title"": ""Do left-handed programmers use the mouse with the left hand?""
    },
    {
      ""title"": ""What&#39;s the best way to set up the distance and height of a triple monitor system""
    },
    {
      ""title"": ""How to set up for selective pushing with Mercurial?""
    },
    {
      ""title"": ""Can you create a VB6 class constructor which takes parameters""
    },
    {
      ""title"": ""3d point cloud reconstruction using in c++""
    },
    {
      ""title"": ""Are there any millionaires here?""
    },
    {
      ""title"": ""How do you accept arguments in the main.cpp file and reference another file?""
    },
    {
      ""title"": ""Does following TDD inevitably lead to DI?""
    },
    {
      ""title"": ""Multiple javaScript libraries | Emerging standards - compare to C++""
    },
    {
      ""title"": ""How to create a common interface for classes with different subsets of members""
    },
    {
      ""title"": ""Automation at GUI or API Level in Scrum""
    },
    {
      ""title"": ""A place for putting code samples in projects""
    },
    {
      ""title"": ""Starting a new project and need ideas for handling Phishing/Brute Force attacks AND Logging access""
    },
    {
      ""title"": ""Fractional and Cartesian Coordinates""
    },
    {
      ""title"": ""Is There Any Benefit To Participating On Sites Like TopCoder And Or GoogleCode?""
    },
    {
      ""title"": ""How do I create my own programming language and a compiler for it""
    },
    {
      ""title"": ""What does the suffix after software engineer/developer job titles mean?  (e.g. Software Developer III)""
    },
    {
      ""title"": ""Why is OOP difficult?""
    },
    {
      ""title"": ""Asking back technical questions during the interview (as the interviewee)""
    },
    {
      ""title"": ""Why rpm and deb package formats are not unified into one standard system?""
    },
    {
      ""title"": ""What if globals make sense?""
    },
    {
      ""title"": ""Are code reviews necessary for junior developers?""
    },
    {
      ""title"": ""What do you consider the 1st principle(s) of programming?""
    },
    {
      ""title"": ""Open Source but not Free Software (or vice versa)""
    },
    {
      ""title"": ""Why doesn&#39;t Microsoft release UX frameworks to build UI&#39;s like their current UIs?""
    },
    {
      ""title"": ""What are some concepts people should understand before programming &quot;big&quot; projects?""
    },
    {
      ""title"": ""Running time insensitive to input?""
    },
    {
      ""title"": ""How to encode spaces in E-Mail headers?""
    },
    {
      ""title"": ""Outlook of XBAP technology""
    },
    {
      ""title"": ""version control security""
    },
    {
      ""title"": ""Recurring Problem - need instruction to run only once inside code which executes multiple times""
    },
    {
      ""title"": ""Is it possible for a one-man start-up to follow agile methods like Scrum?""
    },
    {
      ""title"": ""How to properly design classes for a big project?""
    },
    {
      ""title"": ""How to split large, tightly coupled classes?""
    },
    {
      ""title"": ""What payment processors do the big boys use?""
    },
    {
      ""title"": ""Fixing &quot;fried brain&quot; syndrome""
    },
    {
      ""title"": ""Beginner/Noob in Programming Need help""
    },
    {
      ""title"": ""How do you deal with the costs of too-rapid change?""
    },
    {
      ""title"": ""Work division and process in mature iOS app Development?""
    },
    {
      ""title"": ""Sporadic unittests or TDD?""
    },
    {
      ""title"": ""How far should &#39;var&#39; and null coalescing operator &#39;??&#39; be entertained without hampering readability?""
    },
    {
      ""title"": ""Where did the notion of &quot;one return only&quot; come from?""
    },
    {
      ""title"": ""Nearest color algorithm using Hex Triplet""
    },
    {
      ""title"": ""Is it a good idea to install Mercurial on your server and hg pull to deploy?""
    },
    {
      ""title"": ""I want to create a new language""
    },
    {
      ""title"": ""At run time finding the current row of a table?""
    },
    {
      ""title"": ""Best Javascript animation Library""
    },
    {
      ""title"": ""Recommend Video Series for Android Development""
    },
    {
      ""title"": ""Hiding away complexity with sub functions""
    },
    {
      ""title"": ""Losing my team player skills""
    },
    {
      ""title"": ""What are the qualifications for working with development of operating systems?""
    },
    {
      ""title"": ""What language should an 11-year old start with to learn game programming?""
    },
    {
      ""title"": ""Order of subject and modifiers in variable names""
    },
    {
      ""title"": ""Application that provides unique keys to multiple threads""
    },
    {
      ""title"": ""C++ is easy once you know PHP""
    },
    {
      ""title"": ""Using UML and Use Cases in Game Design""
    },
    {
      ""title"": ""Why is the big pricing difference of game software on iPad and iPhone vs on consoles or PCs?""
    },
    {
      ""title"": ""DI: Can a stable dependency have a volatile dependency?""
    },
    {
      ""title"": ""How do you avoid working on the wrong branch?""
    },
    {
      ""title"": ""What other Windows Technologies are avaialable""
    },
    {
      ""title"": ""Multiple Same Object Instantiation""
    },
    {
      ""title"": ""Public-private key pair handling on a Windows ecosystem""
    },
    {
      ""title"": ""Should I take help of Internet and other programmers or I should do all programming myself?""
    },
    {
      ""title"": ""Does custom created code for a client imply copyright ownership?""
    },
    {
      ""title"": ""Standards of unit testing output""
    },
    {
      ""title"": ""Advice for young developer""
    },
    {
      ""title"": ""Language Certifications""
    },
    {
      ""title"": ""What kind of metrics, if any, can be collected from requirements development?""
    },
    {
      ""title"": ""Using custom jar libraries in Command Prompt and IntelliJ IDEA""
    },
    {
      ""title"": ""Function declaration as var instead of function""
    },
    {
      ""title"": ""I want to learn web development, where do I start?""
    },
    {
      ""title"": ""Why does Clrver apparently not show all CLR processes""
    },
    {
      ""title"": ""What&#39;s the difference between connecting to a database with &quot;Remote Desktop Connection&quot; vs SSMS?""
    },
    {
      ""title"": ""What&#39;s the BEST way to really understand OOP?""
    },
    {
      ""title"": ""When to use C over C++, and C++ over C?""
    },
    {
      ""title"": ""TTS on App Engine""
    },
    {
      ""title"": ""What version of MSDN to get""
    },
    {
      ""title"": ""Can display issues affect SEO?""
    },
    {
      ""title"": ""How to find a programming mentor?""
    },
    {
      ""title"": ""Can anyone point me to some examples / reading material that outline updating the dom automatically?""
    },
    {
      ""title"": ""What do you call a pair of function that are complements of each other?""
    },
    {
      ""title"": ""Checkstyle &amp; Findbugs Install""
    },
    {
      ""title"": ""Should I be involved in my project&#39;s business side?""
    },
    {
      ""title"": ""Best practices for coding date sensitive websites""
    },
    {
      ""title"": ""How to convince boss to start using Codeigniter or YII at work?""
    },
    {
      ""title"": ""Is there a better approach to speech synthesis than text-to-speech for more natural output?""
    },
    {
      ""title"": ""Web workflow solution - how should I approach the design?""
    },
    {
      ""title"": ""What are the definitive books or references on MVC, MVVM, and other architectural patterns related to UI?""
    },
    {
      ""title"": ""Can a function be too short?""
    },
    {
      ""title"": ""What do you call an interface with no defining methods used as property setters""
    },
    {
      ""title"": ""Entity System - interaction between systems""
    },
    {
      ""title"": ""Design pattern in non object oriented language""
    },
    {
      ""title"": ""What kind of software is done best with The Go Programming language?""
    },
    {
      ""title"": ""Is it possible to call an ASPX from UNIX shell script""
    },
    {
      ""title"": ""What degree of low-level programming can be achieved with Languages like Go?""
    },
    {
      ""title"": ""QoS implementation algorithm""
    },
    {
      ""title"": ""WCF service and security""
    },
    {
      ""title"": ""Limitations of Polymorphism in statically typed languages""
    },
    {
      ""title"": ""Current iOS version/device statistics?""
    },
    {
      ""title"": ""How can I make refactoring a priority for my team?""
    },
    {
      ""title"": ""Dynamic pages and static pages""
    },
    {
      ""title"": ""Are factors such as Intellisense support and strong typing enough to justify the use of an &#39;Anaemic Domain Model&#39;?""
    },
    {
      ""title"": ""Advice sought: highly technical app idea - do I employ someone or do it myself?""
    },
    {
      ""title"": ""Django application strategy""
    },
    {
      ""title"": ""Why this code create object as interface?""
    },
    {
      ""title"": ""Advantages of learning Javascript""
    },
    {
      ""title"": ""Is it OK for a function to modify a parameter""
    },
    {
      ""title"": ""Project Management - Asana / activeCollab / basecamp / alternative / none""
    },
    {
      ""title"": ""Wrapping REST based Web Service""
    },
    {
      ""title"": ""Computer Science and statistic minor? good idea?""
    },
    {
      ""title"": ""It it rational to view an operating system as a runtime?""
    },
    {
      ""title"": ""How are objects modelled in a functional programming language?""
    },
    {
      ""title"": ""How to introduce code to a colleague""
    },
    {
      ""title"": ""Which aspect of normal forms do entity-attribute-value tables violate, if any?""
    },
    {
      ""title"": ""Why is Javascript used in MongoDB and CouchDB instead of other languages such as Java, C++?""
    },
    {
      ""title"": ""Converting data-structure in Perl""
    },
    {
      ""title"": ""What are some good resources to learn C# And/or C++ And which would be better to master first?""
    },
    {
      ""title"": ""The different types of CMS - Pros and cons""
    },
    {
      ""title"": ""Is it ok to replace optimized code with readable code?""
    },
    {
      ""title"": ""What are programming communities you attend to?""
    },
    {
      ""title"": ""C# GetFiles with Date Filter""
    },
    {
      ""title"": ""Is it a better practice pre-initialize attributes in a class, or to add them along the way?""
    },
    {
      ""title"": ""Side-by-side Configuration on Linux/ELF""
    },
    {
      ""title"": ""SQL: empty string vs NULL value""
    },
    {
      ""title"": ""Is it smart to design a command and control server, that will monitor system resources and spin up/spin down servers at times of peak? ""
    },
    {
      ""title"": ""Should I put newlines before or after binary operators?""
    },
    {
      ""title"": ""Does Fred Brooks&#39; &quot;Surgical Team&quot; effectively handle the bus factor?""
    },
    {
      ""title"": ""What do you call classes without methods?""
    },
    {
      ""title"": ""Why should I push if I&#39;m working alone in a local repository?""
    },
    {
      ""title"": ""1 to 1 Comparison and Ranking System""
    },
    {
      ""title"": ""Why should NoSQL databases and name/value stores be avoided""
    },
    {
      ""title"": ""Where to begin software Analysis? Wanna be BI software""
    },
    {
      ""title"": ""G95 - Missing lapack, blas libraries""
    },
    {
      ""title"": ""Book or resources to gain a better DB understanding""
    },
    {
      ""title"": ""study materials for Mysql certification?""
    },
    {
      ""title"": ""how to insert dash and underscore after matching pattern in perl""
    },
    {
      ""title"": ""Full Trust level: should be a concern?""
    },
    {
      ""title"": ""thick client migration to web based application""
    },
    {
      ""title"": ""Can functional programming be used to develop a full enterprise application?""
    },
    {
      ""title"": ""How to get started with this project?""
    },
    {
      ""title"": ""Estimating the right number of clusters in remote sensing images""
    },
    {
      ""title"": ""How to get the length of a TextField in Django?""
    },
    {
      ""title"": ""Folder classification app""
    },
    {
      ""title"": ""Is slower performance of programming languages a bad thing?""
    },
    {
      ""title"": ""Why is the use of abstractions (such as LINQ) so taboo?""
    },
    {
      ""title"": ""Good tutorial or book about creating a DSL and interpret it and explaining the computer language parsing and interpreting theory""
    },
    {
      ""title"": ""Why do programmers use or recommend Mac OS X?""
    },
    {
      ""title"": ""What statements and approaches should I avoid when learning functional programming? ""
    },
    {
      ""title"": ""How would you test Google Maps &quot;Get Directions&quot; feature?""
    },
    {
      ""title"": ""What do employers look for in self-taught applicants?""
    },
    {
      ""title"": ""Eclipse or Netbeans, which has better support for android development?""
    },
    {
      ""title"": ""Do you start migrating your Swing project to JavaFX""
    },
    {
      ""title"": ""Optimistic work sharing on sparsely distributed systems""
    },
    {
      ""title"": ""Finding the balance between the important and the interesting""
    },
    {
      ""title"": ""Is it good/safe OOP practice to have a method whose only purpose is to send/retrieve data from another class?""
    },
    {
      ""title"": ""Teaching kids to program - how to teach syntax?""
    },
    {
      ""title"": ""Debugging: understanding details on why certain fixes worked?""
    },
    {
      ""title"": ""ios intern training""
    },
    {
      ""title"": ""Flexible cloud file storage for a web.py app?""
    },
    {
      ""title"": "" Full Text Search""
    },
    {
      ""title"": ""Why would more CPU cores on virtual machine slow compile times?""
    },
    {
      ""title"": ""How to force evaluation in Haskell?""
    },
    {
      ""title"": ""Hibernate Full Text Search""
    },
    {
      ""title"": ""What tricks/tips do you use for innovation?""
    },
    {
      ""title"": ""What useful expressiveness will be impossible in a language where an expression is not a statement?""
    },
    {
      ""title"": ""Why put a simple query into a stored procedure in a web service?""
    },
    {
      ""title"": ""andegine or unity or monogame""
    },
    {
      ""title"": ""What are the career options available to an under graduate""
    },
    {
      ""title"": ""Which CUNY Computer Science curriculum is better?""
    },
    {
      ""title"": ""Is CS the right major for me?""
    },
    {
      ""title"": ""Comparing languages dependant on runtimes with compiled languages""
    },
    {
      ""title"": ""Why would it take hours to decode transmissions from Curiosity rover via MRO?""
    },
    {
      ""title"": ""What technique can I use to choose between several projects ideas?""
    },
    {
      ""title"": ""Learning Django by example""
    },
    {
      ""title"": ""How to get rid of the warning &quot;WARNING:PhysDesignRules:367 - The signal IBUF is incomplete.&quot;""
    },
    {
      ""title"": ""What exactly is &quot;Web API&quot; in ASP.Net MVC4?""
    },
    {
      ""title"": ""Arrow =&gt; in Perl""
    },
    {
      ""title"": ""Dynamic fields from database on jsp""
    },
    {
      ""title"": ""How do these hotshot developers keep changing their technology base?""
    },
    {
      ""title"": ""How to unit test with lots of IO""
    },
    {
      ""title"": ""How can uTorrent be multi-platform while keeping such a small binary size?""
    },
    {
      ""title"": ""Does continuous integration involve automatic merging between branches?""
    },
    {
      ""title"": ""Android, OpenGL and extending GLSurfaceView?""
    },
    {
      ""title"": ""What is the Mars Curiosity Rover&#39;s software built in?""
    },
    {
      ""title"": ""Can we guarantee a program will never go wrong?""
    },
    {
      ""title"": ""What do I need to know about writing a liability warranty disclaimer when selling source code?""
    },
    {
      ""title"": ""Real programmers use debuggers?""
    },
    {
      ""title"": ""Omit terminating semicolon in a tag - a good idea?""
    },
    {
      ""title"": ""how do I know I&#39;ve split my program into too small pieces?""
    },
    {
      ""title"": ""What job titles are at an ISP""
    },
    {
      ""title"": ""What does an IT consultant exactly do?""
    },
    {
      ""title"": ""Difference between Software Engineering and IT?""
    },
    {
      ""title"": ""Why does PHP have interfaces?""
    },
    {
      ""title"": ""Any experience with Mono on production servers?""
    },
    {
      ""title"": ""Zernike Moments for Hand Gesture Recognition""
    },
    {
      ""title"": ""If condition not true: default value or else clause?""
    },
    {
      ""title"": ""Scoring/analysis of Subjective testing for skills assessment""
    },
    {
      ""title"": ""Why do variables need a type?""
    },
    {
      ""title"": ""TDD: Mocking out tightly coupled objects""
    },
    {
      ""title"": ""How to quickly adapt to a project?""
    },
    {
      ""title"": ""When you won&#39;t need a language anymore, should you still use it?""
    },
    {
      ""title"": ""Programming language trends""
    },
    {
      ""title"": ""How to quickly find the cause of errors when it appears out of nowhere?""
    },
    {
      ""title"": ""How could you reconcile declarative database development and non-trivial data &#39;motions&#39;?""
    },
    {
      ""title"": ""How to achieve a loosely coupled REST API but with a defined and well understood contract?""
    },
    {
      ""title"": ""Need advice on which route I should take for porting app from Android to iOS""
    },
    {
      ""title"": ""How can software be protected from piracy?  ""
    },
    {
      ""title"": ""How can I find more information about a startup?""
    },
    {
      ""title"": ""Code obfuscator for C++""
    },
    {
      ""title"": ""What&#39;s a good light-weight source repository for local development?""
    },
    {
      ""title"": ""How to get started in opensource projects and programs""
    },
    {
      ""title"": ""What programming languages should every computer science student be taught?""
    },
    {
      ""title"": ""Should Android development be done on Windows or OSX? Is there any difference?""
    },
    {
      ""title"": ""What is the best way to use inheritance with ORMs?""
    },
    {
      ""title"": ""Why is Java the lingua franca at so many institutions?""
    },
    {
      ""title"": ""TFS non-chronological deployment?""
    },
    {
      ""title"": ""Windows XP Installation Stop Error Screen""
    },
    {
      ""title"": ""Determining ethics of substance use to enhance programming""
    },
    {
      ""title"": ""Tools for modelling data and workflows using structured text files""
    },
    {
      ""title"": ""Where can I find some samples of C# programming interview questions?""
    },
    {
      ""title"": ""How to create interest in programming?""
    },
    {
      ""title"": ""Ruby on Rails resources""
    },
    {
      ""title"": ""hash with file instead of array""
    },
    {
      ""title"": ""How Learn new technologies""
    },
    {
      ""title"": ""What are the major difference between .Net framework?""
    },
    {
      ""title"": ""Dynamic programming in codeforces round 86""
    },
    {
      ""title"": ""Automatic programming: write code that writes code""
    },
    {
      ""title"": ""Best remedial math strategy for CS students?""
    },
    {
      ""title"": ""I-Framing Third Party Pages In Chrome Extensions""
    },
    {
      ""title"": ""How do you plan for the future? Short-term or long-term?""
    },
    {
      ""title"": ""What is a legitimate reason to use Cucumber?""
    },
    {
      ""title"": ""What are some feasible ideas for building a smart-device/console development portfolio?""
    },
    {
      ""title"": ""can a regex search engine for the web be done and if so, how?""
    },
    {
      ""title"": ""How can I organize personal git repositories?""
    },
    {
      ""title"": ""Is micro-optimisation important when coding?""
    },
    {
      ""title"": ""Advice for a project suitable for a year of full-time hours (not a job searching post)""
    },
    {
      ""title"": ""Good resources to understand how a program interacts with machine hardware""
    },
    {
      ""title"": ""Should I represent special cases (like errors / exceptions) in UML diagrams?""
    },
    {
      ""title"": ""Justification for bidirectional relationship""
    },
    {
      ""title"": ""Sencha Ext JS run time license""
    },
    {
      ""title"": ""What software models are appropriate for daily builds and continuous integration?""
    },
    {
      ""title"": ""What&#39;s the benefit of a singleton over a class in objective-c?""
    },
    {
      ""title"": ""Looking for a very memory-efficient way of finding exporting all relations in a family tree""
    },
    {
      ""title"": ""Is an Inner Function Justified in this Situation""
    },
    {
      ""title"": ""Are there any theories or books about how to debug &quot;in general&quot;?""
    },
    {
      ""title"": ""What do you need from a client to create a site""
    },
    {
      ""title"": ""What actions to take when people leave the team?""
    },
    {
      ""title"": ""Is this program possible?""
    },
    {
      ""title"": ""If your algorithm is correct, does it matter how long it took you to write it?""
    },
    {
      ""title"": ""Does writing a programming blog reduce your learning speed, compared to reading other people&#39;s blogs only?""
    },
    {
      ""title"": ""Should I use events in this case?""
    },
    {
      ""title"": ""Alternatives to code migration for distributed systems""
    },
    {
      ""title"": ""Recursion algorithm Stops running after finding the first Leaf node""
    },
    {
      ""title"": ""How can I animate the display property of css using javascript?""
    },
    {
      ""title"": ""Resources relating to Java EE and Scala""
    },
    {
      ""title"": ""What should I do to practice?""
    },
    {
      ""title"": ""std::shared_ptr as a last resort?""
    },
    {
      ""title"": ""Interview books on data structures, tree operations, algorithms and algorithms complexity, etc""
    },
    {
      ""title"": ""Unit Tests code duplication?""
    },
    {
      ""title"": ""Should a github maintainer rewrite author&#39;s in pull requests?""
    },
    {
      ""title"": ""Mixing languages in .Net""
    },
    {
      ""title"": ""Do all big developer teams make short-time project developments?""
    },
    {
      ""title"": ""Being a team manager and a developer in a Scrum team""
    },
    {
      ""title"": ""What is block level design in context of mobile application?""
    },
    {
      ""title"": ""What is the best book about creating video editing interfaces?""
    },
    {
      ""title"": ""recommend C# source code for reading""
    },
    {
      ""title"": ""Using a openid in a &quot;closed system&quot;""
    },
    {
      ""title"": ""Preferring Python over C for Algorithmic Programming""
    },
    {
      ""title"": ""Which geographic framework should I use for cross-platform mobile development?""
    },
    {
      ""title"": ""Software developer career track""
    },
    {
      ""title"": ""Best practices / Design patterns for code generation""
    },
    {
      ""title"": ""What Discrete Mathematics topics should the average computer science student know?""
    },
    {
      ""title"": ""Consumer/Producer approach for a Video capturing software""
    },
    {
      ""title"": ""URL parameters in RESTful web services""
    },
    {
      ""title"": ""How can we handle multiple instances of a method through a single class instance""
    },
    {
      ""title"": ""Changing frontend cache""
    },
    {
      ""title"": ""User Stories with design elements in Scrum""
    },
    {
      ""title"": ""How to handle a egocentric Manager?""
    },
    {
      ""title"": ""Is there a different usage rationale for abstract classes/interfaces in C++ and Java""
    },
    {
      ""title"": ""Do you consider mainframe as part of large application deployments?""
    },
    {
      ""title"": ""Making a calculator""
    },
    {
      ""title"": ""Does NetBeans Keep a cache?""
    },
    {
      ""title"": ""In which fields does quality of the software product matter as much as the completion time?""
    },
    {
      ""title"": ""How can I update the session id in a JSF application?""
    },
    {
      ""title"": ""What are some practical uses of the &quot;new&quot; modifier in C# with respect to hiding? ""
    },
    {
      ""title"": ""Implementing cache system in Java Web Application""
    },
    {
      ""title"": ""Best method to implement a filtered search""
    },
    {
      ""title"": ""User stories as a contract definition?""
    },
    {
      ""title"": ""What is required for a scope in an injection framework?""
    },
    {
      ""title"": ""Is there an alternative to Google Code Search?""
    },
    {
      ""title"": ""How do I Integrate Production Database Hot Fixes into Shared Database Development model?""
    },
    {
      ""title"": ""What should every programmer know about audio programming?""
    },
    {
      ""title"": ""How to implement a simulation pattern for a repository?""
    },
    {
      ""title"": ""INSTALL ALL AT ONCE""
    },
    {
      ""title"": ""How to inspire an intern with programming?""
    },
    {
      ""title"": ""Best way to remove list items from an existing record""
    },
    {
      ""title"": ""Is it a good practice to capture build artifacts in Artifactory that Jenkins produces?""
    },
    {
      ""title"": ""JavaScript vs third party libraries""
    },
    {
      ""title"": ""Best way to approach a partner business to understand the need for B2B web-services""
    },
    {
      ""title"": ""Is individual code ownership important?""
    },
    {
      ""title"": ""critique my implementation of hash map""
    },
    {
      ""title"": ""Are there any benefits in switching to another technology other than my core skills?""
    },
    {
      ""title"": ""How to make images alternate on a home page of a website?""
    },
    {
      ""title"": ""How do I make the most of the chance to meet one on one with a programming guru?""
    },
    {
      ""title"": ""How to become a web developer from scratch without a degree?""
    },
    {
      ""title"": ""Android development using C and C++""
    },
    {
      ""title"": ""What is the term for this type of refactoring""
    },
    {
      ""title"": ""File comparison tool that will allow me to manually remap lines between two files when comparing?""
    },
    {
      ""title"": ""finding min distance between the elements of array?""
    },
    {
      ""title"": ""WPF, MVVM, EF, POCO guidance required on simple architecture""
    },
    {
      ""title"": ""What does an MSDN subscription offer?""
    },
    {
      ""title"": ""Explanation of satellite data from a programmers perspective""
    },
    {
      ""title"": ""Discussions of simplicity""
    },
    {
      ""title"": ""Why is SQL&#39;s BETWEEN inclusive?""
    },
    {
      ""title"": ""Pricing of a collaborative work""
    },
    {
      ""title"": ""Given my programming history, should I learn Ruby or C#?""
    },
    {
      ""title"": ""Writing a new programming language - when and how to bootstrap datastructures?""
    },
    {
      ""title"": ""What client/server communiation mechanism shall I use in this scenario""
    },
    {
      ""title"": ""Methods for testing a very large application""
    },
    {
      ""title"": ""What is the correct way to implement Auth/ACL in MVC?""
    },
    {
      ""title"": ""Abstracting away the type of a property""
    },
    {
      ""title"": ""Is there much thought towards internal diagnosis testing within an enterprise application?""
    },
    {
      ""title"": ""Any requirements tool recommendation?""
    },
    {
      ""title"": ""Alt key shortcuts anymore?""
    },
    {
      ""title"": ""Class design for internationalized object""
    },
    {
      ""title"": ""What are the practical ways to implement the SRP?""
    },
    {
      ""title"": ""How to jmock Final class""
    },
    {
      ""title"": ""How to convert the datetime (timestamp according to sql server) to set,get in my struts2 application pojo class?""
    },
    {
      ""title"": ""How can we use ASP.Net RegularExpressionValidator for multilanguage support?""
    },
    {
      ""title"": ""Pass ID or Object?""
    },
    {
      ""title"": ""How can I find the next available slot with a hibernate query?""
    },
    {
      ""title"": ""How to search for information related to Go programming language?""
    },
    {
      ""title"": ""Intellij autobuild - non existent""
    },
    {
      ""title"": ""Feature vs. Function""
    },
    {
      ""title"": ""Logic of Java barcode reader for libraryMS""
    },
    {
      ""title"": ""Javascript - Start function after a specific time""
    },
    {
      ""title"": ""Web application without libraries | Relation to job postings""
    },
    {
      ""title"": ""What are the similarities between Perl and Java?""
    },
    {
      ""title"": ""How have languages influenced CPU design?""
    },
    {
      ""title"": ""How big does my project need to be for me to unit test it?""
    },
    {
      ""title"": ""How to optimize methodology to collect Market Data?""
    },
    {
      ""title"": ""Analogy for Thread Pools""
    }
  ]
}	";
            }
        }

        private string StackOverflowTrainingJson
        {
            get
            {
                return @"{
  ""items"": [
    {
      ""title"": ""Commenting out sections of code for testing""
    },
    {
      ""title"": ""getLinkerUrl not passing cookies across domains - Google Analytics""
    },
    {
      ""title"": ""PHPBB2 + Themes - Changing the Error Messages / Delete Messages Appearance""
    },
    {
      ""title"": ""How to close fancybox at facebook canvas application?""
    },
    {
      ""title"": ""ListView Footer positioning changes""
    },
    {
      ""title"": ""Replacing InnerHTML""
    },
    {
      ""title"": ""mongotop show reading local.oplog.rs taking more than 2 seconds""
    },
    {
      ""title"": ""how to get number of items in listbox c#/.net""
    },
    {
      ""title"": ""Is there any refactoring tool for Python 3?""
    },
    {
      ""title"": ""Is there a better way to get custom repeats and ordering for mysql_query() in PHP?""
    },
    {
      ""title"": ""Check the state validity of a Spring proxied bean without try-catch""
    },
    {
      ""title"": ""How to remove subversion remote in git?""
    },
    {
      ""title"": ""Java Logger Console Stream Duplicate Output""
    },
    {
      ""title"": ""Solutions to convert &amp; export charts/tables generated from web app using HTML/Javascript to editable Powerpoint (.pptx, .ppt)?""
    },
    {
      ""title"": ""Use of atomic properties in Objective C: Any side effects?""
    },
    {
      ""title"": ""Clear User data or Clear cache on Phonegap android""
    },
    {
      ""title"": ""Error occurrs while processing this directive PHP""
    },
    {
      ""title"": ""firefox not showing whole download name and extension""
    },
    {
      ""title"": ""How do I select a node in jsTree and open all parents""
    },
    {
      ""title"": ""Wordpress url shows page not found on localhost, but online its fine""
    },
    {
      ""title"": ""lists not showing bullets in html5 css""
    },
    {
      ""title"": ""Creating a C++ Template Like Functions With Local Variables in C""
    },
    {
      ""title"": ""Wordpress - Frontend User Profile Page with Associated Post Types""
    },
    {
      ""title"": ""Google Maps - Array""
    },
    {
      ""title"": ""any good eclipse plugin for javascript?""
    },
    {
      ""title"": ""Handling photo resizing PHP""
    },
    {
      ""title"": ""elasticsearch query and cURL in PHP""
    },
    {
      ""title"": ""Google OpenID - programmatically select session if user is logged into several google accounts""
    },
    {
      ""title"": ""Serializing a Java Process and then De-Serializing it -- Preferrably in a non-invasive fashion""
    },
    {
      ""title"": ""Inline &lt;style&gt; tags vs. inline css properties""
    },
    {
      ""title"": ""&#39;fatal error&#39; during bundle install in ruby on rails""
    },
    {
      ""title"": ""Youtube HTML5 iframe embed has outline in IE on play""
    },
    {
      ""title"": ""How can I make my capturing link matcher &quot;lazy&quot;?""
    },
    {
      ""title"": ""How to change reference to ItemsSource Ilist collection modify to ItemsSource collection generated from XMLDataProvider?""
    },
    {
      ""title"": ""Why is this element moving?""
    },
    {
      ""title"": ""Read only version of class for const parameter""
    },
    {
      ""title"": ""Permissions issue with WMI custom performance markers inside IIS7 application""
    },
    {
      ""title"": ""Choosing div placement using append in jQuery""
    },
    {
      ""title"": ""convert sql-server *.mdf file into sqlite file""
    },
    {
      ""title"": ""Android - How to edit the size of an image for using as an icon via XML""
    },
    {
      ""title"": ""ASP.Net HTML Document viewer""
    },
    {
      ""title"": ""Powerpoint does not close when using custom addin""
    },
    {
      ""title"": ""Qt 4.7 Emitting a signal to a specific thread""
    },
    {
      ""title"": ""Two rails models both with a habtm relationship to a third model""
    },
    {
      ""title"": ""How to create virtual field with &#39;complex&#39; math function""
    },
    {
      ""title"": ""How to change the text color of comments line in a batch file""
    },
    {
      ""title"": ""Protected properties prefixed with underscores""
    },
    {
      ""title"": ""Difference between .NET Sockets from the AsyncState and the EndAccept() function""
    },
    {
      ""title"": ""100 threads TIMED_WAITING in tomcat, causing it to stall as total number of threads crosses 200""
    },
    {
      ""title"": ""Blotter R Slowness""
    },
    {
      ""title"": ""Replacing one substring multiple times without looping? (using an array of replacements)""
    },
    {
      ""title"": ""Getting Stylus to work with Express and Node""
    },
    {
      ""title"": ""PHPUnit code coverage for inherited from abstract classes""
    },
    {
      ""title"": ""Git rename directory with submodules""
    },
    {
      ""title"": ""does someone know a good educacional sample about use mef with controllers?""
    },
    {
      ""title"": ""Is there any way in Android to force open a link to open in Chrome?""
    },
    {
      ""title"": ""django: cleanest way to save every page served""
    },
    {
      ""title"": ""Logging java varibales online""
    },
    {
      ""title"": ""Android: wrong orientation of files opened with BitmapFactory.decodeFile""
    },
    {
      ""title"": ""Page loading only half of content""
    },
    {
      ""title"": ""Using Named Keys in Array C# MVC razor""
    },
    {
      ""title"": ""What .NET features will allow me to &quot;reload&quot; and &quot;restart&quot; an embedded server?""
    },
    {
      ""title"": ""Java NoSQL, file-based database to store JSON?""
    },
    {
      ""title"": ""Not able to include AndroidHttpTransport in my project""
    },
    {
      ""title"": ""randomly replace a substring in a block of text in javascript or jquery""
    },
    {
      ""title"": ""undefined method &#39;path&#39; for nil:NilClass using chargify_api_ares gem""
    },
    {
      ""title"": ""jQuery interference between Two scripts""
    },
    {
      ""title"": ""ScriptIgnore, JsonSerializer not paying any attention?""
    },
    {
      ""title"": ""ColdFusion JDBC Connection String Issue""
    },
    {
      ""title"": ""Ajax Fragment Meta tag - Googlebot isn&#39;t reading the page&#39;s content""
    },
    {
      ""title"": ""Encapsulation vs Data Hiding - Java""
    },
    {
      ""title"": ""How to callback a function on 404 in JSON ajax request with jQuery? ""
    },
    {
      ""title"": ""Activate Wifi on Android from adb (not rooted phone)""
    },
    {
      ""title"": ""Which line is chosen to be reported in exception""
    },
    {
      ""title"": ""Count() returns incorrect result on IEnumerable&lt;T&gt; filled using LINQ""
    },
    {
      ""title"": ""What should i worry about python template engines and web frameworks?""
    },
    {
      ""title"": ""How can I create an expense table using HTML tables and jQuery?""
    },
    {
      ""title"": ""JBoss AS 6 Resteasy Setup""
    },
    {
      ""title"": ""Using a UIGestureRecognizer, a ScrollView and forwarding touches? (cancelsTouchesInView = NO not working)""
    },
    {
      ""title"": ""Batch Program to copy into another folder and make a text file from the copied files""
    },
    {
      ""title"": ""Parse an HTML string to get contents of elements with Javascript""
    },
    {
      ""title"": ""Add extra js in backend block (tab edit form) in Magento (solved)""
    },
    {
      ""title"": ""permissions for custum functions in Spreadsheets (Google Document)""
    },
    {
      ""title"": ""how can I find and isolate a qr code in a complex image?""
    },
    {
      ""title"": ""Does all the C compiler strictly follows operator precedence order""
    },
    {
      ""title"": ""Perl: Merge multiple text files and append current file name at the end of each line""
    },
    {
      ""title"": ""Parse AVI video with Perl""
    },
    {
      ""title"": ""Restrictions on Maven artifactID values""
    },
    {
      ""title"": ""CSV to XML using xslt - how to have incrementing column name""
    },
    {
      ""title"": ""Migrating from Googlecode to Github with full revision history""
    },
    {
      ""title"": ""How do I send an XML document from jQuery (JS) to C# in an MVC project?""
    },
    {
      ""title"": ""Interpolating RGB values for holes in pictures""
    },
    {
      ""title"": ""What are the security risks of allowing all characters in the website URL?""
    },
    {
      ""title"": ""extjs calendar 4.1.1 restricted time slot for booking""
    },
    {
      ""title"": ""How do I sign an Android application as a system app to use DEVICE_POWER permission""
    },
    {
      ""title"": ""Presenting ModalViewController from SplitView sidebar - iPad""
    },
    {
      ""title"": ""Display JS scripts block (for example Ad words script) within Switch block / C#""
    },
    {
      ""title"": "".NET Workflow 4 Create a Custom Persistence Store that is used by a WCF-enabled Workflow""
    },
    {
      ""title"": ""Go from html form to PDF using PHP""
    },
    {
      ""title"": ""Which assemblies should I emit to trace asp.net web-api exceptions?""
    },
    {
      ""title"": ""CSV to XML using xslt - how to have incrementing column name""
    },
    {
      ""title"": ""Migrating from Googlecode to Github with full revision history""
    },
    {
      ""title"": ""How do I send an XML document from jQuery (JS) to C# in an MVC project?""
    },
    {
      ""title"": ""Interpolating RGB values for holes in pictures""
    },
    {
      ""title"": ""extjs calendar 4.1.1 restricted time slot for booking""
    },
    {
      ""title"": ""How do I sign an Android application as a system app to use DEVICE_POWER permission""
    },
    {
      ""title"": ""Presenting ModalViewController from SplitView sidebar - iPad""
    },
    {
      ""title"": ""Display JS scripts block (for example Ad words script) within Switch block / C#""
    },
    {
      ""title"": "".NET Workflow 4 Create a Custom Persistence Store that is used by a WCF-enabled Workflow""
    },
    {
      ""title"": ""Go from html form to PDF using PHP""
    },
    {
      ""title"": ""Which assemblies should I emit to trace asp.net web-api exceptions?""
    },
    {
      ""title"": ""how to cast multiple integers as a ctypes array of c_ubyte in Python""
    },
    {
      ""title"": ""Open a View Controller from a UITableViewCell Without a Segue""
    },
    {
      ""title"": ""Targeting device groups using CSS media queries""
    },
    {
      ""title"": ""Annotation-based wiring in Spring for only part of a collection""
    },
    {
      ""title"": ""Sending commands to a console application?""
    },
    {
      ""title"": ""Batch file to list all folders in a directory and output to txt with""
    },
    {
      ""title"": ""Azure and Ninject (No parameterless constructor defined for this object.)""
    },
    {
      ""title"": ""How to write a generic function to compute recursive combination""
    },
    {
      ""title"": ""specifying :hover of a link with a specific ID""
    },
    {
      ""title"": ""Does &quot;more threads&quot; mean more speed?""
    },
    {
      ""title"": ""MVC 3 Razor Syntax for straight text output?""
    },
    {
      ""title"": ""Converting a pcm file to mono""
    },
    {
      ""title"": ""Source not found The JAR file mysql-connector-java-5.1.20-bin.jar has no source attachement""
    },
    {
      ""title"": ""Only IE will correctly run my PHP/ MySQL query!""
    },
    {
      ""title"": ""How to parse an arbitrary option string into a python dictionary""
    },
    {
      ""title"": ""Unable to deploy twitter-bootstrap-rails gem on Heroku because therubyracer fails compilation""
    },
    {
      ""title"": ""JList too wide?""
    },
    {
      ""title"": ""When submitting an app on app store for cocos2d""
    },
    {
      ""title"": ""Servlet forward response to caller/previous page""
    },
    {
      ""title"": ""Java storing sensitive &#39;key&#39; as String or char[]?""
    },
    {
      ""title"": ""What are some user friendly image processing libraries?""
    },
    {
      ""title"": ""Normalize array subscripts for 1-dimensional array so they start with 1""
    },
    {
      ""title"": ""scrollLeft easing animation using mouse position""
    },
    {
      ""title"": ""Android NDK Video Capturing""
    },
    {
      ""title"": ""gitignore file syntax""
    },
    {
      ""title"": ""saving data into arraylist in a loop""
    },
    {
      ""title"": ""Why does my song restart when I change from vertical to horizontal view?""
    },
    {
      ""title"": ""Stateless ejb does not remove the pool""
    },
    {
      ""title"": ""Layered drawing in java""
    },
    {
      ""title"": ""Searching web and displaying the results in a nattive app""
    },
    {
      ""title"": ""Performance bottleneck Url.Action - can I workaround it?""
    },
    {
      ""title"": ""Facebook Login, automatically post on users wall?""
    },
    {
      ""title"": ""what does $ mean here in JQuery""
    },
    {
      ""title"": ""MySQL find invalid foreign keys""
    },
    {
      ""title"": ""Web server processing power required (per web page load) --&gt; What is considered economic?""
    },
    {
      ""title"": ""How can I cast an object to a table?""
    },
    {
      ""title"": ""JAXB and property ordering -""
    },
    {
      ""title"": ""Where can I find up-to-date browser compatibility guides?""
    },
    {
      ""title"": ""MVC Skinny Controller""
    },
    {
      ""title"": ""OpenGL Dynamic Object Motion Blur""
    },
    {
      ""title"": ""Exporting Sqlite table data to csv file programatically""
    },
    {
      ""title"": ""Custom overlay map types and mapTypeControl""
    },
    {
      ""title"": ""removing characters of a specific unicode range from a string""
    },
    {
      ""title"": ""What free software, and portable GUI libraries are non object-oriented?""
    },
    {
      ""title"": ""Xcode: Scheme scripts vs Target scripts""
    },
    {
      ""title"": ""Changing font (or amount of text) on UITextView makes text disappear""
    },
    {
      ""title"": ""SQL Pivot table returning NULL for non-existent child table values""
    },
    {
      ""title"": ""How can I send a image over the network in XNA?""
    },
    {
      ""title"": ""Best practice: How to authenticate/authorize AJAX Calls on the server""
    },
    {
      ""title"": ""Can the IMX.53 LOCO be configured to boot off of SATA?""
    },
    {
      ""title"": ""Good practice for app that times when it crashes""
    },
    {
      ""title"": ""NLP text annotation storage and access""
    },
    {
      ""title"": ""interface vs abstract in C#""
    },
    {
      ""title"": ""NUnit, TestCaseSource/ TestData and incepting before and after - with the actual values?""
    },
    {
      ""title"": ""Optimizing function that searches for binary patterns""
    },
    {
      ""title"": ""Coordinates of a point on a ext.js googlemap panel""
    },
    {
      ""title"": ""how to have multiple colors in a batch file?""
    },
    {
      ""title"": ""Upload DBFile.mdf in App_Data to Azure""
    },
    {
      ""title"": ""Reading from raw_data.txt and writing to a results.txt file with processing in Python""
    },
    {
      ""title"": ""How do you create a transparent demo screen for an Android app?""
    },
    {
      ""title"": ""Apex SOQL subquery in Visualforce""
    },
    {
      ""title"": ""Subversion: Add all unversioned files to subversion using one linux command""
    },
    {
      ""title"": ""Backbone Marionette Routing""
    },
    {
      ""title"": ""Concatenate Strings in Knockout""
    },
    {
      ""title"": ""Why use Mono?""
    },
    {
      ""title"": ""Cells content not shown in JTable until JTable getting focus""
    },
    {
      ""title"": ""In Python, determine if a function calls another function""
    },
    {
      ""title"": ""Parallel.Foreach with MSMQ""
    },
    {
      ""title"": ""How to use move semantics with std::string during function return?""
    },
    {
      ""title"": ""how to work with servlet and business layer?""
    },
    {
      ""title"": ""using CSS or javascript/jQuery, which method would I use to make my site&#39;s nav bar look more 3d-ish?""
    },
    {
      ""title"": ""SetWindowsHookEx not calling hook proc. Win32 console app. External dll. System wide hook""
    },
    {
      ""title"": ""highcharts rescale yaxis aftr addSeries""
    },
    {
      ""title"": ""How to programmatically read permissions on MSMQ queues?""
    },
    {
      ""title"": ""How to check input validation equals &quot;0&quot; and only &quot;0&quot;?""
    },
    {
      ""title"": ""Not understanding why text file contents are not being stored in individual elements of vector""
    },
    {
      ""title"": ""Layout for Android. Fluid layout and a fixed layout""
    },
    {
      ""title"": ""responce time google spreadsheet with large number of rows""
    },
    {
      ""title"": ""find based filename autocomplete in Bash script""
    },
    {
      ""title"": ""Cyrillic encoding in C#""
    },
    {
      ""title"": ""VBA Script hard code email address""
    },
    {
      ""title"": ""Hiding a div upon login with php""
    },
    {
      ""title"": ""node.js how to access an already required object""
    },
    {
      ""title"": ""to insert an image in word document which is generated by php""
    },
    {
      ""title"": ""iOS: iCloud: Using NSUserDefaults to save a data and app storage shows 0.2KB""
    },
    {
      ""title"": ""How to recognize Facebook User-Agent""
    },
    {
      ""title"": ""rails encapsulate tags in textfield""
    },
    {
      ""title"": ""Confusion with setting up a remote git repository""
    },
    {
      ""title"": ""formatting text in jdialog box""
    },
    {
      ""title"": ""How to recognize Facebook User-Agent""
    },
    {
      ""title"": ""rails encapsulate tags in textfield""
    },
    {
      ""title"": ""Confusion with setting up a remote git repository""
    },
    {
      ""title"": ""formatting text in jdialog box""
    },
    {
      ""title"": ""Non-contiguous array pointers in C++""
    },
    {
      ""title"": ""Rails: Create a new entry in a model that belongs_to two other models""
    },
    {
      ""title"": ""permutations/combinatorics library for java?""
    },
    {
      ""title"": ""Jersey/Jaxb aliasing a List of beans""
    },
    {
      ""title"": ""How does J2se applications running as services on my windows NT server""
    },
    {
      ""title"": ""Post image with text on facebook from android""
    },
    {
      ""title"": ""CURLOPT_POSTFIELDS has a length or size limit?""
    },
    {
      ""title"": ""Integrate Solr &amp; Mahout""
    },
    {
      ""title"": ""Re rake after you edit a model?""
    },
    {
      ""title"": ""how to simulate 2d water surface?""
    },
    {
      ""title"": ""Having difficulty with my JS/jQuery creating a div""
    },
    {
      ""title"": ""Sendgrid Newsletter App Custom Tags""
    },
    {
      ""title"": ""How to Set Timeout on WebClient call?""
    },
    {
      ""title"": ""iOS Copy to Clipboard from Browser""
    },
    {
      ""title"": ""Selecting multiple xpath elements using Selenium IDE""
    },
    {
      ""title"": ""Calling a WCF service from classic ASP""
    },
    {
      ""title"": ""Are there any good Bugzilla Skins""
    },
    {
      ""title"": ""How to change Spring version in Grails project?""
    },
    {
      ""title"": ""c++ persisting data""
    },
    {
      ""title"": ""Alternative to Scribd?""
    },
    {
      ""title"": ""XSLT beginner - not sure how to apply template""
    },
    {
      ""title"": ""Dividing text file lines to two different tables of a database""
    },
    {
      ""title"": ""Personality test logic in PHP""
    },
    {
      ""title"": ""Scroll a div when hovering over a clipped item""
    },
    {
      ""title"": ""CSS gradient at pixel location (not %)""
    },
    {
      ""title"": ""Formatting the JSON date""
    },
    {
      ""title"": ""HttpWebRequest POST method runs only once""
    },
    {
      ""title"": ""Is SpiderMonkey safe for running user-submitted scripts?""
    },
    {
      ""title"": ""upload xls file to web server""
    },
    {
      ""title"": ""How to remove files starting with #! or ending with .sh in the name""
    },
    {
      ""title"": ""how to guarantee that update to &quot;reference type&quot; item in array is visible to other threads?""
    },
    {
      ""title"": ""AVPlayer only plays in simulator""
    },
    {
      ""title"": ""Disable HTML link if the URL&#39;s link returns an error""
    },
    {
      ""title"": ""jquery slideToggle prolem with image""
    },
    {
      ""title"": ""Parse error: syntax error, unexpected T_VARIABLE, expecting T_FUNCTION - in php""
    },
    {
      ""title"": ""backbone.js views and session variables""
    },
    {
      ""title"": ""Calling R with RDotNet from C# DLL in Tradelink""
    },
    {
      ""title"": ""Replace a running executable on mac osx""
    },
    {
      ""title"": ""Download pdf or image trough ajax""
    },
    {
      ""title"": ""SQL Server calculate percentage of sql pivot value""
    },
    {
      ""title"": ""Cannot push to Heroku because key fingerprint""
    },
    {
      ""title"": ""Tomcat, HTTP Keep-Alive and Java&#39;s HttpsUrlConnection""
    },
    {
      ""title"": ""spinning network icon with reachability""
    },
    {
      ""title"": ""Objected oriented javascript - use of &quot;this&quot; in IE""
    },
    {
      ""title"": ""Perl Extracting Text""
    },
    {
      ""title"": ""Launching an HTML/Javascript Adobe Air application from a local HTML file.""
    },
    {
      ""title"": ""C# HTTP Body with GET method""
    },
    {
      ""title"": ""Verify if a point is Land or Water in Google Maps""
    },
    {
      ""title"": ""Parsing string to int, C# inside of an XSL stylesheet""
    },
    {
      ""title"": ""Globalizing Large Resources in Metro without Network""
    },
    {
      ""title"": ""Does an Index on Part of a MongoDB Query Offer Any Benefit?""
    },
    {
      ""title"": ""SQL Pivot in this case?""
    },
    {
      ""title"": ""How would I write this query with DBIX::Class?""
    },
    {
      ""title"": ""Android EditText over canvas""
    },
    {
      ""title"": ""EF4 One to many to one with Db foreign key""
    },
    {
      ""title"": ""Ruby: How to create commits programmatically and push to github?""
    },
    {
      ""title"": ""How to make a source package for a software in linux?""
    },
    {
      ""title"": ""Solr RSS DIH dateTimeFormat does NOT work""
    },
    {
      ""title"": ""Detect embedded Youtube player captions""
    },
    {
      ""title"": ""unix bash - extract a line from file""
    },
    {
      ""title"": ""What are the Best MongoDB Video tutorial websites for free?""
    },
    {
      ""title"": ""When to use CCScene vs CCLayer?""
    },
    {
      ""title"": ""SQLite data showing""
    },
    {
      ""title"": ""Boost Parameters library""
    },
    {
      ""title"": ""mediaelement.js how do I set the pluginPath variable""
    },
    {
      ""title"": ""Knockout Style binding Top and Left not working""
    },
    {
      ""title"": ""How to update binding or remove earlier binding in knockoutjs""
    },
    {
      ""title"": ""Eclipse Plugin Project does not display Window added in Application Model""
    },
    {
      ""title"": ""Git: How to merge two folder pointing to the same repository""
    },
    {
      ""title"": ""Enable php-curl features in ubuntu""
    },
    {
      ""title"": ""why is one plugin not loading using requireJS (on iOS)?""
    },
    {
      ""title"": ""Insert external data into persistence.xml""
    },
    {
      ""title"": ""SQL timestamp does not change when UPDATE happens""
    },
    {
      ""title"": ""Android cannot find accessory device after enabling WiFi debugging""
    },
    {
      ""title"": ""Bind to datagrid when one of the DataGridTextColumn is a list""
    },
    {
      ""title"": ""Difference between adding separate table rows and using rowspan""
    },
    {
      ""title"": ""printf output convert from one row to number of arguments""
    },
    {
      ""title"": ""How to handle data types in conversion from PHP Mysql API to PDO""
    },
    {
      ""title"": ""Take (n) characters from a string""
    },
    {
      ""title"": ""Global low level keyboard hook - Race Condition""
    },
    {
      ""title"": ""Doxygen does not show how the documented member functions (including the constructors) are called from other classes.?""
    },
    {
      ""title"": ""How to add a copy of an existing library in Add-on Builder""
    },
    {
      ""title"": ""REST API MongoDB Authentication""
    },
    {
      ""title"": ""How to get default value for form field in symfony2 formbuilder""
    },
    {
      ""title"": ""What to do with chrome sending extra requests?""
    },
    {
      ""title"": ""SqlCeException-Expressions in the ORDER BY list cannot contain aggregate functions""
    },
    {
      ""title"": ""override ActionLink Behavior , and parameters to process link appearance according to user privileges (asp.net mvc3)""
    },
    {
      ""title"": ""Force simultaneous threads/tasks for C# load testing app?""
    },
    {
      ""title"": ""Populating dropdown menus using XMLHTTPREQUEST object""
    },
    {
      ""title"": ""Visual Studio 2012 with ClearCase?""
    },
    {
      ""title"": ""SSIS WINSCP SFTP the latest file on everyday basis""
    },
    {
      ""title"": ""Redirect URL for client and Rewrite for server with mod_rewrite""
    },
    {
      ""title"": ""What does -@ operator do in Ruby?""
    },
    {
      ""title"": ""Django: Implementing a referral program""
    },
    {
      ""title"": ""jQuery AJAX POST to PHP file to UPDATE database not updating""
    },
    {
      ""title"": ""Stretch=&quot;uniform&quot; not behaving as expected on Silverlight MediaElement""
    },
    {
      ""title"": ""What does -@ operator do in Ruby?""
    },
    {
      ""title"": ""Django: Implementing a referral program""
    },
    {
      ""title"": ""jQuery AJAX POST to PHP file to UPDATE database not updating""
    },
    {
      ""title"": ""Stretch=&quot;uniform&quot; not behaving as expected on Silverlight MediaElement""
    },
    {
      ""title"": ""Any way to exclude binary data from SMO Transfer operation?""
    },
    {
      ""title"": ""Sproc and Linq to sql""
    },
    {
      ""title"": ""&#39;cfx&#39; is not recognized as internal or external command""
    },
    {
      ""title"": ""undefined method `paginate&#39;""
    },
    {
      ""title"": ""Creating a generic List &lt;T&gt; in F#""
    },
    {
      ""title"": ""Load user controls asynchronously ASP.NET""
    },
    {
      ""title"": ""Attaching a PDF to an emil in Appengine (Python)""
    },
    {
      ""title"": ""Extract time from datetime efficiently (as decimal or datetime)""
    },
    {
      ""title"": ""Remove default sample text in YUI Rich Text Editor""
    },
    {
      ""title"": ""IE9 setting background-image to &quot;none&quot; via inline style""
    },
    {
      ""title"": ""IKDeviceBrowserView displays no contents on OSX Mountain Lion""
    },
    {
      ""title"": ""How to detect SUB character and remove it from a text file in C#?""
    },
    {
      ""title"": ""Alternate Data Streams on a folder""
    },
    {
      ""title"": ""How to set default category in MovbleType""
    },
    {
      ""title"": ""Why is my delete account function in rails not working?""
    },
    {
      ""title"": ""Open URL in default browser without giving it focus""
    },
    {
      ""title"": ""Converting lists whose elements&#39; types are paramertized their type-encoded indexes""
    },
    {
      ""title"": ""Passing data to activity""
    },
    {
      ""title"": ""Design Django&#39;s models using ManyToManyField for multivendor scheme?""
    },
    {
      ""title"": ""How to handle a big database in j2me""
    },
    {
      ""title"": ""Changing a stuct inside another struct in a foreach loop""
    },
    {
      ""title"": ""Form Submission vs AJAX Polling Call""
    },
    {
      ""title"": ""ListView wont refresh or update in C# Forms application""
    },
    {
      ""title"": ""Mockito verify no more interactions with any mock""
    },
    {
      ""title"": ""replace a value in NSDictionary in iphone""
    },
    {
      ""title"": ""Get POST Values in ASP.NET using NameValueCollection""
    },
    {
      ""title"": ""PIM testing in netbeans simulator [J2ME]""
    },
    {
      ""title"": ""Browser does not remember password during login""
    },
    {
      ""title"": ""mongodb insert and return id with REST API""
    },
    {
      ""title"": ""What&#39;s wrong? Defining a JavaScript namespace via string""
    },
    {
      ""title"": ""What is the purpose of Hibernate?""
    },
    {
      ""title"": ""Translating multi-level mongodb documents into Backbone.js Models/Collections""
    },
    {
      ""title"": ""Avoiding Repetition in Javascript Using a Master Page""
    },
    {
      ""title"": ""Google Maps Multiple Search Results""
    },
    {
      ""title"": ""Network bandwidth throttling on Android device""
    },
    {
      ""title"": ""Synthesize ivar declaration without overriding parent getter/setters""
    },
    {
      ""title"": ""Load initial data in Play 2.0+""
    },
    {
      ""title"": ""Should I add custom methods to Ext.data.store derived classes""
    },
    {
      ""title"": ""Visual Editor for editing CSS to lessen coding workflow""
    },
    {
      ""title"": ""Get values from string in querystring format vb.net""
    },
    {
      ""title"": ""Export SQL query results to XML format using powershell""
    },
    {
      ""title"": ""Scanning ports with AsyncTask""
    },
    {
      ""title"": ""iOS: How to parse XML for real-time search""
    },
    {
      ""title"": ""Visual Basic 2010 - Looping through listbox issue and other things""
    },
    {
      ""title"": ""How to replicate this fancy green background on Aolitaire games in Java2D?""
    },
    {
      ""title"": ""Preparing list in multithreaded application""
    },
    {
      ""title"": ""Eclipse, Egit and Symlinks or symbolic links - Is it possible to make a dirty eclipse plugin to fix this?""
    },
    {
      ""title"": ""jQuery - on() issue on dynamically added elements""
    },
    {
      ""title"": ""Matlab calling fvtool from freqz, if DFILT object is passed""
    },
    {
      ""title"": ""HTML saving some text in my PC?""
    },
    {
      ""title"": ""Having a SOAP handler pass data back to the webservice client""
    },
    {
      ""title"": ""Published date in Doc List API -- what does it mean?""
    },
    {
      ""title"": ""Processing a list of lists in Python""
    },
    {
      ""title"": ""Using Drools to perform an Action based on Events""
    },
    {
      ""title"": ""How to process string in php""
    },
    {
      ""title"": ""Why doesn&#39;t it use all of the processor?""
    },
    {
      ""title"": ""Apache Wicket - PageExpiredErrorPage in modal window""
    },
    {
      ""title"": ""Facebook Like button&#39;s comment box sizing""
    },
    {
      ""title"": ""Include Sidebar Generator plug-in into my WP theme""
    },
    {
      ""title"": ""PHP Regex to determine relative or absolute path""
    },
    {
      ""title"": ""JQuery dialog only shows up after JS processing""
    },
    {
      ""title"": ""Compass/Aircraft Gauge Control""
    },
    {
      ""title"": ""JodaTime DateTimeFormatter""
    },
    {
      ""title"": ""How to embed Vimeo video without Video_ID""
    },
    {
      ""title"": ""C# Conversion from String to HtmlEdit""
    },
    {
      ""title"": ""Forced extra DOM layer using backbone with underscore template""
    },
    {
      ""title"": ""querystring to json in classic asp""
    },
    {
      ""title"": ""Python - Removing certain lines from input file based on content""
    },
    {
      ""title"": ""openssl 1.0.1 with ruby 1.9.3""
    },
    {
      ""title"": ""Formatting MM/DD/YYYY dates in textbox in VBA""
    },
    {
      ""title"": ""what is apache user""
    },
    {
      ""title"": ""How do I unit test a non-generic method that calls a generic method in a sealed non-generic class?""
    },
    {
      ""title"": ""Firefox addon ignore iframes""
    },
    {
      ""title"": ""Programically adding child controls to a asp.net composite control""
    },
    {
      ""title"": ""Parsing/populating the options array on my first jQuery plugin""
    },
    {
      ""title"": ""Tool to test website tracking""
    },
    {
      ""title"": ""ios scrolling background""
    },
    {
      ""title"": ""MBTA Google Places API""
    },
    {
      ""title"": ""IntelliJ Idea opens main, context menus and autocomplete on main display only""
    },
    {
      ""title"": ""Launching .exe from PowerShell window SOMETIMES causes it to be run in separate window, so I can&#39;t see output or get $lastexitcode""
    },
    {
      ""title"": ""Is it possible to override a Spring Namespace handler?""
    },
    {
      ""title"": ""Check module compatibility with different Perl versions""
    },
    {
      ""title"": ""How to make a browser automation program like one that I&#39;ve seen?""
    },
    {
      ""title"": ""Django/Jquery post ajax. Error 500""
    },
    {
      ""title"": ""Rails 3.2.8 - Multiple database access within one single Rails App""
    },
    {
      ""title"": ""Orchard CMS Sidebar Menu""
    },
    {
      ""title"": ""New to php: need to understand password-protecting folder/files""
    },
    {
      ""title"": ""Friend operator declaration in a template class""
    },
    {
      ""title"": ""Moving a Docs collection to a user&#39;s Drive?""
    },
    {
      ""title"": ""only assignment, call, increment, decrement, await and new object expressions can be used as a statement""
    },
    {
      ""title"": ""Extending data from one data frame to multiple rows in another in R""
    },
    {
      ""title"": ""Multiple Quartz schedule for each time zone""
    },
    {
      ""title"": ""Check domains end (url validation)""
    },
    {
      ""title"": ""Consume multivalue Web Service, vb.net""
    },
    {
      ""title"": ""How to check if WriteFile function is done""
    },
    {
      ""title"": ""Populate sibling cells on keyup event""
    },
    {
      ""title"": ""Consume multivalue Web Service, vb.net""
    },
    {
      ""title"": ""How to check if WriteFile function is done""
    },
    {
      ""title"": ""Populate sibling cells on keyup event""
    },
    {
      ""title"": ""SSRS: Would this work to suppress empty reports?""
    },
    {
      ""title"": ""Parsing xml in Go""
    },
    {
      ""title"": ""HTML - display an image as large as possible while preserving aspect ratio""
    },
    {
      ""title"": ""neo4j node property type suggestion, indexes or parent object relatinship?""
    },
    {
      ""title"": ""Multiple expandable list views not scrollable""
    },
    {
      ""title"": ""Search the webpage source for urls?""
    },
    {
      ""title"": ""PHP / CentOS - failed to open stream: Too many open files""
    },
    {
      ""title"": ""Reading Excel files with roo /rails""
    },
    {
      ""title"": ""is it possible to have a unique on a group of rows instead of entire table?""
    },
    {
      ""title"": ""Mule 3.3.0 - Exception occuring with Dynamic enpoint - flow - Inbound Ajax ---&gt; Outbound Http endpoint --&gt;File Outbound""
    },
    {
      ""title"": ""JsonArray parsing""
    },
    {
      ""title"": ""Visual Studio not rebuilding after content file change""
    },
    {
      ""title"": ""how to specify target binary format for cross compiler mips-gcc?""
    },
    {
      ""title"": ""App crashes with setValuesForKeysWithDictionary""
    },
    {
      ""title"": ""SimpleAudioEngine background volume with CCActionTween""
    },
    {
      ""title"": ""Using absolute_import and handling relative module name confilcts in python""
    },
    {
      ""title"": ""Why doesn&#39;t my TableLayoutPanel use all of its size when divided by percentages?""
    },
    {
      ""title"": ""How to version control a directory in a Mercurial repository using CVS?""
    },
    {
      ""title"": ""jsLint still giving warning &quot;X was used before it was defined.&quot; even when undef is set""
    },
    {
      ""title"": ""jQuery - Adding a smooth animation, or fade in / out to make the transition cleaner""
    },
    {
      ""title"": ""table with closeable? columns""
    },
    {
      ""title"": ""How to do a Rails 3 join query?""
    },
    {
      ""title"": ""fields_for nested form""
    },
    {
      ""title"": ""Java getGraphics() returning null""
    },
    {
      ""title"": ""Accessing Java BigQuery Tools in Maven with Nexus""
    },
    {
      ""title"": ""adbrite with cloudflare""
    },
    {
      ""title"": ""How can I send a kml file to Google Earth, like MyTracks (open source) do?""
    },
    {
      ""title"": ""Mount ext2 sd card with BusyBox""
    },
    {
      ""title"": ""Single sign on multiple django sites""
    },
    {
      ""title"": ""How can I split a PDF file by file size using C#?""
    },
    {
      ""title"": ""Retargeting solution from .Net 4.0 to 4.5 - how to retarget the NuGet packages?""
    },
    {
      ""title"": ""Remove unsaved entity form related entitie&#39;s collection""
    },
    {
      ""title"": ""Listing the XPath to an XML node from the command line""
    },
    {
      ""title"": ""How to get selected input:radio""
    },
    {
      ""title"": ""Use offline voice-to-text in Android 4.1 (Jelly Bean) from my application?""
    },
    {
      ""title"": ""Slow SDK download through Android SDK manager - solved""
    },
    {
      ""title"": ""Added Data On Other Page using ItextSharp""
    },
    {
      ""title"": ""eclipse team ( Mercurial Eclipse ) edit highlighting""
    },
    {
      ""title"": ""In the Oracle JDBC driver, what happens to the time zone when you write a Java date to a TIMESTAMP column?""
    },
    {
      ""title"": ""Pascal: Store classes in a 2d array with New and Dispose""
    },
    {
      ""title"": ""how to create a jQuery loading bar? (like the ones used on flash sites)""
    },
    {
      ""title"": ""WCF Web Service serialization error - returning null values""
    },
    {
      ""title"": ""Need a good reference for Google Generic Widget class""
    },
    {
      ""title"": ""SQL query in SQL developer atacking mdb, PIVOT table""
    },
    {
      ""title"": ""Linq Extension to Return Grouped results""
    },
    {
      ""title"": ""Using Rails models with accepts_nested_attributes_for""
    },
    {
      ""title"": ""Changing wallpaper and stretch it on Windows platform using Java(with JNA)""
    },
    {
      ""title"": ""Javascript doesnt contact php page""
    },
    {
      ""title"": ""What is the most efficient way to calculate installed base?""
    },
    {
      ""title"": ""Filtering through a Django Countries list""
    },
    {
      ""title"": ""TCPDF UTF-8 problem""
    },
    {
      ""title"": ""SQL - What is the performance impact of having multiple CASE statements in SELECT - Teradata""
    },
    {
      ""title"": ""Create partial class for referenced assembly""
    },
    {
      ""title"": ""Moving to the last line of a file while reading it in a .each loop in Ruby""
    },
    {
      ""title"": ""Zend Translate doesn&#39;t find language""
    },
    {
      ""title"": ""How do I hash the admin password in my Users table?""
    },
    {
      ""title"": ""kartograph.js - very simple map""
    },
    {
      ""title"": ""WCF and localhost""
    },
    {
      ""title"": ""Extract body text from Email PHP""
    },
    {
      ""title"": ""Countdown Timer keeps starting over on button press""
    },
    {
      ""title"": ""How to get IndexReader from custom request handler?""
    },
    {
      ""title"": ""Reference variable in class definition""
    },
    {
      ""title"": ""Getting authentication token from IP with nunit""
    },
    {
      ""title"": ""How to show balloon only 1 balloon in 1 map?""
    },
    {
      ""title"": ""ASP.Net MVC3 Controller Authorize attribute issue""
    },
    {
      ""title"": ""Is a strange code? UIGraphicsGetImageFromCurrentImageContext""
    },
    {
      ""title"": ""Back button in UINavigationController""
    },
    {
      ""title"": ""max heap and binary tree""
    },
    {
      ""title"": ""Wordpress: Adding &quot;Tags&quot; Section""
    },
    {
      ""title"": ""C# Regex to remove single quotes between single quotes""
    },
    {
      ""title"": ""Guarantees given by the Java Memory Model""
    },
    {
      ""title"": ""DIFFERENCE BETWEEN ORACLE AND MYSQL""
    },
    {
      ""title"": ""How to use MVC3 client-side form validation in an Ajax form?""
    },
    {
      ""title"": ""How can I set the PATH variable for javac so I can manually compile my .java works?""
    },
    {
      ""title"": ""ignore versioned files from being checked out to a working copy""
    },
    {
      ""title"": ""Rails will_paginate and paging newest records in ASC order""
    },
    {
      ""title"": ""bash. Usage of: &#39;./&#39;""
    },
    {
      ""title"": ""Are SCJP 6 and OCJP both same?""
    },
    {
      ""title"": ""Free hosting providers for ASP.Net and SQL server apps""
    },
    {
      ""title"": ""Managing code with C# Windows Forms""
    },
    {
      ""title"": ""Parse error in variable parsing PHP""
    },
    {
      ""title"": ""TSQL not inserting when supposed to""
    },
    {
      ""title"": ""Cannot execute JMX MXBean operation of type CompositeData from JConsole""
    },
    {
      ""title"": ""CSS / margin above first paragraph""
    },
    {
      ""title"": ""CodeIgniter 2.1.1 .htaccess issue after upgrade""
    },
    {
      ""title"": ""What went wrong with my first attempt at running an Android app?""
    },
    {
      ""title"": ""Get data from mysql depending on what the user has done""
    },
    {
      ""title"": ""Loop over a sorted dictionary a specific number of times in a django template""
    },
    {
      ""title"": ""How to check/uncheck a QTableView and trigger setData()""
    },
    {
      ""title"": ""Batch script: how to check for admin rights""
    },
    {
      ""title"": ""GStreamer How to extract video frame from the flow?""
    },
    {
      ""title"": ""Android Status Bar Icon Only""
    },
    {
      ""title"": "".NET Dependency Management and Tagging/Branching""
    },
    {
      ""title"": ""Watch file create events using ruby guard gem""
    },
    {
      ""title"": ""setPage() not working in Macs""
    },
    {
      ""title"": ""jQuery Slider Step Attribute Not Working""
    },
    {
      ""title"": ""Should I put effort on learning Matlab if I already know and use Numpy/Scipy""
    }
  ]
}";
            }
        }
    }
}
