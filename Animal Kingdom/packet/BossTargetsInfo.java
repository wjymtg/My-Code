package packet;

import java.util.Random;
import java.util.Stack;
import java.util.Vector;

public class BossTargetsInfo extends Info{

	/**
	 * 
	 */
	private static final long serialVersionUID = 100233;
	private Stack<String> targets;
	
	BossTargetsInfo(){
		
	}
	
	public BossTargetsInfo(String info, Vector<PlayerInfo> players){
		super(info);
		Random rd = new Random();
		targets = new Stack<String>();
		Vector<String> names = new Vector<String>();
		for(PlayerInfo pi : players){
			names.add(pi.getUserInfo().getUsername());
		}
		//System.out.println("names size: "+names.size());
		for(int i = 0; i < 200; i++){
			int index = rd.nextInt(1000)%names.size();
			//System.out.println("Index"+index);
			targets.push(names.get(index));
		}
	}
	
	public Stack<String> getTargets(){
		return targets;
	}
}
