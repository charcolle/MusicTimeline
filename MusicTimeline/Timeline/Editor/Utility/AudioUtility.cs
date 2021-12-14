using System.Reflection;
using UnityEngine;

namespace MusicTimeline
{
    public class AudioUtility : MonoBehaviour
    {
        private readonly static MethodInfo getClipPosition;

        static AudioUtility()
        {
            var audioUtilType = Assembly.Load( "UnityEditor.dll" ).GetType( "UnityEditor.AudioUtil" );

            getClipPosition = audioUtilType.GetMethod( "GetClipPosition" );

        }

        public static float GetClipPosition( AudioClip clip )
        {
            return 0f;
        }

    }
}