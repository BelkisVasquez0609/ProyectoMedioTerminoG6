using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProyectoMedioTerminoG6
{
	internal class infoGeneral
	{
		public infoGeneral(string host, string? become)
			=> (this.host, this.become) = (host, become);

		public string host;
		public string? become;
		public string ObtenerOutput()
		{
			return (
				$"HOST [{host}]    *********************************************************************************************\n\n" +
				$"{(become is not null ? "BECOME" : "")} {(become is not null ? $"[{become}]" : "*************")}**********************************************************************************************\n");
		}
	}
	internal class task
	{
		public task(string name, string command)
			=> (this.name, this.command) = (name, command);

		public string name;
		public string command;
		public string? resultado = "";

		string ObtenerResultado()
		{
			try
			{
				//Creamos un espacio de ejecución para capturar el resultado del comando
				Runspace espacioEjecucion = RunspaceFactory.CreateRunspace();
				//Lo iniciamos
				espacioEjecucion.Open();
				//Creamos el objeto PowerShell
				PowerShell objPowerShell = PowerShell.Create();
				//Al objeto PowerShell le asignamos el espacio de ejecución
				objPowerShell.Runspace = espacioEjecucion;
				//Agregamos el comando PowerShell a ejecutar
				var result = objPowerShell.AddScript($@"{command}");
				//Ejecutamos el comando PowerShell y guardamos su resultado
				Collection<PSObject> resultadoEjecucion = objPowerShell.Invoke();

				if (result.HadErrors == false)
				{
					resultado = "Ejecutado de manera exitosa";
				}
				else
				{
					foreach (ErrorRecord error in result.Streams.Error.ReadAll())
					{
						resultado = $"Error: {error.ToString()}";
					}
				}

				espacioEjecucion.Close();
			}
			catch (Exception ex)
			{
				resultado = "Exception - " + ex.Message;
			}

			return resultado;
		}

		public string ObtenerOutput()
		{
			string result = ObtenerResultado();
			return (
				$"TASK [{name}] \n" +
				$"El comando ingresado fué: {command} \n" +
				$"El resultado de la operación fué: {result}  \n");
		}
	}
	internal class AnalizadorComandos : ProyectoBaseVisitor<object>
	{
		List<string> outPut = new List<string>();
		public List<string> Output
        {
			get => outPut;

		}
		public override object VisitHost([NotNull] ProyectoParser.HostContext context) //listo
		{
			return context.TEXT().GetText();
		}
		public override object VisitName([NotNull] ProyectoParser.NameContext context) //listo
		{
			return context.TEXT().GetText();
		}
		public override object VisitTasks([NotNull] ProyectoParser.TasksContext context) //listo
		{
			string Name = (string)Visit(context.name());
			string apt = (string)Visit(context.apt());
			task tasks = new task(Name, apt);
			return tasks;
		}
		public override object VisitTasks_lb([NotNull] ProyectoParser.Tasks_lbContext context)
		{
			List<task> tasks_list = new List<task>();
			foreach (var task_tree in context.tasks())
			{
				tasks_list.Add((task)Visit(task_tree));
			}
			return tasks_list;
		}
		public override object VisitApt([NotNull] ProyectoParser.AptContext context)//listo
		{
			string comando = context.command().GetText();
			string Direccion = context.DIR().GetText();
			string ComandReal = comando + ' ' + Direccion;
			return ComandReal;
		}

		public override object VisitScript([NotNull] ProyectoParser.ScriptContext context)
		{
            try
            {
				string host = (string)Visit(context.host());
				string? become = (string?)context.become()?.TEXT().GetText();
				infoGeneral info = new infoGeneral(host, become);
				List<task> task = (List<task>)Visit(context.tasks_lb());

				outPut.Add(info.ObtenerOutput());
				foreach (var task_tree in task)
                {
					var out_errors = task_tree.ObtenerOutput();
					outPut.Add(out_errors);
                    if (task_tree.resultado.Contains("Error"))
                    {
						break;
                    }
                }

            }
            catch (Exception ex)
            {
				Console.WriteLine("Method VisitScript Exception - " + ex.Message);
            }
			return new Object();
		}

		public override object VisitProgram([NotNull] ProyectoParser.ProgramContext context)
		{
			string insertResult = "";

            try
            {
				base.VisitProgram(context);
				foreach(var i in outPut)
					insertResult+= i + "\n";
            }
            catch (Exception ex)
            {

				Console.WriteLine("Method VisitProgram exception - " + ex.Message);
			}
			return insertResult;
		}
	}
}
