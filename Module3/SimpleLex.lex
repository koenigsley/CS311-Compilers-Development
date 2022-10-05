%using ScannerHelper;
%using System.Collections.Generic;
%namespace SimpleScanner

Alpha 	[a-zA-Z_]
Digit   [0-9] 
AlphaDigit {Alpha}|{Digit}
INTNUM  {Digit}+
REALNUM {INTNUM}\.{INTNUM}
ID {Alpha}{AlphaDigit}*
DotChr [^\r\n]
SingleCmnt \/\/{DotChr}*
STRINGAP \'[^']*\'

%x COMMENT

// «десь можно делать описани€ типов, переменных и методов - они попадают в класс Scanner
%{
  public int LexValueInt;
  public double LexValueDouble;
  public List<string> idsInComment = new List<string>();
%}

%%
{INTNUM} {
  LexValueInt = int.Parse(yytext);
  return (int)Tok.INUM;
}

{REALNUM} {
  LexValueDouble = double.Parse(yytext);
  return (int)Tok.RNUM;
}

begin {
  return (int)Tok.BEGIN;
}

end {
  return (int)Tok.END;
}

cycle {
  return (int)Tok.CYCLE;
}

{ID} {
  return (int)Tok.ID;
}

":" {
  return (int)Tok.COLON;
}

":=" {
  return (int)Tok.ASSIGN;
}

";" {
  return (int)Tok.SEMICOLON;
}

{SingleCmnt} {
	return (int)Tok.COMMENT;
}

{STRINGAP} {
	return (int)Tok.STRINGAP;
}

"{" { 
    // переход в состо€ние COMMENT
    BEGIN(COMMENT);
}

<COMMENT> begin {
}

<COMMENT> end {
}

<COMMENT> cycle {
}

<COMMENT> {ID} {
    idsInComment.Add(yytext);
}


<COMMENT> "}" { 
    // переход в состо€ние INITIAL
    BEGIN(INITIAL);
	return (int)Tok.LONGCOMMENT;
}

[^ \r\n] {
	LexError();
	return 0; // конец разбора
}


%%

// «десь можно делать описани€ переменных и методов - они тоже попадают в класс Scanner

public void LexError()
{
	Console.WriteLine("({0},{1}): Ќеизвестный символ {2}", yyline, yycol, yytext);
}

public string TokToString(Tok tok)
{
	switch (tok)
	{
		case Tok.ID:
			return tok + " " + yytext;
		case Tok.INUM:
			return tok + " " + LexValueInt;
		case Tok.RNUM:
			return tok + " " + LexValueDouble;
		default:
			return tok + "";
	}
}

