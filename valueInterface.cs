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
    public enum valueMode
    {
        None,
        resource,
        flightData
    }

    public enum flightData
    {
        None,
        throttle,
        airspeed,
        mach,
        speed,
        horizontalSpeed,
        orbitalSpeed,
        altitude_asl,
        altitude,
        apoapsis,
        periapsis,
        currentG,
        timeToAP,
        timeToAPFormatted,
        timeToPE,
        timeToPEFormatted,
        missionTime,
        missionTimeFormatted,
    }

    public interface valueInterface
    {
        valueMode Mode { get; }

        bool autoScale { get; }
        float value { get; }
        float maxValue { get; }
        string resourceType { get; }

        void updateValue();
    }
}
