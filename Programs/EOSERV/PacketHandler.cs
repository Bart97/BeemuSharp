using System;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
using EOHax.EO.Communication;

namespace EOHax.Programs.EOSERV
{
	// TODO: Cache handler state attributes

	[AttributeUsage(AttributeTargets.Method, AllowMultiple=true)]
	public sealed class HandlerStateAttribute : Attribute
	{
		public ClientState State { get; set; }

		public HandlerStateAttribute(ClientState State)
		{
			this.State = State;
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public sealed class HandlerStateListAttribute : Attribute
	{
		public ClientState[] States { get; set; }

		public HandlerStateListAttribute(ClientState[] States)
		{
			this.States = States;
		}
	}

	public static class PacketHandler
	{
		private delegate void Handler(Packet packet, IClient client, bool fromQueue);

		private static Handler[,] handlers = InitializeHandlers();

		private static Handler[,] InitializeHandlers()
		{
			handlers = new Handler[256, 256];

			// NOTE: Using Parallel.ForEach here causes random thread lockups
			foreach (var type in Assembly.GetCallingAssembly().GetTypes())
			{
				if (type.Namespace == typeof(Handler).Namespace + ".Handlers" && type.Name.EndsWith("Handler"))
				{
					PacketFamily family = (PacketFamily)Enum.Parse(typeof(PacketFamily), type.Name.Substring(0, type.Name.Length - 7));

					foreach (MethodInfo method in type.GetMethods())
					{
						if (method.IsStatic && method.Name.StartsWith("Handle"))
						{
							PacketAction action = (PacketAction)Enum.Parse(typeof(PacketAction), method.Name.Substring(6));

							handlers[(byte)family, (byte)action] = (Handler)Delegate.CreateDelegate(typeof(Handler), method);
						}
					}
				}
			}

			return handlers;
		}

		public static void Handle(Packet packet, IClient client, bool fromQueue)
		{
			Handler handler = handlers[(byte)packet.Family, (byte)packet.Action];

			if (handler == null)
				throw new NotSupportedException("No handler set for " + packet.Family + "_" + packet.Action);

			bool allowed = true;
			var stateAttributes = handler.GetInvocationList()[0].Method.GetCustomAttributes(typeof(HandlerStateAttribute), false);
			HandlerStateAttribute stateAttribute = stateAttributes.Length > 0 ? (HandlerStateAttribute)stateAttributes[0] : null;

			if (stateAttribute != null)
			{
				ClientState requiredState = (ClientState)stateAttribute.State;
				allowed = (requiredState == client.State);
			}
			else
			{
				var stateListAttributes = handler.GetInvocationList()[0].Method.GetCustomAttributes(typeof(HandlerStateListAttribute), false);
				HandlerStateListAttribute stateListAttribute = stateListAttributes.Length > 0 ? (HandlerStateListAttribute)stateListAttributes[0] : null;

				if (stateListAttribute != null)
				{
					allowed = false;

					foreach (ClientState state in stateListAttribute.States)
					{
						if (state == client.State)
						{
							allowed = true;
							break;
						}
					}
				}
			}

			if (!allowed)
				throw new Exception("Invalid client state to call " + packet.Family + "_" + packet.Action + " (" + client.State + ")");

			lock (client)
			{
				handler(packet, client, fromQueue);
			}
		}
	}
}
