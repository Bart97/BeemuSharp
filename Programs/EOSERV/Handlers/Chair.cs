using System;
using EOHax.EO;
using EOHax.EO.Communication;
using EOHax.EOSERV.Data;

namespace EOHax.Programs.EOSERV.Handlers
{
	class ChairHandler
	{
		// Player sitting on a chair / standing
		[HandlerState(ClientState.Playing)]
		public static void HandleRequest(Packet packet, IClient client, bool fromQueue)
		{
			SitCommand command = (SitCommand)packet.GetChar();

			if (command == SitCommand.Sitting)
			{
				if (client.Character.SitState != SitState.Stand) throw new InvalidOperationException("Not standing.");

				byte x = packet.GetChar();
				byte y = packet.GetChar();

				if ((x + y - client.Character.X - client.Character.Y) > 1)
					throw new Exception("Incorrect chair coordinates.");

				switch (client.Character.Map.Data.Tiles[y, x].spec)
				{
					case MapTileSpec.ChairDown:
						if (client.Character.Y == y+1 && client.Character.X == x)
						{
							client.Character.Face(Direction.Down);
						}
						else
						{
							throw new Exception("Incorrect chair coordinates.");
						}
						break;

						case MapTileSpec.ChairUp:
							if (client.Character.Y == y-1 && client.Character.X == x)
							{
								client.Character.Face(Direction.Up);
							}
							else
							{
								throw new Exception("Incorrect chair coordinates.");
							}
							break;

						case MapTileSpec.ChairLeft:
							if (client.Character.Y == y && client.Character.X == x-1)
							{
								client.Character.Face(Direction.Left);
							}
							else
							{
								throw new Exception("Incorrect chair coordinates.");
							}
							break;

						case MapTileSpec.ChairRight:
							if (client.Character.Y == y && client.Character.X == x+1)
							{
								client.Character.Face(Direction.Right);
							}
							else
							{
								throw new Exception("Incorrect chair coordinates.");
							}
							break;

						case MapTileSpec.ChairDownRight:
							if (client.Character.Y == y && client.Character.X == x+1)
							{
								client.Character.Face(Direction.Right);
							}
							else if (client.Character.Y == y+1 && client.Character.X == x)
							{
								client.Character.Face(Direction.Down);
							}
							else
							{
								throw new Exception("Incorrect chair coordinates.");
							}
							break;

						case MapTileSpec.ChairUpLeft:
							if (client.Character.Y == y && client.Character.X == x-1)
							{
								client.Character.Face(Direction.Left);
							}
							else if (client.Character.Y == y-1 && client.Character.X == x)
							{
								client.Character.Face(Direction.Up);
							}
							else
							{
								throw new Exception("Incorrect chair coordinates.");
							}
							break;

						case MapTileSpec.ChairAll:
							if (client.Character.Y == y && client.Character.X == x+1)
							{
								client.Character.Face(Direction.Right);
							}
							else if (client.Character.Y == y && client.Character.X == x-1)
							{
								client.Character.Face(Direction.Left);
							}
							else if (client.Character.Y == y-1 && client.Character.X == x)
							{
								client.Character.Face(Direction.Up);
							}
							else if (client.Character.Y == y+1 && client.Character.X == x)
							{
								client.Character.Face(Direction.Down);
							}
							else
							{
								throw new Exception("Incorrect chair coordinates.");
							}
							break;

						default:
							throw new Exception("Incorrect chair coordinates.");
				}
				
				client.Character.SitChair(x, y);
			}
			else if (command == SitCommand.Standing)
			{
				if (client.Character.SitState != SitState.Chair) throw new InvalidOperationException("Not sitting on the floor.");

				client.Character.Stand();
			}
			else throw new ArgumentException("Invalid sit command");
		}
	}
}
