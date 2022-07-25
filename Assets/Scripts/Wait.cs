using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Wait : MonoBehaviour
{
    VideoPlayer videoplayer;
    public float waitTime;

    public int sceneIndex; // 1 == Main Scene
    
    // Start is called before the first frame update
    void Start()
    {
        //videoplayer = GetComponentInParent<VideoPlayer>();
        StartCoroutine(WaitForIntro());
    }

    private void Update()
    {
        if (Input.anyKey)
            SceneManager.LoadScene(sceneIndex);
    }

    IEnumerator WaitForIntro()
    {
        //waitTime = (float)videoplayer.length;

        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneIndex);
    }
    
}
