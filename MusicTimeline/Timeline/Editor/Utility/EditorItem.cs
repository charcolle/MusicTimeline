using UnityEditor;
using UnityEngine;

namespace MusicTimeline {

    public abstract class EditorItem<T> {

        [SerializeField]
        protected T data;

        public EditorItem( T data ) {
            this.data = data;
        }

        public T Data {
            get {
                return data;
            }
        }

        public void OnGUI() {

            if( data == null ) {
                DrawIfDataIsNull();
                return;
            }

            EditorGUI.BeginChangeCheck();

            Draw();

            if( EditorGUI.EndChangeCheck() )
                GUI.changed = true;
        }

        public void OnGUI( Rect rect ) {
            if( data == null ) {
                DrawIfDataIsNull();
                return;
            }

            EditorGUI.BeginChangeCheck();
            Draw( rect );
            if( EditorGUI.EndChangeCheck() )
                GUI.changed = true;
        }

        protected virtual void Draw() { }
        protected virtual void Draw( Rect rect ) { }
        protected virtual void DrawIfDataIsNull() { }

    }

}