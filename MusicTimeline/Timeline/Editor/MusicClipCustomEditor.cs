using UnityEngine;
using UnityEditor;

namespace MusicTimeline
{
    [CustomEditor( typeof( MusicClip ) )]
    public class MusicClipCustomEditor : Editor
    {
        private bool isInitialized = false;

        private MusicClip musicClip = default;
        [SerializeField]
        private MusicClipEditorGUI musicClipEditorGUI = default;

        public override void OnInspectorGUI()
        {
            if (!isInitialized)
                onInitialize();

            musicClipEditorGUI.OnGUI();
        }

        private void onInitialize()
        {
            musicClip = target as MusicClip;
            if (musicClipEditorGUI == null)
                musicClipEditorGUI = new MusicClipEditorGUI( musicClip );
            isInitialized = true;
        }
    }

}