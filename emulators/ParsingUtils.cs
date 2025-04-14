using System;
using System.Text.RegularExpressions;

namespace Bheithir.Emulators
{
    public static class ParsingUtils
    {
        public static string RemoveBeforeSecondPipe(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            int firstPipe = input.IndexOf('|');
            if (firstPipe == -1) return input;

            int secondPipe = input.IndexOf('|', firstPipe + 1);
            if (secondPipe == -1) return input;

            int cutIndex = secondPipe + 1;
            if (cutIndex < input.Length && input[cutIndex] == ' ') cutIndex++;

            return input.Substring(cutIndex);
        }

        public static string RemoveAfter64Bit(string input)
        {
            // string marker = "(64-bit)";
            // int index = input.IndexOf(marker);
            // return index == -1 ? input : input.Substring(0, index).TrimEnd();
            string[] markers = { "(64-bit)", "(32-bit)" };

            foreach (var marker in markers)
            {
                int index = input.IndexOf(marker);
                if (index != -1)
                    return input.Substring(0, index).TrimEnd();
            }

            return input;
        }
        public static string RemoveFpsInParentheses(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return Regex.Replace(input, @"\s*\(([^)]*fps[^)]*)\)", "", RegexOptions.IgnoreCase).Trim();
        }

        public static string RemoveAfterSecondPipe(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            int firstPipe = input.IndexOf('|');
            if (firstPipe == -1) return input;

            int secondPipe = input.IndexOf('|', firstPipe + 1);
            if (secondPipe == -1) return input;

            int cutIndex = secondPipe;
            if (cutIndex > 0 && input[cutIndex - 1] == ' ') cutIndex--;

            return input.Substring(0, cutIndex).TrimEnd();
        }

        public static string RemoveParenthesesAndBrackets(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return Regex.Replace(input, @"\s*[\(\[].*?[\)\]]", "").Trim();
        }

        public static string RemoveBeforeDash(string input)
        {
            int dashIndex = input.IndexOf('-');
            if (dashIndex == -1) return input;
            int startIndex = dashIndex + 1;
            if (startIndex < input.Length && input[startIndex] == ' ') startIndex++;
            return input.Substring(startIndex);
        }

        public static string ParseTitle(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var parts = input.Split(new[] { " - " }, 2, StringSplitOptions.None);
            if (parts.Length != 2)
                return input;

            string title = parts[0].Trim();
            string subtitle = parts[1].Trim();

            string[] articles = { ", The", ", A", ", An" };
            foreach (var article in articles)
            {
                if (title.EndsWith(article))
                {
                    title = article.Replace(",", "").Trim() + " " + title.Substring(0, title.Length - article.Length).Trim();
                    break;
                }
            }

            return $"{title}: {subtitle}";
        }

        public static string RemoveBeforeColon(string input)
        {
            int colonIndex = input.IndexOf(':');
            if (colonIndex == -1) return input;
            int startIndex = colonIndex + 1;
            if (startIndex < input.Length && input[startIndex] == ' ') startIndex++;
            return input.Substring(startIndex);
        }
    }
}
