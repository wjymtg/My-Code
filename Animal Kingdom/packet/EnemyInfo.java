package packet;

import java.util.Vector;

import character.Boss;
import character.Enemy;

public class EnemyInfo extends Info {
	/**
	 * 
	 */
	private static final long serialVersionUID = 7;
	private Vector<Integer> enemies;
	
	EnemyInfo(){
		super("");
	}
	public EnemyInfo(Vector<Enemy> enemies){
		this.enemies = new Vector<Integer>();
		Integer BossIndex = null;
		for(Enemy enemy : enemies){
			if(!(enemy instanceof Boss))
				this.enemies.add(enemy.getUID());
			else
				BossIndex = enemy.getUID();
				
		}
		this.enemies.add(BossIndex);
	}
	
	public Vector<Integer> getEnemies(){
		return enemies;
	}
}
