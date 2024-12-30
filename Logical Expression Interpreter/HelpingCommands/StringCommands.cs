using Logical_Expression_Interpreter.Structures;

namespace Logical_Expression_Interpreter.HelpingCommands
{
    public class StringCommands
    {
        public int FindCharacter(string input, char character)
        {
            for (var i = 0; i < input.Length; i++)
            {
                if (input[i] == character)
                    return i;
            }
            return -1;
        }

        public string Substring(string input, int start, int length)
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

        public string Trim(string input)
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

        public string[] SplitByComma(string input)
        {
            var result = new string[10];
            var count = 0;

            var start = 0;
            for (int i = 0; i <= input.Length; i++)
            {
                if (i == input.Length || input[i] == ',')
                {
                    var segment = Trim(Substring(input, start, i - start));
                    result[count++] = segment;
                    start = i + 1;
                }
            }

            var finalResult = new string[count];
            for (var i = 0; i < count; i++)
                finalResult[i] = result[i];
            return finalResult;
        }

        public void ValidateName(string name)
        {
            if (name.Length == 0 || !IsLetter(name[0]))
                ThrowError($"Invalid name: {name}");
            for (var i = 1; i < name.Length; i++)
            {
                if (!IsLetter(name[i]) && !IsDigit(name[i]))
                    ThrowError($"Invalid character in name: {name}");
            }
        }

        public void SkipWhitespace(string input, ref int position)
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

        public bool StartsWithIgnoreCase(string input, string prefix)
        {
            if (input.Length < prefix.Length) return false;

            for (var i = 0; i < prefix.Length; i++)
            {
                char c1 = ToLower(input[i]);
                char c2 = ToLower(prefix[i]);
                if (c1 != c2)
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

        public bool IsParameterOrFunction(string token, string[] parameters, FunctionTable functionTable)
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

        public void ThrowError(string message)
        {
            throw new InvalidOperationException(message);
        }

        public string GetErrorMessage(string input, int position, string reason)
        {
            return position >= input.Length ? $"Error: Unexpected end of input. {reason}" : $"Error near '{input[position]}' at position {position}: {reason}";
        }
    }
}
