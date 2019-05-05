using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kuazoo.helper
{
    public abstract class DistanceApi
    {
        public abstract double GetDistance(double locationAlat, double LocationAlong, double locationBlat, double locationBlong, DistanceApi.DistanceType type);

        protected double ToRadian(double val)
        {
            return (Math.PI / 180) * val;
        }

        public enum DistanceType 
        { 
            Miles, 
            Kilometers 
        };
    }
}
