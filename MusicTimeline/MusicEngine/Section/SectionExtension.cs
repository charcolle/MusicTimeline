namespace MusicTimeline
{
    public static class SectionExtension
    {
        public static int EndBar( this Section section )
        {
            return section.StartBar + section.BarCount;
        }

        public static int EndSample( this Section section )
        {
            return section.StartSample + section.Samples;
        }

        public static float DurationTime( this Section section )
        {
            return (((float)section.UnitPerBar / (float)section.UnitPerBeat) * 60f * (float)section.BarCount) / (float)section.Bpm;
        }

        public static float BarDurationTime( this Section section )
        {
            return DurationTime( section ) / (float)section.BarCount;
        }

        public static float BeatDurationTime( this Section section )
        {
            return BarDurationTime( section ) / ((float)section.UnitPerBar / (float)section.UnitPerBeat);
        }

    }

}