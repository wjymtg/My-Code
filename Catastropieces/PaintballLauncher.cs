using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// attach this script to the paint can
public class PaintballLauncher : MonoBehaviour {

    public GameObject paintball;
    public int paintballNumber = 0;
    // spawn position relative to the gameObject this script is attached to
    public Vector3 relativePos = Vector3.zero;
    // the global launch direction, normalized in Start()
    public Vector3 launchDir = Vector3.zero;
    public float launchSpeed = 0;

	// Use this for initialization
	void Start () {
        launchDir.Normalize();
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    private void OnCollisionEnter(Collision collision)
    {

    }

    // launch a paintball in every fixed update
    public IEnumerator LaunchPaintballs()
    {
        for(int i = 0; i < paintballNumber; i++)
        {
            GameObject ball = Instantiate(paintball, gameObject.transform.position + relativePos, Quaternion.identity);
            Vector3 forceDir = launchDir + new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f);
            forceDir.Normalize();
            ball.GetComponent<Rigidbody>().AddForce(forceDir * launchSpeed, ForceMode.VelocityChange);
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
        }
    }

    public void Launch()
    {
        StartCoroutine("LaunchPaintballs");
    }

    public void StopLaunching()
    {
        StopCoroutine("LaunchPaintballs");
    }
}
