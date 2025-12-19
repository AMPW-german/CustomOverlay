// Custom Overlay
// This mod allows you to use fully functional UIs in KSP
// Copyright (C) 2025 AMPW

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/

namespace CustomOverlay
{
    public static class FlightDataManager
    {
        public static flightData stringToValue(string s)
        {
            switch (s)
            {
                case "throttle":
                    return flightData.throttle;
                case "airspeed":
                    return flightData.airspeed;
                case "mach":
                    return flightData.mach;
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
                case "timeToAP":
                    return flightData.timeToAP;
                case "timeToAPFormatted":
                    return flightData.timeToAPFormatted;
                case "timeToPEormatted":
                    return flightData.timeToPEFormatted;
                case "missionTime":
                    return flightData.missionTime;
                case "missionTimeFormatted":
                    return flightData.missionTimeFormatted;
                case "heading":
                    return flightData.heading;
                case "None":
                default:
                    return flightData.None;
            }
        }

        public static float FlightData(flightData type)
        {
            switch (type)
            {
                case flightData.throttle:
                    return FlightGlobals.ActiveVessel.ctrlState.mainThrottle * 100;
                case flightData.airspeed:
                    return (float)FlightGlobals.ActiveVessel.indicatedAirSpeed;
                case flightData.mach:
                    return (float)FlightGlobals.ActiveVessel.mach;
                case flightData.speed:
                    return (float)FlightGlobals.ActiveVessel.speed;
                case flightData.horizontalSpeed:
                    return (float)FlightGlobals.ActiveVessel.horizontalSrfSpeed;
                case flightData.orbitalSpeed:
                    return (float)FlightGlobals.ActiveVessel.obt_speed;
                case flightData.altitude_asl:
                    return (float)FlightGlobals.ActiveVessel.altitude;
                case flightData.altitude:
                    return (float)FlightGlobals.ActiveVessel.radarAltitude;
                case flightData.apoapsis:
                    return (float)FlightGlobals.ActiveVessel.GetCurrentOrbit().ApA;
                case flightData.periapsis:
                    return (float)FlightGlobals.ActiveVessel.GetCurrentOrbit().ApR;
                case flightData.currentG:
                    return (float)FlightGlobals.ActiveVessel.geeForce;
                case flightData.timeToAP:
                case flightData.timeToAPFormatted:
                    return (float)FlightGlobals.ActiveVessel.orbit.timeToAp;
                case flightData.timeToPE:
                case flightData.timeToPEFormatted:
                    return (float)FlightGlobals.ActiveVessel.orbit.timeToPe;
                case flightData.missionTime:
                case flightData.missionTimeFormatted:
                    return (float)FlightGlobals.ActiveVessel.missionTime;
                case flightData.heading:
                    return FlightGlobals.ship_heading;
                case flightData.None:
                default:
                    return -1;
            }
        }
    }
}
