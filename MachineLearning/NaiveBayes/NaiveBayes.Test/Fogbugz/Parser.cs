using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NaiveBayes.Data;

namespace NaiveBayes.Test.Fogbugz
{
    public class BugEvents
    {
        public String Text { get; set; }

        public int UserId { get; set; }
    }

    public class Case
    {
        public int OpenedById { get; set; }

        public String Title { get; set; }

        public List<BugEvents> Events { get; set; }

        public string Priority { get; set; }

        public string AssignedTo { get; set; }

        public int CaseId { get; set; }

        public String OpenedBy { get; set; }

        public int ResolvedById { get; set; }

        public String ResolvedBy { get; set; }

        public string Area { get; set; }

        public Document ToDoc(Func<Case, string> classSelector, Func<Case, string> textSelector)
        {
            return new Document
                   {
                       Class = new Class
                               {
                                   Name = classSelector(this)
                               },
                       Words = VocabBuilder.Tokenize(textSelector(this)).Words
                   };
        }
    }    

    public class Parser
    {
        public Dictionary<int, string> Users { get; set; } 

        public void BuildUsers(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            var root = XDocument.Load(path).Root.AsFluent();

            Users = ((root.people.person) as FxElements).Elements
                .Select(i => new { id = i.AsFluent().ixPerson(), name = i.AsFluent().sFullName() })
                .ToDictionary(i => Convert.ToInt32(i.id as string), i => i.name as string);
        }

        private string FindUser(int id)
        {
            string name;
            Users.TryGetValue(id, out name);

            return name;
        }
        public List<Case> GetCases(string usersXml, string caseXml)
        {
            BuildUsers(usersXml);

            if (!File.Exists(caseXml))
            {
                throw new FileNotFoundException();
            }

            var root = XDocument.Load(caseXml).Root.AsFluent();

            var foundCases = (root.cases.@case as FxElements).Elements;

            var caseList = new List<Case>();

            foreach(var @case in foundCases)
            {
                var events = (@case.AsFluent().events.@event as FxElements).Elements;

                var caseObj = new Case();

                var fluentCase = @case.AsFluent();

                caseObj.AssignedTo = fluentCase.sPersonAssignedTo();
                caseObj.Priority = fluentCase.sPriority();
                caseObj.Area = fluentCase.sArea();
                caseObj.OpenedById = Convert.ToInt32(fluentCase.ixPersonOpenedBy());
                caseObj.Title = fluentCase.sTitle();
                caseObj.ResolvedById = Convert.ToInt32(fluentCase.ixPersonResolvedBy());
                caseObj.ResolvedBy = FindUser(caseObj.ResolvedById);
                caseObj.OpenedBy = FindUser(caseObj.OpenedById);
                caseObj.CaseId = Convert.ToInt32(fluentCase["ixBug"]);
                
                caseObj.Events = new List<BugEvents>();

                foreach (var eventElement in events)
                {
                    var fluentEvent = eventElement.AsFluent();

                    caseObj.Events.Add(new BugEvents
                                       {
                                           Text = fluentEvent.s(),
                                           UserId = Convert.ToInt32(fluentEvent.ixPerson())
                                       });
                }

                caseList.Add(caseObj);
            }

            return caseList;
        } 
    }
}
