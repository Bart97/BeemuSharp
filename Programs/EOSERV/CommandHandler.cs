using System;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
using EOHax.EO;
using EOHax.EO.Communication;
using System.Collections.Generic;

namespace EOHax.Programs.EOSERV
{
	// TODO: Cache handler state attributes

	public enum AccessLevel
	{
		Player = 0,
		Guide = 1,
		Guardian = 2,
		GM = 3,
		HGM = 4,
		Console = 5
	}

	[AttributeUsage(AttributeTargets.Method)]
	public sealed class AccessLevelAttribute : Attribute
	{
		public AccessLevel Level { get; set; }

		public AccessLevelAttribute(AccessLevel Level)
		{
			this.Level = Level;
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public sealed class ArgumentAmountAttribute : Attribute
	{
		public int Amount { get; set; }

		public ArgumentAmountAttribute(int Amount)
		{
			this.Amount = Amount;
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public sealed class PlayerOnlyAttribute : Attribute
	{
		public PlayerOnlyAttribute()
		{
		}
	}

	public class CommandParameters
	{
		private List<object> parameters = new List<object>();
		public CommandParameters(String rawParameters)
		{
			string param_buff = String.Empty;
			int int_buff = 0;
			double double_buff = 0.0;
			bool in_string = false;
			bool escape = false;
			bool expecting_end = false;
			foreach (char c in rawParameters)
			{
				if (expecting_end && c != ' ')
				{
					throw new FormatException("Expected ' ' but found '" + c + "'");
				}
				else if (!in_string & c == ' ')
				{
					expecting_end = false;
					if (int.TryParse(param_buff, out int_buff))
						parameters.Add(int_buff);
					else if (double.TryParse(param_buff, out double_buff))
						parameters.Add(double_buff);
					else
						parameters.Add(param_buff);
					param_buff = String.Empty;
					int_buff = 0;
					double_buff = 0.0;
				}
				else if (in_string && c == '\\' && !escape)
				{
					escape = true;
				}
				else if (c == '"' || c == '\'' && !escape)
				{
					if (!in_string) in_string = true;
					else
					{
						expecting_end = true;
						in_string = false;
					}
				}
				else
				{
					escape = false;
					param_buff += c;
				}
			}
			if (in_string)
			{
				throw new FormatException("String not terminated");
			}
			if (param_buff != String.Empty)
			{
				if (int.TryParse(param_buff, out int_buff))
					parameters.Add(int_buff);
				else if (double.TryParse(param_buff, out double_buff))
					parameters.Add(double_buff);
				else
					parameters.Add(param_buff);
				param_buff = String.Empty;
				int_buff = 0;
				double_buff = 0.0;
			}
		}
		public T Get<T>(int id, T def)
		{
			if (parameters.Count - 1 < id)
			{
				return def;
			}
			if (!typeof(T).IsInstanceOfType(parameters[id]))
			{
				throw new ArgumentException("Parameter id " + id + " was expected to be " + typeof(T).FullName + " but is " + parameters[id].GetType().FullName);
			}
			return (T)parameters[id];
		}
		public T Get<T>(int id)
		{
			if (parameters.Count - 1 < id)
			{
				throw new ArgumentException("Parameter id " + id + " does not exist");
			}
			if (!typeof(T).IsInstanceOfType(parameters[id]))
			{
				throw new ArgumentException("Parameter id " + id + " was expected to be " + typeof(T).FullName + " but is " + parameters[id].GetType().FullName);
			}
			return (T)parameters[id];
		}
		public int Count
		{
			get { return parameters.Count; }
		}
	}

	public static class CommandParser
	{
		public static void Parse(String raw, ICommandExecutor executor)
		{/*
			String[] param = command.Split(' ');
			String command = param[0];
			
			param = param.Where((ValueType, idx) => idx != 0).ToArray();
			Program.Logger.LogDebug(String.Format("Command: {0}", command));
			CommandHandler.Handle(command, param, this);*/
			int loc = raw.IndexOf(' ');
			String command = "", raw_params = "";
			if (loc != -1)
			{
				command = raw.Substring(0, loc);
				raw_params = raw.Substring(loc + 1);
				Console.WriteLine(command);
				Console.WriteLine(raw_params);
			}
			else
				command = raw;
			
			CommandParameters parameters = new CommandParameters(raw_params);
			CommandHandler.Handle(command, parameters, executor);
		}
	}

	public static class CommandHandler
	{

		private delegate void Command(CommandParameters parameters, ICommandExecutor executor);

		private static Dictionary<String, Command> commands = InitializeCommands();

		private static Dictionary<String, Command> InitializeCommands()
		{
			commands = new Dictionary<String, Command>();

			// NOTE: Using Parallel.ForEach here causes random thread lockups
			foreach (var type in Assembly.GetCallingAssembly().GetTypes())
			{
				if (type.Namespace == typeof(Command).Namespace + ".Commands" && type.Name.EndsWith("Command"))
				{
					String command = type.Name.Substring(0, type.Name.Length - 7).ToLower();

					foreach (MethodInfo method in type.GetMethods())
					{
						if (method.IsStatic && method.Name == "CommandMain")
						{
							commands.Add(command, (Command)Delegate.CreateDelegate(typeof(Command), method));
						}
					}
				}
			}

			return commands;
		}

		public static void Handle(String commandString, CommandParameters parameters, ICommandExecutor executor)
		{
			Command command;
			try
			{
				command = commands[commandString];
			}
			catch (KeyNotFoundException)
			{
				throw new NotSupportedException("No command found for " + commandString);
			}

			if (command == null)
				throw new NotSupportedException("No command found for " + commandString);

			var accessLevelAttributes = command.GetInvocationList()[0].Method.GetCustomAttributes(typeof(AccessLevelAttribute), false);
			AccessLevelAttribute accessLevelAttribute = accessLevelAttributes.Length > 0 ? (AccessLevelAttribute)accessLevelAttributes[0] : null;

			if (accessLevelAttribute != null)
			{
				if (accessLevelAttribute.Level > executor.AccessLevel)
					throw new Exception("Too low access level for command " + commandString);
			}

			var playerOnlyAttributes = command.GetInvocationList()[0].Method.GetCustomAttributes(typeof(PlayerOnlyAttribute), false);
			PlayerOnlyAttribute playerOnlyAttribute = playerOnlyAttributes.Length > 0 ? (PlayerOnlyAttribute)playerOnlyAttributes[0] : null;

			if (playerOnlyAttribute != null)
			{
				if (executor.AccessLevel == AccessLevel.Console)
					throw new Exception("Command " + commandString + "can't be called from console");
			}

			var argumentAmountAttributes = command.GetInvocationList()[0].Method.GetCustomAttributes(typeof(ArgumentAmountAttribute), false);
			ArgumentAmountAttribute argumentAmountAttribute = argumentAmountAttributes.Length > 0 ? (ArgumentAmountAttribute)argumentAmountAttributes[0] : null;

			if (argumentAmountAttribute != null)
			{
				if (argumentAmountAttribute.Amount > parameters.Count)
					throw new Exception("Not enough arguments for command " + commandString);
			}

			lock (executor)
			{
				command(parameters, executor);
			}
		}
	}
}
