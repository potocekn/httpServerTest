using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace ServerHTTP
{
    public class Response
    {
        private Byte[] data = null;
        private HttpStatusCode status;
        private string mime;
        private Response(HttpStatusCode status, String mime, Byte[] data)
        {
            this.status = status;
            this.mime = mime;
            this.data = data;
        }

        public static Response From(Request request)
        {
            if (request == null)
            {
                return NullRequestResponse();
            }

            if (request.Type == "GET")
            {
                string file = Environment.CurrentDirectory + HTTPServer.WEB_DIR + request.URL;
                FileInfo f = new FileInfo(file);

                if (f.Exists && f.Extension.Contains('.')) // je to file
                {
                    return FromFileResponse(f);
                }
                else
                {
                    DirectoryInfo di = new DirectoryInfo(f + "/");
                    if (!di.Exists) return PageNotFoundResponse();
                    FileInfo[] files = di.GetFiles();

                    foreach (FileInfo ff in files)
                    {
                        string name = ff.Name;
                        if (name.Contains("default.html") || name.Contains("default.htm") || name.Contains("index.html") || name.Contains("index.htm"))
                        {
                            Console.WriteLine("FOUND INDEX PAGE: " + name);
                            return FromFileResponse(ff);
                        }
                    }
                }                
            }
            else
            {
                return MethodNotAllowedResponse();
            }

            return PageNotFoundResponse();
        }

        private static Response FromFileResponse(FileInfo f)
        {
            FileStream fs = f.OpenRead();
            BinaryReader binaryReader = new BinaryReader(fs);

            Byte[] d = new Byte[fs.Length];
            binaryReader.Read(d, 0, d.Length);
            fs.Close();
            return new Response(HttpStatusCode.OK, "text/html", d);
        }

        private static Response NullRequestResponse()
        {
            string file = Environment.CurrentDirectory + HTTPServer.MSG_DIR + "400.html";

            FileInfo fi = new FileInfo(file);
            FileStream fs = fi.OpenRead();
            BinaryReader binaryReader = new BinaryReader(fs);

            Byte[] d = new Byte[fs.Length];
            binaryReader.Read(d, 0, d.Length);
            fs.Close();
            return new Response(HttpStatusCode.BadRequest, "text/html", d);
        }

        private static Response PageNotFoundResponse()
        {
            string file = Environment.CurrentDirectory + HTTPServer.MSG_DIR + "404.html";

            FileInfo fi = new FileInfo(file);
            FileStream fs = fi.OpenRead();
            BinaryReader binaryReader = new BinaryReader(fs);

            Byte[] d = new Byte[fs.Length];
            binaryReader.Read(d, 0, d.Length);
            fs.Close();
            return new Response(HttpStatusCode.NotFound, "text/html", d);
        }

        private static Response MethodNotAllowedResponse()
        {
            string file = Environment.CurrentDirectory + HTTPServer.MSG_DIR + "405.html";

            FileInfo fi = new FileInfo(file);
            FileStream fs = fi.OpenRead();
            BinaryReader binaryReader = new BinaryReader(fs);

            Byte[] d = new Byte[fs.Length];
            binaryReader.Read(d, 0, d.Length);
            fs.Close();
            return new Response(HttpStatusCode.MethodNotAllowed, "text/html", d);
        }

        public void Post(NetworkStream stream)
        {
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(String.Format("{0} {1}\r\nServer: {2}\r\nContent-Type: {3}\r\nAccept-Ranges: bytes\r\nContent-Length: {4}\r\n",
                HTTPServer.VERSION, status, HTTPServer.NAME, mime, data.Length));

            stream.Write(data, 0, data.Length);
        }
    }
}
