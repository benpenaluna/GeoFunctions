using System;
using System.Collections.Generic;
using System.Text;

namespace GeoFunctions.Core.Coordinates
{
    public class ValueChangedEventArgs : EventArgs
    {
        public double NewValue { get; set; }
        public double OldValue { get; set; }
        
        public ValueChangedEventArgs() : base() { }
    }
}
