using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

/* Quick note on NetworkCommands;
 * Each network command MUST contain a character string describing it's command ID followed by a "$" character
 */
 
public abstract class NetworkCommand
{	
	protected enum Directive { START=0, END, DISCONNECT, ECHO, NONE }

    public abstract string getString();
	protected Directive directive = Directive.ECHO;
	
	public static byte[] assembleCommandBytes(NetworkCommand cmd)
	{
		byte dirByte = (byte) cmd.directive;
		byte[] msgBytes = Encoding.ASCII.GetBytes(cmd.getString());
		byte[] lenBytes = new byte[2] { (byte)(msgBytes.Length >> 8), (byte)(msgBytes.Length) };
		byte[] msgBody = new byte[3 + msgBytes.Length];
		
		msgBody[0] = dirByte;
		msgBody[1] = lenBytes[0];
		msgBody[2] = lenBytes[1];
		Array.Copy(msgBytes, 0, msgBody, 3, msgBytes.Length);
		return msgBody;
	}
}

/* #############################
 * ## ECHO Directive commands ##
 * #############################
 * Echo commands simply echo their message to every client currently connected to the server
 */

// MOVE commands signal a move from location a to location b on the game map
public class MoveCommand : NetworkCommand
{
	public const int ID = 0;
	
	public int clientID;
	public Pos end;
	public MoveCommand(int clientID, Pos end)
	{
		this.directive = Directive.ECHO;
		this.clientID = clientID;
		this.end = end;
	}
	public override string getString() { return ID + "$" + clientID + "," + end.x + "," + end.y; }
	// parse everything to the right of the '$' in the string returned by getString() for this object
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		int[] pts = Array.ConvertAll(cmdString.Split(','), int.Parse);
		return new MoveCommand(pts[0], new Pos(pts[1], pts[2]));
	}
}

// READY commands signal player readiness, typically in the lobby or at the end of a turn
public class ReadyCommand : NetworkCommand
{
	public const int ID = 1;
	
	public int clientID;
	public ReadyCommand(int clientID)
	{
		this.directive = Directive.ECHO;
		this.clientID = clientID;
	}
	public override string getString() { return ID + "$" + clientID; }
	// parse everything to the right of the '$' in the string returned by getString() for this object
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		return new ReadyCommand(int.Parse(cmdString));
	}
}

// NICKNAME commands signal a change in the nickname for a given player
public class NicknameCommand : NetworkCommand
{
	public const int ID = 4;
	
	public int clientID;
	public string nickname;
	public NicknameCommand(string nickname, int clientID)
	{
		this.directive = Directive.ECHO;
		this.nickname = nickname;
		this.clientID = clientID;
	}
	public override string getString() { return ID + "$" + nickname + "," + clientID; }
	
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		string[] nnID = cmdString.Split(',');
		string nickname = nnID[0];
		int clientID = Int32.Parse(nnID[1]);
		return new NicknameCommand(nickname, clientID);
	}
}

public class ClassnameCommand : NetworkCommand
{
	public const int ID = 5;
	
	public int clientID;
	public string classname;
	public ClassnameCommand(string classname, int clientID)
	{
		this.directive = Directive.ECHO;
		this.classname = classname;
		this.clientID = clientID;
	}
	public override string getString() { return ID + "$" + classname + "," + clientID; }
	
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		string[] cnID = cmdString.Split(',');
		string classname = cnID[0];
		int clientID = Int32.Parse(cnID[1]);
		return new ClassnameCommand(classname, clientID);
	}
}

public class UpdateClientInfoCommand : NetworkCommand
{
	public const int ID = 6;
	
	public int clientID;
	public string nickname;
	public string classname;
	public bool ready;
	public UpdateClientInfoCommand(int clientID, string nickname, string classname, bool ready)
	{
		this.clientID = clientID;
		this.nickname = nickname;
		this.classname = classname;
		this.ready = ready;
	}
	public UpdateClientInfoCommand(Client client)
	{
		this.clientID = client.ID;
		this.nickname = client.nickname;
		this.classname = client.classname;
		this.ready = client.ready;
	}
	public override string getString() { 
		return ID + "$" + clientID + "," + nickname + "," + classname + "," + ready; 
	}
	
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		string[] info = cmdString.Split(',');
		int clientID = int.Parse(info[0]);
		string nickname = info[1];
		string classname = info[2];
		bool ready = bool.Parse(info[3]);
		return new UpdateClientInfoCommand(clientID, nickname, classname, ready);
	}
}

