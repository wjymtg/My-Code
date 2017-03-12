package packet;

import com.badlogic.gdx.math.Vector2;

public class PositionInfo extends Info {

	/**
	 * 
	 */
	private static final long serialVersionUID = 8989;
	private Vector2 linVel;
	private Vector2 pos;
	private String name;
	private double health;
	
	PositionInfo(){
		super();
	}
	
	public PositionInfo(Vector2 linVel, Vector2 pos, String name, double hp){
		super();
		health = hp;
		this.linVel = linVel;
		this.pos = pos;
		this.name = name;
	}
	
	public Vector2 getLinVel(){
		return linVel;
	}
	
	public Vector2 getPos(){
		return pos;
	}
	
	public String getName(){
		return name;
	}
	
	public double getHealth(){
		return health;
	}
}

