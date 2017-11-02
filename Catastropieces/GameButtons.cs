using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameButtons : MonoBehaviour
{
    private BaseObject[] objects;
	private GameObject[] anchorPoints;
    protected Button button;
    public BUTTONTYPE buttonType;
    public AudioClip StartSound;
    public AudioClip RestartSound;
    private static bool isPaused = false;

    public GameController controller;

    public Sprite pauseButtonSprite = null;
    public Sprite startButtonSprite = null;
    public GameObject restartPanel;
    private static GameObject pauseButton = null;
    private static GameObject startButton = null;
    private static GameObject goBackButton = null;
    private static GameController.GameState previousState;
    private static Dictionary<Button, bool> buttonsMapping = new Dictionary<Button, bool>();

    public enum BUTTONTYPE
    {
        Play,
        Pause,
        GoBack,
        Restart,
        LeftCamera,
        RightCamera,
        RestartYes,
        RestartNo,
        CameraControl
    }

    public void Start()
    {
        objects = FindObjectsOfType(typeof(BaseObject)) as BaseObject[];
        button = GetComponent<Button>();
		anchorPoints = GameObject.FindGameObjectsWithTag ("AnchorPoint");
        controller = FindObjectOfType<GameController>();
        // load for start/pause
        if (startButton == null)
            startButton = GameObject.Find("Start");
        if (pauseButton == null)
            pauseButton = GameObject.Find("Pause");
        if (goBackButton == null)
            goBackButton = GameObject.Find("GoBack");

        switch (buttonType)
        {
            case BUTTONTYPE.Play:
                button.onClick.AddListener(() => Play());
                break;
            case BUTTONTYPE.Pause:
                button.onClick.AddListener(() => PauseButtonClicked());
                gameObject.SetActive(false);
                break;
            case BUTTONTYPE.GoBack:
                button.onClick.AddListener(() => GoBack());
                break;
            case BUTTONTYPE.Restart:
                button.onClick.AddListener(() => RestartButtonClicked());
                break;
            case BUTTONTYPE.RestartYes:
                button.onClick.AddListener(() => RestartYesClicked());
                break;
            case BUTTONTYPE.RestartNo:
                button.onClick.AddListener(() => RestartNoClicked());
                break;
        }
    }

    public void Play()
    {
        if (controller.state == GameController.GameState.Play)
            return;
        controller.state = GameController.GameState.Play;
        //clock.OpenDoors();
        PlayStartSound(gameObject);

        foreach (BaseObject baseobject in objects)
        {
            baseobject.Play();
        }

        //switch to pause button
        gameObject.SetActive(false);
        pauseButton.SetActive(true);
    }

    // currently not called anywhere
    public void Pause()
    {
        if (controller.state != GameController.GameState.Play)
            return;
        controller.state = GameController.GameState.Pause;

        foreach (BaseObject baseobject in objects)
        {
            baseobject.Pause();
        }
    }

    public void GoBack()
    {
        if (controller.state == GameController.GameState.Build)
            return;

        isPaused = false;

        controller.state = GameController.GameState.Build;
        // drop the currently held object
        FindObjectOfType<PickUpAndMoveBehaviour>().DropCurrentHeldObject();

        foreach (BaseObject baseobject in objects)
        {
            baseobject.GoBack();
        }

        PlayRestartSound(gameObject);

        // stop launching paintballs
        StopPaintballLaunchSequences();

        // switch from pause to start
        startButton.SetActive(true);
        pauseButton.GetComponent<GameButtons>().PauseButtonReset();
    }

    public void Restart()
    {
        Time.timeScale = 1.0f;
        isPaused = false;

        controller.state = GameController.GameState.Build;
        // drop the currently held object
        FindObjectOfType<PickUpAndMoveBehaviour>().DropCurrentHeldObject();

        foreach (BaseObject baseobject in objects)
        {
            baseobject.Restart();
        }

        PlayRestartSound(gameObject);

        foreach (GameObject anchorP in anchorPoints)
        {
            anchorP.GetComponent<AnchorPoint>().IsOccupied = false;
        }

        // stop launching paintballs
        StopPaintballLaunchSequences();

        // switch from pause to start
        startButton.SetActive(true);
        pauseButton.GetComponent<GameButtons>().PauseButtonReset();
        goBackButton.SetActive(true);
        // restartButton is always active
        restartPanel.SetActive(false);
        startButton.GetComponent<Button>().enabled = true;
        pauseButton.GetComponent<Button>().enabled = true;
        goBackButton.GetComponent<Button>().enabled = true;
    }

    public void PauseButtonClicked()
    {
        if (controller.state != GameController.GameState.Play) { 
            controller.state = GameController.GameState.Play;
        }
        else
            controller.state = GameController.GameState.Pause;

        isPaused = !isPaused;
        if (isPaused) {
            GetComponent<Image>().sprite = startButtonSprite;
            if (GetComponent<WWisePauseTrigger>())
            {
                GetComponent<WWisePauseTrigger>().Pause();
            } else
            {
                print("WWise not attatched to " + gameObject);
            }
            
        }
        else
            GetComponent<Image>().sprite = pauseButtonSprite;

        Time.timeScale = Convert.ToInt32(!isPaused);
    }

    public void RestartButtonClicked()
    {
        restartPanel.SetActive(true);
        Time.timeScale = 0.0f;
        // set gamestate
        previousState = controller.state;
        controller.state = GameController.GameState.RestartConfirm;
        // save buttons state for use if canceled
        buttonsMapping[startButton.GetComponent<Button>()] = startButton.GetComponent<Button>().enabled;
        buttonsMapping[pauseButton.GetComponent<Button>()] = pauseButton.GetComponent<Button>().enabled;
        buttonsMapping[goBackButton.GetComponent<Button>()] = goBackButton.GetComponent<Button>().enabled;
        // disable all buttons except for restart
        startButton.GetComponent<Button>().enabled = false;
        pauseButton.GetComponent<Button>().enabled = false;
        goBackButton.GetComponent<Button>().enabled = false;
    }

    public void RestartYesClicked()
    {
        Restart();
    }

    public void RestartNoClicked()
    {
        GetComponent<WWisePauseTrigger>().Pause();
        Time.timeScale = Convert.ToInt32(!isPaused);
        controller.state = previousState;
        restartPanel.SetActive(false);
        // reset all buttons' states
        startButton.GetComponent<Button>().enabled = buttonsMapping[startButton.GetComponent<Button>()];
        pauseButton.GetComponent<Button>().enabled = buttonsMapping[pauseButton.GetComponent<Button>()];
        goBackButton.GetComponent<Button>().enabled = buttonsMapping[goBackButton.GetComponent<Button>()];
    }

    public void PlayStartSound(GameObject Object)
    {
        Debug.Log("Play sound");
        try
        {
            GetComponent<WWiseLevelStop>().Stop();
        }
        catch
        {
            Debug.LogError("You have not attached the Audio Source to " + name);
        }
    }

    public void PlayRestartSound(GameObject Object)
    {
        Debug.Log("Play sound");
        try
        {
            GetComponent<WWiseRestart>().Restart();
        }
        catch
        {
            Debug.LogError("You have not attached the Audio Source to " + name);
        }

    }

    public void PauseButtonReset()
    {
        if (buttonType != BUTTONTYPE.Pause)
            return;
        isPaused = false;
        GetComponent<Image>().sprite = pauseButtonSprite;
        gameObject.SetActive(false);
        Time.timeScale = Convert.ToInt32(true);
    }

    private void StopPaintballLaunchSequences()
    {
        foreach(var launcher in FindObjectsOfType<PaintballLauncher>())
        {
            launcher.StopLaunching();
        }
        FindObjectOfType<PaintTube>().StopActions();
        FindObjectOfType<PaintTube>().squeezed = false;
        FlipperComponent flipper = FindObjectOfType<FlipperComponent>();
        flipper.StopActions();
        flipper.laidDown = false;
        flipper.launched = false;

        foreach(var paintball in GameObject.FindGameObjectsWithTag("Paintball"))
        {
            Destroy(paintball);
        }
    }
}