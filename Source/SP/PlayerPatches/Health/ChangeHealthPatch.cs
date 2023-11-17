﻿using SIT.Core.Misc;
using SIT.Tarkov.Core;
using System.Linq;
using System.Reflection;

namespace SIT.Core.SP.PlayerPatches.Health
{
    internal class ChangeHealthPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            var t = PatchConstants.EftTypes
                .First(x =>
                    ReflectionHelpers.GetMethodForType(x, "ChangeHealth") != null
                    && ReflectionHelpers.GetMethodForType(x, "Kill") != null
                    && ReflectionHelpers.GetMethodForType(x, "DoPainKiller") != null
                    && ReflectionHelpers.GetMethodForType(x, "DoScavRegeneration") != null
                    && x.GetMethod("GetOverallHealthRegenTime", BindingFlags.Public | BindingFlags.Instance) == null // We don't want this one
                    );
            Logger.LogInfo("ChangeHealth:" + t.FullName);
            var method = ReflectionHelpers.GetMethodForType(t, "ChangeHealth");

            Logger.LogInfo("ChangeHealth:" + method.Name);
            return method;
        }

        [PatchPostfix]
        public static void PatchPostfix(
            object __instance
            , EBodyPart bodyPart
            , float value
            , object damageInfo)
        {
            if (__instance == HealthListener.Instance.MyHealthController)
            {
                HealthListener.Instance.CurrentHealth.Health[bodyPart].ChangeHealth(value);
            }
        }
    }
}
