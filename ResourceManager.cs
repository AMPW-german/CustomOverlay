using KSP.UI.Screens;
using System.Collections.Generic;
using System.Linq;

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
    public static class ResourceManager
    {
        public static List<ResourceItem> Resources;

        public static float getResource(PartResourceDefinition partResourceDefinition)
        {
            ResourceItem resource = Resources.Where(res => res.resourceID == partResourceDefinition.id).FirstOrDefault();

            if (resource != null)
            {
                return (float)resource.vesselResourceCurrent;
            }
            return 0f;
        }

        public static float getResourceMax(PartResourceDefinition partResourceDefinition)
        {
            ResourceItem resource = Resources.Where(res => res.resourceID == partResourceDefinition.id).FirstOrDefault();

            if (resource != null)
            {
                return (float)resource.vesselResourceTotal;
            }
            return 0f;
        }

        public static void update()
        {
            Resources = ResourceDisplay.Instance.resourceItems;
        }
    }
}
