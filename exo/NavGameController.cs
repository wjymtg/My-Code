using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavGameController : MonoBehaviour {
    public GameObject ArrowUp;
    public GameObject ArrowDown;
    public GameObject ArrowLeft;
    public GameObject ArrowRight;
    private GameObject prevArrow;

    private Room [] rooms;
    private int currentRoom;
    private GameObject icon;

    private Sprite[] bg;
    private GameObject background;

    string visibleLayer = "Obstacle";
    string invisibleLayer = "Default";

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    };

    private class Room
    {
        public int forward = -1, backward = -1, left = -1, right = -1, bgIndex = 0;
    }

    // Use this for initialization
    void Start () {
        prevArrow = null;
        icon = GameObject.Find("YouIcon");
        bg = new Sprite[4];
        background = GameObject.Find("Background");
        bg[0] = Resources.Load<Sprite>("Sprites/Nav map/Interior1");
        bg[1] = Resources.Load<Sprite>("Sprites/Nav map/Interior2");
        bg[2] = Resources.Load<Sprite>("Sprites/Nav map/Interior3");
        bg[3] = Resources.Load<Sprite>("Sprites/Nav map/Interior4");
        currentRoom = 0;
        rooms = new Room[5];
        for(int i = 0; i < 5; ++i)
        {
            rooms[i] = new Room();
        }
        rooms[0].forward = 1;
        rooms[1].forward = 3;
        rooms[1].backward = 0;
        rooms[1].right = 2;
        rooms[2].left = 1;
        rooms[3].forward = 4;
        rooms[3].backward = 1;
        rooms[4].backward = 3;

        rooms[0].bgIndex = 0;
        rooms[1].bgIndex = 1;
        rooms[2].bgIndex = 2;
        rooms[3].bgIndex = 3;
        rooms[4].bgIndex = 2;
        SwitchRoom(currentRoom);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void VoteForDirection(Direction dir)
    {
        //change arrow color and icon's position
        Vector3 pos = icon.GetComponent<Transform>().position;
        switch (dir)
        {
            case Direction.Up:
                ArrowUp.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                if(prevArrow != null)
                    prevArrow.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                prevArrow = ArrowUp;
                SwitchRoom(rooms[currentRoom].forward);
                currentRoom = rooms[currentRoom].forward;
                icon.GetComponent<Transform>().position = pos + new Vector3(0, 0.95f, 0);
                break;
            case Direction.Down:
                ArrowDown.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                if (prevArrow != null)
                    prevArrow.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                prevArrow = ArrowDown;
                SwitchRoom(rooms[currentRoom].backward);
                currentRoom = rooms[currentRoom].backward;
                icon.GetComponent<Transform>().position = pos + new Vector3(0, -0.95f, 0);
                break;
            case Direction.Left:
                ArrowLeft.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                if (prevArrow != null)
                    prevArrow.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                prevArrow = ArrowLeft;
                SwitchRoom(rooms[currentRoom].left);
                currentRoom = rooms[currentRoom].left;
                icon.GetComponent<Transform>().position = pos + new Vector3(-0.95f, 0, 0);
                break;
            case Direction.Right:
                ArrowRight.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                if (prevArrow != null)
                    prevArrow.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                prevArrow = ArrowRight;
                SwitchRoom(rooms[currentRoom].right);
                currentRoom = rooms[currentRoom].right;
                icon.GetComponent<Transform>().position = pos + new Vector3(0.95f, 0, 0);
                break;
        }
    }

    void SwitchRoom(int index)
    {
        
        //enable and disable arrow buttons
        if (rooms[index].forward != -1)
        {
            ArrowUp.GetComponent<ArrowClicked>().isEnabled = true;
            ArrowUp.GetComponent<SpriteRenderer>().sortingLayerName = visibleLayer;
        }
        else
        {
            ArrowUp.GetComponent<ArrowClicked>().isEnabled = false;
            ArrowUp.GetComponent<SpriteRenderer>().sortingLayerName = invisibleLayer;
        }
        if (rooms[index].backward != -1)
        {
            ArrowDown.GetComponent<ArrowClicked>().isEnabled = true;
            ArrowDown.GetComponent<SpriteRenderer>().sortingLayerName = visibleLayer;
        }
        else
        {
            ArrowDown.GetComponent<ArrowClicked>().isEnabled = false;
            ArrowDown.GetComponent<SpriteRenderer>().sortingLayerName = invisibleLayer;
        }
        if (rooms[index].left != -1)
        {
            ArrowLeft.GetComponent<ArrowClicked>().isEnabled = true;
            ArrowLeft.GetComponent<SpriteRenderer>().sortingLayerName = visibleLayer;
        }
        else
        {
            ArrowLeft.GetComponent<ArrowClicked>().isEnabled = false;
            ArrowLeft.GetComponent<SpriteRenderer>().sortingLayerName = invisibleLayer;
        }
        if (rooms[index].right != -1)
        {
            ArrowRight.GetComponent<ArrowClicked>().isEnabled = true;
            ArrowRight.GetComponent<SpriteRenderer>().sortingLayerName = visibleLayer;
        }
        else
        {
            ArrowRight.GetComponent<ArrowClicked>().isEnabled = false;
            ArrowRight.GetComponent<SpriteRenderer>().sortingLayerName = invisibleLayer;
        }
        //change background
        int current = rooms[index].bgIndex;
        int prev = rooms[currentRoom].bgIndex;
        if (current != prev)
        {
            background.GetComponent<SpriteRenderer>().sprite = bg[current];
            
            if (current == 1)
            {
                background.GetComponent<Transform>().position.Set(0, 0, 0);
                Vector3 scale = new Vector3(2.29f, 1.73f, 1);
                background.GetComponent<Transform>().localScale = scale;
            }
            else if(current == 2)
            {
                background.GetComponent<Transform>().position.Set(0, 1.5f, 0);
                Vector3 scale = new Vector3(2.29f, 1.73f, 1);
                background.GetComponent<Transform>().localScale = scale;
            }
            else if(current == 3)
            {
                background.GetComponent<Transform>().position.Set(0, 1.5f, 0);
                Vector3 scale = new Vector3(4.58f, 3.46f, 1);
                background.GetComponent<Transform>().localScale = scale;
            }
            else
            {
                background.GetComponent<Transform>().position.Set(0, 0, 0);
                Vector3 scale = new Vector3(1.53f, 1.43f, 1);
                background.GetComponent<Transform>().localScale = scale;
            }
        }
    }
}
