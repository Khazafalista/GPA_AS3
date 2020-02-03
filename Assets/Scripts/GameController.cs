using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Variables")]
    public int maxLevel = 4;
    public static int level;
    public int score;
    public int state;
    public bool ballSpawn;
    public int ballThrowCount;
    public int maxBallThrowCount = 3;
    public float ballDestroyDelay = 3.0f;
    public int maxEnemy = 1;

    [Header("References")]
    public GameObject pauseUI;
    public Text throwText;
    public GameObject gameMenu;
    public Text resultText;
    public Text scoreText;
    public GameObject playBtn;
    public GameObject nextBtn;
    public GameObject soundOnBtn;
    public GameObject soundOffBtn;
    public GameObject ball;
    public Rigidbody2D hookPoint;
    public Transform spawnPoint;
    public AudioSource BGMPlayer;
    public AudioSource SEPlayer;
    public AudioClip btnSE;
    public AudioClip WinSE;
    public AudioClip LoseSE;
    public EnemyController enemyController;

    private bool first;

    public enum PlayerState
    {
        PLAYING,
        PAUSE,
        CLEAR,
        FAILED,
        OTHER
    };

    private void Start()
    {
        score = 0;
        ballSpawn = false;
        ballThrowCount = 0;

        level = PlayerPrefs.GetInt("level");
        state = (int)PlayerState.PLAYING;

        pauseUI.SetActive(true);
        gameMenu.SetActive(false);
        resultText.text = "";

        BGMPlayer.loop = true;
        BGMPlayer.time = PlayerPrefs.GetFloat("time");

        if(PlayerPrefs.GetInt("mute") == 0)
        {
            BGMPlayer.volume = 0.1f;
            BGMPlayer.mute = false;
            BGMPlayer.Play();
            StartCoroutine(AudioFadeIn(BGMPlayer, 0.5f, 1.0f));
            soundOnBtn.SetActive(false);
            soundOffBtn.SetActive(true);
        }
        else if (PlayerPrefs.GetInt("mute") == 1)
        {
            BGMPlayer.volume = 0.1f;
            BGMPlayer.mute = true;
            BGMPlayer.Play();
            soundOnBtn.SetActive(true);
            soundOffBtn.SetActive(false);
        }

        first = false;
    }

    private void Update()
    {
        
        if(state == (int)PlayerState.PAUSE)
        {
            Time.timeScale = 0;
        }
        else if(state == (int)PlayerState.OTHER)
        {
            Time.timeScale = 1;
        }
        else if(state == (int)PlayerState.CLEAR)
        {
            if(!first)
            {
                GameClear();
                first = true;
            }
        }
        else if(state == (int)PlayerState.FAILED)
        {
            if (!first)
            {
                GameFailed();
                first = true;
            }
        }
        else if (state == (int)PlayerState.PLAYING)
        {
            Time.timeScale = 1;

            if (!ballSpawn)
            {
                throwText.text = "Thrown: " + ballThrowCount + " / " + maxBallThrowCount;

                if (ballThrowCount < maxBallThrowCount)
                {
                    GameObject ballClone = Instantiate(ball, spawnPoint.position, Quaternion.identity);
                    ballClone.GetComponent<SpringJoint2D>().connectedBody = hookPoint;

                    ballSpawn = true;
                }
                else
                {
                    state = (int)PlayerState.FAILED;
                }
            }
        }
    }

    private void GameClear()
    {
        StartCoroutine(AudioFadeOut(BGMPlayer, 0.5f, 0.0f));
        SEPlayer.volume = 0.5f;
        SEPlayer.PlayOneShot(WinSE);
        
        resultText.text = "Game Win!!";
        scoreText.text = "Score: " + Mathf.Round((float)score * (1.0f - (float)Mathf.Max(ballThrowCount - 1, 0) / (float)maxBallThrowCount * 0.5f));

        if (level < maxLevel)
        {
            pauseUI.SetActive(false);
            gameMenu.SetActive(true);
            nextBtn.SetActive(true);
        }
        else
        {
            pauseUI.SetActive(false);
            gameMenu.SetActive(true);
        }
    }

    private void GameFailed()
    {
        StartCoroutine(AudioFadeOut(BGMPlayer, 0.5f, 0.0f));
        SEPlayer.volume = 0.5f;
        SEPlayer.PlayOneShot(LoseSE);
        
        resultText.text = "Game Failed!!";
        scoreText.text = "Score: " + Mathf.Round((float)score * (1.0f - (float)Mathf.Max(ballThrowCount - 1, 0) / (float)maxBallThrowCount * 0.5f));

        pauseUI.SetActive(false);
        gameMenu.SetActive(true);
    }

    private void ReloadSceneInvoke()
    {
        SceneManager.LoadScene("Level" + level);
    }
    private void ReloadMenuInvoke()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnPauseBtnClicked()
    {
        SEPlayer.PlayOneShot(btnSE);
        state = (int)PlayerState.PAUSE;

        StartCoroutine(AudioFadeOut(BGMPlayer, 0.5f, 0.3f));

        scoreText.text = "Score: " + Mathf.Round((float)score * (1.0f - (float)Mathf.Max(ballThrowCount-1, 0) / (float)maxBallThrowCount * 0.5f));
        pauseUI.SetActive(false);
        gameMenu.SetActive(true);
        playBtn.SetActive(true);
    }

    public void OnPlayBtnClicked()
    {
        SEPlayer.PlayOneShot(btnSE);
        state = (int)PlayerState.PLAYING;

        StartCoroutine(AudioFadeIn(BGMPlayer, 0.5f, 1.0f));

        pauseUI.SetActive(true);
        gameMenu.SetActive(false);
        playBtn.SetActive(false);
    }

    public void OnNextBtnClicked()
    {
        state = (int)PlayerState.OTHER;

        SEPlayer.PlayOneShot(btnSE);
        level++;

        PlayerPrefs.SetInt("level", level);
        PlayerPrefs.SetFloat("time", BGMPlayer.time + 0.5f);
        PlayerPrefs.Save();

        StartCoroutine(AudioFadeOut(BGMPlayer, 0.5f, 0.0f));
        Invoke("ReloadSceneInvoke", 0.7f);
    }

    public void OnHomeBtnClicked()
    {
        state = (int)PlayerState.OTHER;

        SEPlayer.PlayOneShot(btnSE);
        PlayerPrefs.SetInt("level", 1);
        PlayerPrefs.SetFloat("time", 0);
        PlayerPrefs.Save();

        StartCoroutine(AudioFadeOut(BGMPlayer, 0.5f, 0.0f));
        Invoke("ReloadMenuInvoke", 0.7f);
    }

    public void OnRestartBtnClicked()
    {
        state = (int)PlayerState.OTHER;

        SEPlayer.PlayOneShot(btnSE);
        EnemyController.enemiesAlive = 0;

        PlayerPrefs.SetInt("level", level);
        PlayerPrefs.SetFloat("time", BGMPlayer.time + 0.5f);
        PlayerPrefs.Save();

        StartCoroutine(AudioFadeOut(BGMPlayer, 0.5f, 0.0f));
        Invoke("ReloadSceneInvoke", 0.7f);
    }

    public void OnSoundOnBtnClicked()
    {
        SEPlayer.PlayOneShot(btnSE);
        BGMPlayer.mute = false;
        PlayerPrefs.SetInt("mute", 0);
        PlayerPrefs.Save();
        soundOnBtn.SetActive(false);
        soundOffBtn.SetActive(true);
    }
    public void OnSoundOffBtnClicked()
    {
        SEPlayer.PlayOneShot(btnSE);
        BGMPlayer.mute = true;
        PlayerPrefs.SetInt("mute", 1);
        PlayerPrefs.Save();
        soundOffBtn.SetActive(false);
        soundOnBtn.SetActive(true);
    }

    IEnumerator AudioFadeOut(AudioSource audioSource, float duration, float targetVolume)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > targetVolume)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;

            yield return null;
        }
        if (targetVolume == 0)
        {
            audioSource.Stop();
        }
    }

    IEnumerator AudioFadeIn(AudioSource audioSource, float duration, float targetVolume)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += startVolume * Time.deltaTime / duration;

            yield return null;
        }
    }
}
