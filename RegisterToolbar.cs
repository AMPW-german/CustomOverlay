using ToolbarControl_NS;
using UnityEngine;

namespace CustomOverlay
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod("Customoverlay_NS", "CustomOverlay");
        }
    }
}
