using Sandbox.Engine.Utils;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.World;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using VRage.FileSystem;
using VRageMath;
using VRageRender;

namespace MorePPSettings
{
    public class PPSettings
    {
        public static Settings Static;

        private static MyPostprocessSettings.Layout sett;

        public static bool IsDifferent()
        {
            return !MyPostprocessSettingsWrapper.Settings.Data.Equals(sett);
        }

        public struct Settings
        {
            public bool EnableBloom;
            public bool EnableEyeAdaption;

            public bool EnablePlayerShake;
            public bool EnableAmbientOcclusion;
            public bool EnableChromaticAberration;
            public bool EnableVignette;
            public bool EnableFullbright;

            public bool EnableSunGlare;

            public float BloomScale;
            public float BloomDirtRatio;
            public float BloomMult;
            public float BloomSize;

            public float Saturation;
            public float Brightness;

            public float ChromaticFactor;

            public float VignetteStart;
            public float VignetteLength;

            public Color HeadlampColor;
        }

        public static Settings DefaultSettings = new Settings
        {
            EnableBloom = true,
            EnableEyeAdaption = true,

            EnablePlayerShake = true,
            EnableAmbientOcclusion = true,
            EnableChromaticAberration = true,
            EnableVignette = true,
            EnableFullbright = false,

            EnableSunGlare = true,

            BloomScale = 40,
            BloomDirtRatio = 0.75f,
            BloomMult = 0.1f,
            BloomSize = 4,

            Saturation = 1,
            Brightness = 1,

            ChromaticFactor = .03f,

            VignetteStart = 1.84f,
            VignetteLength = 2.5f,

            HeadlampColor = Color.White,
        };

        public static void Apply()
        {

            if (MySector.MainCamera == null)
            {
                return;
            }

            MyPostprocessSettingsWrapper.AllEnabled = true;

            MyFakes.SUN_GLARE = Static.EnableSunGlare;

            //eye
            if (Static.EnableEyeAdaption)
            {
                MySector.SunProperties.EnvironmentLight.EnvShadowFadeoutMultiplier = 0;
            }
            else
            {
                MySector.SunProperties.EnvironmentLight.EnvShadowFadeoutMultiplier = 0.1f;
            }
            
            if (Static.EnableFullbright)
            {
                MySector.SunProperties.EnvironmentLight.EnvShadowFadeoutMultiplier = 1;
                MySector.SunProperties.EnvironmentLight.EnvSkyboxBrightness = 5;
                MySector.SunProperties.EnvironmentLight.SkyboxBrightness = 50;
            } 
            else
            {
                MySector.SunProperties.EnvironmentLight.EnvSkyboxBrightness = 1;
                MySector.SunProperties.EnvironmentLight.SkyboxBrightness = 5;
            }

            if (Static.EnablePlayerShake)
            {
                MySector.MainCamera.CameraShake.MaxShake = 0;
            } 
            else
            {
                MySector.MainCamera.CameraShake.MaxShake = 15;
            }

            //headlamp
            MyCharacter.POINT_COLOR = Static.HeadlampColor;
            MyCharacter.REFLECTOR_COLOR = Static.HeadlampColor;
            MyCharacter.LIGHT_PARAMETERS_CHANGED = true;

            //Occlusion
            MySector.SunProperties.EnvironmentLight.AOIndirectLight = Static.EnableAmbientOcclusion ? 1 : 0;
            MySector.SunProperties.EnvironmentLight.AODirLight = Static.EnableAmbientOcclusion ? 1 : 0;


            //Bloom
            MyPostprocessSettingsWrapper.Settings.Data.BloomEmissiveness = Static.BloomScale;
            MyPostprocessSettingsWrapper.Settings.BloomSize = (int)Static.BloomSize;
            MyPostprocessSettingsWrapper.Settings.Data.BloomMult = Static.BloomMult;

            MyPostprocessSettingsWrapper.Settings.Data.BloomDirtRatio = Static.BloomDirtRatio;

            MyPostprocessSettingsWrapper.Settings.BloomEnabled = Static.EnableBloom;

            //Vignette
            MyPostprocessSettingsWrapper.Settings.Data.VignetteLength = Static.VignetteLength;
            MyPostprocessSettingsWrapper.Settings.Data.VignetteStart = Static.EnableVignette ? Static.VignetteStart : 0;
            
            //Chromatic
            MyPostprocessSettingsWrapper.Settings.Data.ChromaticFactor = Static.EnableChromaticAberration ? Static.ChromaticFactor : 0;

            MyPostprocessSettingsWrapper.Settings.Data.Brightness = Static.Brightness;
            MyPostprocessSettingsWrapper.Settings.Data.Saturation = Static.Saturation;

            sett = MyPostprocessSettingsWrapper.Settings.Data;
        }

        public static void Load()
        {
            if (FileExsists("PPSettings.cfg"))
            {
                var reader = ReadFileStream("PPSettings.cfg");
                Static = SerializeFromXML<Settings>(reader.ReadToEnd());
                reader.Close();
            }
            else
            {
                Save();
            }
        }

        public static void Save()
        {
            if (FileExsists("PPSettings.cfg"))
            {
                DeleteFile("PPSettings.cfg");
            }
            var writer = GetFileStreamToStorage("PPSettings.cfg");
            writer.Write(SerializeToXML(Static));
            writer.Close();
        }

        //Just copy and paste keen because MyApiGateway.Util is null when i need it most---
        private static T SerializeFromXML<T>(string xml)
        {
            T result;
            if (string.IsNullOrEmpty(xml))
            {
                result = default(T);
                return result;
            }
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader stringReader = new StringReader(xml))
            {
                using (XmlReader xmlReader = XmlReader.Create(stringReader))
                {
                    result = (T)((object)xmlSerializer.Deserialize(xmlReader));
                }
            }
            return result;
        }

        private static string SerializeToXML<T>(T objToSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(objToSerialize.GetType());
            StringWriter stringWriter = new StringWriter();
            xmlSerializer.Serialize(stringWriter, objToSerialize);
            return stringWriter.ToString();
        }

        private static StreamReader ReadFileStream(string file)
        {
            Stream stream = MyFileSystem.OpenRead(Path.Combine(MyFileSystem.UserDataPath, "Storage", "MorePPSettings", file));
            return new StreamReader(stream);
        } 

        private static StreamWriter GetFileStreamToStorage(string file)
        {
            Stream stream = MyFileSystem.OpenWrite(Path.Combine(MyFileSystem.UserDataPath, "Storage", "MorePPSettings", file), FileMode.Create);
            return new StreamWriter(stream);
        }

        private static bool FileExsists(string file)
        {
            return file.IndexOfAny(Path.GetInvalidFileNameChars()) == -1 && File.Exists(Path.Combine(MyFileSystem.UserDataPath, "Storage", "MorePPSettings", file));
        }

        private static void DeleteFile(string file)
        {
            if (FileExsists(file))
                File.Delete(Path.Combine(MyFileSystem.UserDataPath, "Storage", "MorePPSettings", file));
        }


    }
}
