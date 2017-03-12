package client;

import java.io.IOException;
import java.net.InetAddress;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.Vector;

import javax.swing.JFrame;

import com.badlogic.gdx.Game;
import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.esotericsoftware.kryonet.Client;
import com.esotericsoftware.kryonet.Connection;
import com.esotericsoftware.kryonet.Listener;

import character.BasePlayer;
import gui.AvatarScreen;
import gui.BaseFrame;
import gui.LoginPage;
import packet.BossTargetsInfo;
import packet.DeathInfo;
import packet.DisconnectionInfo;
import packet.Info;
import packet.InputInfo;
import packet.PlayerInfo;

import packet.PositionInfo;
import packet.SwitchToAvatarScreenMessage;
import packet.UpdateUserInfo;
import packet.UserAuthenticationError;
import packet.UserInfo;

import packet.Packet;
import screens.PlayScreen;
import util.KryoInitializer;


public class AKClient extends Game{
	public SpriteBatch batch;
	//Texture img;
	private static String[] Names = {"Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Zeta"};
	private String name;
	private static int count = 0;
	//client
	private Client client;
	private BasePlayer me;
	private UserInfo userInfo;
	
	private Vector<PlayerInfo> players;
	
	private BaseFrame frame;
	private LoginPage loginPage;
	private AvatarScreen avatarScreen;
	
	
	public static final int V_WIDTH = 800;
	public static final int V_HEIGHT = 408;
	public static final float PPM = 100;
	
	public static final short PLAYER_BIT = 1;
	public static final short ENEMY_BIT = 2;
	public static final short MAP_BIT = 4;
	
	public AKClient(){
	}
	
	@Override
	public void create () {
		//initialize name
		players = new Vector<PlayerInfo>();
		batch = new SpriteBatch();
		
		client = new Client(100000, 100000);
		client.start();
		try {
			List<InetAddress> addresses = client.discoverHosts(9999, 5000);
			System.out.println(addresses.size());
			InetAddress addr = null;
			for(InetAddress a : addresses){
				System.out.println(a.getHostName());
				if(!a.isLoopbackAddress()){
					addr = a;
					break;
				}
				if(a.getHostAddress().equals("127.0.0.1")){
					addr = a;
					break;
				}
			}
			if(addr == null)
				client.connect(100000000, "localhost", 6666, 9999);
			else
				client.connect(100000000, addr, 6666, 9999);
				
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		//register classes
		KryoInitializer.register(client.getKryo());		
		
		client.addListener(new Listener() {
			public void received (Connection connection, Object object) {
				if(object instanceof Packet){
					processPacket((Packet)object);
				}
				else if(object instanceof Info){
					parseInfo((Info)object);
				}
				else if (object instanceof UserAuthenticationError) {
					UserAuthenticationError uar = (UserAuthenticationError)object;
					loginPage.setErrorMessage(uar.getResponse());
				}
				else if (object instanceof SwitchToAvatarScreenMessage) {
					SwitchToAvatarScreenMessage m = (SwitchToAvatarScreenMessage)object;
					userInfo = m.getUi();
					name = m.getUi().getUsername();
					loginPage.switchToAvatarScreen(m.getUi());
					System.out.println("client received the switch to avatar screen message and my name is "+ name);
				}
				else if (object instanceof UpdateUserInfo) {
					UpdateUserInfo m = (UpdateUserInfo) object;
					userInfo = m.getUserInfo();
				}
			}
		});
		
		frame = new BaseFrame(this);
		frame.setVisible(true);

	}
	
	@Override
	public void render () {
		super.render();
	}
	
	@Override
	public void dispose () {
		batch.dispose();
	}
	
	public Client getClient(){
		return client;
	}
	
	public String getName(){
		return name;
	}
	
	public BaseFrame getBaseFrame(){
		return frame;
	}
	
	public void handleInput(Boolean up, Boolean right, Boolean left, Boolean attack){
		client.sendTCP(new InputInfo(name, up, right, left, attack));
	}
	
	public void startPlayScreen(){
		System.out.println("Players are ");
		for(PlayerInfo info : players){
			System.out.print(info.getUserInfo().getUsername() + " ");
		}
		setScreen(new PlayScreen(this, players, userInfo.getGamesPlayed(), userInfo.getGamesWon()));
		//((PlayScreen)screen).setGameStats(userInfo.getGamesPlayed(), userInfo.getGamesWon());
	}
	
	public void sendUserInfo(String type, String weapon, UserInfo userInfo, Boolean isGuest){
		//this.userInfo = userInfo;
		client.sendTCP(new PlayerInfo(type, weapon, userInfo, isGuest));
	}
	
	
	public void setLoginPage(LoginPage loginPage) {
		this.loginPage = loginPage;
	}

	public void setAvatarScreen(AvatarScreen as){
		avatarScreen = as;
	}
	
	public UserInfo getUserInfo() {
		return userInfo;
	}

	public void setName(String str){
		name = str;
	}

	private void processPacket(Packet p){
		for(Info info : p.getInfo()){
			parseInfo(info);
		}
		
		if(p.getMessage() != null){
			//show the gameplay screen
			if(p.getMessage().equals("Character Names")){
				System.out.println("Setup screen");
				if(avatarScreen != null){
					avatarScreen.setWaitingLabel("Initializing");
				}
				character.Character.resetCount();
				new Thread(new Runnable() {
					   @Override
					   public void run() {
					      // do something important here, asynchronously to the rendering thread
					      // post a Runnable to the rendering thread that processes the result
					      Gdx.app.postRunnable(new Runnable() {
					         @Override
					         public void run() {
					            // process the result, e.g. add it to an Array<Result> field of the ApplicationListener.
					           startPlayScreen();
					         }
					      });
					   }
					}).start();
			}
			//Create the boss
			else if(p.getMessage().equals("Create Boss")){
				((PlayScreen)screen).createBoss();
			}
			//Game over
			else if(p.getMessage().equals("Game over")){
				// TODO display game over panel or what I guess?
				System.out.println("Game over!");
			}
		}
	}
	
	
	private void parseInfo(Info info){
		if(info instanceof DeathInfo){
			if(screen != null)
				((PlayScreen)screen).processDeath(((DeathInfo)info).getID());
		}
		else if(info instanceof DisconnectionInfo){
			if(screen != null)
				((PlayScreen)screen).handleDisconnection(((DisconnectionInfo)info).getInfo());
		}
		else if(info instanceof PlayerInfo){
			players.add(((PlayerInfo)info));
		}
		else if(info instanceof InputInfo){
			//update only other players
			if(screen != null)
				((PlayScreen)screen).updatePlayer((InputInfo)info);
		}
		else if(info instanceof PositionInfo){
			if(screen != null){
				((PlayScreen)screen).updatePlayerPos((PositionInfo)info);
			}
		}
		else if(info instanceof BossTargetsInfo){
			if(screen != null){
				((PlayScreen)screen).setTargets(((BossTargetsInfo)info).getTargets());
			}
			for(String str : ((BossTargetsInfo)info).getTargets())
				System.out.println(str);
		}
	}
}
