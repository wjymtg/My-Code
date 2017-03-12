package packet;

public class PlayerInfo extends Info {
	/**
	 * 
	 */
	private static final long serialVersionUID = 2;
	private String type;
	private String weapon;
	private Boolean isGuest;
	private UserInfo userInfo;
	
	PlayerInfo(){
		super();
	}
	
	public PlayerInfo(String type, String weapon, UserInfo userInfo, Boolean isGuest) {
		super("");
		// TODO Auto-generated constructor stub
		this.type = type;
		this.weapon = weapon;
		this.userInfo = userInfo;
		this.isGuest  = isGuest;
	}
	
	public String getType(){
		return type;
	}
	public String getWeapon(){
		return weapon;
	}
	
	public UserInfo getUserInfo() {
		return userInfo;
	}

	public Boolean isGuest(){
		return isGuest;
	}
	
}
