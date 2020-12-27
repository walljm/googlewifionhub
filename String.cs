/*
The MIT License (MIT)
Copyright (c) 2016 Jason Wall

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace onhub
{
    public static class Extensions
    {
        
        public static string If(this string s, Func<string, string[], string> work, params string[] parms)
        {
            return parms.Any(o => s.Contains(o)) ? work(s, parms) : string.Empty;
        }

        #region ParseToIndexOf

        public static string ParseToIndexOf(this string s, params string[] parms)
        {
            return s.ParseToIndexOf(false, parms);
        }

        /// <summary>
        ///   Returns a string from the begining to the index of one of the provided string parameters
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <param name="case_insensitive">Wheather to performs a case insensitive search or not</param>
        /// <param name="parms">The strings to parse to</param>
        /// <returns>Subsection of the string from begining to the index of the first matching parameter</returns>
        public static string ParseToIndexOf(this string s, bool case_insensitive = false, params string[] parms)
        {
            // return empty if string is empty
            if (s.Length == 0) return string.Empty;

            // return full string if no params were provided
            if (parms.Length == 0) return s;

            // lowercase the string if we're working with case insensitive search
            var str = case_insensitive ? s.ToLower() : s;

            var idx = -1;

            // look for earch parm, return first one found.
            foreach (var p in parms)
            {
                // get the index of our search parameter
                var tidx = IndexOf(str, p, case_insensitive);
                if (idx == -1)
                    idx = tidx;
                else if (idx > tidx && tidx > -1)
                    idx = tidx;
            }

            // if its greater than 0, return it.
            if (idx > -1)
            {
                return s.SafeSubstring(0, idx);
            }

            // worst case, return the string as is.
            return s;
        }

        #endregion ParseToIndexOf

        #region ParseToLastIndexOf

        public static string ParseToLastIndexOf(this string s, params string[] parms)
        {
            return s.ParseToLastIndexOf(false, parms);
        }

        /// <summary>
        ///   Returns a string from the begining to the last index of one of the provided string parameters
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <param name="case_insensitive">Wheather to performs a case insensitive search or not</param>
        /// <param name="parms">The strings to parse to</param>
        public static string ParseToLastIndexOf(this string s, bool case_insensitive = false, params string[] parms)
        {
            // return empty if string is empty
            if (s.Length == 0) return string.Empty;

            // return full string if no params were provided
            if (parms.Length == 0) return s;

            // lowercase the string if we're working with case insensitive search
            var str = case_insensitive ? s.ToLower() : s;

            var idx = -1;

            // look for earch parm, return first one found.
            foreach (var p in parms)
            {
                // get the index of our search parameter
                var tidx = IndexOf(str, p, case_insensitive, last: true);
                if (idx == -1)
                    idx = tidx;
                else if (idx < tidx && tidx > -1)
                    idx = tidx;
            }

            // if its greater than 0, return it.
            if (idx > -1)
            {
                return s.SafeSubstring(0, idx);
            }

            // worst case, return the string as is.
            return s;
        }

        #endregion ParseToLastIndexOf

        #region ParseToIndexOf_PlusLength

        public static string ParseToIndexOf_PlusLength(this string s, params string[] parms)
        {
            return s.ParseToIndexOf_PlusLength(false, parms);
        }

        /// <summary>
        ///   Returns a string from the begining to the index of one of the provided string parameters, plus the length of the found parameter
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <param name="case_insensitive">Wheather to performs a case insensitive search or not</param>
        /// <param name="parms">The strings to parse to</param>
        /// <returns>Subsection of the string from begining to the index of the first matching parameter</returns>
        public static string ParseToIndexOf_PlusLength(this string s, bool case_insensitive = false, params string[] parms)
        {
            // return empty if string is empty
            if (s.Length == 0) return string.Empty;

            // return full string if no params were provided
            if (parms.Length == 0) return s;

            // lowercase the string if we're working with case insensitive search
            var str = case_insensitive ? s.ToLower() : s;

            var idx = -1;
            var l = 0;

            // look for earch parm, return first one found.
            foreach (var p in parms)
            {
                // get the index of our search parameter
                var tidx = IndexOf(str, p, case_insensitive);
                if (idx == -1)
                {
                    idx = tidx;
                    l = p.Length;
                }
                else if (idx > tidx && tidx > -1)
                {
                    idx = tidx;
                    l = p.Length;
                }
            }

            // if its greater than 0, return it.
            if (idx > -1)
            {
                return s.SafeSubstring(0, idx + l);
            }

            // worst case, return the string as is.
            return s;
        }

        #endregion ParseToIndexOf_PlusLength

        #region ParseToLastIndexOf_PlusLength

        public static string ParseToLastIndexOf_PlusLength(this string s, params string[] parms)
        {
            return s.ParseToLastIndexOf_PlusLength(false, parms);
        }

        /// <summary>
        ///   Returns a string from the begining to the last index of one of the provided string parameters, plus the length of the found parameter
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <param name="case_insensitive">Wheather to performs a case insensitive search or not</param>
        /// <param name="parms">The strings to parse to</param>
        public static string ParseToLastIndexOf_PlusLength(this string s, bool case_insensitive = false, params string[] parms)
        {
            // return empty if string is empty
            if (s.Length == 0) return string.Empty;

            // return full string if no params were provided
            if (parms.Length == 0) return s;

            // lowercase the string if we're working with case insensitive search
            var str = case_insensitive ? s.ToLower() : s;

            var idx = -1;
            var l = 0;

            // look for earch parm, return first one found.
            foreach (var p in parms)
            {
                // get the index of our search parameter
                var tidx = IndexOf(str, p, case_insensitive, last: true);
                if (idx == -1)
                {
                    idx = tidx;
                    l = p.Length;
                }
                else if (idx < tidx && tidx > -1)
                {
                    idx = tidx;
                    l = p.Length;
                }
            }

            // if its greater than 0, return it.
            if (idx > -1)
            {
                return s.SafeSubstring(0, idx + l);
            }

            // worst case, return the string as is.
            return s;
        }

        #endregion ParseToLastIndexOf_PlusLength

        #region ParseToIndexOf_PlusLength

        public static string ParseAfterIndexOf_PlusLength(this string s, params string[] parms)
        {
            return s.ParseAfterIndexOf_PlusLength(false, parms);
        }

        /// <summary>
        ///   Returns a string starting after the first provided string parameter discovered to the end of the string
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <param name="case_insensitive">Wheather to performs a case insensitive search or not</param>
        /// <param name="parms">The strings to parse to</param>
        public static string ParseAfterIndexOf_PlusLength(this string s, bool case_insensitive = false, params string[] parms)
        {
            // return empty if string is empty
            if (s.Length == 0) return string.Empty;

            // return full string if no params were provided
            if (parms.Length == 0) return s;

            // lowercase the string if we're working with case insensitive search
            var str = case_insensitive ? s.ToLower() : s;

            var idx = -1;
            var l = 0;

            // look for earch parm, return first one found.
            foreach (var p in parms)
            {
                // get the index of our search parameter
                var tidx = IndexOf(str, p, case_insensitive);
                if (idx == -1)
                {
                    idx = tidx;
                    l = p.Length;
                }
                else if (idx > tidx && tidx > -1)
                {
                    idx = tidx;
                    l = p.Length;
                }
            }

            // if its greater than 0, return it.
            if (idx > -1)
            {
                return s.SafeSubstring(idx + l);
            }

            // worst case, return the string as is.
            return s;
        }

        #endregion ParseToIndexOf_PlusLength

        #region ParseAfterLastIndexOf_PlusLength

        public static string ParseAfterLastIndexOf_PlusLength(this string s, params string[] parms)
        {
            return s.ParseAfterLastIndexOf_PlusLength(false, parms);
        }

        /// <summary>
        ///   Returns a string starting after the last index of the first provided string parameter discovered to the end of the string
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <param name="case_insensitive">Wheather to performs a case insensitive search or not</param>
        /// <param name="parms">The strings to parse to</param>
        public static string ParseAfterLastIndexOf_PlusLength(this string s, bool case_insensitive = false, params string[] parms)
        {
            // return empty if string is empty
            if (s.Length == 0) return string.Empty;

            // return full string if no params were provided
            if (parms.Length == 0) return s;

            // lowercase the string if we're working with case insensitive search
            var str = case_insensitive ? s.ToLower() : s;

            var idx = -1;
            var l = 0;

            // look for earch parm, return first one found.
            foreach (var p in parms)
            {
                // get the index of our search parameter
                var tidx = IndexOf(str, p, case_insensitive, last: true);
                if (idx == -1)
                {
                    idx = tidx;
                    l = p.Length;
                }
                else if (idx < tidx && tidx > -1)
                {
                    idx = tidx;
                    l = p.Length;
                }
            }

            // if its greater than 0, return it.
            if (idx > -1)
            {
                return s.SafeSubstring(idx + l);
            }

            // worst case, return the string as is.
            return s;
        }

        #endregion ParseAfterLastIndexOf_PlusLength

        #region ParseAfter/To DesignatedIndexOf_PlusLength

        /// <summary>
        ///   Returns a string from the begining to the last index of one of the provided string parameters, plus the length of the found parameter
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <param name="qty">The number of the search patter to parse after</param>
        /// <param name="case_insensitive">Wheather to performs a case insensitive search or not</param>
        /// <param name="parms">The strings to parse to</param>
        public static string ParseToDesignatedIndexOf_PlusLength(this string s, int qty, bool case_insensitive, params string[] parms)
        {
            var r = string.Empty;
            var v = s;

            for (var i = 0; i < qty; i++)
            {
                r += v.ParseToIndexOf_PlusLength(case_insensitive, parms);
                v = v.ParseAfterIndexOf_PlusLength(case_insensitive, parms);
            }

            // worst case, return the string as is.
            return r;
        }

        /// <summary>
        ///   Returns a string starting after the last index of the first provided string parameter discovered to the end of the string
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <param name="qty">The number of the search patter to parse after</param>
        /// <param name="case_insensitive">Wheather to performs a case insensitive search or not</param>
        /// <param name="parms">The strings to parse to</param>
        public static string ParseAfterDesignatedIndexOf_PlusLength(this string s, int qty, bool case_insensitive = false, params string[] parms)
        {
            var r = s;

            for (var i = 0; i < qty; i++)
            {
                r = r.ParseAfterIndexOf_PlusLength(case_insensitive, parms);
            }

            // worst case, return the string as is.
            return r;
        }

        #endregion ParseAfter/To DesignatedIndexOf_PlusLength

        #region ParseAfterLastIndexOf

        public static string ParseAfterLastIndexOf(this string s, params string[] parms)
        {
            return s.ParseAfterLastIndexOf(false, parms);
        }

        /// <summary>
        ///   Returns a string starting after the last index of the first provided string parameter discovered to the end of the string
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <param name="case_insensitive">Wheather to performs a case insensitive search or not</param>
        /// <param name="parms">The strings to parse to</param>
        public static string ParseAfterLastIndexOf(this string s, bool case_insensitive = false, params string[] parms)
        {
            // return empty if string is empty
            if (s.Length == 0) return string.Empty;

            // return full string if no params were provided
            if (parms.Length == 0) return s;

            // lowercase the string if we're working with case insensitive search
            var str = case_insensitive ? s.ToLower() : s;

            var idx = -1;

            // look for earch parm, return first one found.
            foreach (var p in parms)
            {
                // get the index of our search parameter
                var tidx = IndexOf(str, p, case_insensitive, last: true);
                if (idx == -1)
                    idx = tidx;
                else if (idx < tidx && tidx > -1)
                    idx = tidx;
            }

            // if its greater than 0, return it.
            if (idx > -1)
            {
                return s.SafeSubstring(idx);
            }

            // worst case, return the string as is.
            return s;
        }

        #endregion ParseAfterLastIndexOf

        #region ParseAfterLastIndexOf

        public static string ParseAfterIndexOf(this string s, params string[] parms)
        {
            return s.ParseAfterIndexOf(false, parms);
        }

        /// <summary>
        ///   Returns a string starting after the last index of the first provided string parameter discovered to the end of the string
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <param name="case_insensitive">Wheather to performs a case insensitive search or not</param>
        /// <param name="parms">The strings to parse to</param>
        public static string ParseAfterIndexOf(this string s, bool case_insensitive = false, params string[] parms)
        {
            // return empty if string is empty
            if (s.Length == 0) return string.Empty;

            // return full string if no params were provided
            if (parms.Length == 0) return s;

            // lowercase the string if we're working with case insensitive search
            var str = case_insensitive ? s.ToLower() : s;

            var idx = -1;

            // look for earch parm, return first one found.
            foreach (var p in parms)
            {
                // get the index of our search parameter
                var tidx = IndexOf(str, p, case_insensitive);
                if (idx == -1)
                    idx = tidx;
                else if (idx > tidx && tidx > -1)
                    idx = tidx;
            }

            // if its greater than 0, return it.
            if (idx > -1)
            {
                return s.SafeSubstring(idx);
            }

            // worst case, return the string as is.
            return s;
        }

        #endregion ParseAfterLastIndexOf

        /// <summary>
        /// Finds the index of a search string.  Optionally allows case insensitivity or option to search for last index.
        /// </summary>
        /// <param name="str">String to search</param>
        /// <param name="search_string">String to search for</param>
        /// <param name="case_insensitive">Ignore case</param>
        /// <param name="last">Search for last instance</param>
        public static int IndexOf(this string str, string search_string, bool case_insensitive, bool last = false)
        {
            // get the index of our search parameter
            var idx = -1;
            if (case_insensitive)
                idx = last ? str.LastIndexOf(search_string.ToLower(), StringComparison.Ordinal) : str.IndexOf(search_string.ToLower(), StringComparison.Ordinal);
            else
                idx = last ? str.LastIndexOf(search_string, StringComparison.Ordinal) : str.IndexOf(search_string, StringComparison.Ordinal);
            return idx;
        }

        /// <summary>
        /// Finds the index of a search string.  Optionally allows case insensitivity or option to search for specific index.
        /// </summary>
        /// <param name="str">String to search</param>
        /// <param name="search_string">String to search for</param>
        /// <param name="case_insensitive">Ignore case</param>
        /// <param name="qty">Search for specific instance</param>
        public static int IndexOf(this string str, string search_string, bool case_insensitive, int qty)
        {
            // get the index of our search parameter
            var idx = -1;
            if (case_insensitive)
            {
                var c = 0;
                while (c != qty)
                {
                    idx = str.IndexOf(search_string.ToLower(), idx < 0 ? 0 : idx + search_string.Length, StringComparison.Ordinal);
                    c++;
                }
            }
            else
            {
                var c = 0;
                while (c != qty)
                {
                    idx = str.IndexOf(search_string, idx < 0 ? 0 : idx + search_string.Length, StringComparison.Ordinal);
                    c++;
                }
            }
            return idx;
        }

        /// <summary>
        ///   Safely handles the cases where your start exceeds the length of the string, or your start+length exceeds the length of the string.
        /// </summary>
        /// <param name="s">The string to do the substring on</param>
        /// <param name="start">The index of the string to start parsing from</param>
        /// <param name="length">The number of characters to return</param>
        /// <returns>A subsection of the provided string</returns>
        public static string SafeSubstring(this string s, int start, int? length = null)
        {
            // return empty if the requested start index is past the length of the provided string
            if (start > s.Length) return string.Empty;

            // if length isn't provided, return without it.
            if (length == null) return s.Substring(start);

            // if the length is 0 or less, the string should be empty.
            if (length < 1) return string.Empty;

            var len = (int)length;
            if (start + length > s.Length)
            {
                len = s.Length - start;
            }

            return s.Substring(start, len);
        }

        /// <summary>
        ///  Determines if the string is either null or has no characters
        /// </summary>
        /// <param name="str">String to evaluate</param>
        /// <returns>True or False</returns>
        public static bool IsEmpty(this string str)
        {
            return str == null || str.Length == 0;
        }

        /// <summary>
        ///  Determines if the string is both not null and has at least 1 character.
        /// </summary>
        /// <param name="str">String to evaluate</param>
        /// <returns>True or False</returns>
        public static bool IsNotEmpty(this string str)
        {
            return !str.IsEmpty();
        }

        /// <summary>
        /// Searches a string for a given <see cref="char"/>
        /// </summary>
        /// <param name="str">String to search</param>
        /// <param name="c"><see cref="char"/> to find</param>
        public static bool ContainsFast(this string str, char c)
        {
            for (var i = 0; i < str.Length; i++)
                if (str[i] == c) return true;

            return false;
        }

        /// <summary>
        /// Converts a string to PascalCase.  Removes any illegal file name characters.
        /// </summary>
        /// <param name="str">string to convert</param>
        public static string ToPascalCase(this string str)
        {
            var v = str;
            foreach (var c in Path.GetInvalidFileNameChars())
                v = v.Replace(c.ToString(), string.Empty);

            foreach (var c in Path.GetInvalidFileNameChars())
                v = v.Replace(c.ToString(), string.Empty);

            var words = v.Split(' ');
            var r = string.Empty;
            foreach (var w in words)
                r += w.Substring(0, 1).ToUpper() + w.Substring(1);
            return r;
        }

        /// <summary>
        /// Counts the numbers of instances of the <paramref name="c"/> param in the <paramref
        /// name="test"/> string.
        /// </summary>
        /// <param name="test">string to search</param>
        /// <param name="c">char to search for</param>
        /// <param name="respect_word_boundaries">doesn't count "port" the same as "portindex"</param>
        /// <returns>int</returns>
        public static int CountInstances(this string test, string c, bool respect_word_boundaries = false)
        {
            var s = test + " "; // to avoid geriatrics with strings not ending in a word boundary.
            var cnt = 0;
            if (respect_word_boundaries) c = c + " ";
            while (s.IndexOf(c, StringComparison.Ordinal) != -1)
            {
                s = s.Substring(s.IndexOf(c, StringComparison.Ordinal) + c.Length);
                cnt++;
            }
            return cnt;
        }

        public static Dictionary<char, int> GetCharHistogram(this string line, bool collapse_whitespace = true)
        {
            var hist = new Dictionary<char, int>();

            var prev_char_is_ws = false;

            foreach (var c in line)
            {
                var my_char = c;
                if (collapse_whitespace && char.IsWhiteSpace(c))
                {
                    my_char = ' ';
                    if (prev_char_is_ws)
                        continue;
                }

                // Only the first whitespace counts we ignore the rest
                if (hist.ContainsKey(my_char))
                    hist[my_char]++;
                else
                    hist[my_char] = 1;

                prev_char_is_ws = char.IsWhiteSpace(c);
            }

            return hist;
        }

        public static IEnumerable<string> ToLines(this string s)
        {
            using (var sr = new StringReader(s))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}