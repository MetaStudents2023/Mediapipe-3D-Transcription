using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using System;
using UnityEditor;

public class csvReader : MonoBehaviour
{
    [SerializeField]
    private List<Vector3> pointList = new List<Vector3>();
    private List<GameObject> gameObjects = new List<GameObject>();

    private List<List<Vector3>> globalListCam1;
    private List<List<Vector3>> globalListCam2;
    private List<List<Vector3>> globalListCam3;
    private List<List<Vector3>> globalListCam4;

    private List<List<Vector3>> globalListXsense;

    private List<float> listForMinY;

    [SerializeField]
    private GameObject prefabCube, prefabEmpty, prefabLine;

    private GameObject lines, people;
    private bool runExercice = false;

    [SerializeField]
    private float offsetX, offsetY, offsetZ, multW, multH;

    [SerializeField]
    private int minFrameCount;

    private string[] MediapipeDr, XsenseDr, files, videos;

    [SerializeField]
    private TMP_Dropdown dropdownMediapipe, dropdownMediapipeVR, dropDownXsense, dropDownXsenseVR;

    [SerializeField]
    private Button mediapipeBtn, xsenseBtn, mediapipeBtnVR, xsenseBtnVR;
    public Color darkgreen;
    private bool mediapipeOrXsense = true; // true for mediapipe - false for xsense

    [SerializeField]
    private TMP_Text text;
    private List<string> dropOptions = new List<string>();

    [SerializeField]
    private camManager camManager;

