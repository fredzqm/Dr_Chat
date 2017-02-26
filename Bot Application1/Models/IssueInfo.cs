using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace Bot_Application1
{
    public class IssueInfo
    {
        public string Description { get; set; }
        public string DescriptionShort { get; set; }
        public string MedicalCondition { get; set; }
        public string Name { get; set; }
        public string PossibleSymptoms { get; set; }
        public string ProfName { get; set; }
        public string Synonyms { get; set; }
        public string TreatmentDescription { get; set; }
    }

}