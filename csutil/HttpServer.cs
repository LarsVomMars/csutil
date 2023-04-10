using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace csutil
{
    using RouteHandler = Action<string, Response>;

    public class HttpServer
    {
        private readonly HttpListener _listener;
        private readonly Logger _logger;

        private Dictionary<string, RouteHandler> _routes;

        private static readonly string RequiredUserAgent = "csutil/1.0";
        private static readonly string RequiredAuthorization = "Bearer SGVsbG8gV29ybGQh";

        public HttpServer(Logger logger, params string[] urls)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Clear();
            foreach (var url in urls)
            {
                var urlFixed = url;
                if (!url.EndsWith("/")) urlFixed += "/";
                _listener.Prefixes.Add(urlFixed);
            }

            _logger = logger;
        }

        public HttpServer AddRoute(string path, Action<string, Response> handler)
        {
            _routes.Add(path, handler);
            return this;
        }

        public Task Start()
        {
            _listener.Start();
            return Task.Run(Handle);
        }

        public void Stop()
        {
            _listener.Stop();
        }

        private async Task Handle()
        {
            while (_listener.IsListening)
            {
                var ctx = await _listener.GetContextAsync();
                var req = ctx.Request;
                var res = ctx.Response;

                var ip = req.RemoteEndPoint?.Address;
                var method = req.HttpMethod;
                var path = req.Url.AbsolutePath;
                _logger.Info($"{method} {path}");

                var agent = req.UserAgent;
                var auth = req.Headers["Authorization"];

                // Create some simple restrictions for the requests e.g. User-Agent and Authorization
                if (agent != RequiredUserAgent || auth != RequiredAuthorization)
                {
                    _logger.Info($"Wrong request from {ip} with {agent} and {auth}: {req.HttpMethod} {req.Url}");
                    res.StatusCode = (int) HttpStatusCode.Unauthorized;
                    res.Close();
                    continue;
                }

                if (method != "POST") // only accept POST
                {
                    res.StatusCode = (int) HttpStatusCode.MethodNotAllowed;
                    res.Close();
                    continue;
                }

                if (!req.HasEntityBody)
                {
                    res.StatusCode = (int) HttpStatusCode.BadRequest;
                    res.Close();
                    continue;
                }

                var response = new Response();
                var body = "";
                try
                {
                    body = GetBody(req);
                    if (_routes.ContainsKey(path))
                    {
                        var handler = _routes[path];
                        handler(body, response);
                    }
                    else
                    {
                        response.Success = false;
                        response.Error = "Not Found";
                        res.StatusCode = (int) HttpStatusCode.NotFound;
                    }
                }
                catch (Exception e)
                {
                    response.Success = false;
                    response.Error = e.Message;
                }
                finally
                {
                    var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
                    res.ContentType = "application/json";
                    res.ContentLength64 = data.LongLength;
                    res.ContentEncoding = Encoding.UTF8;
                    await res.OutputStream.WriteAsync(data, 0, data.Length);
                    res.Close();

                    if (!response.Success)
                        File.WriteAllText($"request-{DateTimeOffset.Now.ToUnixTimeSeconds()}.json", body);
                }
            }
        }

        private static string GetBody(HttpListenerRequest request)
        {
            using var reader = new StreamReader(request.InputStream, Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }

    public class Response
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public string Data { get; set; }

        public Response()
        {
            Success = true;
            Error = "";
            Data = "";
        }
    }
}