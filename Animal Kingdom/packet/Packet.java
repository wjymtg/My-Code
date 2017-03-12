package packet;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.List;

public class Packet implements Serializable{

	/**
	 * 
	 */
	private static final long serialVersionUID = 0;
	private List<Info> infos;
	private String message;
	
	public Packet(String message){
		this.message = message;
		infos = new ArrayList<Info>();
	}
	
	public Packet(){
		infos = new ArrayList<Info>();
	}
	
	public List<Info> getInfo(){
		return infos;
	}
	
	public void addInfo(Info info){
		infos.add(info);
	}
	
	public void setMessage(String str){
		message = str;
	}
	
	public String getMessage(){
		return message;
	}
}

