package packet;

import java.io.Serializable;

public abstract class Info implements Serializable{
	/**
	 * 
	 */
	private static final long serialVersionUID = 1;
	private String info;
	Info(){
		
	}
	public Info(String info){
		this.info = info;
	}
	
	public String getInfo(){
		return info; 
	}
}
