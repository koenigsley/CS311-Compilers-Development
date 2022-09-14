using System;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lexer
{

    public class LexerException : Exception
    {
        public LexerException(string msg)
            : base(msg)
        {
        }
    }

    public class Lexer
    {
        protected int position;
        protected char currentCh; // очередной считанный символ
        protected int currentCharValue; // целое значение очередного считанного символа
        protected System.IO.StringReader inputReader;
        protected string inputString;

        public Lexer(string input)
        {
            inputReader = new System.IO.StringReader(input);
            inputString = input;
        }

        public void Error()
        {
            var o = new StringBuilder();
            o.Append(inputString + '\n');
            o.Append(new System.String(' ', position - 1) + "^\n");
            o.AppendFormat("Error in symbol {0}", currentCh);
            throw new LexerException(o.ToString());
        }

        protected void NextCh()
        {
            currentCharValue = inputReader.Read();
            currentCh = (char)currentCharValue;
            position += 1;
        }

        public virtual bool Parse()
        {
            return true;
        }

        protected void EnsureCompletelyParsed()
        {
            if (currentCharValue != -1)
            {
                Error();
            }
        }
    }

    public class IntLexer : Lexer
    {
        protected StringBuilder intString;
        public int parseResult = 0;
        protected bool negative = false;
        private int currentInt
        {
            get { return currentCh - '0'; }
        }

        public IntLexer(string input) : base(input)
        {
            intString = new StringBuilder();
        }

        public override bool Parse()
        {
            ParseSign();
            ParseFirstDigit();
            ParseRestDigits();
            EnsureCompletelyParsed();
            NegateIfNeeded();
            return true;
        }

        protected void ParseSign()
        {
            NextCh();
            if (currentCh == '+' || currentCh == '-')
            {
                negative = currentCh == '-';
                NextCh();
            }
        }

        protected virtual void ParseFirstDigit()
        {
            if (char.IsDigit(currentCh))
            {
                parseResult = currentInt;
            }
            else
            {
                Error();
            }
        }

        protected void ParseRestDigits()
        {
            NextCh();
            while (char.IsDigit(currentCh))
            {
                parseResult = parseResult * 10 + currentInt;
                NextCh();
            }
        }

        protected void NegateIfNeeded()
        {
            if (negative)
            {
                parseResult = -parseResult;
            }
        }
    }
    
    public class IdentLexer : Lexer
    {
        private string parseResult;
        protected StringBuilder builder;
    
        public string ParseResult
        {
            get { return parseResult; }
        }
    
        public IdentLexer(string input) : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        { 
            NextCh();
            if (IsUnderscoreOrLetter(currentCh))
            {
                builder.Append(currentCh);
                NextCh();
            }
            else
            {
                Error();
            }

            while (IsUnderscoreOrLetterOrDigit(currentCh))
            {
                builder.Append(currentCh);
                NextCh();
            }

            EnsureCompletelyParsed();

            parseResult = builder.ToString();

            return true;
        }

        private static bool IsUnderscoreOrLetter(char c)
        {
            return c == '_' || char.IsLetter(c);
        }

        private static bool IsUnderscoreOrLetterOrDigit(char c)
        {
            return IsUnderscoreOrLetter(c) || char.IsDigit(c);
        }
    }

    public class IntNoZeroLexer : IntLexer
    {
        public IntNoZeroLexer(string input) : base(input)
        {
        }

        protected override void ParseFirstDigit()
        {
            base.ParseFirstDigit();
            if (currentCh == '0')
            {
                Error();
            }
        }
    }

    public class LetterDigitLexer : Lexer
    {
        protected StringBuilder builder;
        protected string parseResult;
        private bool takeLetter = true;

        public string ParseResult
        {
            get { return parseResult; }
        }

        public LetterDigitLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();

            while (char.IsLetterOrDigit(currentCh))
            {
                if (IsCurrentCharValid())
                {
                    builder.Append(currentCh);
                    NextCh();
                }
                else
                {
                    Error();
                }
                takeLetter = !takeLetter;
            }

            EnsureCompletelyParsed();

            parseResult = builder.ToString();

            return true;
        }

        private bool IsCurrentCharValid()
        {
            return takeLetter ? char.IsLetter(currentCh) : char.IsDigit(currentCh);
        }
    }

    public class LetterListLexer : Lexer
    {
        protected List<char> parseResult;
        public List<char> ParseResult
        {
            get { return parseResult; }
        }
        private bool takeLetter = true;

        public LetterListLexer(string input)
            : base(input)
        {
            parseResult = new List<char>();
        }

        public override bool Parse()
        {
            NextCh();

            while(IsLetterOrDelimiter(currentCh))
            {
                if (IsCurrentCharValid())
                {
                    if (char.IsLetter(currentCh))
                    {
                        parseResult.Add(currentCh);
                    }
                    NextCh();
                }
                else
                {
                    Error();
                }
                takeLetter = !takeLetter;
            }

            EnsureCompletelyParsed();

            return true;
        }

        protected new void EnsureCompletelyParsed()
        {
            base.EnsureCompletelyParsed();
            if (takeLetter)
            {
                Error();
            }
        }

        private static bool IsLetterOrDelimiter(char c)
        {
            return char.IsLetter(c) || IsDelimiter(c);
        }

        private bool IsCurrentCharValid()
        {
            return takeLetter ? char.IsLetter(currentCh) : IsDelimiter(currentCh);
        }

        private static bool IsDelimiter(char c)
        {
            return c == ',' || c == ';';
        }
    }

    public class DigitListLexer : Lexer
    {
        protected List<int> parseResult;
        private bool digitTaken = true;
        private int currentInt
        {
            get { return currentCh - '0'; }
        }

        public List<int> ParseResult
        {
            get { return parseResult; }
        }

        public DigitListLexer(string input) : base(input)
        {
            parseResult = new List<int>();
        }

        public override bool Parse()
        {
            ParseFirstChar();
            ParseRestChars();
            EnsureCompletelyParsed();
            return true;
        }

        protected void ParseFirstChar()
        {
            NextCh();
            if (char.IsDigit(currentCh))
            {
                parseResult.Add(currentInt);
                digitTaken = true;
                NextCh();
            }
            else
            {
                Error();
            }
        }

        protected void ParseRestChars()
        {
            while (char.IsDigit(currentCh) || char.IsWhiteSpace(currentCh))
            {
                if (char.IsWhiteSpace(currentCh))
                {
                    digitTaken = false;
                }
                else
                {
                    if (digitTaken)
                    {
                        Error();
                    }

                    parseResult.Add(currentInt);
                    digitTaken = true;
                }
                NextCh();
            }
        }

        protected new void EnsureCompletelyParsed()
        {
            base.EnsureCompletelyParsed();
            if (!digitTaken)
            {
                Error();
            }
        }
    }

    public class LetterDigitGroupLexer : Lexer
    {
        protected StringBuilder builder;
        protected string parseResult;

        public string ParseResult
        {
            get { return parseResult; }
        }
        
        public LetterDigitGroupLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            throw new NotImplementedException();
        }
       
    }

    public class DoubleLexer : Lexer
    {
        private StringBuilder builder;
        private double parseResult;

        public double ParseResult
        {
            get { return parseResult; }

        }

        public DoubleLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            throw new NotImplementedException();
        }
       
    }

    public class StringLexer : Lexer
    {
        private StringBuilder builder;
        private string parseResult;

        public string ParseResult
        {
            get { return parseResult; }

        }

        public StringLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            throw new NotImplementedException();
        }
    }

    public class CommentLexer : Lexer
    {
        private StringBuilder builder;
        private string parseResult;

        public string ParseResult
        {
            get { return parseResult; }

        }

        public CommentLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            throw new NotImplementedException();
        }
    }

    public class IdentChainLexer : Lexer
    {
        private StringBuilder builder;
        private List<string> parseResult;

        public List<string> ParseResult
        {
            get { return parseResult; }

        }

        public IdentChainLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
            parseResult = new List<string>();
        }

        public override bool Parse()
        {
            throw new NotImplementedException();
        }
    }

    public class Program
    {
        public static void Main()
        {
            string input = "154216";
            Lexer L = new IntLexer(input);
            try
            {
                L.Parse();
            }
            catch (LexerException e)
            {
                System.Console.WriteLine(e.Message);
            }

        }
    }
}