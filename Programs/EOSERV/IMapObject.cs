﻿using System;
using System.Collections.Generic;
using EOHax.EO;
using EOHax.EO.Communication;

namespace EOHax.Programs.EOSERV
{
	public enum WalkType
	{
		Normal,
		Ghost,
		Admin
	}

	public interface IMapObject
	{
		ushort    MapId     { get; }
		byte      X         { get; }
		byte      Y         { get; }
		Direction Direction { get; }
		short     Hp        { get; }
		short     Tp        { get; }
		IClient   Client    { get; }
		short     MaxHp     { get; }
		short     MaxTp     { get; }
		short     MinDamage { get; }
		short     MaxDamage { get; }
		short     Accuracy  { get; }
		short     Evade     { get; }
		short     Defence   { get; }
		IMap      Map       { get; }
		ushort    Id        { get; }
#region Database
		void Activate(IServer server);
		void Store();
#endregion
		IEnumerable<T> GetInRange<T>(int distance = -1) where T : IMapObject;
		void SendInRange(Packet packet, bool echo = true, MapObject exclude = null);

		void CalculateStats();

		bool Walk(Direction direction, WalkType type = WalkType.Normal, bool echo = true);
		void Warp(ushort map, byte x, byte y, WarpAnimation animation = WarpAnimation.None);

		Packet AddToViewBuilder     (WarpAnimation animation = WarpAnimation.None);
		Packet DeleteFromViewBuilder(WarpAnimation animation = WarpAnimation.None);
		Packet WalkBuilder          ();
		Packet FaceBuilder          ();
		void   InfoBuilder          (ref Packet packet);
	}
}
