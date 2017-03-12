package packet;

public class SwitchToAvatarScreenMessage {
	
	private UserInfo ui;
	
	public SwitchToAvatarScreenMessage(UserInfo ui) {
		this.ui = ui;
	}
	
	SwitchToAvatarScreenMessage() {}

	public UserInfo getUi() {
		return ui;
	}

	public void setUi(UserInfo ui) {
		this.ui = ui;
	}
	
	

}
