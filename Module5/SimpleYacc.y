%{
// Ёти объ€влени€ добавл€ютс€ в класс GPPGParser, представл€ющий собой парсер, генерируемый системой gppg
    public Parser(AbstractScanner<int, LexLocation> scanner) : base(scanner) { }
%}

%output = SimpleYacc.cs

%namespace SimpleParser

%token BEGIN END CYCLE INUM RNUM ID ASSIGN SEMICOLON WHILE DO REPEAT UNTIL FOR TO IF THEN ELSE WRITE LPAR RPAR VAR COMMA PLUS MINUS MUL DIV

%%

progr   : block
		;

stlist	: statement 
		| stlist SEMICOLON statement
		;

statement: assign
		| block  
		| cycle
		| while
		| repeat
		| for
		| if
		| write
		| var
		;

idlist  : ident
        | idlist COMMA ident
		;

ident 	: ID 
		;

var     : VAR idlist
        ;
	
assign 	: ident ASSIGN expr 
		;

expr    : expr2
        | expr PLUS expr2
        | expr MINUS expr2
        ;

expr2   : expr3
        | expr2 MUL expr3
        | expr2 DIV expr3
        ;

expr3   : ident
        | INUM 
        | LPAR expr RPAR
        ;

block	: BEGIN stlist END 
		;

cycle	: CYCLE expr statement 
		;

while   : WHILE expr DO statement
        ;

repeat  : REPEAT stlist UNTIL expr
        ;

for     : FOR ident ASSIGN expr TO expr DO statement
        ;

if      : IF expr THEN statement
        | if ELSE statement
		;

write   : WRITE LPAR expr RPAR
	    ;

%%
