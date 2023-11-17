﻿using Comfort.Common;
using EFT;
using EFT.Airdrop;
using SIT.Core.AkiSupport.Airdrops;
using SIT.Tarkov.Core;
using System.Linq;
using System.Reflection;

namespace Aki.Custom.Airdrops.Patches
{
    public class AirdropFlarePatch : ModulePatch
    {
        private static readonly string[] _usableFlares = { "624c09cfbc2e27219346d955", "62389ba9a63f32501b1b4451" };

        protected override MethodBase GetTargetMethod()
        {
            return typeof(FlareCartridge).GetMethod(nameof(FlareCartridge.Init),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        [PatchPostfix]
        private static void PatchPostfix(BulletClass flareCartridge)
        {
            var gameWorld = Singleton<GameWorld>.Instance;
            var points = LocationScene.GetAll<AirdropPoint>().Any();

            if (gameWorld != null && points && _usableFlares.Any(x => x == flareCartridge.Template._id))
            {
                gameWorld.gameObject.AddComponent<AirdropsManager>().isFlareDrop = true;
            }
        }
    }
}