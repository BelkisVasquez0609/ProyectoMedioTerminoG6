grammar Proyecto;

//Parsing rules 
program:  script EOF;
script: host become? tasks_lb;
host: GUION HOST_LBL SEP TEXT;
become: GUION BECOME_LBL SEP TEXT;
tasks_lb: SP+ TASKS_LBL SEP (tasks)+;
tasks: name apt;
name: SP+ NAME_LBL SEP TEXT;
apt: SP+ TEXT SEP command SP? DIR;
command: TEXT GUION? TEXT? #comando;

//Lexer rules
HOST_LBL: 'host';
BECOME_LBL: 'become';
TASKS_LBL: 'tasks';
NAME_LBL: 'name';

MAYORQUE: '>';
GUION : '-' SP?;
DIR: BACKSLASH WORD PUNTO? WORD? (BACKSLASH WORD PUNTO? WORD? TEXT?)*;
SEP: SP? ':' SP?;
SP : ' '+;
PUNTO: '.';
NUM: [0-9]+;
TEXT : WORD (SP WORD)*;
WORD : [a-zA-Z,']+  NUM*  SP?;
NEWLINE: [\t\r\n] -> skip;
BACKSLASH: '\\';