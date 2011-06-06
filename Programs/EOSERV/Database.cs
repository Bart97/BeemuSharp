using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Config.Encoding;
using EOHax.EO.Data;

namespace EOHax.Programs.EOSERV
{
	public class Database : IDisposable
	{
		public IObjectContainer Container { get; private set; }

		public Database(string fileName)
		{
			IEmbeddedConfiguration config = Db4oEmbedded.NewConfiguration();
			config.Common.ActivationDepth = 0;
			config.Common.UpdateDepth = 1;
			config.Common.CallConstructors = false;
			config.Common.TestConstructors = false;
			config.Common.MessageLevel = 0;
			config.Common.StringEncoding = StringEncodings.Utf8();
			Container = Db4oEmbedded.OpenFile(config, fileName);
		}

		public void Dispose()
		{
			Container.Dispose();
		}

		public void Activate(Object obj, int depth)
		{
			Container.Activate(obj, depth);
		}

		public void Store(Object obj)
		{
			Container.Store(obj);
		}

		public void Commit()
		{
			Container.Commit();
		}

        public void Delete(Object obj)
        {
            Container.Delete(obj);
        }
	}

	/*public class Guild
	{
		private string tag;
		private string name;
		private string description;
		private DateTime created;
		private GuildRank[] ranks = new GuildRank[9];
		private int bank;

		public Guild()
		{
			for (int i = 0; i < 9; ++i)
				ranks[i] = new GuildRank();
		}
	}

	public class GuildRank
	{
		private string rankName;
	}

	public class Citizenship
	{
		private string homeId;
	}

	public class Marriage
	{
		private Character first;
		private Character second;
		private bool legal;
	}*/

    public class Item : DatabaseObject
	{
		public short id;

		[Transient] private EIF.Entry data;

        public short Id
        {
            get { return id; }
            private set { id = value; }
        }

        public EIF.Entry Data
        {
            get { return data; }
        }

        public Item(IServer server, short id): base(server)
        {
            this.id = id;
            data = Server.ItemData[(ushort)(id - 1)];
        }

        public new void Activate(IServer server)
        {
            base.Activate(server);

            data = Server.ItemData[(ushort)(id - 1)];
        }
    }

    public class ItemStack : Item
    {
        public int amount;

        public int Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public ItemStack(IServer server, short id, int amount)
            : base(server, id)
        {
            Amount = amount;
        }
    }

	/*public class BankAccount
	{
		private int gold;
		private byte upgrades;
		private List<Item> items;
	}

	public class GuildMembership
	{
		private Guild guild;
		private byte rankIndex;
	}*/
}
