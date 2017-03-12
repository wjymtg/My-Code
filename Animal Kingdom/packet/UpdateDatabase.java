package packet;

public class UpdateDatabase {
	
	private UserInfo userinfo;
	private int gamesplayed;
	private int gameswon;
	
	public UpdateDatabase(UserInfo userinfo, int gamesplayed, int gameswon) {
		this.userinfo = userinfo;
		this.gamesplayed = gamesplayed;
		this.gameswon = gameswon;
	}
	
	UpdateDatabase() {}

	public int getGamesplayed() {
		return gamesplayed;
	}

	public void setGamesplayed(int gamesplayed) {
		this.gamesplayed = gamesplayed;
	}

	public int getGameswon() {
		return gameswon;
	}

	public void setGameswon(int gameswon) {
		this.gameswon = gameswon;
	}

	public UserInfo getUserinfo() {
		return userinfo;
	}

	public void setUserinfo(UserInfo userinfo) {
		this.userinfo = userinfo;
	}

}
