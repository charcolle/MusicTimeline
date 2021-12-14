using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;


namespace MusicTimeline
{
    public class MusicClipEditorGUI : EditorItem<MusicClip>
    {

        public MusicClipEditorGUI( MusicClip data ) : base( data )
        {
            OnUpdateAudioData();
            //foreach (var e in data.outputs)
            //{
            //    Debug.Log( e.outputTargetType );
            //    Debug.Log( e.sourceObject );
            //}
        }

        protected override void Draw()
        {
            EditorGUIUtility.labelWidth = 100f;
            EditorGUILayout.BeginVertical();
            {
                BasicSettingArea();
                GUILayout.Space( 10 );
                SectionArea();
            }
            EditorGUILayout.EndVertical();
            EditorGUIUtility.labelWidth = 0f;

            OnUpdateAudioData();
        }

        #region draw process

        private void BasicSettingArea()
        {
            EditorGUILayout.BeginVertical( EditorStyles.helpBox );
            {
                GUILayout.Label( "Basic AudioSettings", EditorStyles.boldLabel );

                EditorGUI.BeginDisabledGroup( true );
                EditorGUILayout.ObjectField( "AudioClip", audioClip, typeof( AudioClip ), false );
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.LabelField( "Total Basic Bar Count", barCount.ToString() );
                GUILayout.Space( 5 );

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( "BPM", GUILayout.Width( 80f ) );
                var tmpBpm = EditorGUILayout.IntSlider( (int)basicBpm, 60, 250 );
                if( tmpBpm != basicBpm )
                {
                    Undo.RecordObject( Data, "Change BasicBPM" );
                    basicBpm = tmpBpm;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( "Bar", GUILayout.Width( 80f ) );
                var tmpBeatPerBar = EditorGUILayout.IntSlider( basicBeatPerBar, 2, 9 );
                if (tmpBeatPerBar != basicBeatPerBar)
                {
                    Undo.RecordObject( Data, "Change BasicBeatPerBar" );
                    basicBpm = tmpBeatPerBar;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( "Beat", GUILayout.Width( 80f ) );
                var tmpUnitPerBeat = EditorGUILayout.IntPopup( basicUnitPerBeat, new string[] { "4", "8" }, new int[] { 4, 8 } );
                if (tmpUnitPerBeat != basicUnitPerBeat)
                {
                    Undo.RecordObject( Data, "Change BasicBeat" );
                    basicUnitPerBeat = tmpUnitPerBeat;
                }
                EditorGUILayout.EndHorizontal();

                basicUnitPerBar = basicBeatPerBar * basicUnitPerBeat;

                var tmpNumLoopBar = EditorGUILayout.IntField( "BarCount", basicNumLoopBar );
                if (tmpNumLoopBar < 1)
                    tmpNumLoopBar = 1;
                if(tmpNumLoopBar != basicNumLoopBar)
                {
                    Undo.RecordObject( Data, "Change BarCount");
                    basicNumLoopBar = tmpNumLoopBar;
                }

                GUILayout.Space( 5 );
            }
            EditorGUILayout.EndVertical();
        }

        [SerializeField]
        private string searchText;
        private void SectionArea()
        {
            EditorGUILayout.BeginVertical( GUI.skin.box );
            {
                SectionHeader();
                sectionLists();
            }
            EditorGUILayout.EndVertical();
        }

        private void SectionHeader()
        {
            EditorGUILayout.BeginHorizontal( EditorStyles.toolbar, GUILayout.ExpandWidth( true ) );
            {
                searchText = EditorGUILayout.TextField( searchText, GUIUtility.Styles.SearchField );
                if (GUILayout.Button( "", GUIUtility.Styles.SearchFieldCancel ))
                {
                    searchText = "";
                    EditorGUIUtility.keyboardControl = 0;
                }
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button( "+", EditorStyles.toolbarButton, GUILayout.Width( 120 ) ))
                {
                    OnAddSection();
                }
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndHorizontal();

        }

        private Vector2 scrollView = Vector2.zero;
        private void sectionLists()
        {
            scrollView = EditorGUILayout.BeginScrollView( scrollView );
            {
                for (int i = 0; i < sections.Count; i++)
                {
                    if (string.IsNullOrEmpty( searchText ) || (!string.IsNullOrEmpty( searchText ) && sections[i].Name.Contains( searchText )))
                        SectionDrawer( sections[i], i );
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private bool isFoldout = false;
        private void SectionDrawer( Section section, int index )
        {
            var guiColor = GUI.backgroundColor;
            GUI.backgroundColor = GetSectionColor( index );
            EditorGUILayout.BeginHorizontal( GUIUtility.Styles.SectionHeader, GUILayout.ExpandWidth( true ) );
            {
                GUILayout.Label( section.Name );
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button( "x" ))
                    sections.Remove( section );
            }
            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = guiColor;

            EditorGUILayout.BeginVertical( GUIUtility.Styles.MemoBack );
            {
                var tmpName = EditorGUILayout.TextField( "SectionName", section.Name );
                if( tmpName != section.Name )
                {
                    Undo.RecordObject( Data, "Change SectionName" );
                    section.Name = tmpName;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( "BPM", GUILayout.Width( 80f ) );
                var tmpBPM = EditorGUILayout.IntSlider( (int)section.Bpm, 60, 250 );
                if (tmpBPM != section.Bpm)
                {
                    Undo.RecordObject( Data, "Change SectionBPM" );
                    section.Bpm = tmpBPM;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( "Bar", GUILayout.Width( 80f ) );
                var tmpBeatPerBar = EditorGUILayout.IntSlider( section.BeatPerBar, 2, 9 );
                if (tmpBeatPerBar != section.BeatPerBar)
                {
                    Undo.RecordObject( Data, "Change SectionBeatPerBar" );
                    section.BeatPerBar = tmpBeatPerBar;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( "Beat", GUILayout.Width( 80f ) );
                var tmpUnitPerBeat = EditorGUILayout.IntPopup( section.UnitPerBeat, new string[] { "4", "8" }, new int[] { 4, 8 } );
                if (tmpUnitPerBeat != section.UnitPerBeat)
                {
                    Undo.RecordObject( Data, "Change SectionUnitPerBeat" );
                    section.UnitPerBeat = tmpUnitPerBeat;
                }
                EditorGUILayout.EndHorizontal();

                section.UnitPerBar = section.BeatPerBar * section.UnitPerBeat;

                GUILayout.Space( 3 );
                GUILayout.Box( "", GUILayout.Height( 2 ), GUILayout.ExpandWidth( true ) );
                GUILayout.Space( 3 );

                var tmpBarCount = EditorGUILayout.IntField( "BarCount", section.BarCount );
                if (section.BarCount < 1)
                    section.BarCount = 1;
                else if (section.BarCount > barCount)
                    section.BarCount = barCount;
                if (tmpBarCount != section.BarCount)
                {
                    Undo.RecordObject( Data, "Change SectionBarCount" );
                    section.BarCount = tmpBarCount;
                }

                var tmpIsLoop = EditorGUILayout.Toggle( "Loop", section.IsLoop);
                if( tmpIsLoop != section.IsLoop )
                {
                    Undo.RecordObject( Data, "Change SectionLoop" );
                    section.IsLoop = tmpIsLoop;
                }

                GUILayout.Space( 3 );
                GUILayout.Box( "", GUILayout.Height( 2 ), GUILayout.ExpandWidth( true ) );
                GUILayout.Space( 3 );

                isFoldout = EditorGUILayout.Foldout( isFoldout, "Section Info" );
                if (isFoldout)
                {
                    EditorGUIUtility.labelWidth = 130f;
                    EditorGUILayout.LabelField( "StartBar", section.StartBar.ToString() );
                    EditorGUILayout.LabelField( "StartSample", section.StartSample.ToString() );
                    EditorGUILayout.LabelField( "EndBar", section.EndBar.ToString() );
                    EditorGUILayout.LabelField( "EndSample", section.EndSample.ToString() );
                    EditorGUILayout.LabelField( "Samples", section.Samples.ToString() );
                    EditorGUILayout.LabelField( "SamplesPerBar", section.SamplesPerBar.ToString() );
                    EditorGUILayout.LabelField( "SamplesPerBeat", section.SamplesPerBeat.ToString() );
                    EditorGUILayout.LabelField( "SamplesPerUnit", section.SamplesPerUnit.ToString() );
                    EditorGUILayout.LabelField( "SectionStartTime", section.SectionStartTime.ToString() );
                    EditorGUILayout.LabelField( "BarTime", section.BarDurationTime().ToString() );
                    EditorGUILayout.LabelField( "BeatTime", section.BeatDurationTime().ToString() );
                    EditorGUIUtility.labelWidth = 0f;
                }
                GUILayout.Space( 3 );
            }
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region callback

        private void OnUpdateAudioData()
        {
            barCount = Mathf.RoundToInt( (basicBpm * (basicUnitPerBeat / 4)) * ((float)Data.duration / 60f) ) / (basicUnitPerBar / basicUnitPerBeat);
        }

        private void OnAddSection()
        {
            var sec = new Section( 0, basicUnitPerBar, basicUnitPerBeat, basicBpm, basicNumLoopBar )
            {
                Name = $"Section{sections.Count + 1}",

            };
            sections.Add( sec );
            EditorGUIUtility.keyboardControl = 0;
        }

        #endregion

        #region properties

        private AudioClip audioClip {
            get {
                return Data.audioClip;
            }
        }

        private int basicBpm {
            get {
                return Data.basicBpm;
            }
            set {
                Data.basicBpm = value;
            }
        }

        private int barCount {
            get {
                return Data.barCount;
            }
            set {
                Data.barCount = value;
            }
        }

        private int basicBeatPerBar {
            get {
                return Data.basicBeatPerBar;
            }
            set {
                Data.basicBeatPerBar = value;
            }
        }

        private int basicNumLoopBar {
            get {
                return Data.basicNumLoopBar;
            }
            set {
                Data.basicNumLoopBar = value;
            }
        }

        private int basicUnitPerBar {
            get {
                return Data.basicUnitPerBar;
            }
            set {
                Data.basicUnitPerBar = value;
            }
        }

        private int basicUnitPerBeat {
            get {
                return Data.basicUnitPerBeat;
            }
            set {
                Data.basicUnitPerBeat = value;
            }
        }

        private List<Section> sections {
            get {
                return Data.sections;
            }
            set {
                Data.sections = value;
            }
        }

        #endregion
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

    }
}