﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Puzzle.Masroofi.Core.ViewModels;
using Serilog;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.Core.Helpers
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            try
            {
                if (IsSwag(context) || !IsApi(context))
                    await _next(context);
                else
                {
                    Stream originalBody = context.Response.Body;
                    try
                    {
                        string responseBody = null;
                        using (var memStream = new MemoryStream())
                        {
                            context.Response.Body = memStream;
                            await _next(context);
                            memStream.Position = 0;
                            responseBody = new StreamReader(memStream).ReadToEnd();
                        }
                        if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 400 && !IsHtml(responseBody))
                        {
                            object data = responseBody;
                            if (IsValidJson(responseBody))
                                data = JsonSerializer.Deserialize<object>(responseBody);
                            responseBody = JsonSerializer.Serialize(data, options);
                        }
                        var buffer = Encoding.UTF8.GetBytes(responseBody);
                        using (var output = new MemoryStream(buffer))
                        {
                            await output.CopyToAsync(originalBody);
                        }
                    }
                    finally
                    {
                        context.Response.Body = originalBody;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                Log.Error(ex, ex.Message);
                context.Response.ContentType = "application/json";
                PuzzleApiResponse response;
                if (ex is BusinessException || ex is AggregateException)
                {
                    if (ex.Message.ToLower().Contains("not found"))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        response = new PuzzleApiResponse(message: ex.Message, statusCode: (int)HttpStatusCode.NotFound);
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        response = new PuzzleApiResponse(message: ex.Message, statusCode: (int)HttpStatusCode.BadRequest);
                    }
                }
                else if (ex is DbUpdateConcurrencyException)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    response = new PuzzleApiResponse(message: "Concurrency conflict! Please retry.", statusCode: (int)HttpStatusCode.Conflict);
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response = new PuzzleApiResponse(message: ex.Message, (int)HttpStatusCode.InternalServerError);
                }
                var json = JsonSerializer.Serialize(response, options);
                await context.Response.WriteAsync(json);
            }
        }

        private bool IsHtml(string text)
        {
            Regex tagRegex = new Regex(@"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");

            return tagRegex.IsMatch(text);
        }

        private bool IsApi(HttpContext context)
        {
            return !context.Request.Path.Value.Contains(".js") && !context.Request.Path.Value.Contains(".css") && !context.Request.Path.Value.Contains(".html");
        }

        private bool IsSwag(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments(new PathString("/swagger"));
        }

        private bool IsValidJson(string text)
        {
            text = text.Trim();
            if ((text.StartsWith("{") && text.EndsWith("}")) || //For object
                (text.StartsWith("[") && text.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(text);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
