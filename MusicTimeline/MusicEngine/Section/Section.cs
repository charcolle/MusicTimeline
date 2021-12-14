using System;
using UnityEngine;

//
// algorism by geekdrums
// https://github.com/geekdrums/MusicEngine
//

namespace MusicTimeline
{
    [Serializable]
    public class Section
    {

        public string Name;
        public bool IsLoop;

        #region section varies

        public double Bpm;
        public int UnitPerBeat;
        public int UnitPerBar;
        public int BarCount;
        public int BeatPerBar;

        public int StartBar;
        public int StartSample;
        public int EndBar;
        public int EndSample;

        public int Samples;
        public int SamplesPerUnit;
        public int SamplesPerBeat;
        public int SamplesPerBar;

        public float SectionStartTime;

        #endregion

        #region public method

        public Section( int startbar, int mtBar = 16, int mtBeat = 4, double bpm = 120, int numLoopBar = 4 )
        {
            StartBar = startbar;
            UnitPerBar = mtBar;
            UnitPerBeat = mtBeat;
            Bpm = bpm;
            BarCount = numLoopBar;
            BeatPerBar = 4;
        }

        public void Fix( float startTime, int samplingRate )
        {
            SectionStartTime = startTime;

            double beatSec = 60F / (Bpm * (UnitPerBeat / 4 ) );
            SamplesPerUnit = (int)(samplingRate * (beatSec / UnitPerBeat));
            SamplesPerBeat = (int)(samplingRate * beatSec);
            SamplesPerBar = (int)(samplingRate * UnitPerBar * (beatSec / UnitPerBeat));

            Samples = SamplesPerBar * BarCount;
            EndBar = StartBar + BarCount - 1;
            EndSample = StartSample + Samples;
        }

        public void Adjust()
        {
            Samples = EndSample - StartSample;
            BarCount = Mathf.RoundToInt( Samples / (float)SamplesPerBar );
            EndBar = StartBar + BarCount;
        }
        #endregion

    }
}