%{
// Эти объявления добавляются в класс GPPGParser, представляющий собой парсер, генерируемый системой gppg
    public BlockNode root; // Корневой узел синтаксического дерева 
    public Parser(AbstractScanner<ValueType, LexLocation> scanner) : base(scanner) { }
%}

%output = SimpleYacc.cs

%union { 
			public double dVal; 
			public int iVal; 
			public string sVal; 
			public Node nVal;
			public ExprNode eVal;
			public StatementNode stVal;
			public BlockNode blVal;
       }

%using ProgramTree;

%namespace SimpleParser

%token BEGIN END CYCLE ASSIGN SEMICOLON WHILE DO REPEAT UNTIL FOR TO WRITE LPAR RPAR IF THEN ELSE VAR COMMA PLUS MINUS MUL DIV
%token <iVal> INUM 
%token <dVal> RNUM 
%token <sVal> ID

%type <eVal> expr ident expr2 expr3
%type <stVal> assign statement cycle while repeat for write if var
%type <blVal> stlist block

%%

progr   : block { root = $1; }
		;

stlist	: statement 
			{ 
				$$ = new BlockNode($1); 
			}
		| stlist SEMICOLON statement 
			{ 
				$1.Add($3); 
				$$ = $1; 
			}
		;

statement: assign { $$ = $1; }
		| block   { $$ = $1; }
		| cycle   { $$ = $1; }
		| while   { $$ = $1; }
		| repeat  { $$ = $1; }
		| for     { $$ = $1; }
		| write   { $$ = $1; }
		| if      { $$ = $1; }
		| var     { $$ = $1; }
	;

ident 	: ID { $$ = new IdNode($1); }	
		;
	
assign 	: ident ASSIGN expr { $$ = new AssignNode($1 as IdNode, $3); }
		;

expr    : expr2 { $$ = $1; }
        | expr PLUS expr2  { $$ = new BinaryNode($1, $3, '+'); }
        | expr MINUS expr2 { $$ = new BinaryNode($1, $3, '-'); }
        ;

expr2   : expr3 { $$ = $1; }
        | expr2 MUL expr3 { $$ = new BinaryNode($1, $3, '*'); }
        | expr2 DIV expr3 { $$ = new BinaryNode($1, $3, '/'); }
        ;

expr3   : ident { $$ = $1; }
        | INUM { $$ = new IntNumNode($1); }
        | LPAR expr RPAR { $$ = $2; }
        ;

block	: BEGIN stlist END { $$ = $2; }
		;

cycle	: CYCLE expr statement { $$ = new CycleNode($2, $3); }
		;

while   : WHILE expr DO statement { $$ = new WhileNode($2, $4); }
        ;

repeat  : REPEAT statement UNTIL expr { $$ = new RepeatNode($2, $4); }
        ;

for     : FOR ident ASSIGN expr TO expr DO statement { $$ = new ForNode($2 as IdNode, $4, $6, $8); }
        ;

write   : WRITE LPAR expr RPAR { $$ = new WriteNode($3); }
        ;

if      : IF expr THEN statement { $$ = new IfNode($2, $4); }
        | if ELSE statement { 
		    ($1 as IfNode).Stat2 = $3;
		    $$ = $1; 
		}
		;

var     : VAR ident { $$ = new VarDefNode($2 as IdNode); }
        | var COMMA ident { 
		    ($1 as VarDefNode).Add($3 as IdNode);
			$$ = $1;
		} 
		;

%%
