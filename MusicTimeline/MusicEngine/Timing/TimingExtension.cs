namespace MusicTimeline
{
    public static class TimingExtension
    {
        public static int GetMusicalTime( this Timing timing, Section section )
        {
            return timing.Bar * section.UnitPerBar + timing.Beat * section.UnitPerBeat + timing.Unit;
        }

        public static void Fix( this Timing timing, Section section )
        {
            int totalUnit = timing.Bar * section.UnitPerBar + timing.Beat * section.UnitPerBeat + timing.Unit;
            var bar = totalUnit / section.UnitPerBar;
            var beat = (totalUnit - timing.Bar * section.UnitPerBar) / section.UnitPerBeat;
            var unit = (totalUnit - timing.Bar * section.UnitPerBar - timing.Beat * section.UnitPerBeat);
            timing.Set( bar, beat, unit );
        }

    }
}