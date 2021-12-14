using System;
using UnityEngine;

//
// algorism by geekdrums
// https://github.com/geekdrums/MusicEngine
//

namespace MusicTimeline
{
    [Serializable]
    public class Timing : IComparable<Timing>, IEquatable<Timing>
    {

        [SerializeField]
        private int bar = 0;
        public int Bar {
            get {
                return bar;
            }
            private set {
                bar = value;
            }
        }

        [SerializeField]
        private int beat = 0;
        public int Beat {
            get {
                return beat;
            }
            private set {
                beat = value;
            }
        }

        [SerializeField]
        private int unit = 0;
        public int Unit {
            get {
                return unit;
            }
            private set {
                unit = value;
            }
        }

        public Timing() { }

        public Timing( int bar = 0, int beat = 0, int unit = 0 )
        {
            Bar = bar;
            Beat = beat;
            Unit = unit;
        }

        public Timing( Timing copy )
        {
            Copy( copy );
        }

        public void Copy( Timing copy )
        {
            Bar = copy.Bar;
            Beat = copy.Beat;
            Unit = copy.Unit;
        }

        public void Set( int bar, int beat = 0, int unit = 0 )
        {
            Bar = bar;
            Beat = beat;
            Unit = unit;
        }

        public void Reset()
        {
            Bar = 0;
            Beat = 0;
            Unit = 0;
        }

        public void Increment()
        {
            Unit++;
        }

        public void Decrement()
        {
            Unit--;
        }

        public bool OnBar()
        {
            return Beat == 0 && Unit == 0;
        }

        public bool OnBeat()
        {
            return Unit == 0;
        }

        public static bool operator >( Timing t, Timing t2 )
        {
            return t.Bar > t2.Bar || (t.Bar == t2.Bar && t.Beat > t2.Beat) || (t.Bar == t2.Bar && t.Beat == t2.Beat && t.Unit > t2.Unit);
        }

        public static bool operator <( Timing t, Timing t2 )
        {
            return !(t > t2) && !(t.Equals( t2 ));
        }

        public static bool operator <=( Timing t, Timing t2 )
        {
            return !(t > t2);
        }

        public static bool operator >=( Timing t, Timing t2 )
        {
            return t > t2 || t.Equals( t2 );
        }

        public override bool Equals( object obj )
        {
            if (object.ReferenceEquals( obj, null ))
            {
                return false;
            }
            if (object.ReferenceEquals( obj, this ))
            {
                return true;
            }
            if (this.GetType() != obj.GetType())
            {
                return false;
            }
            return this.Equals( obj as Timing );
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals( Timing other )
        {
            return (this.Bar == other.Bar && this.Beat == other.Beat && this.Unit == other.Unit);
        }

        public bool LoopEquals( Timing other )
        {
            return (Beat == other.Beat && Unit == other.Unit);
        }

        public int CompareTo( Timing tother )
        {
            if (this.Equals( tother ))
                return 0;
            else if (this > tother)
                return 1;
            else
                return -1;
        }

        public override string ToString()
        {
            return Bar + " " + Beat + " " + Unit;
        }
    }


}