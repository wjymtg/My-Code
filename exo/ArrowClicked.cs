using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowClicked : MonoBehaviour {
    public bool isEnabled;
    private NavGameController nav;

	// Use this for initialization
	void Start () {
        nav = GameObject.Find("Nav Controller").GetComponent<NavGameController>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseDown()
    {
        // this button was clicked
        if (isEnabled)
        {
            string str = gameObject.name.Substring(6);
            if (str.Equals("Up"))
                nav.VoteForDirection(NavGameController.Direction.Up);
            else if (str.Equals("Down"))
                nav.VoteForDirection(NavGameController.Direction.Down);
            else if (str.Equals("Left"))
                nav.VoteForDirection(NavGameController.Direction.Left);
            else if (str.Equals("Right"))
                nav.VoteForDirection(NavGameController.Direction.Right);
        }
    }
}
