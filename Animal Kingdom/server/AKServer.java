package server;

import java.io.IOException;
import java.util.HashMap;
import java.util.Map;
import java.util.Vector;

import com.esotericsoftware.kryonet.Connection;
import com.esotericsoftware.kryonet.Listener;
import com.esotericsoftware.kryonet.Server;

import character.BasePlayer;
import game_logic.JDBCAuthentication;
import packet.BossTargetsInfo;
import packet.DamageInfo;
import packet.DeathInfo;
import packet.DisconnectionInfo;
import packet.EnemyInfo;
import packet.Info;
import packet.Packet;
import packet.PlayerInfo;
import packet.SwitchToAvatarScreenMessage;
import packet.UpdateDatabase;
import packet.UpdateGamesPlayedStats;
import packet.UserAuthenticationError;
import packet.UserInfo;
import util.KryoInitializer;


public class AKServer extends Thread{

	private final static int NumberOfPlayers = 4;

	
	private Vector<PlayerInfo> players;
	private Server server;
	private Map<Connection, String> namesMap;
	private Map<Integer, Integer> enemiesMap;
	private Map<String, Integer> playersMap;
	
	private Boolean enemyReceived;
	private JDBCAuthentication jdbc;
	
	public AKServer(){
		enemyReceived = false;
		enemiesMap = new HashMap<Integer,Integer>();
		playersMap = new HashMap<String, Integer>();
		namesMap = new HashMap<Connection, String>();
		players = new Vector<PlayerInfo>();
		jdbc = new JDBCAuthentication();
		server = new Server();
		server.start();
		try {
			server.bind(6666, 9999);
			KryoInitializer.register(server.getKryo());
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		server.addListener(new Listener() {
		       public void received (Connection connection, Object object) {
		    	  if(object instanceof PlayerInfo){
		    		  String name = ((PlayerInfo)object).getUserInfo().getUsername();
		    		  System.out.println(name+" has connected, he is the "+server.getConnections().length+"th player");
		    		  namesMap.put(connection, name);
		    		  players.add((PlayerInfo)object);
		    	  }

		    	  else if(object instanceof Packet){
		        	  processPacket((Packet)object);
		          }
		    	  else if (object instanceof UpdateGamesPlayedStats) {
		    		  UpdateGamesPlayedStats m = (UpdateGamesPlayedStats) object;
		    		  jdbc.updateGameStats(m.getUserInfo(), m.getUserInfo().getGamesPlayed() + 1, m.getUserInfo().getGamesWon());
		    	  }

		    	  else if (object instanceof UserInfo) {
		    		  UserInfo ui = (UserInfo)object;
		    		  System.out.println("server received user info!");
		    		  System.out.println("username: " + ui.getUsername());
		    		  System.out.println("password: " + ui.getPassword());
		    		  System.out.println("games played: " + ui.getGamesPlayed());
		    		  System.out.println("games won: " + ui.getGamesWon());
		    		  ui.setGamesPlayed(jdbc.getGamesPlayed(ui));
		    		  ui.setGamesWon(jdbc.getGamesWon(ui));
		    		  if (ui.isLoggingIn()) {
		    			  System.out.println("user is logging in");
		    			  if (jdbc.containsUser(ui)) {
		  					if (!jdbc.isValidUser(ui)) {
		  						// send error message to client
		  						server.sendToTCP(connection.getID(), new UserAuthenticationError("Error: This password is incorrect for given user"));
		  						//errorMessage.setText("Error: This password is incorrect for given user");
		  					}
		  					else {
		  						//server.sendToTCP(connection.getID(), new UpdateUserInfo(ui));
		  						//ui.setGamesPlayed(jdbc.getGamesPlayed(ui));
		  						server.sendToTCP(connection.getID(), new SwitchToAvatarScreenMessage(ui));
		  						//switchToGame(ui);
		  					}
		  				}
		  				else {
		  					// send error message to client
		  					server.sendToTCP(connection.getID(), new UserAuthenticationError("Error: This username does not exist"));
		  					//errorMessage.setText("Error: This username does not exist");
		  				}
		    		  } 
		    		  else if (ui.isCreatingAccount()) {
		    			  System.out.println("user is creating an account");
		    			  // create account logic
		    			  if (jdbc.containsUser(ui)) {
		    				    server.sendToTCP(connection.getID(), new UserAuthenticationError("Error: A user with this username already exists"));
								//errorMessage.setText("Error: A user with this username already exists");
							}
							else {
								jdbc.writeToDatabase(ui);	
								server.sendToTCP(connection.getID(), new SwitchToAvatarScreenMessage(ui));
								System.out.println("sent switch to avatar screen message to client from server");
								//switchToGame(ui);
							}
		    		  } 
		    		  else if (ui.isPlayingAsGuest()) {
		    			  System.out.println("user is playing as a guest");
		    			  // play as guest logic
		    			  server.sendToTCP(connection.getID(), new SwitchToAvatarScreenMessage(ui));
		    			  //switchToGame(ui);	
		    		  }
		    	  }
		    	  else if(object instanceof Info){
		    		  parseInfo((Info)object);
		    	  }
		    	  else if (object instanceof UpdateDatabase) {
		    		  UpdateDatabase m = (UpdateDatabase) object;
		    		  jdbc.updateGameStats(m.getUserinfo(), m.getGamesplayed(), m.getGameswon());
		    	  }
		       }
		       //Handle disconnection
		       public void disconnected(Connection connection){
		    	   System.out.println(namesMap.get(connection)+ " has disconnected");
		    	   server.sendToAllTCP(new DisconnectionInfo(namesMap.get(connection)));
		    	   removePlayer(namesMap.get(connection));
		    	   namesMap.remove(connection);
		       }
		       
		    });	
		System.out.println("Server setup");
		
		
		//start
		start();
	}
	
	
	public void addPlayer(PlayerInfo info){
		players.addElement(info);
	}

	public synchronized void removePlayer(String name){
		for(PlayerInfo info : players){
			if(info.getUserInfo().getUsername().equals(name)){
				players.remove(info);
				return;
			}
		}
	}

	@Override
	public void run() {
		// TODO Auto-generated method stub
		while(players.size() != NumberOfPlayers){
			try {
				sleep(1000);
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
		//enough players, start
		System.out.println("I got "+NumberOfPlayers+" players! And they are ");
		Packet p = new Packet();
		p.setMessage("Character Names");
		for(PlayerInfo info: players){
			p.addInfo(info);
			//put the players into the map
			BasePlayer bp = new BasePlayer();
			playersMap.put(info.getUserInfo().getUsername(), 160);
			System.out.print(info.getUserInfo().getUsername()+ " ");
		}
		System.out.println("");
		//sending names
		server.sendToAllTCP(p);
	}
	

	
	public void applyDamage(DamageInfo dmgInfo){
		//process dmg
		int UID = dmgInfo.getID();
//		enemiesMap.put(63, 100);
//		Packet packet = new Packet("Create Boss");
//		packet.addInfo(new BossTargetsInfo("",players));
//		server.sendToAllTCP(packet);
		if(enemiesMap.get(UID) == null) return;
		int hp = enemiesMap.get(UID).intValue() - dmgInfo.getDamage();
		enemiesMap.put(UID, hp);
		System.out.println("Character "+ UID+" takes "+dmgInfo.getDamage()+" dmg");
		//broadcast the death
		if(hp <= 0){
			server.sendToAllTCP(new DeathInfo(UID));
			System.out.println("Character "+ UID+" is dead!");
			enemiesMap.remove(UID);
			System.out.println("There are " + enemiesMap.size() + " enemies left.");
			//check for creating boss
			if(enemiesMap.size() == 0){
				enemiesMap.put(63, 100);
				Packet packet = new Packet("Create Boss");
				packet.addInfo(new BossTargetsInfo("", players));
				server.sendToAllTCP(packet);
			}
		}
	}
	
	public void processPacket(Packet p){
		for(Info info : p.getInfo()){
			parseInfo(info);
		}
	}
	
	public void parseInfo(Info info){
		if(info instanceof PlayerInfo){
			addPlayer((PlayerInfo)info);
		}
		else if(info instanceof DamageInfo){
			applyDamage((DamageInfo)info);
		}
		
		else if(info instanceof EnemyInfo){
			if(!enemyReceived){
				Vector<Integer> enemies = ((EnemyInfo)info).getEnemies();
				for(int i = 0; i < enemies.size()-1; i++){
					enemiesMap.put(enemies.get(i), 160);
				}
			}
		}
		else if(!(info instanceof UserInfo))
			server.sendToAllTCP(info);
	}
	
	
	public static void main(String [] args){
		new AKServer();
		
	}
}
