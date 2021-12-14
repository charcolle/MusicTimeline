using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace MusicTimeline
{
    [Serializable]
    [DisplayName( "Music Timeline" )]
    public class MusicClip : PlayableAsset, ITimelineClipAsset
    {
        public int basicBpm = 120;

        public int barCount = 0;
        public int basicNumLoopBar = 4;
        public int basicBeatPerBar = 4;
        public int basicUnitPerBar = 16;
        public int basicUnitPerBeat = 4;

        public AudioClip audioClip;
        public List<Section> sections = new List<Section>();

        public TimelineClip TimelineClip { get; set; }

        public double ClipStartTime {
            get {
                return TimelineClip.start;
            }
        }

        public override Playable CreatePlayable( PlayableGraph graph, GameObject go )
        {
            if (audioClip == null)
                return Playable.Null;

            var musicalTimelineAudioPlayable = ScriptPlayable<MusicBehaviour>.Create( graph );
            musicalTimelineAudioPlayable.GetBehaviour().audioClip = audioClip;
            musicalTimelineAudioPlayable.GetBehaviour().sections = sections;
            musicalTimelineAudioPlayable.GetBehaviour().timelineClip = TimelineClip;
            return musicalTimelineAudioPlayable;
        }

        public ClipCaps clipCaps => ClipCaps.SpeedMultiplier;

        public override double duration {
            get {
                if (audioClip == null)
                    return base.duration;
                return (double)audioClip.samples / audioClip.frequency;
            }
        }

    }
}