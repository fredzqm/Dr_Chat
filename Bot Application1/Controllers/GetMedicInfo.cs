using System;
using System.Net;
using RestSharp;
using Newtonsoft.Json;
using Bot_Application1.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

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
        const string meta = "&language=en-gb&format=json";
        string token = "";

        public MedicInfo()
        {
            populateToken();
            List<MedicObject> symList = getRespons<MedicObject>("symptoms?");
            foreach (MedicObject sym in symList)
            {
                symptoms2id.Add(sym.Name.ToLowerInvariant(), sym.ID);
            }
        }

        private void populateToken()
        {
            string uri = "https://sandbox-authservice.priaid.ch/login";
            string api_key = "rollink.yang@gmail.com";
            string secret_key = "Qw42JsFt8f3B6Nig9";
            byte[] secretBytes = Encoding.UTF8.GetBytes(secret_key);
            string computedHashString = "";
            using (System.Security.Cryptography.HMACMD5 hmac = new HMACMD5(secretBytes))
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(uri);
                byte[] computedHash = hmac.ComputeHash(dataBytes);
                computedHashString = Convert.ToBase64String(computedHash);
            }

            using (WebClient client = new WebClient())
            {
                client.Headers["Authorization"] = string.Concat("Bearer ", api_key, ":", computedHashString);
                token = JsonConvert.DeserializeObject<Dictionary<string, string>>(client.UploadString(uri, "POST", ""))["Token"];
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

            string diagnose_url = String.Format("diagnosis?symptoms=[{0}]&gender={1}&year_of_birth={2}&", symptomIdList, gender, yearOfBirth);

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