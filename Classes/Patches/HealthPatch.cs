using Aki.Reflection.Patching;
using EFT.HealthSystem;
using System;
using System.Reflection;

namespace Tarky_Menu.Classes.Patches
{
    public class HealthPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(ActiveHealthController).GetMethod("HandleFall");
        }

        [PatchPrefix]
        static bool Prefix(ActiveHealthController __instance, float height)
        {
            if (PlayerStats.Health.NoFall.Value || PlayerStats.Health.Godmode.Value)
            {
                return false;
            }

            return true;
        }
    }
}
