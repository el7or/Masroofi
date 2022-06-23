using System;

namespace Puzzle.Masroofi.Core.Extensions
{
    public static class TimeExtensions
    {
        public static long ToUnixTime(this DateTime input)
        {
            return ((DateTimeOffset)input).ToUnixTimeSeconds();
        }
    }
}
