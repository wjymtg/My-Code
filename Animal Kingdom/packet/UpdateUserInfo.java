package packet;

public class UpdateUserInfo {
	
	private UserInfo userInfo;
	
	public UpdateUserInfo(UserInfo userInfo) {
		this.userInfo = userInfo;
	}
	
	UpdateUserInfo() {}

	public UserInfo getUserInfo() {
		return userInfo;
	}

	public void setUserInfo(UserInfo userInfo) {
		this.userInfo = userInfo;
	}

}
