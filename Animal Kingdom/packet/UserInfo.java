package packet;

import java.io.Serializable;

public class UserInfo extends Info implements Serializable {

	private static final long serialVersionUID = 1L;
	
	private String username;
	private String password;
	private Integer gamesPlayed;
	private Integer gamesWon;
	private boolean isLoggingIn;
	private boolean isCreatingAccount;
	private boolean isPlayingAsGuest;
	
	public UserInfo(String username, String password, Integer gamesPlayed, Integer gamesWon, boolean isLoggingIn, boolean isCreatingAccount, boolean isPlayingAsGuest) {
		this.username = username;
		this.password = password;
		this.gamesPlayed = gamesPlayed;
		this.gamesWon = gamesWon;
		this.isLoggingIn = isLoggingIn;
		this.isCreatingAccount = isCreatingAccount;
		this.isPlayingAsGuest = isPlayingAsGuest;
	}

	UserInfo() {}
	
	public boolean isLoggingIn() {
		return isLoggingIn;
	}

	public void setLoggingIn(boolean isLoggingIn) {
		this.isLoggingIn = isLoggingIn;
	}

	public boolean isCreatingAccount() {
		return isCreatingAccount;
	}

	public void setCreatingAccount(boolean isCreatingAccount) {
		this.isCreatingAccount = isCreatingAccount;
	}

	public boolean isPlayingAsGuest() {
		return isPlayingAsGuest;
	}

	public void setPlayingAsGuest(boolean isPlayingAsGuest) {
		this.isPlayingAsGuest = isPlayingAsGuest;
	}

	public String getUsername() {
		return username;
	}

	public void setUsername(String username) {
		this.username = username;
	}

	public String getPassword() {
		return password;
	}

	public void setPassword(String password) {
		this.password = password;
	}

	public Integer getGamesPlayed() {
		return gamesPlayed;
	}

	public void setGamesPlayed(Integer gamesPlayed) {
		this.gamesPlayed = gamesPlayed;
	}

	public Integer getGamesWon() {
		return gamesWon;
	}

	public void setGamesWon(Integer gamesWon) {
		this.gamesWon = gamesWon;
	}

	@Override
	public String toString() {
		return "UserInfo [username=" + username + ", password=" + password + ", gamesPlayed=" + gamesPlayed
				+ ", gamesWon=" + gamesWon + "]";
	}

}