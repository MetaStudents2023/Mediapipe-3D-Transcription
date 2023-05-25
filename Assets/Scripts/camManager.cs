using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camManager : MonoBehaviour
{
    [SerializeField]
    private List<Camera> cameras;
    private int indexActiveCamera;

    public void ManageCamera()
    {
        indexActiveCamera++;
        if (indexActiveCamera >= cameras.Count)
        {
            indexActiveCamera = 0;
        }

        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].enabled = false;
        }
        cameras[indexActiveCamera].enabled = true;
    }
}
