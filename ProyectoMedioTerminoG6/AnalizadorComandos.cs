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
		public override object VisitApt([NotNull] ProyectoParser.AptContext context)
		{
			return base.VisitApt(context);
		}

		public override object VisitHost([NotNull] ProyectoParser.HostContext context)
		{
			return base.VisitHost(context);
		}

		public override object VisitName([NotNull] ProyectoParser.NameContext context)
		{
			return base.VisitName(context);
		}

		public override object VisitProgram([NotNull] ProyectoParser.ProgramContext context)
		{
			return base.VisitProgram(context);
		}

		public override object VisitScript([NotNull] ProyectoParser.ScriptContext context)
		{
			return base.VisitScript(context);
		}

		public override object VisitTasks([NotNull] ProyectoParser.TasksContext context)
		{
			return base.VisitTasks(context);
		}

		public override object VisitTasks_lb([NotNull] ProyectoParser.Tasks_lbContext context)
		{
			return base.VisitTasks_lb(context);
		}
	}
}
