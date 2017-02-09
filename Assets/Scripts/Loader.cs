using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour {

    public GameObject LoaderCamera;
    public GameObject MainMenuCanvas;
    string activelevel;

    // Use this for initialization
    void Start ()
    {
        MainScript.GetInstance();
        //StartCoroutine(LoadLevel("Scenes/LevelTest01"));
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void LoadLevelFakeForNow(string level)
    {
        StartCoroutine(LoadLevel(level, true));
    }

    public IEnumerator UnLoadLevel()
    {
        AsyncOperation async;

        LoaderCamera.transform.SetParent(this.transform);
        LoaderCamera.transform.localPosition = Vector3.zero;

        async = SceneManager.UnloadSceneAsync(activelevel);
      

        while (!async.isDone)
        {
            //yield return new WaitForSeconds(.1f);
            yield return null;
        }

        MainMenuCanvas.SetActive(true);
    }

    public IEnumerator LoadLevel(string level, bool add)
    {
        AsyncOperation async;

        MainMenuCanvas.SetActive(false);

        if (add)
        {
            async = SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive);
        }
        else
        {
            async = SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);
        }
        async.allowSceneActivation = false;
        while (async.progress < 0.9f)
        {
            var scaledPerc = 0.5f * async.progress / 0.9f;
            //StatusText.text = "<Loading Map : " + LevelInfo.LevelName + " : " + (100f * scaledPerc).ToString("F0") + ">";
        }

        async.allowSceneActivation = true;
        float perc = 0.5f;
        while (!async.isDone)
        {
            yield return null;
            perc = Mathf.Lerp(perc, 1f, 0.05f);
            //StatusText.text = "<Loading Map : " + LevelInfo.LevelName + " : " + (100f * perc).ToString("F0") + ">";
        }

        //StatusText.text = "<Loading Complete : " + LevelInfo.LevelName + " : 100>";
        LoaderCamera.transform.SetParent(Camera.main.transform);
        LoaderCamera.transform.localPosition = Vector3.zero;
        activelevel = level;
        MainScript.GetInstance().InitLevel(true);
    }
}
