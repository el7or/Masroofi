using System.ComponentModel.DataAnnotations;

namespace Puzzle.Masroofi.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidEmailAddress(this string emailAddress) => emailAddress != null && new EmailAddressAttribute().IsValid(emailAddress);
        public static bool IsValidPhoneNumber(this string phoneNumber) => phoneNumber != null && new RegularExpressionAttribute("^201[0125][0-9]{8}$").IsValid(phoneNumber);
        public static bool IsValidShortPhoneNumber(this string shortPhoneNumber) => shortPhoneNumber != null && new RegularExpressionAttribute("^01[0125][0-9]{8}$").IsValid(shortPhoneNumber);
        public static bool IsValidThreeDigits(this string digits) => digits != null && new RegularExpressionAttribute("^[0-9]{1,3}$").IsValid(digits);
        public static bool IsValidFourDigits(this string digits) => digits != null && new RegularExpressionAttribute("^[0-9]{1,4}$").IsValid(digits);
        public static bool IsValidSixDigits(this string digits) => digits != null && new RegularExpressionAttribute("^[0-9]{1,6}$").IsValid(digits);
        public static bool IsValidSixteenDigits(this string digits) => digits != null && new RegularExpressionAttribute("^[0-9]{1,16}$").IsValid(digits);
        public static bool IsValidEnglishLetters(this string name) => name != null && new RegularExpressionAttribute("^[A-Za-z]+$").IsValid(name);
    }
}
