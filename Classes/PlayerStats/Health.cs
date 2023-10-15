using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Comfort.Common;
using static Tarky_Menu.Entry;

namespace Tarky_Menu.Classes.PlayerStats
{
    internal class Health
    {
        private static String[] TargetBones = { "head", "spine3", "spine2", "spine1" };
        public static ConfigEntry<Boolean> Godmode { get; private set; }
        public static ConfigEntry<Boolean> Demigod { get; private set; }
        public static ConfigEntry<float> DamageMultiplier { get; private set; }
        public static Boolean Heal { get; private set; }
        public static ConfigEntry<BepInEx.Configuration.KeyboardShortcut> HealButton { get; private set; }
        public static ConfigEntry<Boolean> NoFall { get; private set; }
        public static ConfigEntry<Boolean> HungerEnergyDrain { get; private set; }
        public static ConfigEntry<Boolean> InfiniteHealthBoost { get; private set; }
        public static Boolean HasDoneGodMode { get; private set; }
        public static Boolean HasDoneNoFall { get; private set; }



        public void Awake()
        {
            Godmode = Instance.Config.Bind("Player | Health", "Godmode", false, "Invincible");
            Demigod = Instance.Config.Bind("Player | Health", "Demi-God", false, "Only ur head and thorax are invincible");
            DamageMultiplier = Instance.Config.Bind("Player | Health", "Damage Multiplier", 1f);
            HealButton = Instance.Config.Bind("Player | Health", "Heal", new BepInEx.Configuration.KeyboardShortcut());
            NoFall = Instance.Config.Bind("Player | Health", "No Fall Damage", false);
            HungerEnergyDrain = Instance.Config.Bind("Player | Health", "No Energy/Hunger Drain", false);
        }

        public void godMod()
        {
            if (Instance.LocalPlayer != null && Instance.LocalPlayer.ActiveHealthController != null)
            {
                if (Godmode.Value)
                {
                    if (Instance.LocalPlayer.ActiveHealthController.DamageCoeff != -1f)
                    {
                        Instance.LocalPlayer.ActiveHealthController.SetDamageCoeff(-1f);
                        Instance.LocalPlayer.ActiveHealthController.RemoveNegativeEffects(EBodyPart.Common);
                        Instance.LocalPlayer.ActiveHealthController.RestoreFullHealth();
                    }
                }
                if (!Godmode.Value)
                {
                    if (Instance.LocalPlayer.ActiveHealthController.DamageCoeff != 1f)
                    {
                        Instance.LocalPlayer.ActiveHealthController.SetDamageCoeff(1f);
                    }
                }

                if (Demigod.Value)
                {
                    var HeadHP = Instance.LocalPlayer.ActiveHealthController.GetBodyPartHealth(EBodyPart.Head, true);
                    var ChestHP = Instance.LocalPlayer.ActiveHealthController.GetBodyPartHealth(EBodyPart.Chest, true);

                    // if ChestHP.Current is less than 45

                    if (ChestHP.Current < ChestHP.Maximum / 2)
                    {
                        Instance.LocalPlayer.ActiveHealthController.RemoveNegativeEffects(EBodyPart.Chest);
                        Instance.LocalPlayer.ActiveHealthController.ChangeHealth(EBodyPart.Chest, ChestHP.Maximum, default);
                    }
                    if (HeadHP.Current < HeadHP.Maximum / 2)
                    {
                        Instance.LocalPlayer.ActiveHealthController.RemoveNegativeEffects(EBodyPart.Head);
                        Instance.LocalPlayer.ActiveHealthController.ChangeHealth(EBodyPart.Head, HeadHP.Maximum, default);
                    }

                    if (Instance.HasDemiGodRan == false)
                    {
                        foreach (Transform transform in Health.EnumerateHierarchyCore(Entry.Instance.LocalPlayer.gameObject.transform).Where(t => TargetBones.Any(u => t.name.ToLower().Contains(u))))
                        {
                            if (transform.gameObject.layer != LayerMask.NameToLayer("PlayerSpiritAura"))
                            {
                                transform.gameObject.layer = LayerMask.NameToLayer("PlayerSpiritAura");
                                Instance.HasDemiGodRan = true;
                            }
                        }
                    }
                }

                if (HealButton.Value.IsDown())
                {
                    Heal = true;
                    if (Heal == true)
                    {
                        Instance.LocalPlayer.ActiveHealthController.RemoveNegativeEffects(EBodyPart.Common);
                        Instance.LocalPlayer.ActiveHealthController.RestoreFullHealth();
                        Heal = false;
                    }
                }

                if (HungerEnergyDrain.Value)
                {
                    Instance.LocalPlayer.ActiveHealthController.ChangeEnergy(100f);
                    Instance.LocalPlayer.ActiveHealthController.ChangeHydration(100f);
                }

            }
        }

        private static IEnumerable<Transform> EnumerateHierarchyCore(Transform root)
        {
            Queue<Transform> transformQueue = new Queue<Transform>();
            transformQueue.Enqueue(root);

            while (transformQueue.Count > 0)
            {
                Transform parentTransform = transformQueue.Dequeue();

                if (!parentTransform)
                {
                    continue;
                }

                for (Int32 i = 0; i < parentTransform.childCount; i++)
                {
                    transformQueue.Enqueue(parentTransform.GetChild(i));
                }

                yield return parentTransform;
            }
        }
    }
}
