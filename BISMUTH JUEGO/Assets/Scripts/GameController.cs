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
    int destScene;
    GameState state = GameState.MAIN_MENU;

    void Start ()
    {
        // Set GameController instance (for scene conservation)
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

        if(currentScene != 0) // not in Main Menu
        {

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
                GameObject.Find("Main Camera").GetComponent<CameraCutSceneController>().StartCutscene(Cutscene.INTRO);
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


    // Game Utility =============================================================================================================================

    public GameState GetState()
    {
        return state;
    }

    public void SetState(GameState newState)
    {
        state = newState;
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


