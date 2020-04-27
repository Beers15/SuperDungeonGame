import java.io.File;
import java.io.IOException;
import java.net.Socket;
import java.net.URISyntaxException;
import java.sql.Connection;
import java.net.ServerSocket;
import java.util.concurrent.Executors;
import java.util.concurrent.ExecutorService;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.List;
import java.util.ArrayList;

enum Directive
{ START, END, DISCONNECT, ECHO; public static Directive fromByte(byte b) { return Directive.values()[b];} }; // enum starts$

public class DungeonServerNoSQL {

    private static final int PORT = 1337;

    private static ServerSocket server;
    private static final ExecutorService clientThreads = Executors.newCachedThreadPool();
    private static final List<DungeonClient> activeClients = new ArrayList<>();
    
    private static boolean acceptClients = true;
    private static int maxClients = 4;
    private static int currentClients = 0;
    private static String serverIdentifier = "NA01";
    private static boolean connectedtoSQL = false;
    
    Connection connection;

    private static void echoToClient(DungeonClient client, byte[] buffer)
    {
        try {
            if (!client.socket.isClosed()) {
                byte[] lenBytes = new byte[] { (byte)(buffer.length >> 8), (byte)(buffer.length) };
                OutputStream out = client.socket.getOutputStream();
                out.write(lenBytes);
                out.write(buffer);
            }
        }
        catch (IOException e1) {
            System.out.println("Output stream closed by: " + client);
            try { client.socket.close();}
            catch (IOException e2) { System.out.println(e2.toString()); }
        }
    }

    private static void echoToAll(byte[] buffer)
    {
        // waits for other threads to finish their work
        synchronized(activeClients) {
            // echoes packet to all other clients
            for (DungeonClient client : activeClients) {
                echoToClient(client, buffer);
            }
        }
    }

    private static class DungeonClient implements Runnable
    {
        public final Socket socket;
        private final int ID;
        public DungeonClient(Socket socket, int ID)
        { this.socket = socket; this.ID = ID; }

        @Override
        public void run()
        {
            try {
                // sends join signal to client, then starts listening loop
                System.out.println("run DungeonClient");
                echoToAll(("-2$" + Integer.toString(ID)).getBytes());

                InputStream in = socket.getInputStream();
                while (!socket.isClosed()) 
                {

                    if (in.available() > 0) {
                        Directive d = Directive.fromByte((byte)in.read());
                        // gets length of packet to read
                        short length = (short)((in.read() << 8) | in.read());
                        byte[] buffer = new byte[length];
                        in.read(buffer, 0, length);
                        System.out.println(this + " sent: " + new String(buffer));
                        
                        switch (d) {
                        case START:
                            // don't accept any more clients for this server
                            acceptClients = false;
                            echoToAll(buffer);
                            break;
                        case END:
                            // accept more clients to this server
                            acceptClients = true;
                            echoToAll(buffer); break;
                        case DISCONNECT:
                            // end this client's connection
                            socket.close(); break;
                        case ECHO:
                            // writes packet to buffer
                        	acceptClients = true;
                            echoToAll(buffer); break;
                        }
                    } else
                    {
                    	
                    }
                    
                    Thread.sleep(1); // prevents excessive thread cpu usage
                }
                Thread.sleep(1); // prevents excessive thread cpu usage
            }
            catch (IOException | InterruptedException e) {
                System.out.println(e + " experienced an error");
                e.printStackTrace(System.out);
            }
            finally 
            {
                // waits for other threads to finish using activeClients
                synchronized(activeClients) { activeClients.remove(this); }
                System.out.println(this + " thread terminated!");
                echoToAll(("-1$" + Integer.toString(ID)).getBytes()); // sends disconnect signal to all other clients
                
                if(activeClients.size() == 0) //Restart whole jar file on last client quit to avoid some errors.
                {
					return;
                }
            }
        }

        @Override
        public String toString() { return "Client #" + Integer.toString(ID); }
        
    	}

    public static void main(String[] args) throws IOException
    {
        try {
            server = new ServerSocket(PORT);
            server.setReuseAddress(true);
            System.out.println("Server starting on port " + PORT + "...");

        	System.out.println("Not connecting to database.");
            while (activeClients.size() <= maxClients) {
				if (acceptClients && activeClients.size() <= maxClients) {
					DungeonClient newClient = new DungeonClient(server.accept(), activeClients.size() + 1);

					System.out.println(newClient + " Connected!");
					activeClients.add(newClient);
					clientThreads.submit(newClient);
					System.out.println("Number of active threads: " + activeClients.size());
				} else {
					System.out.println("Max clients reached(?): " + acceptClients);
				}
            }
        } catch (IOException e)
        {
        	e.printStackTrace();
        }
    }
}