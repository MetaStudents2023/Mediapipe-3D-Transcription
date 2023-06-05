using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using TMPro;

public class csvReader : MonoBehaviour
{
    private List<Vector3> pointList = new List<Vector3>();
    private List<GameObject> gameObjects = new List<GameObject>();

    private List<List<Vector3>> globalListCam1;
    private List<List<Vector3>> globalListCam2;
    private List<List<Vector3>> globalListCam3;
    private List<List<Vector3>> globalListCam4;

    private float xVal, yVal, zVal;

    [SerializeField]
    private GameObject prefabCube, prefabEmpty, prefabLine;

    private GameObject lines, people;
    private bool runExercice = false;

    private int indexFrame = 0;
    private int frameWait = 0;

    [SerializeField]
    private float offsetX, offsetY, offsetZ, multiplicator;

    private int minFrameCount;

    public string[] directories;
    public string[] files;

    [SerializeField]
    private TMP_Dropdown dropdown;
    [SerializeField]
    private TMP_Text text;
    private List<string> dropOptions = new List<string>();

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
        if(runExercice)
        {
            if (frameWait == 4)
            {
                indexFrame++;

                if (indexFrame >= minFrameCount)
                {
                    indexFrame = 0;
                }

                for (int i = 0; i < pointList.Count; i++)
                {
                    gameObjects[i].transform.position = globalListCam4[indexFrame][i];
                }

                frameWait = 0;
            }
            frameWait++;
        }
        
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

            if (globalListCam3.Count != globalListCam4.Count)
            {
                if (globalListCam3.Count < globalListCam4.Count)
                {
                    minFrameCount = globalListCam3.Count;
                }
                else
                {
                    minFrameCount = globalListCam4.Count;
                }
            }

            for (int n = 0; n < minFrameCount; n++)
            {
                for (int i = 0; i < pointList.Count; i++)
                {
                    float newX = globalListCam4[n][i].x;
                    float newY = (globalListCam4[n][i].y + globalListCam3[n][i].y) / 2;
                    float newZ = globalListCam3[n][i].x;

                    globalListCam4[n][i] = new Vector3(newX, newY, newZ);
                }
            }

            InstanciateCube();
            InstanciateLines();

            runExercice = true;
            text.text = dropdown.options[dropdown.value].text + " is running ...";
        }
        else
        {
            text.text = "Files missing for : " + dropdown.options[dropdown.value].text;
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
                data_float[i] *= multiplicator;
            }
            
            pointList = new List<Vector3>();

            for (int i = 0; i < data_values.Length; i += 3)
            {
                xVal = offsetX + data_float[i];
                yVal = offsetY + data_float[i + 1] * -1;
                zVal = offsetZ + data_float[i + 2] * -1;

                pointList.Add(new Vector3(xVal, yVal, zVal));
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
