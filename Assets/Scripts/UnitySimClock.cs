using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using simProcess;
using System.IO;

public class UnitySimClock : MonoBehaviour {

    static public UnitySimClock instance;

    public double timeScale = 1;

    public bool simOn = false;
    bool simStarted = false;
    public bool pause;

    public bool simRestarted = false;
    float pastTime = 0.0f;

    public SimClock clock = new SimClock();

    public Text timeCounter;
    public Text earningCounter;
    public float maxTime;

    public List<SElement> elements = new List<SElement>();
    public List<UnityMultiLink> mLinks = new List<UnityMultiLink>();

    Timer updateTime;

    //UI
    public GameObject initialPanel;
    public GameObject controlPanel;
    public CameraController theCameraController;
    //Data to Export
    string fileName;
    StreamWriter sr;

    void Awake()
    {
        UnitySimClock.instance = this;

        simOn = false;
        initialPanel.SetActive(true);
        controlPanel.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {
        updateTime = new Timer();
    }

    // Update is called once per frame
    void Update()
    {
        if (simOn && !simRestarted)
        {
            if (simStarted)
            {
                clock.advanceClock((Time.time - pastTime + Time.deltaTime) * timeScale);

                if (timeCounter != null)
                {
					timeCounter.text = Math.Round(clock.getSimulationTime(), 1).ToString();
                }
                if (earningCounter != null)
                {
					earningCounter.text = Math.Round(SimCosts.getEarnings()).ToString();
                }

                if (Time.time - pastTime > maxTime)
                {
                    generateReport();
                    Debug.Log("Time is over");
                    Application.Quit();
                }
            }
            else
            {
                foreach (SElement theElem in elements)
                {
                    theElem.initializeSim(); //Es necesario darle nombres fijos a cada uno. Los assembler y Multiserver se identifican por él
                }
                foreach (SElement theElem in elements)
                {
                    theElem.connectSim();
                }
                foreach (UnityMultiLink umLink in mLinks)
                {
                    umLink.connectSim();

                }
                foreach (SElement theElem in elements)
                {
                    theElem.startSim();
                }

                clock.advanceClock((Time.time - pastTime + Time.deltaTime) * timeScale);
                simStarted = true;

                clock.scheduleEvent(updateTime, 0.1);

            }

        }
        

        if(Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void paused()
    {

        pause = !pause;

        if (pause)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void configureScenario()
    {
        simOn = !simOn;
        
        SimCosts.restartEarnings();

        initialPanel.SetActive(false);
        controlPanel.SetActive(true);
        if (theCameraController != null)
        {
            theCameraController.changeCamera();
        }
        
        if (simRestarted == true)
        {
            foreach (SElement theElem in elements)
            {
                theElem.restartSim();
            }

            pastTime = Time.time;
            simRestarted = false;
        }
    }

    public void restartSim()
    {
        simRestarted = true;
        simOn = false;
       
        initialPanel.SetActive(true);
        controlPanel.SetActive(false);

        earningCounter.text = "0";
        timeCounter.text = "0";

        clock.reset();   
    }

    public float getPastTime()
    {
        return pastTime;
    }


    //UI
    public void generateReport()
    {
        DateTime moment = new DateTime();

        fileName = "Report_" + moment.Hour + "_" + moment.Day + "_" + moment.Month + ".txt";
        sr = File.CreateText(fileName);
        sr.Write("On" + DateTime.Today + System.Environment.NewLine);
        foreach (SElement theElem in elements)
        {
            if (theElem.getReport() != null)
            {
                sr.Write(theElem.getReport());
                sr.WriteLine(System.Environment.NewLine);
            }
        }

        sr.Write("Beneficio total:" + this.earningCounter.text);
        sr.WriteLine(System.Environment.NewLine);

        sr.Close();
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
