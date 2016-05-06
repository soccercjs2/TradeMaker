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
        private CookieContainer Cookies { get; set; }

        public WebScraper(): this(null, null) { }

        public WebScraper(string loginUrl, string postData)
        {
            if (loginUrl != null) { Cookies = Login(loginUrl, postData); }
        }

        private CookieContainer Login(string loginUrl, string postData)
        {
            HttpWebRequest webRequest;
            CookieContainer cookies = new CookieContainer();
            StreamWriter requestWriter;

            try
            {
                //get login  page with cookies
                webRequest = (HttpWebRequest)WebRequest.Create(loginUrl);
                webRequest.CookieContainer = cookies;

                //recieve non-authenticated cookie
                WebResponse response = webRequest.GetResponse();
                response.Close();

                //post form  data to page
                webRequest = (HttpWebRequest)WebRequest.Create(loginUrl);
                webRequest.Method = WebRequestMethods.Http.Post;
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.CookieContainer = cookies;

                webRequest.ContentLength = postData.Length;

                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                requestWriter.Write(postData);
                requestWriter.Close();

                //recieve authenticated cookie
                webRequest.GetResponse().Close();
            }
            catch (Exception e) {
                string asdf = "asdf";
            }

            return cookies;
        }

        public HtmlDocument Scrape(string url)
        {
            HttpWebRequest webRequest;
            StreamReader responseReader;
            string responseData;

            HtmlDocument document = null;

            try
            {
                //now we get the authenticated page
                webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.CookieContainer = Cookies;
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