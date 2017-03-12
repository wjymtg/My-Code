package packet;

public class DamageInfo extends Info {
	/**
	 * 
	 */
	private static final long serialVersionUID = 4;
	private int ID;
	private int dmg;
	
	DamageInfo(){
	
	}
	
	public DamageInfo(String info, int ID, int dmg) {
		super(info);
		// TODO Auto-generated constructor stub
		this.ID = ID;
		this.dmg = dmg;
		
	}

	public int getID(){
		return ID;
	}
	
	public int getDamage(){
		return dmg;
	}
}
