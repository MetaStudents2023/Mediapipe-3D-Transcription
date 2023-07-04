using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class camManager : MonoBehaviour
{
    [SerializeField]
    private List<Camera> cameras;
    private int indexActiveCamera;

    [SerializeField]
    csvReader csvReader;

    [SerializeField]
    private VideoPlayer player;

    private List<string> clipsList = new List<string>();

    [SerializeField]
    private GameObject missingFileLogo, videoPlace;

    [SerializeField]
    private int frameIndex, frameCount;

    [SerializeField]
    private Material citySkybox, snowSkybox;

    private void Start()
    {
        indexActiveCamera = 3;
        ManageCamera();
    }
    private void Update()
    {
        frameIndex = (int)player.frame;
        frameCount = (int)player.frameCount;
    }

    public void ManageCamera()
    {
        if (indexActiveCamera >= cameras.Count)
        {
            indexActiveCamera = 0;
        }

        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].enabled = false;
        }

        cameras[indexActiveCamera].enabled = true;
        
        if(clipsList.Count > indexActiveCamera)
        {
            missingFileLogo.SetActive(false);
            videoPlace.SetActive(true);
            player.url = clipsList[indexActiveCamera];
        }
        else
        {
            missingFileLogo.SetActive(true);
            videoPlace.SetActive(false);
        }
        indexActiveCamera++;
    }

    public void SetClipsList(List<string> clipList)
    {
        this.clipsList = clipList;
        indexActiveCamera = 3;
        ManageCamera();
    }

    public void changeSkyBox(string skyBox)
    {
        if(skyBox == "city")
        {
            RenderSettings.skybox = citySkybox;
        }
        if(skyBox == "snow")
        {
            RenderSettings.skybox = snowSkybox;
        }
    }
    public int GetFrameIndex()
    {
        return this.frameIndex;
    }

    public int GetFrameCount()
    {
        return this.frameCount;
    }
}
