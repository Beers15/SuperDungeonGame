using System;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Static class that deals with low-level network I/O
 * Responsible for sending data to and receiving data from the game server, as well as connecting and disconnecting from said server
 */

public class NetworkBase
{
	protected static bool networkEnabled = false;
	
	static TcpClient client = null;
	static Thread listenThread = null;
	static Thread sendThread = null;
	static bool threadShouldTerminate = false;
	
	static Queue<byte[]> toSend = new Queue<byte[]>();
	static Queue<byte[]> received = new Queue<byte[]>();
	
	protected static void connect(string ip, int port)
	{
		disconnect(); // closes current connection & threads, if they exist
		
		Debug.Log("Connecting to server... IP: " + ip + " at port " + port);
		client = new TcpClient(ip, port);
		
		threadShouldTerminate = false;
		listenThread = new Thread(clientListen);
		sendThread = new Thread(clientSend);
		
		listenThread.Start();
		sendThread.Start();
	}
	
	protected static void disconnect()
	{
		if (client != null && client.Connected) {
			Debug.Log("Terminating listen/send threads and disconnecting from server...");
			threadShouldTerminate = true;
			
			while (listenThread.IsAlive || sendThread.IsAlive) Thread.Sleep(1);
			
			client.GetStream().Close();
			client.Close();
			
			toSend.Clear();
			received.Clear();
		}
	}

	// infinite loop that listens for messages from the server
    static void clientListen() 
	{
		NetworkStream streamIn = client.GetStream();
		while (!threadShouldTerminate) {
			if (streamIn.DataAvailable) {
				
				byte[] lenBytes = new byte[2];
				streamIn.Read(lenBytes, 0, 2);
				short len = (short) (((short)lenBytes[0] << 8) | ((short)lenBytes[1] << 0));
				
				byte[] buffer = new byte[len];
				streamIn.Read(buffer, 0, len);
				received.Enqueue(buffer);
			}
			Thread.Sleep(1);
		}
	}
	
	// infinite loop that sends messages to the server
	static void clientSend()
	{
		NetworkStream streamOut = client.GetStream();
		while (!threadShouldTerminate) {
			if (toSend.Count > 0) {
				while (toSend.Count > 0) {
					
					byte[] messageBytes = toSend.Dequeue();
					streamOut.Write(messageBytes, 0, messageBytes.Length);
				}
			}
			Thread.Sleep(1);
		}
		byte[] disconnect = NetworkCommand.assembleCommandBytes(new DisconnectCommand(0));
		streamOut.Write(disconnect, 0, disconnect.Length); // sends DC packet, signalling client disconnect from server
	}
	
	protected static bool hasPending()
	{
		return received.Count > 0;
	}
	
	protected static void submit(byte[] command)
	{
		// enqueue new message to be sent if network is enabled
		if (networkEnabled) {
			toSend.Enqueue(command);
		}
		else { // if network is disabled (i.e singleplayer) immediately receive submitted messages
			// remove the directive & length bytes in the command
			//Debug.Log("Submitting command [NetworkBase]");
			byte[] moddedCmd = new byte[command.Length - 3];
			Array.Copy(command, 3, moddedCmd, 0, command.Length - 3);
			received.Enqueue(moddedCmd);
		}
	}
	
	protected static byte[][] getReceived()
	{
		byte[][] receivedArray = new byte[received.Count][];
		for (int i = 0; i < receivedArray.Length; i++)
			receivedArray[i] = received.Dequeue();
		return receivedArray;
	}
}
