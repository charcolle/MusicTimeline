using UnityEngine;
using UnityEngine.Playables;

namespace MusicTimeline
{
    public class MusicMixerBehaviour : PlayableBehaviour
    {

        private MusicEngine musicEngine = default;
        private int playingClipIndex = -1;

        public override void ProcessFrame( Playable playable, FrameData info, object playerData )
        {
            if (musicEngine == null)
            {
                musicEngine = playerData as MusicEngine;
                if (musicEngine == null)
                    return;
            }

#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                if (info.seekOccurred)
                {
                    musicEngine.AudioSource.Stop();
                    return;
                }
            }
#endif

            var inputCount = playable.GetInputCount();
            bool hasInputWeight = false;
            int inputIndex = 0;
            MusicBehaviour currentBehaviour = new MusicBehaviour();
            for (int i = 0; i < inputCount; i++)
            {
                if (playable.GetInputWeight( i ) > 0f )
                {
                    hasInputWeight = true;
                    inputIndex = i;
                    currentBehaviour = ((ScriptPlayable<MusicBehaviour>)playable.GetInput( i )).GetBehaviour();
                }
            }

            if( hasInputWeight )
            {
                if (inputIndex != playingClipIndex)
                    OnFirstPlay( currentBehaviour, playable );
                ProcessSection( currentBehaviour, playable );
                playingClipIndex = inputIndex;
            } else
            {
                musicEngine.AudioSource.Stop();
                playingClipIndex = -1;
            }
        }

        public override void OnBehaviourPause( Playable playable, FrameData info )
        {
            if (musicEngine == null)
                return;
            musicEngine.AudioSource.Stop();
            playingClipIndex = -1;
        }


        private Section currentSection {
            get; set;
        }

        private Timing justTiming { get; set; }
        private Timing nearTiming { get; set; }
        private Timing prevJustTiming { get; set; }
        private Timing prevNearTiming { get; set; }
        private double timeSecFromJust { get; set; }

        private void OnFirstPlay( MusicBehaviour audioBehaviour, Playable playable )
        {
            UnityEngine.Debug.Log( "OnFirstPlay" );
            musicEngine.AudioSource.clip = audioBehaviour.audioClip;
            musicEngine.AudioSource.Play();

            nearTiming = new Timing( 0, 0, -1 );
            justTiming = new Timing( 0, 0, -1 );
            prevNearTiming = new Timing( nearTiming );
            prevJustTiming = new Timing( justTiming );

            timeSecFromJust = 0;
            currentSection = null;
        }

        private void ProcessSection( MusicBehaviour audioBehaviour, Playable playable )
        {
            // Change if musicEngine has section change cue.
            if (justTiming.OnBar())
            {
                if (IsMusicEngineHasCue)
                {
                    if(musicEngine.ChangeSectionIndexCue >= audioBehaviour.sections.Count)
                    {
                        Debug.LogWarning( "MusicTimeline: Section Index is Invalid." );
                        musicEngine.ChangeSectionIndexCue = -1;
                    } else
                    {
                        ChangeSection( audioBehaviour.sections[musicEngine.ChangeSectionIndexCue] );
                        musicEngine.ChangeSectionIndexCue = -1;
                        if(currentSection != null)
                        prevJustTiming.Copy( audioBehaviour.SamplesToTiming( currentSection.StartSample, currentSection ) );
                    }
                }
            }

            // Check if section is loop when section change.
            var section = audioBehaviour.GetCurrentSection( musicEngine.AudioSource.timeSamples );
            if( section != currentSection )
            {
                if (currentSection != null && currentSection.IsLoop )
                {
                    ChangeSection( currentSection );
                    Debug.Log( "change with loop" );
                } else
                {
                    OnSectionChange();
                    currentSection = section;
                }
            }

            if (currentSection == null)
                return;

            // now  MusicEngine process
            var sample = musicEngine.AudioSource.timeSamples;
            var timing = audioBehaviour.SamplesToTiming( sample, currentSection );
                //timeSecFromJust = audioBehaviour.TimeSecFromJust( currentSection, timing, sample );

            justTiming.Copy( timing );
                //nearTiming.Copy( justTiming );
                //if (!audioBehaviour.IsFormerHalf( currentSection, timeSecFromJust ))
                //{
                //    nearTiming.Increment();
                //    nearTiming.Fix( currentSection );
                //}
                //if (sample + currentSection.SamplesPerUnit / 2 >= currentSection.Samples)
                //    nearTiming.Reset();

            if (justTiming.Equals( prevJustTiming ) == false)
            {
                Debug.Log( justTiming + " " + prevJustTiming );
                OnTiming( justTiming );
            }

            prevJustTiming.Copy( justTiming );
                //prevNearTiming.Copy( nearTiming );

            musicEngine.MusicEngineUpdate( currentSection );
        }

        private void ChangeSection( Section nextSection )
        {
            musicEngine.PlayableDirector.time = nextSection.SectionStartTime;
            musicEngine.AudioSource.timeSamples = nextSection.StartSample;
            currentSection = nextSection;
        }

        private void OnTiming( Timing timing )
        {
            if (timing.OnBar())
            {
                musicEngine.OnBar?.Invoke();
            }

            if (timing.OnBeat())
                musicEngine.OnBeat?.Invoke();
        }

        private void OnSectionChange()
        {
        }

        private bool IsMusicEngineHasCue {
            get {
                return musicEngine.ChangeSectionIndexCue > -1;
            }
        }

    }
}