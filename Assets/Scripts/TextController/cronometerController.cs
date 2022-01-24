using System;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using simProcess;

//La tecla C comienza la cuenta, R resetea el cronometro

public class cronometerController : MonoBehaviour {

    Text Display;
    bool started = false;
    TimeSpan timeSpan;
    string timeText;
    float pastTime;
    int i = 0;

    StreamWriter sr;
	FileStream file;
	string path = "Assets/Resources/";
    public string fileName;

    void Awake() {

        this.Display = this.GetComponentInChildren<Text>();
        Display.text = "00:00:00";

        if (fileName == null)
        {
            fileName = "MyData.txt";
        }
        //File.OpenWrite(path + fileName);

        sr = File.CreateText(fileName);
        sr.Close();


    }

    // Update is called once per frame
    void Update()
    {
        if (started)
        {
            time();

            if (Input.GetKeyUp(KeyCode.C))
            {
                using (sr = File.AppendText(fileName))
                {
                    //sr = File.CreateText(fileName);
                    sr.WriteLine(i + ":   " + timeText);
					i++;
					//sr.WriteLine("\t");
                    //sr.Close();
                }
            }
            if (Input.GetKeyUp(KeyCode.R))
            {
                pastTime = Time.time;
                i = 0;
                time();
                using (sr = File.AppendText(fileName))
                {
                    //sr = File.CreateText(fileName);
                    sr.WriteLine(i + ":   " + timeText);
                    i++;
                    //sr.WriteLine("\t");
                    //sr.Close();
                }
            }
        }
        else if (Input.GetKey(KeyCode.C) && !started)
        {
            pastTime = Time.time;
            started = true;
            time();
        }        
    }

    public void time()
    {
        timeSpan = TimeSpan.FromSeconds(Time.time - pastTime);
        timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, Mathf.RoundToInt(timeSpan.Milliseconds/10));
        Display.text = timeText;     
    }
}

