using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using TMPro;
using UnityEngine.UI;

public class csvReader : MonoBehaviour
{
    private List<Vector3> pointList = new List<Vector3>();
    private List<GameObject> gameObjects = new List<GameObject>();

    private List<List<Vector3>> globalListCam1;
    private List<List<Vector3>> globalListCam2;
    private List<List<Vector3>> globalListCam3;
    private List<List<Vector3>> globalListCam4;

    [SerializeField]
    private GameObject prefabCube, prefabEmpty, prefabLine;

    private GameObject lines, people;
    private bool runExercice = false;

    [SerializeField]
    private float offsetX, offsetY, offsetZ, multW, multH;

    [SerializeField]
    private int minFrameCount;

    private string[] directories;
    private string[] files;
    private string[] videos;

    [SerializeField]
    private TMP_Dropdown dropdown;
    [SerializeField]
    private TMP_Text text;
    private List<string> dropOptions = new List<string>();

    [SerializeField]
    private camManager camManager;

    [SerializeField]
    private Toggle toggle;
    private bool OneCSVmode = false;

    // Start is called before the first frame update
    void Start()
    {
        directories = Directory.GetDirectories(@"..\Mediapipe-3D-Transcription-main\Assets\CSV");

        foreach (string dir in directories)
        {
            System.IO.FileInfo monDir = new System.IO.FileInfo(dir);
            string name = monDir.Name;
            dropOptions.Add(name);
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(dropOptions);
        OnDropDownChange();
    }

    // Update is called once per frame
    void Update()
    {
        int frameCountOnVideo = camManager.GetFrameCount();
        int currentFrameOnVideo = camManager.GetFrameIndex();

        if (runExercice)
        {
            int frameDiff = frameCountOnVideo - minFrameCount;
            if(currentFrameOnVideo > frameDiff)
            {
                for (int i = 0; i < pointList.Count; i++)
                {
                    gameObjects[i].transform.position = globalListCam4[currentFrameOnVideo-frameDiff][i];
                }
            }
        }
    }

    public void OnToggleChangeCSV()
    {
        if (toggle.isOn)
        {
            OneCSVmode = true;
        }
        else
        {
            OneCSVmode = false;
        }
        OnDropDownChange();
    }

    public void OnDropDownChange()
    {
        Destroy(people);
        Destroy(lines);
        runExercice = false;

        files = Directory.GetFiles(directories[dropdown.value], "*.csv");
        

        if(files.Length == 4) 
        {
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
                        globalListCam4 = ReadCSVFile(monfile.FullName);
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

            for (int n = 0; n < minFrameCount; n++)
            {
                for (int i = 0; i < pointList.Count; i++)
                {
                    float newX, newY, newZ;

                    if (!OneCSVmode)
                    {
                        newX = globalListCam4[n][i].x;
                        newX *= multW;
                        newX += offsetX;

                        newY = (globalListCam4[n][i].y + globalListCam3[n][i].y) / 2;
                        newY *= -1;
                        newY *= multH;
                        newY += offsetY;

                        newZ = globalListCam3[n][i].x;
                        newZ *= multW;
                        newZ += offsetZ;
                    }
                    else
                    {
                        newX = globalListCam4[n][i].x;
                        newY = globalListCam4[n][i].y;
                        newZ = globalListCam4[n][i].z;
                    }
                    

                    globalListCam4[n][i] = new Vector3(newX, newY, newZ);
                }
            }

            videos = Directory.GetFiles(directories[dropdown.value], "*.mp4");
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
        else { runExercice = false; }

        if (runExercice)
        {
            InstanciateCube();
            InstanciateLines();
            text.text = dropdown.options[dropdown.value].text + " is running ...";
        }
        else
        {
            text.text = "Files missing for : " + dropdown.options[dropdown.value].text;
            camManager.SetClipsList(new List<string>());
        }
    }


    public List<List<Vector3>> ReadCSVFile(string filename)
    {
        StreamReader strReader = new StreamReader(filename);
        List<List<Vector3>> globalList = new List<List<Vector3>>();
        
        bool endOfFile = false;

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
            float[] data_float = new float[data_values.Length];

            for(int i = 0; i < data_values.Length; i++)
            {
                data_float[i] = float.Parse(data_values[i], CultureInfo.InvariantCulture.NumberFormat);
                //data_float[i] *= multiplicator;
            }
            
            pointList = new List<Vector3>();

            for (int i = 0; i < data_values.Length; i += 3)
            {
                pointList.Add(new Vector3(data_float[i], data_float[i + 1], data_float[i + 2]));
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
            GameObject cube = Instantiate(prefabCube, globalListCam4[0][i], Quaternion.identity);
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
