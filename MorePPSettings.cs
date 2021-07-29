using HarmonyLib;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using Sandbox.ModAPI;
using SpaceEngineers.Game.GUI;
using VRage.Plugins;
using VRage.Utils;
using VRageMath;
using VRageRender;

namespace MorePPSettings
{
    public class MorePPSettings : IPlugin
    {
        public void Init(object gameInstance)
        {
            var x = new Harmony("MorePPSettings");
            PPSettings.Static = PPSettings.DefaultSettings;
            PPSettings.Load();
            x.PatchAll();

            MySession.AfterLoading += Loaded;
        }

        [HarmonyPatch(typeof(MyGuiScreenOptionsGraphics))]
        public class ControlPanelOverridePatch
        {

            [HarmonyPostfix]
            [HarmonyPatch("RecreateControls")]
            static void ControlsAddition(ref MyGuiScreenOptionsGraphics __instance)
            {
                Vector2 value = new Vector2(90f) / MyGuiConstants.GUI_OPTIMAL_SIZE;
                float y = MyGuiConstants.SCREEN_CAPTION_DELTA_Y * 0.5f;
                Vector2 vector = (__instance.Size.Value / 2f - value) * new Vector2(-1f, -1f) + new Vector2(0f, y);
                Vector2 value8 = vector + new Vector2(0.4f, 0f);
                Vector2 value4 = new Vector2(0f, 0.045f);
                var button = new MyGuiControlButton()
                {
                    Position = value8 + 15f * value4,
                    Text = "More PP Settings",
                };
                button.ButtonClicked += OpenPPGui;
                __instance.Controls.Add(button);
            }

            public static void OpenPPGui(MyGuiControlButton btn)
            {
                MyGuiSandbox.AddScreen(new PPGui());
            }

        }

        //probably a dumb idea- but sometimes it just resets
        int count = 300;
        public void Update()
        {
            if (count > 0)
            {
                count--;
                return;
            }
            if (MyAPIGateway.Session != null)
            {
                if (PPSettings.IsDifferent())
                {
                    PPSettings.Apply();
                }
            }
        }

        public void Loaded()
        {
            PPSettings.Apply();
        }

        public void Dispose()
        {

        }
    }
}
