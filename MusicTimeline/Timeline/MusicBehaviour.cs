using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace MusicTimeline
{
    [Serializable]
    public class MusicBehaviour : PlayableBehaviour
    {
        public List<Section> sections = default;
        public AudioClip audioClip = default;
        public TimelineClip timelineClip { get; set; }

        public Timing SamplesToTiming( int currentSample, Section section )
        {
            var timing = new Timing( 0 );

            var musicSample = currentSample - section.StartSample;
            var bar = (musicSample / section.SamplesPerBar) + section.StartBar;
            var beat = (musicSample % section.SamplesPerBar) / section.SamplesPerBeat;
            var unit = ((musicSample % section.SamplesPerBar) % section.SamplesPerBeat) / section.SamplesPerUnit;
            timing.Set( bar, beat, unit );
            timing.Fix( section );

            return timing;
        }

        public Section GetCurrentSection( int currentSample )
        {
            for (int i = 0; i < sections.Count; i++)
            {
                if (sections[i].StartSample <= currentSample && currentSample < sections[i].EndSample)
                    return sections[i];
            }
            return null;
        }

        public double TimeSecFromJust( Section currentSection, Timing currentTiming, int currentSample )
        {
            return (currentSample
                    - currentTiming.Bar * currentSection.SamplesPerBar
                    - currentTiming.Beat * currentSection.SamplesPerBeat
                    - currentTiming.Unit * currentSection.SamplesPerUnit) / audioClip.frequency;
        }

        public bool IsFormerHalf( Section currentSection, double timeSecFromJust )
        {
            return (timeSecFromJust * audioClip.frequency) < currentSection.SamplesPerUnit / 2;
        }

    }
}