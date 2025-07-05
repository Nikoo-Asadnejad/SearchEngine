using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SearchEngine.Domain;

internal static class TagGenerator
{
    private static readonly Dictionary<char, char> PersianToEnglishKeyboard = new()
    {
        ['ض'] = 'q', ['ص'] = 'w', ['ث'] = 'e', ['ق'] = 'r', ['ف'] = 't', ['غ'] = 'y', ['ع'] = 'u', ['ه'] = 'i', ['خ'] = 'o', ['ح'] = 'p',
        ['ش'] = 'a', ['س'] = 's', ['ی'] = 'd', ['ب'] = 'f', ['ل'] = 'g', ['ا'] = 'h', ['ت'] = 'j', ['ن'] = 'k', ['م'] = 'l',
        ['ظ'] = 'z', ['ط'] = 'x', ['ز'] = 'c', ['ر'] = 'v', ['ذ'] = 'b', ['د'] = 'n', ['پ'] = 'm', ['ژ'] = '[',
    };

    private static readonly Dictionary<char, char[]> SoundAlikeCharGroups = new()
    {
        ['س'] = new[] { 'س', 'ص', 'ث' },
        ['ص'] = new[] { 'س', 'ص', 'ث' },
        ['ث'] = new[] { 'س', 'ص', 'ث' },

        ['ت'] = new[] { 'ت', 'ط' },
        ['ط'] = new[] { 'ت', 'ط' },

        ['ز'] = new[] { 'ز', 'ذ', 'ض', 'ظ' },
        ['ذ'] = new[] { 'ز', 'ذ', 'ض', 'ظ' },
        ['ض'] = new[] { 'ز', 'ذ', 'ض', 'ظ' },
        ['ظ'] = new[] { 'ز', 'ذ', 'ض', 'ظ' },

        ['ح'] = new[] { 'ح', 'ه' },
        ['ه'] = new[] { 'ح', 'ه' },

        ['ق'] = new[] { 'ق', 'غ' },
        ['غ'] = new[] { 'ق', 'غ' },
        
        ['آ'] = new []{'ا'},
        ['ا'] = new []{'آ'},
    };

    private static readonly Dictionary<char, char> EnglishToPersianKeyboard = new()
    {
        ['q'] = 'ض',
        ['w'] = 'ص',
        ['e'] = 'ث',
        ['r'] = 'ق',
        ['t'] = 'ف',
        ['y'] = 'غ',
        ['u'] = 'ع',
        ['i'] = 'ه',
        ['o'] = 'خ',
        ['p'] = 'ح',

        ['a'] = 'ش',
        ['s'] = 'س',
        ['d'] = 'ی',
        ['f'] = 'ب',
        ['g'] = 'ل',
        ['h'] = 'ا',
        ['j'] = 'ت',
        ['k'] = 'ن',
        ['l'] = 'م',

        ['z'] = 'ظ',
        ['x'] = 'ط',
        ['c'] = 'ز',
        ['v'] = 'ر',
        ['b'] = 'ذ',
        ['n'] = 'د',
        ['m'] = 'پ',

        ['[' ] = 'ژ',
        [']' ] = ']',
        ['\\'] = '\\',

        [' '] = ' ' 
    };
    public static string GenerateNormalizedTag(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

        var normalized = NormalizeText(input);

        return normalized;
    }

    public static string GenerateReversedKeyboardTag(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

        var reversed = new string(input.Select(c =>
            PersianToEnglishKeyboard.TryGetValue(c, out var mapped) ? mapped : c
        ).ToArray());

        return reversed;
    }
    
    public static string ConvertToPersianKeyboard(string englishInput)
    {
        if (string.IsNullOrWhiteSpace(englishInput)) return englishInput;

        var result = new StringBuilder();

        foreach (var c in englishInput.ToLowerInvariant())
        {
            result.Append(EnglishToPersianKeyboard.TryGetValue(c, out var mapped)
                ? mapped
                : c); 
        }

        return result.ToString();
    }

    public static HashSet<string> GeneratePhoneticDictationVariants(string word)
    {
        var results = new HashSet<string>();
        if (string.IsNullOrWhiteSpace(word)) return results;

        void GenerateVariants(char[] current, int index)
        {
            if (index >= current.Length)
            {
                results.Add(new string(current));
                return;
            }

            char originalChar = current[index];

            if (SoundAlikeCharGroups.TryGetValue(originalChar, out var replacements))
            {
                foreach (char replacement in replacements)
                {
                    var temp = (char[])current.Clone();
                    temp[index] = replacement;
                    GenerateVariants(temp, index + 1);
                }
            }
            else
            {
                GenerateVariants(current, index + 1);
            }
        }

        GenerateVariants(word.ToCharArray(), 0);
        return results;
    }

    public static HashSet<string> GenerateTransliterationTags(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return [];

        var reversed = GenerateReversedKeyboardTag(input);
        var persianReversed = ConvertToPersianKeyboard(input);
        var normalized = GenerateNormalizedTag(input);
        return [reversed ,normalized ,persianReversed];
    }
    
    private static string NormalizeText(string input)
    {
        input = input.ToLowerInvariant();
        input = input.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (char c in input)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark &&
                !char.IsPunctuation(c) &&
                !char.IsSymbol(c))
            {
                builder.Append(c);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static HashSet<string> Tokenize(string text)
    {
        return Regex.Split(text, @"[\s\-_,]+")
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .ToHashSet();
    }
}
