using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace TradeMakerScraper.Tools
{
    public class WebScraper
    {
        public string Url { get; set; }
        public string LoginUrl { get; set;}
        public string PostData { get; set; }

        public WebScraper(string url): this(url, null, null) { }

        public WebScraper(string url, string loginUrl, string postData)
        {
            Url = url;
            LoginUrl = loginUrl;
            PostData = postData;
        }

        public HtmlDocument Scrape()
        {
            HttpWebRequest webRequest;
            StreamReader responseReader;
            string responseData;
            CookieContainer cookies = new CookieContainer();
            StreamWriter requestWriter;

            HtmlDocument document = null;

            try
            {
                //get login  page with cookies
                webRequest = (HttpWebRequest)WebRequest.Create(Url);
                webRequest.CookieContainer = cookies;

                //recieve non-authenticated cookie
                webRequest.GetResponse().Close();

                //post form  data to page
                webRequest = (HttpWebRequest)WebRequest.Create(LoginUrl);
                webRequest.Method = WebRequestMethods.Http.Post;
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.CookieContainer = cookies;

                if (LoginUrl != null)
                {
                    webRequest.ContentLength = PostData.Length;

                    requestWriter = new StreamWriter(webRequest.GetRequestStream());
                    requestWriter.Write(PostData);
                    requestWriter.Close();
                }

                //recieve authenticated cookie
                webRequest.GetResponse().Close();

                //now we get the authenticated page
                webRequest = (HttpWebRequest)WebRequest.Create(Url);
                webRequest.CookieContainer = cookies;
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
                responseReader.Close();

                //load html into htmlagilitypack
                document = new HtmlDocument();
                document.LoadHtml(responseData);
            }
            catch { }

            return document;
        }
    }
}