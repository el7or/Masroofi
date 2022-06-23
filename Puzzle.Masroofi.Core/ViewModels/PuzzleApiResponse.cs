using System;

namespace Puzzle.Masroofi.Core.ViewModels
{
    public class PuzzleApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public bool Ok { get; set; }
        public object Result { get; set; }

        public PuzzleApiResponse(string message = "", int statusCode = 200)
        {
            StatusCode = statusCode;
            Message = message == string.Empty ? "Success" : message;
            Ok = statusCode >= 200 && statusCode < 400 ? true : false;
        }

        public PuzzleApiResponse(DateTime sentDate, object result = null)
        {
            StatusCode = 200;
            Ok = (result == null) ? false : true;
            Result = result;
            Message = "Success";
        }

        public PuzzleApiResponse(object result)
        {
            Ok = (result == null) ? false : true;
            StatusCode = 200;
            Result = result;
        }
    }
}
