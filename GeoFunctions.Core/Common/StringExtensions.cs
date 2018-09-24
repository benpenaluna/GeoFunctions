﻿using System.Collections.Generic;
using System.Linq;

namespace GeoFunctions.Core.Common
{
    public static class StringExtensions
    {
        public static IEnumerable<FormatElementHelper> FindConsecutiveChars(this string testObject, char charToReplace)
        {
            var helpers = new List<FormatElementHelper>();

            var helper = new FormatElementHelper();

            var formatCharacters = testObject.ToCharArray();
            var charPreviouslyFound = false;
            for (var i = 0; i < formatCharacters.Length; i++)
            {
                if (ExaminingCharToReplace(charToReplace, formatCharacters, i))
                {
                    UpdateHelper('0', charToReplace, helper);
                    charPreviouslyFound = true;
                }
                else if (charPreviouslyFound && ExaminingFullStop(formatCharacters, i) && NextCharacterIsCharToReplace(charToReplace, formatCharacters, i))
                    UpdateHelper('.', '.', helper);
                else
                    charPreviouslyFound = false;

                if (HelpersNotReadyToBeUpdated(helper, formatCharacters, charPreviouslyFound, i))
                    continue;

                helpers.Add(helper);
                helper = new FormatElementHelper();
            }

            return helpers.OrderByDescending(x => x.StringReplacement.Length)
                          .ThenBy(x => x.StringReplacement.Contains('.'));
        }

        private static bool ExaminingCharToReplace(char charToReplace, IReadOnlyList<char> formatCharacters, int i)
        {
            return char.ToUpper(formatCharacters[i]) == charToReplace;
        }

        private static void UpdateHelper(char formatAddition, char replacementAddition, FormatElementHelper helper)
        {
            helper.FormatSpecifier += formatAddition;
            helper.StringReplacement += replacementAddition;
        }

        private static bool ExaminingFullStop(IReadOnlyList<char> formatCharacters, int i)
        {
            return char.ToUpper(formatCharacters[i]) == '.';
        }

        private static bool NextCharacterIsCharToReplace(char charToReplace, IReadOnlyList<char> formatCharacters, int i)
        {
            return i != formatCharacters.Count - 1 && char.ToUpper(formatCharacters[i + 1]) == charToReplace;
        }

        private static bool HelpersNotReadyToBeUpdated(FormatElementHelper helper, IReadOnlyCollection<char> formatCharacters, bool charPreviouslyFound, int i)
        {
            return helper.StringReplacement == null || (charPreviouslyFound && i != formatCharacters.Count - 1);
        }
    }
}
