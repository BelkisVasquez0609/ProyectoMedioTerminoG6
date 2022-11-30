using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using ProyectoMedioTerminoG6;

string getInput()
{
    Console.WriteLine("Digite el nombre del script: ");
    var script = Console.ReadLine();

    return script;
}

IParseTree parse(string input)
{
    ImmediateErrorListener errListener = ImmediateErrorListener.Instance;
    var inputStream = CharStreams.fromPath($@"..\..\..\{input}");
    var lexer = new ProyectoLexer(inputStream);
    var tokenStream = new CommonTokenStream(lexer);
    var parser = new ProyectoParser(tokenStream);
    parser.RemoveErrorListeners();
    parser.AddErrorListener(errListener);
    //var tree = parser.script();
    var tree = parser.program();
    return tree;
}

bool todoBien = false;
IParseTree? tree = null;
string input;

do
{
    input = getInput();
    if (input == "" || !input.Contains(".txt"))
    {
        Console.WriteLine("Debe ingresar el nombre de un script válido. \n\n");
        todoBien = false;
    }
    else
    {
        try
        {
            tree = parse(input);
            todoBien = true;
        }
        catch (ParseCanceledException e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Su script contiene el siguiente error de gramatica: {e.Message}\n");
            Console.ResetColor();
            Console.WriteLine("Ingrese otro script o corrija este. \n\n");
            continue;
        }
    }

} while (!todoBien);

AnalizadorComandos scrip_analizador = new AnalizadorComandos();
Console.WriteLine("**HOLA, SOMOS EL GRUPO 6* \n" +
                  "TASK [Iniciando proceso] ** \n");
Console.WriteLine(scrip_analizador.Visit(tree));


Console.WriteLine("Presione cualquier tecla para finalizar...");
Console.ReadKey();
