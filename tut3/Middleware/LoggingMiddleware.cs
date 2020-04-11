using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using tut3.Logging;

namespace tut3.Middleware
{
    public class LoggingMiddleware
    {
        private const string path = @"Logs\log.txt";

        private readonly RequestDelegate _next;
        private readonly Logger _logger;

        public LoggingMiddleware(RequestDelegate next) 
        {
            _next = next;
            _logger = new Logger(path);
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            string method = httpContext.Request.Method;
            string endpoint = httpContext.Request.Path;

            string body;
            using (var reader = new StreamReader(httpContext.Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }

            string queryString = httpContext.Request.QueryString.ToString();

            if (queryString.Equals(string.Empty)) queryString = "None";

            string separator = "------------------------------------------";

            _logger.PrintMessage($"Method: {method}\nEndpoint: {endpoint}\nBody: {body}\nQuery strings: {queryString}\n{separator}\n");

            await _next(httpContext);
        }

    }
}
