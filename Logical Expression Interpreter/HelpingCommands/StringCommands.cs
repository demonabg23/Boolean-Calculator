using Logical_Expression_Interpreter.Structures;
using Logical_Expression_Interpreter.Structures.CustomStructures;

namespace Logical_Expression_Interpreter.HelpingCommands
{
    public class StringCommands
    {
        public int FindCharacter(string? input, char character)
        {
            for (var i = 0; i < input.Length; i++)
            {
                if (input[i] == character)
                    return i;
            }
            return -1;
        }

        public string? Substring(string? input, int start, int length)
        {
            var result = new char[length];
            for (var i = 0; i < length; i++)
                result[i] = input[start + i];
            return new string(result);
        }

        public string CharToString(char c)
        {
            var arr = new[] { c };
            return new string(arr);
        }

        public string? Trim(string? input)
        {
            var start = 0;
            var end = input.Length - 1;

            while (start <= end && input[start] == ' ')
                start++;
            while (end >= start && input[end] == ' ')
                end--;

            return Substring(input, start, end - start + 1);
        }



        public bool IsLetter(char c)
        {
            return c is >= 'a' and <= 'z' or >= 'A' and <= 'Z';
        }

        public bool IsDigit(char c)
        {
            return c is >= '0' and <= '9';
        }

        public bool IsQuoted(string? str)
        {
            if (str == null || str.Length < 2) return false;
            return str[0] == '"' && str[^1] == '"';
        }

        public string TrimQuotes(string? input)
        {
            if (!IsQuoted(input))
                ThrowError("Input string is not quoted.");

            return Substring(input, 1, input.Length - 2);
        }

        public string?[] SplitByComma(string? input)
        {
            if (input == null || input.Length == 0)
                ThrowError("Input cannot be null or empty.");

            string?[] result = new string[10];
            var count = 0;
            var start = 0;

            if (input != null)
                for (var i = 0; i <= input.Length; i++)
                {
                    if (i != input.Length && input[i] != ',') continue;
                    if (start == i)
                        ThrowError($"Unexpected comma at position {i}.");

                    var segment = Trim(Substring(input, start, i - start));
                    result[count++] = segment;
                    start = i + 1;
                }

            string?[] finalResult = new string[count];
            for (var i = 0; i < count; i++)
                finalResult[i] = result[i];

            return finalResult;
        }

        public CustomList<string?> SplitBySemicolon(string? input)
        {
            return SplitByDelimiter(input, ';');
        }

        public CustomList<string?> SplitByDelimiter(string? input, char delimiter)
        {
            var segments = new CustomList<string?>();
            var start = 0;

            for (var i = 0; i <= input.Length; i++)
            {
                if (i != input.Length && input[i] != delimiter) continue;
                var length = i - start;
                if (length > 0)
                {
                    segments.Add(Trim(Substring(input, start, length)));
                }
                start = i + 1;
            }
            return segments;
        }

        public int ParseInt(string? input)
        {
            if (string.IsNullOrEmpty(input))
                ThrowError("Input cannot be null or empty for parsing an integer.");

            var result = 0;
            var isNegative = false;
            var start = 0;

            if (input[0] == '-')
            {
                isNegative = true;
                start = 1;
            }

            for (var i = start; i < input.Length; i++)
            {
                var c = input[i];
                if (!IsDigit(c))
                    ThrowError($"Invalid character '{c}' in integer input.");

                result = result * 10 + (c - '0');
            }

            return isNegative ? -result : result;
        }

        public string Join(string delimiter, CustomList<string>? items)
        {
            if (items == null || items.Count == 0) return string.Empty;

            var result = new char[ComputeJoinLength(items, delimiter.Length)];
            var index = 0;

            for (var i = 0; i < items.Count; i++)
            {
                var current = items.Get(i);
                foreach (var t in current)
                {
                    result[index++] = t;
                }

                if (i >= items.Count - 1) continue;
                foreach (var t in delimiter)
                {
                    result[index++] = t;
                }
            }

            return new string(result);
        }

        private int ComputeJoinLength(CustomList<string> items, int delimiterLength)
        {
            var length = 0;
            for (var i = 0; i < items.Count; i++)
            {
                length += items.Get(i).Length;
            }

            return length + delimiterLength * (items.Count - 1);
        }

        public void ValidateName(string? name)
        {
            name = Trim(name); 

            if (name.Length == 0 || !IsLetter(name[0]))
                ThrowError($"Invalid name: {name}");

            for (var i = 1; i < name.Length; i++)
            {
                if (!IsLetter(name[i]) && !IsDigit(name[i]))
                    ThrowError($"Invalid character in name: {name}");
            }
        }

        public void SkipWhitespace(string? input, ref int position)
        {
            while (position < input.Length && input[position] == ' ')
            {
                position++;
            }
        }
        public bool IsNullOrWhiteSpace(string? input)
        {
            if (input == null) return true;

            foreach (var t in input)
            {
                if (t != ' ')
                    return false;
            }
            return true;
        }

        public bool StartsWithIgnoreCase(string? input, string? prefix)
        {
            if (input.Length < prefix.Length) return false;

            string lowerPrefix = ToLower(prefix);

            for (var i = 0; i < prefix.Length; i++)
            {
                if (ToLower(input[i]) != lowerPrefix[i])
                    return false;
            }
            return true;
        }


        public bool EqualsIgnoreCase(string? str1, string? str2)
        {
            if (str1 == null || str2 == null) return (str1 == str2);
            if (str1.Length != str2.Length) return false;

            for (var i = 0; i < str1.Length; i++)
            {
                if (ToLower(str1[i]) != ToLower(str2[i]))
                    return false;
            }
            return true;
        }

        public char ToLower(char c)
        {
            if (c is >= 'A' and <= 'Z')
                return (char)(c + 32);
            return c;
        }

        public string ToLower(string? input)
        {
            var result = new char[input.Length];
            for (var i = 0; i < input.Length; i++)
            {
                result[i] = ToLower(input[i]);
            }
            return new string(result);
        }

        public bool IsParameterOrFunction(string? token, string?[] parameters, FunctionTable functionTable)
        {
            foreach (var parameter in parameters)
            {
                if (parameter == token)
                {
                    return true;
                }
            }
            return functionTable.Contains(token);
        }

        public string?[] ReadFileLines(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                ThrowError("File path cannot be null or empty.");

            try
            {
                string?[] lines = File.ReadAllLines(filePath);
                var result = new string?[lines.Length];
                for (var i = 0; i < lines.Length; i++)
                {
                    result[i] = lines[i];
                }
                return result;
            }
            catch (Exception ex)
            {
                ThrowError($"Error reading file: {ex.Message}");
                return []; // Unreachable due to ThrowError, but added for safety.
            }
        }

        public void ThrowError(string message)
        {
            throw new InvalidOperationException(message);
        }

        public string GetErrorMessage(string? input, int position, string reason)
        {
            return position >= input.Length ? $"Error: Unexpected end of input. {reason}" : $"Error near '{input[position]}' at position {position}: {reason}";
        }
    }
}