public class AttackCommand : NetworkCommand
{
	public const int ID = 7;
	
	public Pos end;
	public int actionNo, clientID;
	public AttackCommand(int clientID, Pos end, int actionNo)
	{
		this.directive = Directive.ECHO;
		this.clientID = clientID;
		this.end = end;
		this.actionNo = actionNo;
	}
	public override string getString() { return ID + "$" + clientID + "," + end.x + "," + end.y + "," + actionNo; }
	// parse everything to the right of the '$' in the string returned by getString() for this object
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		int[] pts = Array.ConvertAll(cmdString.Split(','), int.Parse);
		return new AttackCommand(pts[0], new Pos(pts[1], pts[2]), pts[3]);
	}
}

public class SetSeedCommand : NetworkCommand
{
	public const int ID = 8;
	
	public int seed;
	public SetSeedCommand(int seed)
	{
		this.directive = Directive.ECHO;
		this.seed = seed;
	}
	public override string getString() { return ID + "$" + seed; }
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		return new SetSeedCommand(int.Parse(cmdString));
	}
}

public class WaitCommand : NetworkCommand
{
	public const int ID = 9;
	
	public int clientID;
	public WaitCommand(int clientID)
	{
		this.directive = Directive.ECHO;
		this.clientID = clientID;
	}
	public override string getString() { return ID + "$" + clientID; }
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		return new WaitCommand(int.Parse(cmdString));
	}
}

public class InteractCommand : NetworkCommand
{
	public const int ID = 10;
	
	public Pos end;
	public int clientID;
	public InteractCommand(int clientID, Pos end)
	{
		this.directive = Directive.ECHO;
		this.clientID = clientID;
		this.end = end;
	}
	public override string getString() { return ID + "$" + clientID + "," + end.x + "," + end.y; }
	// parse everything to the right of the '$' in the string returned by getString() for this object
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		int[] pts = Array.ConvertAll(cmdString.Split(','), int.Parse);
		return new InteractCommand(pts[0], new Pos(pts[1], pts[2]));
	}
}

public class UseItemCommand : NetworkCommand
{
	public const int ID = 11;
	
	public int clientID;
	public int slotIndex;
	public UseItemCommand(int clientID, int slotIndex) {
		this.clientID = clientID;
		this.slotIndex = slotIndex;
	}
	public override string getString() { return ID + "$" + clientID + "," + slotIndex; }
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		int[] IDSlot = Array.ConvertAll(cmdString.Split(','), int.Parse);
		return new UseItemCommand(IDSlot[0], IDSlot[1]);
	}
}

/* #######################
 * ## NON-ECHO Commands ##
 * #######################
 * Non-standard commands that modify the state of the server
 * e.g; adding/removing a client, or blocking/allowing new clients
 */

// JOIN commands are sent by the server to all clients when a new client connects 
// JOIN commands are never sent by the client, only received from the server
// the getString() method should not be invoked client-side, but is there for reference's sake
public class JoinCommand : NetworkCommand
{
	public const int ID = -2;
	
	public int clientID;
	public JoinCommand(int clientID) 
	{
		this.directive = Directive.NONE;
		this.clientID = clientID;
	}
	public override string getString() { return ID + "$" + clientID; }
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		return new JoinCommand(Int32.Parse(cmdString));
	}
}

// START commands signal the beginning of the game
// In addition, they tell the server to stop accepting new clients
public class StartCommand : NetworkCommand
{
	public const int ID = 2;
	
	public StartCommand() 
	{
		this.directive = Directive.START;
	}
	public override string getString() { return ID + "$Directive.START"; }
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		return new StartCommand();
	}
}

// END commands signal the end of the game
// In addition, they tell the server to begin accepting new clients (provided there is room)
public class EndCommand : NetworkCommand
{
	public const int ID = 3;
	
	public EndCommand() 
	{
		this.directive = Directive.END;
	}
	public override string getString() { return ID + "$Directive.END"; }
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		return new EndCommand();
	}
}

// DISCONNECT commands are sent by the server to all clients when a client disconnects
// Unlike the JOIN command, the client may send this command to the server
// Only when the client is not responding should the server take responsibility for disconnecting them
public class DisconnectCommand : NetworkCommand
{
	public const int ID = -1;
	
	public int clientID;
	public DisconnectCommand(int clientID) 
	{
		this.directive = Directive.DISCONNECT;
		this.clientID = clientID;
	}
	public override string getString() { return ID + "$" + clientID; }
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		return new DisconnectCommand(Int32.Parse(cmdString));
	}
}
