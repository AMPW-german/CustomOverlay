using Expansions.Missions.Tests;
using KSP.UI.Screens;
using KSP.UI.Screens.SpaceCenter.MissionSummaryDialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                return (float) resource.vesselResourceCurrent;
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

            //Resources.ForEach(res =>
            //{   
            //    res.Update();
            //});
        }
    }
}
