using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class containing client properties
public class Client
{
	public int ID;
	public Player playerObject = null;
	public string nickname;
	public string classname;
	public bool ready = false;
	public Client(int ID) { this.ID = ID; nickname = "Client" + ID; classname = "Warrior"; }
}

/* The Network class serves as an abstraction layer on top of NetworkBase 
 * Where NetworkBase deals with raw I/O, Network deals with higher level game commands
 * Network is also the API visible to the rest of the code, whereas NetworkBase is exclusively private
 */
public class Network : NetworkBase
{
	// callback dictionary for parsing incoming commands
	private static Dictionary<int, Func<string, NetworkCommand>> decoderCallbacks = new Dictionary<int, Func<string, NetworkCommand>>()
	{
		[MoveCommand.ID] 		= MoveCommand		.ConvertFromString,
		[ReadyCommand.ID] 		= ReadyCommand		.ConvertFromString,
		[NicknameCommand.ID] 	= NicknameCommand	.ConvertFromString,
		[JoinCommand.ID] 		= JoinCommand		.ConvertFromString,
		[StartCommand.ID]		= StartCommand		.ConvertFromString,
		[EndCommand.ID] 		= EndCommand		.ConvertFromString,
		[DisconnectCommand.ID] 	= DisconnectCommand	.ConvertFromString,
		[ClassnameCommand.ID]	= ClassnameCommand	.ConvertFromString,
		[UpdateClientInfoCommand.ID] = UpdateClientInfoCommand.ConvertFromString,
		[AttackCommand.ID]		= AttackCommand		.ConvertFromString,
		[SetSeedCommand.ID]		= SetSeedCommand	.ConvertFromString,
		[WaitCommand.ID]		= WaitCommand		.ConvertFromString,
		[InteractCommand.ID]	= InteractCommand	.ConvertFromString,
		[UseItemCommand.ID]		= UseItemCommand	.ConvertFromString
	};
	
	private static List<Client> peers = new List<Client>();
	
    public static void submitCommand(NetworkCommand cmd)
	{
		//Debug.Log("Sumbitting command [NetworkAccess]");
		submit(NetworkCommand.assembleCommandBytes(cmd));
	}
	
	public static List<NetworkCommand> getPendingCommands()
	{
		if (hasPending()) {
			
			List<NetworkCommand> cmds = new List<NetworkCommand>();
			foreach(byte[] command in getReceived()) {
				string[] cmdBits = Encoding.ASCII.GetString(command).Split('$');
				int ID = int.Parse(cmdBits[0]);
				NetworkCommand decodedCmd = decoderCallbacks[ID](cmdBits[1]);
				cmds.Add(decodedCmd);
			}
			return cmds;
		}
		else return null;
	}
	
	public static void connectToServer(string ip, int port)
	{
		networkEnabled = true;
		connect(ip, port);
	}
	
	public static void disconnectFromServer()
	{
		networkEnabled = false;
		peers.Clear();
		disconnect();
	}
	
	public static bool connected()
	{
		return networkEnabled;
	}
	
	public static List<Player> getPlayers()
	{
		List<Player> players = new List<Player>();
		foreach (Client peer in getPeers()) players.Add(peer.playerObject);
		return players;
	}
	
	public static Player getPlayer(int clientID)
	{
		return getPeer(clientID).playerObject;
	}
	
	public static int playerCount()
	{
		return peers.Count;
	}
	
	public static bool allPlayersReady()
	{
		foreach (Client player in getPeers()) {
			if (!player.ready) return false;
		}
		return true;
	}
	
	public static List<Client> getPeers()
	{
		// sorts list by ID before returning so that list order is the same on all clients
		peers.Sort(delegate(Client c1, Client c2){ return c1.ID.CompareTo(c2.ID); });
		return peers;
	}
	
	public static Client localClient()
	{
		return getPeer(NetworkManager.clientID);
	}
	
	public static Client getPeer(int ID)
	{
		foreach (Client p in peers) if (p.ID == ID) return p;
		return null;
	}
	
	public static void setPeer(int ID)
	{
		foreach (Client p in peers) if (p.ID == ID) return;
		peers.Add(new Client(ID));
	}
	
	public static void removePeer(int ID)
	{
		Client toRemove = null;
		foreach (Client p in peers) if (p.ID == ID) { toRemove = p; break; }
		peers.Remove(toRemove);
		
		// modify all client IDs to reflect this removal
		foreach (Client p in peers) if (p.ID >= ID) p.ID--;
	}
}