using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject menu01;
    public GameObject menu02;

    public AudioSource BGMPlayer;
    public AudioClip BGM;

    public AudioSource SEPlayer;
    public AudioClip btnSE;

    private void Start()
    {
        menu01.SetActive(true);
        menu02.SetActive(false);

        BGMPlayer.loop = true;
        BGMPlayer.volume = 0.1f;
        //BGMPlayer.volume = PlayerPrefs.GetFloat("volume");
        BGMPlayer.PlayOneShot(BGM);
        StartCoroutine(AudioFadeIn(BGMPlayer, 0.5f, 1.0f));
    }

    public void OnStartBtn01Clicked()
    {
        SEPlayer.PlayOneShot(btnSE);
        menu01.SetActive(false);
        menu02.SetActive(true);
    }

    public void OnStartLevel1BtnClicked()
    {
        SEPlayer.PlayOneShot(btnSE);
        PlayerPrefs.SetInt("level", 1);
        PlayerPrefs.SetFloat("time", 0);
        PlayerPrefs.Save();
        StartCoroutine(AudioFadeOut(BGMPlayer, 0.5f, 0.0f));
        Invoke("OnStartBtnLevelClickedInvoke", 0.7f);
    }

    public void OnStartLevel2BtnClicked()
    {
        SEPlayer.PlayOneShot(btnSE);
        PlayerPrefs.SetInt("level", 2);
        PlayerPrefs.SetFloat("time", 0);
        PlayerPrefs.Save();
        StartCoroutine(AudioFadeOut(BGMPlayer, 0.5f, 0.0f));
        Invoke("OnStartBtnLevelClickedInvoke", 0.7f);
    }

    public void OnStartLevel3BtnClicked()
    {
        SEPlayer.PlayOneShot(btnSE);
        PlayerPrefs.SetInt("level", 3);
        PlayerPrefs.SetFloat("time", 0);
        PlayerPrefs.Save();
        StartCoroutine(AudioFadeOut(BGMPlayer, 0.5f, 0.0f));
        Invoke("OnStartBtnLevelClickedInvoke", 0.7f);
    }

    public void OnStartLevel4BtnClicked()
    {
        SEPlayer.PlayOneShot(btnSE);
        PlayerPrefs.SetInt("level", 4);
        PlayerPrefs.SetFloat("time", 0);
        PlayerPrefs.Save();
        StartCoroutine(AudioFadeOut(BGMPlayer, 0.5f, 0.0f));
        Invoke("OnStartBtnLevelClickedInvoke", 0.7f);
    }

    private void OnStartBtnLevelClickedInvoke()
    {
        SceneManager.LoadScene("Level" + PlayerPrefs.GetInt("level"));
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
