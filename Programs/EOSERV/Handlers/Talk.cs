using System;
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

			Packet talkPacket = new Packet(PacketFamily.Talk, PacketAction.Player);
			talkPacket.AddShort((short)client.Id);
			talkPacket.AddString(message);

			Program.Logger.LogLevel = EOHax.Logging.LogLevel.Debug;

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
						client.Server.ScriptHost.Execute(script);
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
            else if (message[0] == '$')
            {
                Program.Logger.LogDebug(String.Format("Adding item {0}", Convert.ToInt16(message.Substring(1))));
                client.Character.AddItem(Convert.ToInt16(message.Substring(1)), 1);
            }
			client.Character.SendInRange(talkPacket, false);
		}
	}
}
