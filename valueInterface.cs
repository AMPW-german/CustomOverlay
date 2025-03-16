using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        airspeed,
        speed,
        horizontalSpeed,
        orbitalSpeed,
        altitude_asl,
        altitude,
        apoapsis,
        periapsis,
        currentG,
        missionTime,
        missionTimeFormatted,
    }

    public interface valueInterface
    {
        valueMode Mode { get; }

        bool autoScale { get; }
        float value { get; }
        float maxValue {  get; }

        void updateValue();
    }
}
