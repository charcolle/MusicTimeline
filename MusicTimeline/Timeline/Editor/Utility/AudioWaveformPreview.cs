using System;
using System.Reflection;
using UnityEngine;

namespace MusicTimeline
{

    public static class WaveformPreviewFactory
    {

        private static MethodInfo waveFormPreviewFactoryMethod;

        static WaveformPreviewFactory()
        {
            var waveformPreviewFactoryType = Assembly.Load( "UnityEditor.dll" ).GetType( "UnityEditor.WaveformPreviewFactory" );
            var bindingFlags = BindingFlags.Public | BindingFlags.Static;
            waveFormPreviewFactoryMethod = waveformPreviewFactoryType.GetMethod( "Create", bindingFlags );
        }

        public static WaveformPreview Create( int initialSize, AudioClip clip )
        {
            var waveFormPreviewObj = (object)waveFormPreviewFactoryMethod.Invoke( null, new object[] { initialSize, clip } );
            return new WaveformPreview( waveFormPreviewObj );
        }

    }

    public class WaveformPreview
    {

        private object waveformPreview;
        private Type waveformPreviewType;

        private Type channelModeEnumType;

        private MethodInfo setTimeInfoMethod;
        private MethodInfo setChannelModeMethod;
        private MethodInfo setChannelModeSpecificMethod;
        private MethodInfo optimizedForSizeMethod;
        private MethodInfo applyModificationsMethod;
        private MethodInfo renderMethod;

        private PropertyInfo waveColorProperty;
        private PropertyInfo backgroundColorProperty;
        private PropertyInfo loopProperty;
        private EventInfo updatedEvent;

        public WaveformPreview( object waveformPreview )
        {
            this.waveformPreview = waveformPreview;
            waveformPreviewType = waveformPreview.GetType();

            var bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            channelModeEnumType = Assembly.Load( "UnityEditor.dll" ).GetType( "UnityEditor.WaveformPreview+ChannelMode" );

            setTimeInfoMethod = waveformPreviewType.GetMethod( "SetTimeInfo", bindingFlags );
            setChannelModeMethod = waveformPreviewType.GetMethod( "SetChannelMode", new Type[] { channelModeEnumType } );
            setChannelModeSpecificMethod = waveformPreviewType.GetMethod( "SetChannelMode", new Type[] { channelModeEnumType, typeof( Int64 ) } );
            optimizedForSizeMethod = waveformPreviewType.GetMethod( "OptimizeForSize", bindingFlags );
            applyModificationsMethod = waveformPreviewType.GetMethod( "ApplyModifications", bindingFlags );
            renderMethod = waveformPreviewType.GetMethod( "Render", bindingFlags );

            waveColorProperty = waveformPreviewType.GetProperty( "waveColor", bindingFlags );
            backgroundColorProperty = waveformPreviewType.GetProperty( "backgroundColor", bindingFlags );
            loopProperty = waveformPreviewType.GetProperty( "looping", bindingFlags );
            updatedEvent = waveformPreviewType.GetEvent( "updated", bindingFlags );
        }

        public Color waveColor {
            set {
                waveColorProperty.SetValue( waveformPreview, value );
            }
        }

        public Color backgroundColor {
            set {
                backgroundColorProperty.SetValue( waveformPreview, value );
            }
        }

        public bool looping {
            set {
                loopProperty.SetValue( waveformPreview, value );
            }
        }

        public Action updated {
            set {
                updatedEvent.AddEventHandler( waveformPreview, value );
            }
        }

        public void SetTimeInfo( double start, double length )
        {
            setTimeInfoMethod.Invoke( waveformPreview, new object[] { start, length } );
        }

        public void SetChannelMode( ChannelMode mode )
        {
            var enumType = Enum.Parse( channelModeEnumType, Enum.GetName( typeof( ChannelMode ), mode ) );
            setChannelModeMethod.Invoke( waveformPreview, new object[] { enumType } );
        }

        public void SetChannelMode( ChannelMode mode, int specificChannelToRender )
        {
            var enumType = Enum.Parse( channelModeEnumType, Enum.GetName( typeof( ChannelMode ), mode ) );
            setChannelModeMethod.Invoke( waveformPreview, new object[] { enumType, specificChannelToRender } );
        }

        public void OptimizeForSize( Vector2 newSize )
        {
            optimizedForSizeMethod.Invoke( waveformPreview, new object[] { newSize } );
        }

        public bool ApplyModifications()
        {
            return (bool)applyModificationsMethod.Invoke( waveformPreview, null );
        }

        public void Render( Rect rect )
        {
            renderMethod.Invoke( waveformPreview, new object[] { rect } );
        }

        public enum ChannelMode
        {
            MonoSum = 0,
            Separate = 1,
            SpecificChannel = 2
        }
    }
}
