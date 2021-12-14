using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace MusicTimeline
{
    [Serializable]
    [TrackClipType( typeof( MusicClip ), false )]
    [TrackBindingType( typeof( MusicEngine ) )]
    public class MusicTrack : TrackAsset
    {
        public override Playable CreateTrackMixer( PlayableGraph graph, GameObject go, int inputCount )
        {
            var clips = GetClips();
            foreach (var clip in clips)
            {
                var audioPlayable = clip.asset as MusicClip;
                clip.displayName = " ";
                audioPlayable.TimelineClip = clip;
            }
            return ScriptPlayable<MusicMixerBehaviour>.Create( graph, inputCount );
        }
    }
}