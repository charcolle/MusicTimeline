using UnityEngine;
using UnityEditor;

namespace MusicTimeline
{
    public static class GUIUtility
    {
        public static class Styles
        {
            static Styles()
            {
                NoSpaceBox = new GUIStyle( GUI.skin.box )
                {
                    margin = new RectOffset( 0, 0, 0, 0 ),
                    padding = new RectOffset( 1, 1, 1, 1 )
                };

                SectionHeader = new GUIStyle( "RL Header" )
                {
                    margin = new RectOffset( 0, 0, 0, 0 ),
                    padding = new RectOffset( 0, 0, 0, 0 ),
                };

                MemoBack = new GUIStyle( "RL Background" )
                {
                    margin = new RectOffset( 0, 0, 0, 0 ),
                    padding = new RectOffset( 1, 1, 1, 1 ),
                    alignment = TextAnchor.MiddleCenter,
                    stretchHeight = false,
                    stretchWidth = false
                };

                MemoLabel = new GUIStyle( GUI.skin.label )
                {
                    wordWrap = true,
                    richText = true,
                };

                LabelWordWrap = new GUIStyle( GUI.skin.label )
                {
                    wordWrap = true,
                    richText = true,
                };

                TextAreaWordWrap = new GUIStyle( GUI.skin.textArea )
                {
                    wordWrap = true
                };

                TextFieldWordWrap = new GUIStyle( GUI.skin.textField )
                {
                    wordWrap = true
                };

                GUIStyleState state = new GUIStyleState();
                state = GUI.skin.box.normal;
                state.textColor = EditorGUIUtility.isProSkin ? new Color( 1f, 1f, 1f, 0.7f ) : Color.black;
                MemoBox = new GUIStyle( GUI.skin.box )
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = state
                };

                SearchField = new GUIStyle( "ToolbarSeachTextField" );

                SearchFieldCancel = new GUIStyle( "ToolbarSeachCancelButton" );

                LargeButtonLeft = new GUIStyle( "LargeButtonLeft" );

                LargeButtonMid = new GUIStyle( "LargeButtonMid" );

                LargeButtonRight = new GUIStyle( "LargeButtonRight" );
            }

            public static GUIStyle NoSpaceBox {
                get;
                private set;
            }

            public static GUIStyle SectionHeader {
                get;
                private set;
            }

            public static GUIStyle MemoBack {
                get;
                private set;
            }

            public static GUIStyle MemoLabel {
                get;
                private set;
            }

            public static GUIStyle LabelWordWrap {
                get;
                private set;
            }

            public static GUIStyle TextAreaWordWrap {
                get;
                private set;
            }
            public static GUIStyle TextFieldWordWrap {
                get;
                private set;
            }

            public static GUIStyle MemoBox {
                get;
                private set;
            }

            public static GUIStyle NoOverflow {
                get;
                private set;
            }

            public static GUIStyle SearchField {
                get;
                private set;
            }

            public static GUIStyle SearchFieldCancel {
                get;
                private set;
            }

            public static GUIStyle LargeButtonLeft {
                get;
                private set;
            }

            public static GUIStyle LargeButtonMid {
                get;
                private set;
            }

            public static GUIStyle LargeButtonRight {
                get;
                private set;
            }
        }
    }
}