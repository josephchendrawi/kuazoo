using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kuazoo.helper
{
    public class HarversineFormula : DistanceApi
    {
        /// <summary>
        /// a = sin²(Δlat/2) + cos(lat1).cos(lat2).sin²(Δlong/2)
        /// c = 2.atan2(√a, √(1−a))
        /// d = R.c
        /// </summary>
        /// <param name="locationA"></param>
        /// <param name="locationB"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public override double GetDistance(double locationAlat, double LocationAlong, double locationBlat, double locationBlong, DistanceApi.DistanceType type)
        {
            double distance = 0.0;
            double R = (type == DistanceType.Miles) ? 3960 : 6371;

            double dLat = ToRadian(locationBlat - locationAlat);
            double dLon = ToRadian(locationBlong - LocationAlong);
            double latA = ToRadian(locationAlat);
            double latB = ToRadian(locationBlat);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(latA) * Math.Cos(latB) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            distance = R * c;

            return distance;
        }
    }
}
