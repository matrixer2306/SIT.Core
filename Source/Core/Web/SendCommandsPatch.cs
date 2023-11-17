﻿using SIT.Core.Misc;
using SIT.Tarkov.Core;
using System.Reflection;

namespace SIT.Core.Core.Web
{
    /// <summary>
    /// This patch removes the wait to push changes from Inventory
    /// </summary>
    internal class SendCommandsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return ReflectionHelpers.GetMethodForType(typeof(BackEnd0.BackEndSession2), "TrySendCommands");
        }

        [PatchPrefix]
        public static bool Prefix(

            ref float ___float_0
            )
        {
            ___float_0 = 0;
            return true;
        }

    }
}
