using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEditor;
using UnityEditor.Timeline;

namespace MusicTimeline
{

    [CustomTimelineEditor( typeof( MusicClip ) )]
    public class MusicClipEditor : ClipEditor
    {

        private WaveformPreview waveformPreview;
        private MusicEngine musicalTimeline;

        public override ClipDrawOptions GetClipOptions( TimelineClip clip )
        {
            return new ClipDrawOptions()
            {
                errorText = GetErrorText( clip ),
                tooltip = string.Empty,
                highlightColor = Color.grey,
                icons = System.Linq.Enumerable.Empty<Texture2D>()
            };
        }

        public override void OnCreate( TimelineClip clip, TrackAsset track, TimelineClip clonedFrom )
        {
            var c = clip.asset as MusicClip;
            var sec = new Section( 0, c.basicUnitPerBar, c.basicUnitPerBeat, c.basicBpm, 4 )
            {
                Name = $"Section{c.sections.Count + 1}",
            };
            c.sections.Add( sec );
        }

        public override void DrawBackground( TimelineClip clip, ClipBackgroundRegion region )
        {
            var rect = region.position;
            if (rect.width <= 0)
                return;

            EditorGUI.DrawRect( rect, Color.grey );
            var audioPlayableAsset = clip.asset as MusicClip;
            var musicRect = GetPreviewRect( audioPlayableAsset.audioClip, clip.duration, region );
            var sampleWidthOffset = musicRect.width / (float)audioPlayableAsset.audioClip.samples;

            BackgroundDrawer( musicRect, sampleWidthOffset, audioPlayableAsset );
            AudioWaveDrawer( region, audioPlayableAsset.audioClip );
            BeatDrawer( musicRect, sampleWidthOffset, audioPlayableAsset );
            BarDrawer( musicRect, sampleWidthOffset, audioPlayableAsset );

            OnUpdateSectionData( clip, audioPlayableAsset );
        }

        #region drawer

        private void BackgroundDrawer( Rect rect, float sampleWidthOffset, MusicClip asset )
        {
            var sections = asset.sections;
            for (int i = 0; i < sections.Count; i++)
            {
                var sec = sections[i];
                var secWidth = sec.Samples * sampleWidthOffset;

                var secRect = new Rect( rect.x + (sampleWidthOffset * sec.StartSample), rect.y, secWidth, rect.height );
                EditorGUI.DrawRect( secRect, GetSectionColor( i ) );
            }
        }

        private void AudioWaveDrawer( ClipBackgroundRegion region, AudioClip audioClip )
        {
            var previewRect = region.position;
            previewRect.yMax -= 3f;
            previewRect.yMin += 3f;

            if (waveformPreview == null)
            {
                waveformPreview = WaveformPreviewFactory.Create( 0, audioClip );
                Color waveColor = Color.black;
                Color transparent = waveColor;
                waveColor.a = 0.5f;
                transparent.a = 0;
                waveformPreview.backgroundColor = transparent;
                waveformPreview.waveColor = waveColor;
                waveformPreview.SetChannelMode( WaveformPreview.ChannelMode.MonoSum );
                waveformPreview.updated = () => TimelineEditor.Refresh( RefreshReason.WindowNeedsRedraw );
            }

            waveformPreview.looping = false;
            waveformPreview.SetTimeInfo( region.startTime, region.endTime - region.startTime );
            waveformPreview.OptimizeForSize( previewRect.size );

            if (Event.current.type == EventType.Repaint)
            {
                waveformPreview.ApplyModifications();
                waveformPreview.Render( previewRect );
            }
        }

        private void BarDrawer( Rect rect, float sampleWidthOffset, MusicClip asset )
        {

            var sections = asset.sections;
            var icon = EditorGUIUtility.Load( "icons/blendKeySelected.png" ) as Texture2D;
            for (int i = 0; i < sections.Count; i++)
            {
                var sec = sections[i];
                for (int j = 0; j < sec.BarCount; j++)
                {
                    var barSample = sec.StartSample + sec.SamplesPerBar * j;

                    var barRect = new Rect( rect.x + (sampleWidthOffset * barSample), rect.y, 1f, rect.height );
                    EditorGUI.DrawRect( barRect, Color.red );

                    var guiColor = GUI.color;
                    GUI.color = Color.clear;
                    var iconRect = new Rect( rect.x + (sampleWidthOffset * barSample) - 3.75f, rect.y + (rect.height * 0.5f) - 4f, 10f, 10f );
                    EditorGUI.DrawTextureTransparent( iconRect, icon );
                    GUI.color = guiColor;
                }
            }
        }

        private void BeatDrawer( Rect rect, float sampleWidthOffset, MusicClip asset )
        {

            var sections = asset.sections;
            var icon = EditorGUIUtility.Load( "icons/blendSampler.png" ) as Texture2D;
            for (int i = 0; i < sections.Count; i++)
            {
                var sec = sections[i];
                for (int j = 0; j < sec.BarCount * sec.BeatPerBar; j++)
                {
                    var barSample = sec.StartSample + sec.SamplesPerBeat * j;


                    var barRect = new Rect( rect.x + (sampleWidthOffset * barSample), rect.y, 1f, rect.height );
                    EditorGUI.DrawRect( barRect, Color.grey );

                    var guiColor = GUI.color;
                    GUI.color = Color.clear;
                    var iconRect = new Rect( rect.x + (sampleWidthOffset * barSample) -3f, rect.y + (rect.height * 0.5f) - 3f, 6f, 6f );
                    EditorGUI.DrawTextureTransparent( iconRect, icon );
                    GUI.color = guiColor;
                }
            }

        }

        #endregion
        private void OnUpdateSectionData( TimelineClip clip, MusicClip asset )
        {
            var barSum = -1;
            var sampleSum = -1;
            var startTime = (float)clip.start;
            for (int i = 0; i < asset.sections.Count; i++)
            {
                var section = asset.sections[i];

                section.StartBar = barSum + 1;
                section.StartSample = sampleSum + 1;
                asset.sections[i].Fix( startTime, asset.audioClip.frequency );

                barSum += section.BarCount;
                sampleSum += section.Samples;
                startTime += section.DurationTime();
            }
        }

        #region utility

        private Rect GetPreviewRect( AudioClip clip, double duration, ClipBackgroundRegion region )
        {
            var previewRect = region.position;

            var visibleTime = region.endTime - region.startTime;
            var pixelPerSecond = region.position.width / visibleTime;

            var inVisibleEndLength = pixelPerSecond * (duration - region.endTime);

            previewRect.xMin -= previewRect.x;
            previewRect.width += previewRect.x + (float)inVisibleEndLength;
            return previewRect;
        }

        private Color GetSectionColor( int index )
        {
            Color color;
            switch (index)
            {
                case 0:
                    color = Color.red;
                    break;
                case 1:
                    color = Color.yellow;
                    break;
                case 2:
                    color = Color.green;
                    break;
                case 3:
                    color = Color.cyan;
                    break;
                case 4:
                    color = Color.blue;
                    break;
                case 5:
                    color = Color.magenta;
                    break;
                case 6:
                    color = Color.white;
                    break;
                default:
                    color = Color.black;
                    break;
            }
            color = new Color( color.r, color.g, color.b, 0.5f );
            return color;
        }

        #endregion

    }
}