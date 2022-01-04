using System;

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

    public int nbTag = 0;
    public int nbChangeTag = 0;

    public int nbSyncTp  = 0;
    public int nbAsyncTP = 0;
       
    /**
        public int nbSyncTpWall = 0;
        public int nbAsyncTPWall = 0;

        public int nbSyncTpGround = 0;
        public int nbAsyncTPGround = 0;
     **/

    public Trial(
        string g_, string p_, string train,
        string cardS, string colabEnv
        )
    {
        group = g_; participant = p_;  training = train;
        cardSet = cardS; collabEnvironememn = colabEnv;


    }
    public string StringToLog()
    {
        string str = group + ";" + participant + ";" + training + ";" + cardSet + ";" + collabEnvironememn;

        return str;
    }

    public void incNbTag()
    {
        nbTag = nbTag + 1;
    }

    public void incNbChangeTag()
    {
        nbChangeTag = nbChangeTag + 1;
    }

    public void incNbSyncTp()
    {
        nbSyncTp = nbSyncTp + 1;
    }

    public void incNbAsyncTP()
    {
        nbAsyncTP = nbAsyncTP + 1;
    }

}