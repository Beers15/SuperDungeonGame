using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using static MapUtils.Dir;
using static MapUtils.Type;
using static GameAgentAction;

namespace MapUtils 
{
	public enum Dir  { LEFT = 0, UP, RIGHT, DOWN, NONE };
	
	public enum Type { LINE = 1, CORNER, ALLEY, INVALID };
	
	public static class EnumUtils
	{
		public static Dir right(this Dir dir, int amt = 1)
		{
			return (Dir) (((int)dir + amt) % 4);
		}
		public static Dir left(this Dir dir, int amt = 1)
		{
			amt %= 4;
			int ndir = (int)dir - amt;
			if (ndir < 0) {
				return (Dir) (4 + ndir);
			}
			else {
				return (Dir) ndir;
			}
		}
		public static Dir relativeDir(Pos a, Pos b)
		{
			Pos diff = b - a;
			if (Math.Abs(diff.y) > Math.Abs(diff.x)) {
				if (diff.y < 0)
					return UP;
				else if (diff.y > 0)
					return DOWN;
			} else {
				if (diff.x < 0)
					return LEFT;
				else if (diff.x > 0)
					return RIGHT;
			}
			return NONE;
		}
		public static string GetString(this Dir dir)
		{
			switch (dir) {
				case LEFT  : return "LEFT ";
				case UP    : return "UP   ";
				case RIGHT : return "RIGHT";
				case DOWN  : return "DOWN ";
			}
			return "NONE ";
		}
		public static string GetString(this Type type)
		{
			switch (type) {
				case LINE   : return "LINE  ";
				case CORNER : return "CORNER";
				case ALLEY  : return "ALLEY ";
			}
			return "NONE  ";
		}
		public static Pos toVector(this Dir dir)
		{
			switch (dir) {
				case LEFT  : return Pos.LEFT;
				case UP    : return Pos.UP;
				case RIGHT : return Pos.RIGHT;
				case DOWN  : return Pos.DOWN;
			}
			return Pos.NONE;
		}
		/*public static string GetString(this GameAgentAction action)
		{
			switch (action) {
				case Move: return "MOVE";
				case Wait: return "WAIT";
				case Potion: return "POTION";
				case MeleeAttack: return "ATTACK";
				case Taunt: return "TAUNT";
				case RangedAttack: return "SHOOT";
				case RangedAttackMultiShot: return "MULTISHOT";
				case MagicAttackSingleTarget: return "BOLT";
				case MagicAttackAOE: return "STORM";
				case Heal: return "HEAL";
				case Neutral: return "NEUTRAL";
			}
			return "NONE";
		}*/
		/*public static string GetMode(this GameAgentAction action)
		{
			switch (action) {
				case Move: return "MOVE";
				case Wait: return "NONE";
				case Potion: return "NONE";
				case MeleeAttack: return "ACT";
				case Taunt: return "ACT";
				case RangedAttack: return "ACT";
				case RangedAttackMultiShot: return "ACT";
				case MagicAttackSingleTarget: return "ACT";
				case MagicAttackAOE: return "AOE";
				case Heal: return "ACT";
				case Neutral: return "NONE";
			}
			return "NONE";
		}*/
	}
	
	public class Pos 
	{
		public static Pos LEFT = new Pos(-1, 0);
		public static Pos UP = new Pos(0, -1);
		public static Pos RIGHT = new Pos(1, 0);
		public static Pos DOWN = new Pos(0, 1);
		public static Pos NONE = new Pos(0, 0);
		public static Pos[] Directions = new Pos[] { Pos.LEFT, Pos.UP, Pos.RIGHT, Pos.DOWN };
		
		public int x;
		public int y;
		public Pos(int x, int y) 
		{
			this.x = x;
			this.y = y;
		}
		public static bool in_bounds(Pos p, int width, int height)
		{
			return p.x < width && p.x >= 0 && p.y < height && p.y >= 0;
		}
		public static int abs_dist(Pos a, Pos b)
		{
			return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
		}
		public static Pos operator +(Pos a, Pos b)
		{
			return new Pos(a.x + b.x, a.y + b.y);
		}
		public static Pos operator -(Pos a, Pos b)
		{
			return new Pos(a.x - b.x, a.y - b.y);
		}
		public static bool operator ==(Pos a, Pos b)
		{
			if (object.ReferenceEquals(a, null))
				return object.ReferenceEquals(b, null);
			if (object.ReferenceEquals(b, null))
				return object.ReferenceEquals(a, null);
			return a.x == b.x && a.y == b.y;
		}
		public static bool operator !=(Pos a, Pos b)
		{
			if (object.ReferenceEquals(a, null))
				return !object.ReferenceEquals(b, null);
			if (object.ReferenceEquals(b, null))
				return !object.ReferenceEquals(a, null);
			return a.x != b.x || a.y != b.y;
		}
		public override string ToString()
		{
			return "(" + x.ToString() + "," + y.ToString() + ")";
		}
		public override bool Equals(object o)
		{
			if (o is Pos) {
				Pos p = (Pos)o;
				return p.x == x && p.y == y;
			}
			return false;
		}
		public override int GetHashCode()
		{
			return 0; // getting rid of annoying compile warning
		}
		public byte[] toBytes()
		{
			byte[] bytes = new byte[8];
			byte[] xBytes = BitConverter.GetBytes(x);
			byte[] yBytes = BitConverter.GetBytes(y);
			if (BitConverter.IsLittleEndian) {
				Array.Reverse(xBytes);
				Array.Reverse(yBytes);
			}
			Array.Copy(xBytes, 0, bytes, 0, 4);
			Array.Copy(yBytes, 0, bytes, 4, 4);
			return bytes;
		}
	}
	
	public class Cmd
	{
		public Dir dir;
		public Type type;
		public Pos pos;
		public Cmd(Dir dir, Pos pos, Type type)
		{
			this.dir = dir;
			this.pos = pos;
			this.type = type;
		}
		public static bool operator ==(Cmd a, Cmd b)
		{
			return a.dir == b.dir && a.type == b.type && a.pos == b.pos;
		}
		public static bool operator !=(Cmd a, Cmd b)
		{
			return a.dir != b.dir || a.type != b.type || a.pos != b.pos;
		}
		public override string ToString()
		{
			return pos.ToString() + " | " + dir.GetString() + " | " + type.GetString();
		}
		public override bool Equals(object o)
		{
			if (o is Cmd) {
				Cmd c = (Cmd)o;
				return c.dir == dir && c.type == type && c.pos == pos;
			}
			return false;
		}
		public override int GetHashCode()
		{
			return 0; // getting rid of annoying compile warning
		}
	}
	
	public static class MapConstants
	{
		// all values greater than filled indicate a region marker
		public const int FILLED = 1;
		public const int EMPTY = 0;
		public const int BRIDGE = -1;
		public const int PLATFORM = -2;
		public const int EDGE = -3;
		public const int INNER_REGION = -4;
		
		public static bool traversable(int tile)
		{
			if (tile >= FILLED || tile == BRIDGE || tile == PLATFORM)
				return true;
			return false;
		}
		
		public static bool isBridge(int tile)
		{
			return tile == PLATFORM || tile == BRIDGE;
		}
		
		public static bool isEdge(int tile)
		{
			return tile == EDGE;
		}
	}
}