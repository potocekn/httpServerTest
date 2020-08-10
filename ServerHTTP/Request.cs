using System;
using System.Collections.Generic;
using System.Text;

namespace ServerHTTP
{
    public class Request
    {
        public string Type { get; set; }
        public string URL { get; set; }
        public string Host { get; set; }

        private Request(string type, string url, string host)
        {
            this.Type = type;
            this.URL = url;
            this.Host = host;
        }

        public static Request GetRequest(string request)
        {
            if (String.IsNullOrEmpty(request))
            {
                return null; 
            }

            string[] tokens = request.Split(' ');
            string type = tokens[0];
            string url = tokens[1];
            string host = tokens[4];
            return new Request(type, url, host);
        }
    }
}
