using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camManager : MonoBehaviour
{
    [SerializeField]
    private List<Camera> cameras;
    public int indexActiveCamera;

    private void Start()
    {
        ManageCamera();
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
        indexActiveCamera++;
    }
}