    int n = 1;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            MediapipeDr = Directory.GetDirectories(@"..\Assets\Data\Mediapipe");
            XsenseDr = Directory.GetDirectories(@"..\Assets\Data\Xsense");
        }
        catch (Exception e)
        {
            MediapipeDr = Directory.GetDirectories(@"Assets\Data\Mediapipe");
            XsenseDr = Directory.GetDirectories(@"Assets\Data\Xsense");
        }

        foreach (string dir in MediapipeDr)
        {
            System.IO.FileInfo monDir = new System.IO.FileInfo(dir);
            string name = monDir.Name;
            dropOptions.Add(name);
        }
        dropdownMediapipeVR.ClearOptions();
        dropdownMediapipeVR.AddOptions(dropOptions);

        dropdownMediapipe.ClearOptions();
        dropdownMediapipe.AddOptions(dropOptions);

        dropOptions.Clear();

        foreach (string dir in XsenseDr)
        {
            System.IO.FileInfo monDir = new System.IO.FileInfo(dir);
            string name = monDir.Name;
            dropOptions.Add(name);
        }
        dropDownXsenseVR.ClearOptions();
        dropDownXsenseVR.AddOptions(dropOptions);

        dropDownXsense.ClearOptions();
        dropDownXsense.AddOptions(dropOptions);

        UpdateExercice();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        if (mediapipeOrXsense)
        {
            int frameCountOnVideo = camManager.GetFrameCount();
            int currentFrameOnVideo = camManager.GetFrameIndex();

            if (runExercice)
            {
                int frameDiff = frameCountOnVideo - minFrameCount;
                if (currentFrameOnVideo > frameDiff)
                {
                    for (int i = 0; i < pointList.Count; i++)
                    {
                        gameObjects[i].transform.position = globalListCam4[currentFrameOnVideo - frameDiff][i];
                    }
                }
            }
        }

        else
        {
            if(n>=globalListXsense.Count-1)
            {
                n = 1;
            }
            else
            {
                n++;
            }

            for(int i = 0; i <pointList.Count; i++)
            {
                gameObjects[i].transform.position = globalListXsense[n][i];
            }
        }
        
    }

    public void OnClickMediapipeBtn()
    {
        mediapipeOrXsense = true;
        
        xsenseBtn.GetComponent<Image>().color = Color.black;
        xsenseBtnVR.GetComponent<Image>().color = Color.black;
        mediapipeBtn.GetComponent<Image>().color = darkgreen;
        mediapipeBtnVR.GetComponent<Image>().color = darkgreen;
        
        dropdownMediapipe.gameObject.SetActive(true);
        dropDownXsense.gameObject.SetActive(false);
        dropdownMediapipeVR.gameObject.SetActive(true);
        dropDownXsenseVR.gameObject.SetActive(false);

        UpdateExercice();
    }

    public void OnClickXsenseBtn()
    {
        mediapipeOrXsense = false;
        
        mediapipeBtn.GetComponent<Image>().color = Color.black;
        xsenseBtn.GetComponent<Image>().color = darkgreen;
        mediapipeBtnVR.GetComponent<Image>().color = Color.black;
        xsenseBtnVR.GetComponent<Image>().color = darkgreen;

        dropdownMediapipe.gameObject.SetActive(false);
        dropDownXsense.gameObject.SetActive(true);
        dropdownMediapipeVR.gameObject.SetActive(false);
        dropDownXsenseVR.gameObject.SetActive(true);

        UpdateExercice();
    }


    public void OnVRDropDownChange()
    {
        dropdownMediapipe.value = dropdownMediapipeVR.value;
        dropdownMediapipe.RefreshShownValue();
        UpdateExercice();
    }

    public void OnDropDownChange()
    {
        dropdownMediapipeVR.value = dropdownMediapipe.value;
        dropdownMediapipeVR.RefreshShownValue();
        UpdateExercice();
    }

    public void UpdateExercice()
    {
        Destroy(people);
        Destroy(lines);
        runExercice = false;

        if (mediapipeOrXsense)
        {
            files = Directory.GetFiles(MediapipeDr[dropdownMediapipe.value], "*.csv");

            foreach (string file in files)
            {
                System.IO.FileInfo monfile = new System.IO.FileInfo(file);
                string name = monfile.Name;

                char number = name[name.Length - 5];

                switch (number)
                {
                    case '1':
                        globalListCam1 = ReadCSVFile(monfile.FullName);
                        break;
                    case '2':
                        globalListCam2 = ReadCSVFile(monfile.FullName);
                        break;
                    case '3':
                        globalListCam3 = ReadCSVFile(monfile.FullName);
                        break;
                    case '4':
                        globalListCam4 = ReadCSVFile(monfile.FullName, true);
                        break;
                }
            }

            if (globalListCam3.Count < globalListCam4.Count)
            {
                minFrameCount = globalListCam3.Count;
            }
            else
            {
                minFrameCount = globalListCam4.Count;
            }

            offsetY = listForMinY.Max() * multH * -1.0f;

            for (int n = 0; n < minFrameCount; n++)
            {
                for (int i = 0; i < pointList.Count; i++)
                {
                    float newX, newY, newZ;

                    newX = globalListCam4[n][i].x;
                    newY = (globalListCam4[n][i].y + globalListCam3[n][i].y) / 2;
                    newZ = globalListCam3[n][i].x;

                    newX *= multW;
                    newX += offsetX;
                    newY *= multH;
                    newY += offsetY;
                    newZ *= multW;
                    newZ += offsetZ;

                    globalListCam4[n][i] = new Vector3(newX, newY, newZ);
                }
            }

            videos = Directory.GetFiles(MediapipeDr[dropdownMediapipe.value], "*.mp4");
            List<string> list = new List<string>();

            if (videos.Length == 4)
            {
                foreach (string video in videos)
                {
                    System.IO.FileInfo myvideo = new System.IO.FileInfo(video);
                    list.Add(myvideo.FullName);
                }
                runExercice = true;
            }
            else { runExercice = false; }

            camManager.SetClipsList(list);

            
        }
        else    // Xsense loading
        {
            files = Directory.GetFiles(XsenseDr[dropDownXsense.value], "*.csv");

            foreach (string file in files)
            {
                System.IO.FileInfo monfile = new System.IO.FileInfo(file);
                string name = monfile.Name;

                if(name == "CIIRC_Emilio-055_positions.csv")
                {
                    globalListXsense = ReadCSVFile(monfile.FullName);
                }
            }

            for(int i=1; i<globalListXsense.Count; i++)
            {
                for(int n=0; n < globalListXsense[i].Count; n++) 
                {
                    globalListXsense[i][n] = new Vector3(globalListXsense[i][n].x - 2, globalListXsense[i][n].y, globalListXsense[i][n].z + 8);
                }
            }

            InstanciateCube();
        }

        if (runExercice)
        {
            InstanciateCube();
            InstanciateLines();
            text.text = dropdownMediapipe.options[dropdownMediapipe.value].text + " is running";
        }
        else
        {
            if (mediapipeOrXsense)
            {
                text.text = "Videos missing for : " + dropdownMediapipe.options[dropdownMediapipe.value].text;
            }
            else
            {
                text.text = "Videos missing for : " + dropDownXsense.options[dropDownXsense.value].text;
            }
            
            camManager.SetClipsList(new List<string>());
        }
    }


    public List<List<Vector3>> ReadCSVFile(string filename, bool takeMinY = false)
    {
        StreamReader strReader = new StreamReader(filename);
        List<List<Vector3>> globalList = new List<List<Vector3>>();
        
        bool endOfFile = false;
        if(takeMinY)
        {
            listForMinY = new List<float>();
        }
        

        while (!endOfFile)
        {
            //endOfFile = true;
            string data_string = strReader.ReadLine();
            
            if(data_string == null)
            {
                endOfFile = true;
                break;
            }

            string[] data_values = data_string.Split(',');
            float[] data_float;

            if (mediapipeOrXsense)
            {
                data_float = new float[data_values.Length];

                for (int i = 0; i < data_values.Length; i++)
                {
                    data_float[i] = float.Parse(data_values[i], CultureInfo.InvariantCulture.NumberFormat);
                    //data_float[i] *= multiplicator;
                }
            }
            else
            {
                data_float = new float[data_values.Length-1];

                for (int i = 1; i < data_values.Length; i++)
                {
                    data_float[i-1] = float.Parse(data_values[i], CultureInfo.InvariantCulture.NumberFormat);
                }
            }

            pointList = new List<Vector3>();

            for (int i = 0; i < data_float.Length; i += 3)
            {
                if (takeMinY)
                {
                    listForMinY.Add(data_float[i + 1]);
                }
                if (mediapipeOrXsense)
                {
                    pointList.Add(new Vector3(data_float[i], data_float[i + 1], data_float[i + 2]));
                }
                else
                {
                    pointList.Add(new Vector3(data_float[i], data_float[i + 2], data_float[i + 1]));
                }
                
            }
            globalList.Add(pointList);
        }

        return globalList;
    }

    void InstanciateCube()
    {
        // Instanciate all the cubes
        people = Instantiate(prefabEmpty, new Vector3(0, 0, 0), Quaternion.identity);
        people.name = "People";

        gameObjects.Clear();
        for (int i = 0; i < pointList.Count; i++)
        {
            GameObject cube;
            if (mediapipeOrXsense)
            {
                cube = Instantiate(prefabCube, globalListCam4[0][i], Quaternion.identity);
            }
            else
            {
                cube = Instantiate(prefabCube, globalListXsense[1][i], Quaternion.identity);
            }

            cube.transform.SetParent(people.transform);
            cube.name = i.ToString();
            gameObjects.Add(cube);
        }
    }

    void InstanciateLines()
    {
        // Instanciate the lines
        lines = Instantiate(prefabEmpty, new Vector3(0, 0, 0), Quaternion.identity);
        lines.name = "Lines";

        for (int i = 0; i < 9; i++)
        {
            GameObject line = Instantiate(prefabLine);
            line.GetComponent<lineTracer>().setGameObjects(gameObjects);
            line.transform.SetParent(lines.transform);
            line.name = "line "+ i.ToString();
            if(i == 8) 
            { 
                line.GetComponent<lineTracer>().setCircle(true);
            }
            else
            {
                line.GetComponent<lineTracer>().setIndexLine(i);
            }
        }
    }
}
