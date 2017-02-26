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
            List<MedicObject> symList = getRespons<MedicObject>("symptoms?");
            foreach (MedicObject sym in symList)
            {
                symptoms2id.Add(sym.Name.ToLowerInvariant(), sym.ID);
            }
        }

        private List<T> getRespons<T>(String url)
        {
            url = String.Format("{0}{1}token={2}{3}", base_url, url, token, meta);
            Debug.WriteLine(url);
            var client = new RestClient(url);
            var request = new RestRequest("", Method.GET);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            return JsonConvert.DeserializeObject<List<T>>(content);
        }

        public string getTreatments(List<string> entityList)
        {
            if (entityList.Count == 0)
                return "We don't know your issues";
            var issueId = issues2id[entityList[0]];
            String treatement_url = "issues/" + issueId + "/info?";
            var symList = getRespons<IssueInfo>(treatement_url);
            return symList[0].TreatmentDescription;
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

            string diagnose_url = String.Format("diagnosis?symptoms=[{0}]&gender={1}&year_of_birth={2}", symptomIdList, gender, yearOfBirth);

            List<Diagnose> symList = getRespons<Diagnose>(diagnose_url);
            return symList[0].Issue.Name;
        }


        public String getSymptoms(String issue)
        {
            if (issues2id.ContainsKey(issue))
            {
                var issueId = issues2id[issue];

                return null;
            }
            else
            {
                return "Cannot find the issue, please describe your symptoms";
            }

        }
    }
}