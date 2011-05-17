using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace EOHax.Programs.EOSERV
{
	public class Account : DatabaseObject
	{
		private static RandomNumberGenerator rng    = new RNGCryptoServiceProvider();
		private static HMACSHA256            hasher = new HMACSHA256();

		// These have to be public to allow queries to work
		public string          username;
		public byte[]          nonce      = new byte[4];
		public byte[]          password   = null;
		public string          fullName;
		public string          location;
		public string          email;
		public string          computer;
		public UInt32          driveId;
		public IPAddress       registerIP;
		public IPAddress       lastIP;
		public DateTime        created    = DateTime.Now;
		public DateTime        lastUsed;
		public List<Character> characters;

		[NonSerialized] private IClient client;

#region Null Accessors
		public IClient Client
		{
			get { return client; }
			private set { client = value; }
		}

		public string Username
		{
			get { return username; }
			private set { username = value; }
		}

		public byte[] Nonce
		{
			get { return nonce; }
			private set { nonce = value; }
		}

		public byte[] Password
		{
			get { return password; }
			private set { password = value; }
		}

		public string FullName
		{
			get { return fullName; }
			private set { fullName = value; }
		}

		public string Location
		{
			get { return location; }
			private set { location = value; }
		}

		public string Email
		{
			get { return email; }
			private set { email = value; }
		}

		public string Computer
		{
			get { return computer; }
			private set { computer = value; }
		}

		public UInt32 DriveId
		{
			get { return driveId; }
			private set { driveId = value; }
		}

		public IPAddress RegisterIP
		{
			get { return registerIP; }
			private set { registerIP = value; }
		}

		public IPAddress LastIP
		{
			get { return lastIP; }
			private set { lastIP = value; }
		}

		public DateTime Created
		{
			get { return created; }
			private set { created = value; }
		}

		public DateTime LastUsed
		{
			get { return lastUsed; }
			private set { lastUsed = value; }
		}

		public List<Character> Characters
		{
			get { return characters; }
			private set { characters = value; }
		}
#endregion

		public Account(IServer server, IClient client, IPAddress ip, string username, string password, string location, string email, string computer, UInt32 driveId) : base(server)
		{
			this.Client = client;

			this.username = username;
			this.location = location;
			this.email = email;
			this.computer = computer;
			this.driveId = driveId;
			this.characters = new List<Character>();

			rng.GetBytes(this.nonce);
			registerIP = ip;
			lastIP = ip;

			SetPassword(password);
		}
#region Database
        public void Activate(IServer server, IClient client)
		{
			base.Activate(server);

			this.Client = client;

			SafeActivate(nonce);
			SafeActivate(password);
			SafeActivate(registerIP);
			SafeActivate(lastIP);
			SafeActivate(characters);

			foreach (Character character in characters)
			{
				character.Activate(server, client);
			}
		}

		public new void Store()
		{
			base.Store();

			SafeStore(nonce);
			SafeStore(password);
			SafeStore(registerIP);
			SafeStore(lastIP);
			SafeStore(characters);
		}
#endregion
        public void SetPassword(string password)
		{
			SetPassword(ASCIIEncoding.ASCII.GetBytes(password));
		}

		public void SetPassword(byte[] password)
		{
			hasher.Key = this.nonce;
			this.password = hasher.ComputeHash(password);
		}

		public bool CheckPassword(string password)
		{
			return CheckPassword(ASCIIEncoding.ASCII.GetBytes(password));
		}

		public bool CheckPassword(byte[] password)
		{
			hasher.Key = this.nonce;
			byte[] passwordHash = hasher.ComputeHash(password);

			for (int i = 0; i < 32; ++i)
			{
				if (passwordHash[i] != this.password[i])
					return false;
			}

			return true;
		}

		public static bool ValidUsername(string username)
		{
			return new Regex("[a-z0-9 ]{4,16}").Match(username).Success;
		}
	}
}
