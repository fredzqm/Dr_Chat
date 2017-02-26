using System;
using System.Net;
using System.IO;
using RestSharp;
using Newtonsoft.Json;
using Bot_Application1.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bot_Application1
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    class MedicInfo
    {
        Dictionary<string, int> symptoms2id = new Dictionary<string, int>();
        Dictionary<String, int> issues2id = new Dictionary<string, int>();
        const string base_url = "https://sandbox-healthservice.priaid.ch/";
        const string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJlbWFpbCI6InJvbGxpbmsueWFuZ0BnbWFpbC5jb20iLCJyb2xlIjoiVXNlciIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL3NpZCI6IjEyMzEiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3ZlcnNpb24iOiIyMDAiLCJodHRwOi8vZXhhbXBsZS5vcmcvY2xhaW1zL2xpbWl0IjoiOTk5OTk5OTk5IiwiaHR0cDovL2V4YW1wbGUub3JnL2NsYWltcy9tZW1iZXJzaGlwIjoiUHJlbWl1bSIsImh0dHA6Ly9leGFtcGxlLm9yZy9jbGFpbXMvbGFuZ3VhZ2UiOiJlbi1nYiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvZXhwaXJhdGlvbiI6IjIwOTktMTItMzEiLCJodHRwOi8vZXhhbXBsZS5vcmcvY2xhaW1zL21lbWJlcnNoaXBzdGFydCI6IjIwMTctMDItMjUiLCJpc3MiOiJodHRwczovL3NhbmRib3gtYXV0aHNlcnZpY2UucHJpYWlkLmNoIiwiYXVkIjoiaHR0cHM6Ly9oZWFsdGhzZXJ2aWNlLnByaWFpZC5jaCIsImV4cCI6MTQ4ODA5MDkxOCwibmJmIjoxNDg4MDgzNzE4fQ.wfqf5JreH1SOsxLqm35MAZR2yFBAJjra8pPk2Y_XG4g";
        const string meta = "&language=en-gb&format=json";

        public MedicInfo()
        {
            string url = String.Format("{0}symptoms?token={1}{2}", base_url, token, meta);
            var response = getRespons(url);
            var content = response.Content;
            var symList = JsonConvert.DeserializeObject<List<MedicObject>>(content);
            foreach (MedicObject sym in symList)
            {
                symptoms2id.Add(sym.Name.ToLowerInvariant(), sym.ID);
            }
        }

        private IRestResponse getRespons(String url)
        {
            var client = new RestClient(url);
            var request = new RestRequest("", Method.GET);
            IRestResponse response = client.Execute(request);
            return response;
         
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

            string diagnose_url = String.Format("{0}diagnosis?symptoms=[{1}]&gender={2}&year_of_birth={3}&token={4}{5}",
                base_url, symptomIdList,  gender, yearOfBirth, token, meta);

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