using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PresetLoadExceptionSkip
{
    [BepInPlugin("COM3D2.PresetLoadExceptionSkip.Plugin", "COM3D2.PresetLoadExceptionSkip.Plugin", "21.6.6")]// 버전 규칙 잇음. 반드시 2~4개의 숫자구성으로 해야함. 미준수시 못읽어들임
    //[BepInProcess("COM3D2x64.exe")]
    public class PresetLoadExceptionSkip : BaseUnityPlugin
    {
        static ManualLogSource log;

        public void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(PresetLoadExceptionSkip));
            log = Logger;
        }

        static string preset;//= Environment.CurrentDirectory + @"\preset\";
        static string error;//= Environment.CurrentDirectory + @"\preset\ERROR\";

        public void Start()
        {
            preset = Environment.CurrentDirectory + @"\preset\";
            error = Environment.CurrentDirectory + @"\preset\ERROR\";
        }

        [HarmonyPatch(typeof(CharacterMgr), "PresetLoad", new Type[] { typeof(BinaryReader), typeof(string) })]
        [HarmonyFinalizer]
        public static void PresetLoadPostfix(ref Exception __exception, string f_strFileName)
        {
            if (__exception == null)
            {
                return;
            }
            __exception = null;

            log.LogWarning("Preset Load error file : " + f_strFileName);

            if (!Directory.Exists(error) )
            {
                Directory.CreateDirectory(error);
            }
            File.Move(preset+f_strFileName, error+ f_strFileName);
        }
    }
}
