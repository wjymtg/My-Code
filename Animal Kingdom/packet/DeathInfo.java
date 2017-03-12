package packet;

public class DeathInfo extends Info {

	/**
	 * 
	 */
	private static final long serialVersionUID = 3;
	private int ID;
	DeathInfo(){
		
	}
	public DeathInfo(int ID) {
		super("");
		// TODO Auto-generated constructor stub
		this.ID = ID;
	}
	
	public int getID(){
		return ID;
	}
}
