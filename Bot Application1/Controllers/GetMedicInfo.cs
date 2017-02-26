using System;
using System.Net;
using System.IO;
using RestSharp;
using Newtonsoft.Json;
using Bot_Application1.Models;
using System.Collections.Generic;

namespace Bot_Application1
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    class MedicInfo
    {
        Dictionary<string, int> symptoms2id = new Dictionary<string, int>();
        Dictionary<String, int> issues2id = new Dictionary<string, int>();
        String base_url;
        String token;
        String meta;
        
        public MedicInfo()
        {
            base_url = "https://sandbox-healthservice.priaid.ch/";
            meta = "&language=en-gb&format=json";
            token = "token=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJlbWFpbCI6InJvbGxpbmsueWFuZ0BnbWFpbC5jb20iLCJyb2xlIjoiVXNlciIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL3NpZCI6IjEyMzEiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3ZlcnNpb24iOiIyMDAiLCJodHRwOi8vZXhhbXBsZS5vcmcvY2xhaW1zL2xpbWl0IjoiOTk5OTk5OTk5IiwiaHR0cDovL2V4YW1wbGUub3JnL2NsYWltcy9tZW1iZXJzaGlwIjoiUHJlbWl1bSIsImh0dHA6Ly9leGFtcGxlLm9yZy9jbGFpbXMvbGFuZ3VhZ2UiOiJlbi1nYiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvZXhwaXJhdGlvbiI6IjIwOTktMTItMzEiLCJodHRwOi8vZXhhbXBsZS5vcmcvY2xhaW1zL21lbWJlcnNoaXBzdGFydCI6IjIwMTctMDItMjUiLCJpc3MiOiJodHRwczovL3NhbmRib3gtYXV0aHNlcnZpY2UucHJpYWlkLmNoIiwiYXVkIjoiaHR0cHM6Ly9oZWFsdGhzZXJ2aWNlLnByaWFpZC5jaCIsImV4cCI6MTQ4ODA4MTM3OCwibmJmIjoxNDg4MDc0MTc4fQ.d7WiEoROwZgX-N18v2OOsGU9bcmzBLIEWurgAOp5oYg";
            init();
        }

        private IRestResponse getRespons(String url)
        {
            var client = new RestClient(url);
            var request = new RestRequest("", Method.GET);
            IRestResponse response = client.Execute(request);
            return response;
         
        }

        private void init()
        { 
            var response = getRespons(base_url + "symptoms?" + token + meta);
            var content = response.Content;
            if (content.Contains("invalid token"))
            {
                return;
            }
            var symList = JsonConvert.DeserializeObject<List<Symptom>>(content);
            foreach (Symptom sym in symList)
            {
                symptoms2id.Add(sym.Name.ToLowerInvariant(), sym.ID);
            }
        }

        public String getDiagonoses(List<String> symptoms, string gender, int yearOfBirth)
        {
            String symptomIdList = "";
            for (var i = 0; i < symptoms.Count; i++)
            {
                String currentSym = symptoms[i];
                if (symptoms2id.ContainsKey(currentSym))
                {
                    if (i == symptoms.Count - 1)
                    {
                        symptomIdList += symptoms2id[currentSym];
                    }
                    else
                    {
                        symptomIdList += symptoms2id[currentSym] + ",";
                    }
                }
            }
            if (symptomIdList.Length == 0)
            {
                return "Cannot diagnose the issue, please enter symptoms in one of the given list";
            }

            string diagnose_url = base_url + "diagnosis?symptoms=[" + symptomIdList + "]&gender="+ gender + "&year_of_birth=" + yearOfBirth +"&"+token+meta;

            return parseDiagnosis(diagnose_url);
        }


        private string parseDiagnosis(String diagnose_url)
        {
            var response = getRespons(diagnose_url);
            var content = response.Content;
            var symList = JsonConvert.DeserializeObject<List<Diagnose>>(content);
            return symList[0].Issue.Name;
        }


        public String getSymptoms(String issue)
        {
            if (issues2id.ContainsKey(issue))
            {
                var issueId = issues2id[issue];

                return null;
            } else
            {
                return "Cannot find the issue, please describe your symptoms";
            }
           
        }


        public String getTreatment(String issue)
        {
            var issueId = issues2id[issue];
            String treatement_url = base_url+"issues/80/info?"+token+meta;
            var response = getRespons(treatement_url);
            var content = response.Content;
            var symList = JsonConvert.DeserializeObject<List<IssueInfo>>(content);
            return symList[0].TreatmentDescription;

        }


    }
}