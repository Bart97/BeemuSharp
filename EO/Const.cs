using System;

namespace EOHax.EO
{
	public static class Const
	{
		public const byte Version1 = 0;
		public const byte Version2 = 0;
		public const byte Version3 = 28;
		public const string VersionString = "0.000.028";

		public const int ChallengeMin = 1000;
		public const int ChallengeMax = 2000;
	}

	public enum AdminLevel : byte
	{
		Player = 0,
		Guide = 1,
		Guardian = 2,
		GM = 3,
		HGM = 4
	}

	public enum AvatarSlot : byte
	{
		Clothes = 1,
		Hair = 2,
		HairColor = 3
	}

	public enum Direction : byte
	{
		Down = 0,
		Left = 1,
		Up = 2,
		Right = 3
	}

	public enum Emote : byte
	{
		Happy = 1,
		Depressed = 2,
		Sad = 3,
		Angry = 4,
		Confused = 5,
		Surprised = 6,
		Hearts = 7,
		Moon = 8,
		Suicidal = 9,
		Embarassed = 10,
		Drunk = 11,
		Trade = 12,
		LevelUp = 13,
		Playful = 14
	}

	public enum FileType : byte
	{
		Map = 1,
		Item = 2,
		NPC = 3,
		Spell = 4,
		Class = 5
	}

	public enum Gender : byte
	{
		Female = 0,
		Male = 1
	}

	public enum MapEffect : byte
	{
		Quake = 1
	}

	public enum PaperdollIcon : byte
	{
		Normal = 0,
		GM = 4,
		HGM = 5,
		Party = 6,
		GMParty = 9,
		HGMParty = 10
	}

	public enum PartyRequestTime : byte
	{
		Join = 0,
		Invite = 1
	}

	public enum SitState : byte
	{
		Stand = 0,
		Chair = 1,
		Floor = 2
	}

	public enum SitCommand : byte
	{
		Sitting = 1,
		Standing = 2
	}

	public enum Skin : byte
	{
		White = 0,
		Yellow = 1,
		Tan = 2,
		Orc = 3,
		Skeleton = 4,
		Panda = 5
	}

	public enum WarpAnimation : byte
	{
		None = 0,
		Scroll = 1,
		Admin = 2
	}

	public enum InitReply : byte
	{
		OutOfDate = 1,
		OK = 2,
		Banned = 3,
		FileMap = 4,
		FileEIF = 5,
		FileENF = 6,
		FileESF = 7,
		Players = 8,
		MapMutation = 9,
		FriendListPlayers = 10,
		FileECF = 11
	}

	public enum InitBanType : byte
	{
		Temp = 0,
		Permanent = 2
	}

	public enum AccountReply : byte
	{
		Exists = 1,
		NotApproved = 2,
		Created = 3,
		ChangeFailed = 5,
		Changed = 6,
		Continue = 64
	}

	public enum LoginReply : short
	{
		WrongUser = 1,
		WrongUserPass = 2,
		OK = 3,
		LoggedIn = 5,
		Busy = 6
	}

	public enum CharacterReply : byte
	{
		Exists = 1,
		Full = 2,
		NotApproved = 4,
		OK = 5,
		Deleted = 6
	}
	
	public enum WelcomeReply : short
	{
		CharacterInfo = 1,
		WorldInfo = 2,
		Rejected = 3
	}

	public enum WarpReply : byte
	{
		Local = 1,
		Switch = 2
	}

	public enum TalkReply : byte
	{
		NotFound = 1
	}

	public enum QuestAction : byte
	{
		Progress = 1,
		History = 2
	}

    public enum SkillMasterReply : byte
    {
        GetNaked = 1, // TODO: Better name?
        WrongClass = 2
    }
}
