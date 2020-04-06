using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class ServerListSocket : MonoBehaviour
{
    //Master server info
    public string masterIP = "127.0.0.1";
    public int masterPort = 1338;
    //Local
    public string localIP = "127.0.0.1";
    public int localPort = 1339;

    private string _label;
    private Thread _t1;
    private Thread _t2;
    private bool _t1Paused = false;
    private bool _t2Paused = false;

    public float timeWaiting = 5.0f;

    void Start()
    {
        //ListFunction();
    }

    private void ListFunction()
    {
        TcpClient client = new TcpClient(masterIP, masterPort);
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientSocket.Connect(masterIP, masterPort);

        if (client.Connected)
        {
            Debug.Log("Client connected with java master list server");
            // Translate the passed message into ASCII and store it as a Byte array.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes("Hello java friend");

            // Get a client stream for reading and writing.

            NetworkStream stream = client.GetStream();

            // Send the message to the connected TcpServer. 
            stream.Write(data, 0, data.Length);

            Debug.Log("Sent: " + Encoding.ASCII.GetString(data));

            // Receive the TcpServer.response.
            //TcpClient tclient = tcpListener.AcceptTcpClient();
            //NetworkStream tstream = tclient.GetStream();
            // Buffer to store the response bytes.
            Byte[] Indata = new Byte[25600];

            // String to store the response ASCII representation.
            String responseData = String.Empty;
            // Read the first batch of the TcpServer response bytes.
            //Int32 bytes = stream.Read(Indata, 0, Indata.Length);
            Indata = new byte[client.ReceiveBufferSize];

            var networkThread = new Thread(() =>
            {
                var session = new NetClientSession(clientSocket);
                byte[] receiveBytes = new byte[10000];
                //ConsoleKeyInfo cki = new ConsoleKeyInfo();

                Socket sender = new Socket(AddressFamily.Unspecified, SocketType.Stream, ProtocolType.Tcp);

                IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Parse(masterIP), masterPort);
                sender.Connect(remoteIpEndPoint);
                Debug.Log("wot");
                sender.Receive(receiveBytes);
                Debug.Log(22);
                        string returnData = Encoding.ASCII.GetString(receiveBytes);
                        Debug.Log(returnData);

            })
            {
                Priority = System.Threading.ThreadPriority.Normal
            };


            networkThread.Start();
        }
    }
}

internal class NetClientSession
{
    private Socket socket;
    private byte[] buffer = new byte[4092];

    StringBuilder sb = new StringBuilder();

    public NetClientSession(Socket client)
    {
        if (client == null)
        {
            throw new InvalidOperationException();
        }

        socket = client;
    }

    public void StartListening()
    {
        //socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveDataCallback, null);
        //socket.Receive(buffer);
        Debug.Log("wob");
        byte[] bytesReceived = new byte[102400];
        int bytes = 0;
        do
        {
            bytes = socket.Receive(bytesReceived, bytesReceived.Length, 0);
            Debug.Log("wob");
            sb.Append(Encoding.ASCII.GetString(bytesReceived, 0, bytes));
            Debug.Log("WOBBLE");
        }
        while (bytes == bytesReceived.Length);
        String rcv = System.Text.Encoding.ASCII.GetString(buffer);
        Debug.Log("wotowt" + rcv);
    }

    public void ReceiveDataCallback(IAsyncResult ar)
    {
        Debug.Log("wotot");
        try
        {
            Debug.Log("wotowt");
            var packetLen = socket.EndReceive(ar);
            Debug.Log("wotowt");
            if (packetLen == 0)
            {
                Debug.Log("rip");
                return;
            }
            Debug.Log("wotowt");
            var data = new byte[packetLen];
            Buffer.BlockCopy(buffer, 0, data, 0, data.Length);

            Debug.Log(data);
            Debug.Log("wotowt");
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveDataCallback, null);
            Debug.Log("wotowt");
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            throw;
        }
    }
}