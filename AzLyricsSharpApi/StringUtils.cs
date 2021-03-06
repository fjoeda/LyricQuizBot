﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzLyricsSharpApi
{
    public static class StringUtils
    {

        public static string RemoveHtmlTags(string text)
        {
            int idx = text.IndexOf('<');
            while (idx >= 0)
            {
                var endIdx = text.IndexOf('>', idx + 1);
                if (endIdx < idx)
                {
                    break;
                }
                text = text.Remove(idx, endIdx - idx + 1);
                idx = text.IndexOf('<', idx);
            }
            return text;
        }

        public static string StripWhiteSpaces(string input)
        {
            return input = input.Replace(" ", string.Empty);
        }

    }
}
