using System;
using System.Linq;
using EOHax.EO;
using EOHax.EO.Communication;
using EOHax.Scripting;

namespace EOHax.Programs.EOSERV.Handlers
{
	static class TalkHandler
	{
		// Player changing direction
		[HandlerState(ClientState.Playing)]
		public static void HandleReport(Packet packet, IClient client, bool fromQueue)
		{
			// TODO: Replace temporary talk code
			string message = packet.GetEndString();

			if (message[0] == '%')
			{
				string scriptFilename = message.Substring(1);

				try
				{
					Program.Logger.LogDebug(String.Format("Loading {0}", scriptFilename));
					Script script = Script.LoadFromFile(scriptFilename);

					Program.Logger.LogDebug("Compiling");
					client.Server.ScriptHost.Compile(script);

					if (script.Compiled)
					{
						Program.Logger.LogDebug("Executing");
						client.Server.ScriptHost.Execute(script, null, new Object[]{client.Character});
					}
					else
					{
						Program.Logger.LogError("Script compilation failed: ");
						foreach (var error in script.results.Errors)
						{
							Console.WriteLine(error);
						}
					}
				}
				catch (Exception ex)
				{
					Program.Logger.LogError("Script execution failed", ex);
				}
			}
			else if (message[0] == '$') // TODO: $ as command macro, = as eval
			{
				try
				{
					CommandParser.Parse(message.Substring(1), client.Character);
				}
				catch (Exception ex)
				{
					Packet warning = new Packet(PacketFamily.Message, PacketAction.Open);
					warning.AddString(String.Format("{0}: {1}", ex.GetType().Name, ex.Message));
					client.Send(warning);
				}
			}

			client.Character.SendMsg((IMessageTarget)client.Character.Map, new MessageLocal(client.Character, message));
		}

		// Player sending a global message
		[HandlerState(ClientState.Playing)]
		public static void HandleMessage(Packet packet, IClient client, bool fromQueue)
		{
			String message = packet.GetEndString();
			client.Character.SendMsg((IMessageTarget)client.Server.Global, new MessageGlobal(client.Character, message));
		}
	}
}
