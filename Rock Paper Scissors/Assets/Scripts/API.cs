using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

    public class API:MonoBehaviour
    {
        private string results;
        private const string domain = "";
        public String Results
        {
            get
            {
                return results;
            }
        }

        public WWW GET(string url, System.Action onComplete)
        {

            WWW www = new WWW(domain+url);
            StartCoroutine(WaitForRequest(www, onComplete));
            return www;
        }

        public WWW POST(string url, Dictionary<string, string> post, System.Action onComplete)
        {
            WWWForm form = new WWWForm();

            foreach (KeyValuePair<string, string> post_arg in post)
            {
                form.AddField(post_arg.Key, post_arg.Value);
            }

            WWW www = new WWW(domain+url, form);

        
            StartCoroutine(WaitForRequest(www, onComplete));
            return www;
        }

        private IEnumerator WaitForRequest(WWW www, System.Action onComplete)
        {
            yield return www;
            // check for errors
            if (www.error == null)
            {
                results = www.text;
            Debug.Log("Results: " + results);
            if (onComplete!=null)
                onComplete();
            }
            else
            {
                Debug.Log(www.error+www.text);
            }
        }
    }
