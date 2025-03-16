using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomOverlay
{
    public static class FlightDataManager
    {
        public static flightData stringToValue(string s)
        {
            switch (s)
            {
                case "airspeed":
                    return flightData.airspeed;
                case "speed":
                    return flightData.speed;
                case "horizontalSpeed":
                    return flightData.horizontalSpeed;
                case "orbitalSpeed":
                    return flightData.orbitalSpeed;
                case "altitude_asl":
                    return flightData.altitude_asl;
                case "altitude":
                    return flightData.altitude;
                case "apoapsis":
                    return flightData.apoapsis;
                case "periapsis":
                    return flightData.periapsis;
                case "currentG":
                    return flightData.currentG;
                case "missionTime":
                    return flightData.missionTime;
                case "missionTimeFormatted":
                    return flightData.missionTimeFormatted;
                case "None":
                default:
                    return flightData.None;
            }
        }

        public static float FlightData(flightData type)
        {
            switch (type)
            {
                case flightData.airspeed:
                    return (float) FlightGlobals.ActiveVessel.indicatedAirSpeed;
                case flightData.speed:
                    return (float) FlightGlobals.ActiveVessel.speed;
                case flightData.horizontalSpeed:
                    return (float)FlightGlobals.ActiveVessel.horizontalSrfSpeed;
                case flightData.orbitalSpeed:
                    return (float)FlightGlobals.ActiveVessel.obt_speed;
                case flightData.altitude_asl:
                    return (float)FlightGlobals.ActiveVessel.altitude;
                case flightData.altitude:
                    return (float)FlightGlobals.ActiveVessel.terrainAltitude;
                case flightData.apoapsis:
                    return (float)FlightGlobals.ActiveVessel.GetCurrentOrbit().ApA;
                case flightData.periapsis:
                    return (float)FlightGlobals.ActiveVessel.GetCurrentOrbit().ApR;
                case flightData.currentG:
                    return (float)FlightGlobals.ActiveVessel.geeForce;
                case flightData.missionTime:
                case flightData.missionTimeFormatted:
                    return (float)FlightGlobals.ActiveVessel.missionTime;
                case flightData.None:
                default:
                    return -1;
            }
        }
    }
}
