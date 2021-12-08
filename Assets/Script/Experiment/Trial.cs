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

    public Trial(
        string g_, string p_, string train,
        string cardSet, string colabEnvironement
        )
    {
        group = g_; participant = p_;  training = train;
        
    }
    public string StringToLog()
    {
        string str = group + ";" + participant + ";" + training + ";" + cardSet + ";" + collabEnvironememn;

        return str;
    }
}