﻿using System;
using System.Collections.Generic;
using System.Text;
using SimpleLexer;
namespace SimpleLangParser
{
    public class ParserException : System.Exception
    {
        public ParserException(string msg) : base(msg)
        {
        }
    }

    public class Parser
    {
        private SimpleLexer.Lexer l;

        public Parser(SimpleLexer.Lexer lexer)
        {
            l = lexer;
        }

        public void Progr()
        {
            Block();
        }

        public void Expr() 
        {
            if (l.LexKind == Tok.ID || l.LexKind == Tok.INUM)
            {
                l.NextLexem();
            }
            else
            {
                SyntaxError("expression expected");
            }
        }

        public void Assign() 
        {
            l.NextLexem();  // пропуск id
            if (l.LexKind == Tok.ASSIGN)
            {
                l.NextLexem();
            }
            else {
                SyntaxError(":= expected");
            }
            Expr();
        }

        public void StatementList() 
        {
            Statement();
            while (l.LexKind == Tok.SEMICOLON)
            {
                l.NextLexem();
                Statement();
            }
        }

        public void Statement() 
        {
            switch (l.LexKind)
            {
                case Tok.BEGIN:
                    {
                        Block(); 
                        break;
                    }
                case Tok.CYCLE:
                    {
                        Cycle(); 
                        break;
                    }
                case Tok.WHILE:
                    {
                        While();
                        break;
                    }
                case Tok.FOR:
                    {
                        For();
                        break;
                    }
                case Tok.ID:
                    {
                        Assign();
                        break;
                    }
                default:
                    {
                        SyntaxError($"Operator expected, got \"{l.LexText}\" instead");
                        break;
                    }
            }
        }

        public void Block() 
        {
            l.NextLexem();    // пропуск begin
            StatementList();
            if (l.LexKind == Tok.END)
            {
                l.NextLexem();
            }
            else
            {
                SyntaxError("end expected");
            }

        }

        public void Cycle() 
        {
            l.NextLexem();  // пропуск cycle
            Expr();
            Statement();
        }

        public void While()
        {
            l.NextLexem();  // пропуск while
            Expr();
            Do();
            Statement();
        }

        public void Do()
        {
            if (l.LexKind == Tok.DO)
            {
                l.NextLexem();
            }
            else
            {
                SyntaxError($"do expected, got \"{l.LexText}\" instead");
            }
        }

        public void For()
        {
            l.NextLexem();  // пропуск for
            ID();
            Assign();
            To();
            Expr();
            Do();
            Statement();
        }

        public void ID()
        {
            if (l.LexKind != Tok.ID)
            {
                SyntaxError($"id expected, got \"{l.LexText}\" instead");
            }
        }

        public void To()
        {
            if (l.LexKind == Tok.TO)
            {
                l.NextLexem();
            } 
            else
            {
                SyntaxError($"to expected, got \"{l.LexText}\" instead");
            }
        }

        public void SyntaxError(string message) 
        {
            var errorMessage = "Syntax error in line " + l.LexRow.ToString() + ":\n";
            errorMessage += l.FinishCurrentLine() + "\n";
            errorMessage += new String(' ', l.LexCol - 1) + "^\n";
            if (message != "")
            {
                errorMessage += message;
            }
            throw new ParserException(errorMessage);
        }
   
    }
}
