using Sandbox;
using Sandbox.Graphics.GUI;
using SpaceEngineers.Game.GUI;
using VRage.Utils;
using VRageMath;

namespace MorePPSettings
{
    class PPGui : MyGuiScreenBase
    {

        private MyGuiControlCheckbox ambient, playerShake, eyeadaption, vignette, fullbright, bloom, chromatic, glare;
        private MyGuiControlSlider bloomSize, bloomMulti, vignetteLength, vignetteStart, bloomSlidr, bloomDirt, chromaticFactor, saturation, brightness;
        private MyGuiControlColor headlamp;

        public PPGui() : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.7f, 0.7f), false, null, MySandboxGame.Config.UIBkOpacity, MySandboxGame.Config.UIOpacity, null)
        {
            base.EnabledBackgroundFade = true;
            this.RecreateControls(true);
        }

        public override void RecreateControls(bool constructor)
        {
            base.RecreateControls(constructor);
            var lbl = new MyGuiControlLabel(new Vector2(-.13f, -.32f), null, "More PP Settings - By: Math0424");
            Controls.Add(lbl);

            MyGuiControlSeparatorList seperator = new MyGuiControlSeparatorList();
            seperator.Position = new Vector2(0, 0);
            seperator.AddHorizontal(new Vector2(-.3f, -.14f), .6f, .002f);
            Elements.Add(seperator);

            //bloom
            bloomSlidr = AddSliderLabel("Bloom Scale", new Vector2(.08f, -.25f), 0, 400, PPSettings.Static.BloomScale);
            bloomDirt = AddSliderLabel("Bloom Dirt", new Vector2(.08f, -.18f), 0, 1, PPSettings.Static.BloomDirtRatio);

            bloomMulti = AddSliderLabel("Bloom Mult", new Vector2(-.17f, -.25f), 0, 1, PPSettings.Static.BloomMult);
            bloomSize = AddSliderLabel("Bloom Size", new Vector2(-.17f, -.18f), 0, 10, PPSettings.Static.BloomSize, true);

            bloom = AddCheckbox("Bloom", new Vector2(.22f, -.215f), PPSettings.Static.EnableBloom);

            //misc slider
            brightness = AddSliderLabel("Brightness", new Vector2(-.23f, .04f), 0, 10, PPSettings.Static.Brightness);
            saturation = AddSliderLabel("Saturation", new Vector2(-.23f, .11f), 0, 10, PPSettings.Static.Saturation);

            //chromatic
            chromatic = AddCheckbox("Chromatic Ab.", new Vector2(.24f, .09f), PPSettings.Static.EnableChromaticAberration);
            chromaticFactor = AddSlider(new Vector2(.1f, .09f), -1.5f, 1.5f, PPSettings.Static.ChromaticFactor);

            //vignette
            vignette = AddCheckbox("Vignette", new Vector2(.24f, -.025f), PPSettings.Static.EnableVignette);
            vignetteLength = AddSliderLabel("Length", new Vector2(.1f, .01f), 0, 10, PPSettings.Static.VignetteLength);
            vignetteStart = AddSliderLabel("Start", new Vector2(.1f, -.07f), 0, 10, PPSettings.Static.VignetteStart);

            //Coloring
            headlamp = AddColorSlider("Headlamp Color", new Vector2(-.2f, -.08f), PPSettings.Static.HeadlampColor);
            headlamp.Color.Alpha(255);

            //ambient
            ambient = AddCheckbox("Ambient Occlusion", new Vector2(-.31f, .17f), PPSettings.Static.EnableAmbientOcclusion);
            eyeadaption = AddCheckbox("Eye Adaptation", new Vector2(-.31f, .22f), PPSettings.Static.EnableEyeAdaption);

            //misc
            playerShake = AddCheckbox("Player shake", new Vector2(-.1f, .17f), PPSettings.Static.EnablePlayerShake);
            fullbright = AddCheckbox("Fullbright", new Vector2(-.1f, .22f), PPSettings.Static.EnableFullbright);

            glare = AddCheckbox("Sun glare", new Vector2(.05f, .17f), PPSettings.Static.EnableSunGlare);

            var button = new MyGuiControlButton()
            {
                Position = new Vector2(-0.23f, .3f),
                Text = "Save & Exit",
            };
            Controls.Add(button);
            button.ButtonClicked += (e) => { ApplySettings(); PPSettings.Save(); CloseScreen(); };


            var button3 = new MyGuiControlButton()
            {
                Position = new Vector2(0f, .3f),
                Text = "Save",
            };
            Controls.Add(button3);
            button3.ButtonClicked += (e) => { ApplySettings(); PPSettings.Save(); };

            var button2 = new MyGuiControlButton()
            {
                Position = new Vector2(0.23f, .3f),
                Text = "Exit",
            };
            Controls.Add(button2);
            button2.ButtonClicked += (e) => { CloseScreen(); };
        }

        public void ApplySettings()
        {
            PPSettings.Static.EnableAmbientOcclusion = ambient.IsChecked;
            PPSettings.Static.EnablePlayerShake = playerShake.IsChecked;
            PPSettings.Static.EnableEyeAdaption = eyeadaption.IsChecked;
            PPSettings.Static.EnableVignette = vignette.IsChecked;
            PPSettings.Static.EnableFullbright = fullbright.IsChecked;
            PPSettings.Static.EnableBloom = bloom.IsChecked;
            PPSettings.Static.EnableChromaticAberration = chromatic.IsChecked;
            PPSettings.Static.EnableSunGlare = glare.IsChecked;

            PPSettings.Static.BloomMult = bloomMulti.Value;
            PPSettings.Static.BloomSize = bloomSize.Value;
            PPSettings.Static.BloomScale = bloomSlidr.Value;
            PPSettings.Static.BloomDirtRatio = bloomDirt.Value;
            PPSettings.Static.VignetteLength = vignetteLength.Value;
            PPSettings.Static.VignetteStart = vignetteStart.Value;
            PPSettings.Static.ChromaticFactor = chromaticFactor.Value;

            PPSettings.Static.Brightness = brightness.Value;
            PPSettings.Static.Saturation = saturation.Value;

            PPSettings.Static.HeadlampColor = headlamp.Color;

            PPSettings.Apply();
        }

        public MyGuiControlSlider AddSlider(Vector2 position, float min, float max, float value, bool intval = false)
        {
            var sldr = new MyGuiControlSlider(position, min, max, .2f, value, null, "{0}", intval ? 0 : 1, 0.8f, 0.04f, "Blue", null, MyGuiControlSliderStyleEnum.Default, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, intval, true);
            Controls.Add(sldr);
            return sldr;
        }

        public MyGuiControlColor AddColorSlider(string name, Vector2 position, Color startColor)
        {
            var sldr = new MyGuiControlColor(name, .8f, position, startColor, startColor, MyStringId.GetOrCompute("AAA"));
            Controls.Add(sldr);
            return sldr;
        }

        public MyGuiControlSlider AddSliderLabel(string name, Vector2 position, float min, float max, float value, bool intval = false)
        {
            var sldr = new MyGuiControlSlider(position, min, max, .2f, value, null, "{0}", intval ? 0 : 1, 0.8f, 0.05f, "Blue", null, MyGuiControlSliderStyleEnum.Default, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, intval, true);
            var lbl = new MyGuiControlLabel(position + new Vector2(-.1f, -.04f), null, name);
            Controls.Add(lbl);
            Controls.Add(sldr);
            return sldr;
        }

        public MyGuiControlCheckbox AddCheckbox(string name, Vector2 position, bool toggle)
        {
            var box = new MyGuiControlCheckbox(position, null, name, toggle);
            var lbl = new MyGuiControlLabel(position + new Vector2(.015f, 0), null, name);
            Controls.Add(lbl);
            Controls.Add(box);
            return box;
        }

        public void OpenPreviousGUI()
        {
            MyGuiSandbox.AddScreen(new MyGuiScreenOptionsGraphics());
        }

        public override string GetFriendlyName()
        {
            return "PPGui";
        }
    }
}
