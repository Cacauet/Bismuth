using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public enum GameState
{
    MAIN_MENU,
    TRANSITION_TO_PAUSE,
    PAUSE,
    TRANSITION_TO_PLAY,
    PLAY,
    CUTSCENE
}

public class GameController : MonoBehaviour
{
    // Instance
    static private GameController instance = null;

    // Game Data
    private GameData gd = null;
    private string savePath = "/BismuthGameInfo.txt";
    private bool gdLoaded = false;

    // Fader
    private Image fader = null;
    private bool fading;
    private float fadeTimer;
    private float fadeDuration;
    private float originAlpha;
    private float destAlpha;

    // General Values
    private int currentScene = 0;
    private int destScene;
    private GameState state = GameState.MAIN_MENU;
    private bool waitForTick;

    // Level Control
    private int maxStars;
    private PortalController[] portals;
    private int redKeys;
    private int blueKeys;
    private int blackKeys;

    private float gameTimer;
    private int currentStars;
    private int currentKeys;

    // UI


    void Start ()
    {
        // Set GameController static instance (for scene conservation)
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }

        // Load GameData
        LoadGameData();

        // Get Fader Image
        fader = GameObject.Find("Fader").GetComponent<Image>();
        fading = false;
    }

    void Update()
    {
        if (fading)
            HandleFading();

        switch (state)
        {
            case GameState.MAIN_MENU:
                {
                    break;
                }
            case GameState.TRANSITION_TO_PAUSE:
                {
                    if (waitForTick)
                        waitForTick = false;
                    else
                        state = GameState.PAUSE;

                    break;
                }
            case GameState.PAUSE:
                {
                    break;
                }
            case GameState.TRANSITION_TO_PLAY:
                {
                    if (waitForTick)
                        waitForTick = false;
                    else
                        state = GameState.PLAY;

                    break;
                }
            case GameState.PLAY:
                {
                    gameTimer += Time.deltaTime;
                    break;
                }
            case GameState.CUTSCENE:
                {
                    break;
                }
            default:
                {
                    break;
                }
        }
    }


    // Fade Handling =============================================================================================================================

    private void HandleFading()
    {
        if (fadeTimer > 1f) // fading finished
        {
            if (destAlpha == 1f) // fader is completely dark
            {
                originAlpha = 1f;
                destAlpha = 0f;
                fadeTimer = 0f;
                ChangeSceneNow(destScene);
            }
            else // fader is transparent
            {
                fading = false;
            }
        }
        else
        {
            fadeTimer += Time.deltaTime / fadeDuration;
            Color fadeColor = Color.black;
            fadeColor.a = Mathf.Lerp(originAlpha, destAlpha, fadeTimer);
            fader.color = fadeColor;
        }
    }

    // Game Control =============================================================================================================================

    public void SetLevelSpecs(int s, PortalController[] p)
    {
        maxStars = s;
        portals = p;
        gameTimer = 0f;
        currentStars = currentKeys = redKeys = blueKeys = blackKeys = 0;
    }

    public void AddStar(string tag, StarController sc)
    {
        bool starAdded = false;

        switch (tag)
        {
            case "WhiteStar":
                {
                    currentStars++;
                    sc.Unlock();
                    starAdded = true;
                    break;
                }
            case "RedStar":
                {
                    if(redKeys > 0)
                    {
                        redKeys--;
                        currentStars++;
                        sc.Unlock();
                        starAdded = true;
                    }
                    break;
                }
            case "BlueStar":
                {
                    if (blueKeys > 0)
                    {
                        blueKeys--;
                        currentStars++;
                        sc.Unlock();
                        starAdded = true;
                    }
                    break;
                }
            case "BlackStar":
                {
                    if (blackKeys > 0)
                    {
                        blackKeys--;
                        currentStars++;
                        sc.Unlock();
                        starAdded = true;
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }

        if(starAdded) // check for portals to open
        {
            if(maxStars == currentStars)
            {
                // WIN CONDITION TRIGGERED
                StartCutScene(Cutscene.FINAL);


            }
            else
            {
                foreach (PortalController pc in portals)
                {
                    if (pc.value == currentKeys)
                    {
                        StartCutScene(Cutscene.WALL_SHOW);
                        pc.Open();
                    }
                }
            }
        }
    }

    public void AddKey(GameObject key)
    {
        switch (key.tag)
        {
            case   "RedKey": redKeys++; break;
            case  "BlueKey": blueKeys++; break;
            case "BlackKey": blackKeys++; break;
            default: break;
        }

        key.SetActive(false);
        currentKeys = redKeys + blueKeys + blackKeys;
    }


    // UI Control =============================================================================================================================

    // Game Utility =============================================================================================================================

    public GameState GetState()
    {
        return state;
    }

    public void SetState(GameState newState)
    {
        state = newState;

        if (state == GameState.TRANSITION_TO_PAUSE || state == GameState.TRANSITION_TO_PLAY)
            waitForTick = true;
    }

    public void ChangeSceneNow(int nextScene)
    {
        currentScene = nextScene;
        SceneManager.LoadScene(currentScene);
    }

    public void SetCurrentScene(int scene) // only for GameControllerSpawner (no friend classes in C#)
    {
        currentScene = scene;
    }

    public void FadeToNextScene(int nextScene, float duration = 1f)
    {
        destScene = nextScene;
        fadeTimer = 0f;
        fadeDuration = duration;
        fading = true;
        originAlpha = 0f;
        destAlpha = 1f;
    }

    public void StartCutScene(Cutscene cs)
    {
        Camera.main.GetComponent<CameraCutSceneController>().StartCutscene(cs, this);
    }

    // Data Utility =============================================================================================================================

    private void LoadGameData()
    {
        if (gdLoaded)
            return;

        if (gd == null)
            gd = new GameData();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;

        if (File.Exists (Application.persistentDataPath + savePath))
        {
            Debug.Log(savePath + " file found.");
			file = File.Open (Application.persistentDataPath + savePath, FileMode.Open);
            gd = (GameData)bf.Deserialize (file);
		}
        else
        {
            Debug.Log(savePath + " file NOT found.");
            file = File.Create(Application.persistentDataPath + savePath);
            bf.Serialize(file, gd);
		}

        file.Close();
        gdLoaded = true;
    }

    public void Save()
    {
        if (!gdLoaded)
            return;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/info.txt", FileMode.Open);
        bf.Serialize(file, gd);
		file.Close ();
	}

    public void ResetToDefault()
    {
        if (!gdLoaded)
            return;

        gd.Reset();
        Save();
    }
}

// Game Data =============================================================================================================================

[System.Serializable]
public class GameData
{
    public int time1Min;
    public int time1Sec;
    public int time1MilSec;
    public int time2Min;
    public int time2Sec;
    public int time2MilSec;
    public int time3Min;
    public int time3Sec;
    public int time3MilSec;
    public float volume;

    public void Reset()
    {
        time1Min = 0;
        time1Sec = 0;
        time1MilSec = 0;
        time2Min = 0;
        time2Sec = 0;
        time2MilSec = 0;
        time3Min = 0;
        time3Sec = 0;
        time3MilSec = 0;
        volume = 1f;
    }
}


