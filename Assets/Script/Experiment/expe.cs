using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading;

public class expe
{

    public string participant = "P01";
    public int startTrial = 1;

    private string expeDescriptionFile = "Experiments/expe";
    //static string[] letters = {"H", "N", "K", "R"};
    static string[] letters = { "evertnone", "ehornone" };
    private List<Trial> theTrials;
    private int ctrial = -1;
    private StreamWriter writer;
    private StreamWriter kineWriter;
    private bool haveEyesCondition = false;

    static class ThreadSafeRandom
    {
        [ThreadStatic] private static System.Random Local;

        public static System.Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new System.Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }



    public expe(string part)
    {
        participant = part;

        Debug.Log("VisExpe :" + expeDescriptionFile + " " + participant);

        TextAsset mytxtData = (TextAsset)Resources.Load(expeDescriptionFile);
        string txt = mytxtData.text;
        List<string> lines = new List<string>(txt.Split('\n'));
        int i = 0;
        

        theTrials = new List<Trial>();
        Trial trial;

        foreach (string str in lines)
        {
            List<string> values = new List<string>(str.Split(';'));
            if (values[1] == participant)
            {
                    trial = new Trial(
                        values[0], values[1], values[2],
                        values[3], values[4]
                    );

                    Debug.Log("Goupe: " + trial.group + "Participant: " + trial.participant +
                              "training: " + trial.training + "cardSet: " + trial.cardSet +
                              "collabEnvironememn: " + trial.collabEnvironememn);
            }
        }
      //  Debug.Log("Goupe: " + trial.group + );
        /*
        // file name should look like  "class-PXX-2019-MM-DD-HH-MM-SS.csv"
        string mydate = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string path = "Assets/Resources/logs/class-" + participant + "-" + mydate + ".csv";
        //string path = "Assets/Resources/logs/test.csv";

        //Write some text to the test.txt file
        writer = new StreamWriter(path, false);
        writer.WriteLine(
            // "factor"
            "Participant;Trial;Training;UEyes;UPos;UDist;DVertPos;DDist;UserX;UserY;UserZ;DiscX;DiscY;Letter;FrontColor;BackColor;Lines"
            // measure
            + ";Size;TCT;ErrorCount;PosErrorCount;MUserX;MUserY;MUserZ"
            + ";AvgUserX;AvgUserY;AvgUserZ;AvgDist"
            + ";PW;PH;GW;GH;MX;MY");
        writer.Flush();
        path = "Assets/Resources/logs/class-" + participant + "-" + mydate + ".kine";
        kineWriter = new StreamWriter(path, false);
        kineWriter.WriteLine(
            "## ");
        kineWriter.Flush();
        */
    }

    public void Finished()
    {
        writer.Close();
        kineWriter.Close();
    }

}
