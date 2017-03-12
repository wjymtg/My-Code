package packet;

import com.badlogic.gdx.math.Vector2;

import character.BasePlayer;
import character.Player;

public class InputInfo extends Info {
	// the vectors of a certain player
	/**
	 * 
	 */
	private static final long serialVersionUID = 5;
	private Boolean up, right, left, attack;
	private String name;
	
	InputInfo(){
		
	}
	
	public InputInfo(String name, Boolean up, Boolean right, Boolean left, Boolean attack) {
		super("");
		// TODO Auto-generated constructor stub
		this.up = up;
		this.right = right;
		this.left = left;
		this.attack = attack;
		this.name = name;
	}
	
	//getters
	public String getName(){
		return name;
	}
	
	public Boolean getUp(){
		return up;
	}
	
	public Boolean getRight(){
		return right;
	}
	
	public Boolean getLeft(){
		return left;
	}
	
	public Boolean getAttack(){
		return attack;
	}
}
