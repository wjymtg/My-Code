package packet;

public class UpdateGamesPlayedStats {
	
	private UserInfo userInfo;
	
	public UpdateGamesPlayedStats(UserInfo userInfo) {
		this.userInfo = userInfo;
	}
	
	UpdateGamesPlayedStats() {}

	public UserInfo getUserInfo() {
		return userInfo;
	}

	public void setUserInfo(UserInfo userInfo) {
		this.userInfo = userInfo;
	}

}
