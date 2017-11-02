using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Es.InkPainter;

public class Paintball : MonoBehaviour {
    public float lifespan = 0;
    int count = 0;

    private void Awake()
    {
        StartCoroutine(DestroyInSeconds(lifespan));    
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != "painttube")
            count++;
        if(count > 4)
            Destroy(gameObject);
    }

    public IEnumerator DestroyInSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
