using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trial
{

    // input
    public string group;
    public string participant;
    public string training;

    public string cardSet;
    public string collabEnvironememn;

    // measures
    // public float size;
    // public int tct;
    // public float mux, muy, muz;

        //tag 
    public int nbTag = 0;
    public int nbChangeTag = 0;

        //tp
    public int nbSyncTp  = 0;
    public int nbAsyncTP = 0;
       
    public int nbSyncTpWall = 0;
    public int nbAsyncTPWall = 0;

    public int nbSyncTpGround = 0;
    public int nbAsyncTPGround = 0;

        //card
    public int nbDragCard = 0;
    public int nbGroupCardTP = 0;

    public int nbDestroyCard = 0;
    public int nbUndoCard = 0;

    public string pathLog = "";
    public StreamWriter kineWriter;
    private float timer = 0;


    public Trial(
        string g_, string p_, string train,
        string cardS, string colabEnv
        )
    {
        group = g_; participant = p_;  training = train;
        cardSet = cardS; collabEnvironememn = colabEnv;
        timer = Time.time;
    }
    public string StringToLog()
    {
        string str = group + ";" + participant + ";" + training + ";" + cardSet + ";" + collabEnvironememn;

        return str;
    }


    // Tag 
    public void incNbTag(string nameR)
    {
        nbTag = nbTag + 1;
        kineWriter.WriteLine(Time.time - timer + ";" + " Tag" + " ; color : " + nameR);
        kineWriter.Flush();
    }

    public void incNbChangeTag(string nameR)
    {
        nbChangeTag = nbChangeTag + 1;
        kineWriter.WriteLine(Time.time - timer + ";" + " Change Tag" + " ; color : " + nameR);
        kineWriter.Flush();
    }

    //TP 
    public void incNbSyncTp()
    {
        nbSyncTp = nbSyncTp + 1;
    }

    public void incNbAsyncTP()
    {
        nbAsyncTP = nbAsyncTP + 1;
    }

    public void incNbSyncTpWall(Vector3 translateVector)
    {
        nbSyncTpWall = nbSyncTpWall + 1;
        kineWriter.WriteLine(Time.time - timer +";" + " Sync TP Wall" + " ;  translateVector : " + translateVector);
        kineWriter.Flush();
    }
    public void incNbAsyncTPWall(Vector3 translateVector)
    {
        nbAsyncTPWall = nbAsyncTPWall + 1;
        kineWriter.WriteLine(Time.time - timer + ";" + " Async TP Wall" + " ; translateVector : " + translateVector);
        kineWriter.Flush();
    }
    public void incNbSyncTpGround(Vector3 translateVector)
    {
        nbSyncTpGround = nbSyncTpGround + 1;
        kineWriter.WriteLine(Time.time - timer + ";" + " Sync Tp Ground" + " ; translateVector : " + translateVector);
        kineWriter.Flush();
    }
    public void incNbAsyncTPGround(Vector3 translateVector)
    {
        nbAsyncTPGround = nbAsyncTPGround + 1;
        kineWriter.WriteLine(Time.time - timer + ";" + " Async Tp Ground" + " ; translateVector : " + translateVector);
        kineWriter.Flush();
    }

    //card
    public void incNbDragCard()
    {
        nbDragCard = nbDragCard + 1;
        kineWriter.WriteLine(Time.time - timer + ";" + " Drag card ");
        kineWriter.Flush();
    }
    public void incNbGroupCardTP(string namewall)
    {
        nbGroupCardTP = nbGroupCardTP + 1;
        kineWriter.WriteLine(Time.time - timer + ";" + " GroupCardTP" + " ; wall : " + namewall);
        kineWriter.Flush();
    }

    public void incNbDestroyCard()
    {
        nbDestroyCard = nbDestroyCard + 1;
        kineWriter.WriteLine(Time.time - timer + ";" + " Destroy card ");
        kineWriter.Flush();
    }
    public void incNbUndoCard()
    {
        nbUndoCard = nbUndoCard + 1;
        kineWriter.WriteLine(Time.time - timer + ";" + " Undo destroy ");
        kineWriter.Flush();
    }
}