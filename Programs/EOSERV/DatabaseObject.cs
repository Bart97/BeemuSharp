using System;

namespace EOHax.Programs.EOSERV
{
    [Serializable]
	public class DatabaseObject
	{
		[NonSerialized] private IServer server;
		
		public IServer Server
		{
			get { return server; }
			private set { server = value; }
		}

		protected DatabaseObject(IServer server)
		{
			Server = server;
		}

		protected void SafeActivate(Object obj)
		{
			SafeActivate(obj, 1);
		}

		protected void SafeActivate(Object obj, int depth)
		{
			if (obj != null)
				Server.Database.Activate(obj, depth);
		}

		protected void SafeStore(Object obj)
		{
			if (obj != null)
				Server.Database.Store(obj);
		}

		public void Activate(IServer server)
		{
			Server = server;

			Server.Database.Activate(this, 1);
		}

        public void Delete()
        {
            Server.Database.Delete(this);
        }

		public void Store()
		{
			Server.Database.Store(this);
		}
	}

}
