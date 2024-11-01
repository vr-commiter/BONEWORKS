using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using StressLevelZero.Props.Weapons;
using StressLevelZero.Interaction;
using StressLevelZero;
using UnityEngine;
using StressLevelZero.Combat;
using MyTrueGear;
using Oculus.Platform.Samples.VrBoardGame;

namespace AfterTheFall_TrueGear
{
    [BepInPlugin("TrueGear.bepinex.plugins.bonework", "TrueGear Mod For BoneWork", "1.0.0")]
    public class Plugin : BepInEx.IL2CPP.BasePlugin
    {
        internal static ManualLogSource Log;

        private static TrueGearMod _TrueGear = null;

        private static string leftHandItem = "";
        private static string rightHandItem = "";
        private static bool isHeartBeat = false;
        private static bool isDeath = false;


        public override void Load()
        {
            // Plugin startup logic
            Plugin.Log = base.Log;

            Plugin.Log.LogInfo("BoneWork_TrueGear Plugin is loaded!");
            //new Harmony("truegear.patch.bonework").PatchAll();
            HarmonyLib.Harmony.CreateAndPatchAll(typeof(Plugin));
            _TrueGear = new TrueGearMod();

            _TrueGear.Play("HeartBeat");
        }


        [HarmonyPostfix, HarmonyPatch(typeof(Player_Health), "TAKEDAMAGE")]
        private static void Player_Health_TAKEDAMAGE_Postfix(Player_Health __instance, float damage)
        {
            Plugin.Log.LogInfo("----------------------------------------");
            switch (__instance.mod_Type)
            {
                case AttackType.Explosive:
                    Plugin.Log.LogInfo("Explosive");
                    _TrueGear.Play("Explosive");
                    return;
                case AttackType.Fire:
                    Plugin.Log.LogInfo("FireDamage");
                    _TrueGear.Play("FireDamage");
                    return;
            }
            if (damage < 5f)
            {
                Plugin.Log.LogInfo("PlayerTakeDamage");
                _TrueGear.Play("PlayerTakeDamage");
            }
            else
            {
                Plugin.Log.LogInfo("PlayerTakeDamage2");
                _TrueGear.Play("PlayerTakeDamage2");
            }
            Plugin.Log.LogInfo(damage);
        }


        [HarmonyPrefix, HarmonyPatch(typeof(SaveSpot), "Save")]
        private static void SaveSpot_Save_Postfix(SaveSpot __instance)
        {
            Plugin.Log.LogInfo("----------------------------------------");
            Plugin.Log.LogInfo("SaveGame");
            _TrueGear.Play("SaveGame");
        }



        //[HarmonyPostfix, HarmonyPatch(typeof(PlayerDamageReceiver), "ReceiveAttack")]
        //private static void PlayerDamageReceiver_ReceiveAttack_Postfix(PlayerDamageReceiver __instance, Attack attack)
        //{
        //    try
        //    {
        //        GameObject gameObject = GameObject.Find("[RigManager (Default Brett)]");
        //        if (gameObject != null && attack != null)
        //        {
        //            Player_Health component = gameObject.GetComponent<Player_Health>();
        //            if (component != null)
        //            {
        //                float multiple = 0.3f;
        //                float damage = Math.Abs(attack.damage * multiple);
        //                if (damage > 15f)
        //                {
        //                    System.Random random = new System.Random();
        //                    damage = GetRandomDamage(random);
        //                }
        //                else if (damage < 11f)
        //                {
        //                    damage = 1f;
        //                }

        //                component.TAKEDAMAGE(damage, false);
        //            }
        //            else
        //            {
        //                Plugin.Log.LogInfo("Player health not found.");
        //            }
        //        }
        //        Plugin.Log.LogInfo("----------------------------------");
        //        switch (attack.attackType)
        //        {
        //            case AttackType.Explosive:
        //                Plugin.Log.LogInfo("Explosive");
        //                _TrueGear.Play("Explosive");
        //                return;
        //            case AttackType.Fire:
        //                Plugin.Log.LogInfo("FireDamage");
        //                _TrueGear.Play("FireDamage");
        //                return;
        //        }
        //        Plugin.Log.LogInfo(attack.damage);
        //        Plugin.Log.LogInfo(attack.force.magnitude);
        //        if (attack.collider.gameObject == null)
        //        {
        //            Plugin.Log.LogInfo("PoisonDamage");
        //            _TrueGear.Play("PoisonDamage");
        //            return;
        //        }

        //        var angle = GetAngle(__instance.gameObject.transform, attack.collider.gameObject.transform.position);


        //        Plugin.Log.LogInfo($"DefaultDamage,{angle.Key},{angle.Value}");
        //        _TrueGear.PlayAngle("DefaultDamage", angle.Key, 0);

        //        Plugin.Log.LogInfo($"PlayerRotation :| x:{gameObject.transform.parent.rotation.eulerAngles.x},y:{gameObject.transform.parent.rotation.eulerAngles.y},z:{gameObject.transform.parent.rotation.eulerAngles.z}");
        //        Plugin.Log.LogInfo($"PlayerPosition :| x:{__instance.gameObject.transform.position.x},y:{__instance.gameObject.transform.position.y},z:{__instance.gameObject.transform.position.z}");
        //        Plugin.Log.LogInfo($"Playerforward :| x:{__instance.gameObject.transform.forward.x},y:{__instance.gameObject.transform.forward.y},z:{__instance.gameObject.transform.forward.z}");
        //        Plugin.Log.LogInfo($"AttackPosition :| x:{attack.collider.gameObject.transform.position.x},y:{attack.collider.gameObject.transform.position.y},z:{attack.collider.gameObject.transform.position.z}");

        //    }
        //    catch
        //    {
        //    }
        //}

        static float GetRandomDamage(System.Random random)
        {
            // 生成一个0到100之间的随机数
            int randomNumber = random.Next(100);

            // 根据概率返回对应的值
            if (randomNumber < 85)
            {
                return 1.5f;
            }
            else
            {
                return 6.5f;
            }
        }



        /////////////////////   Gun     /////////////////////////////////////////////

        [HarmonyPostfix, HarmonyPatch(typeof(Gun), "OnFire")]
        private static void Gun_OnFire_Postfix(Gun __instance)
        {
            if (__instance == null)
            {
                return;
            }
            if (__instance.triggerGrip == null)
            {
                return;
            }

            bool isTwoHand = false;
            if (leftHandItem != "" && (leftHandItem == rightHandItem || (leftHandItem.ToLower().Contains("grip") && !rightHandItem.ToLower().Contains("grip")) || (!leftHandItem.ToLower().Contains("grip") && rightHandItem.ToLower().Contains("grip"))))
            {
                isTwoHand = true;
            }
            if (isTwoHand)
            {
                if (__instance.isAutomatic)
                {
                    Plugin.Log.LogInfo("----------------------------------------");
                    Plugin.Log.LogInfo("LeftHandRifleShoot");
                    Plugin.Log.LogInfo("RightHandRifleShoot");
                    _TrueGear.Play("LeftHandRifleShoot");
                    _TrueGear.Play("RightHandRifleShoot");
                }
                else
                {
                    Plugin.Log.LogInfo("----------------------------------------");
                    Plugin.Log.LogInfo("LeftHandPistolShoot");
                    Plugin.Log.LogInfo("RightHandPistolShoot");
                    _TrueGear.Play("LeftHandPistolShoot");
                    _TrueGear.Play("RightHandPistolShoot");
                }

                return;
            }
            Plugin.Log.LogInfo("----------------------------------------");
            foreach (var hand in __instance.triggerGrip.attachedHands)
            {
                Plugin.Log.LogInfo(hand.handedness);
                if (hand.handedness == Handedness.LEFT)
                {
                    if (__instance.isAutomatic)
                    {
                        Plugin.Log.LogInfo("LeftHandRifleShoot");
                        _TrueGear.Play("LeftHandRifleShoot");
                    }
                    else
                    {
                        Plugin.Log.LogInfo("LeftHandPistolShoot");
                        _TrueGear.Play("LeftHandPistolShoot");
                    }
                }
                else
                {
                    if (__instance.isAutomatic)
                    {
                        Plugin.Log.LogInfo("RightHandRifleShoot");
                        _TrueGear.Play("RightHandRifleShoot");
                    }
                    else
                    {
                        Plugin.Log.LogInfo("RightHandPistolShoot");
                        _TrueGear.Play("RightHandPistolShoot");
                    }
                }
            }
            Plugin.Log.LogInfo(__instance.gameObject.name);
            Plugin.Log.LogInfo(__instance.isAutomatic);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Gun), "OnMagazineInserted")]
        private static void Gun_OnMagazineInserted_Postfix(Gun __instance)
        {
            foreach (var hand in __instance.triggerGrip.attachedHands)
            {
                Plugin.Log.LogInfo(hand.handedness);
                if (hand.handedness == Handedness.LEFT)
                {
                    Plugin.Log.LogInfo("LeftReloadAmmo");
                    _TrueGear.Play("LeftReloadAmmo");
                }
                else
                {
                    Plugin.Log.LogInfo("RightReloadAmmo");
                    _TrueGear.Play("RightReloadAmmo");
                }
            }

        }

        [HarmonyPostfix, HarmonyPatch(typeof(Gun), "OnMagazineRemoved")]
        private static void Gun_OnMagazineRemoved_Postfix(Gun __instance)
        {
            foreach (var hand in __instance.triggerGrip.attachedHands)
            {
                Plugin.Log.LogInfo(hand.handedness);
                if (hand.handedness == Handedness.LEFT)
                {
                    Plugin.Log.LogInfo("LeftMagazineEjected");
                    _TrueGear.Play("LeftMagazineEjected");
                }
                else
                {
                    Plugin.Log.LogInfo("RightMagazineEjected");
                    _TrueGear.Play("RightMagazineEjected");
                }
            }
        }

        //[HarmonyPostfix, HarmonyPatch(typeof(Gun), "SlideRelease")]
        //private static void Gun_SlideRelease_Postfix(Gun __instance)
        //{
        //    foreach (var hand in __instance.triggerGrip.attachedHands)
        //    {
        //        Plugin.Log.LogInfo(hand.handedness);
        //        if (hand.handedness == Handedness.LEFT)
        //        {
        //            Plugin.Log.LogInfo("LeftDownReload");
        //            _TrueGear.Play("LeftDownReload");
        //        }
        //        else
        //        {
        //            Plugin.Log.LogInfo("RightDownReload");
        //            _TrueGear.Play("RightDownReload");
        //        }
        //    }
        //}

        [HarmonyPostfix, HarmonyPatch(typeof(Player_Health), "Death")]
        private static void Player_Health_Death_Postfix(Player_Health __instance)
        {
            Plugin.Log.LogInfo("----------------------------------");
            Plugin.Log.LogInfo("PlayerDeath1");
            _TrueGear.Play("PlayerDeath");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Player_Health), "Update")]
        private static void Player_Health_Update_Postfix(Player_Health __instance)
        {
            if (__instance.curr_Health <= __instance.max_Health * 0.4f && !isHeartBeat)
            {
                isHeartBeat = true;
                Plugin.Log.LogInfo("----------------------------------");
                Plugin.Log.LogInfo("StartHeartBeat");
                _TrueGear.StartHeartBeat();
            }
            else if (__instance.curr_Health > __instance.max_Health * 0.4f && isHeartBeat)
            {
                isHeartBeat = false;
                Plugin.Log.LogInfo("----------------------------------");
                Plugin.Log.LogInfo("StopHeartBeat");
                _TrueGear.StopHeartBeat();
            }
            if (__instance.curr_Health <= 0 && !isDeath)
            {
                isHeartBeat = false;
                isDeath = true;
                Plugin.Log.LogInfo("----------------------------------");
                Plugin.Log.LogInfo("PlayerDeath2");
                _TrueGear.Play("PlayerDeath");
            }
            else if (__instance.curr_Health == __instance.max_Health && isDeath)
            {
                Plugin.Log.LogInfo("----------------------------------");
                Plugin.Log.LogInfo("ReLife");
                isDeath = false;
            }
        }

        /////////////////////   DevManipulatorGun     /////////////////////////////////////////////

        [HarmonyPostfix, HarmonyPatch(typeof(DevManipulatorGun), "Blast")]
        private static void DevManipulatorGun_Blast_Postfix(DevManipulatorGun __instance)
        {
            bool isTwoHand = false;
            if (leftHandItem != "" && (leftHandItem == rightHandItem || (leftHandItem.ToLower().Contains("grip") && !rightHandItem.ToLower().Contains("grip")) || (!leftHandItem.ToLower().Contains("grip") && rightHandItem.ToLower().Contains("grip"))))
            {
                isTwoHand = true;
            }
            if (isTwoHand)
            {
                Plugin.Log.LogInfo("----------------------------------------");
                Plugin.Log.LogInfo("LeftHandShotgunShoot");
                Plugin.Log.LogInfo("RightHandShotgunShoot");
                _TrueGear.Play("LeftHandShotgunShoot");
                _TrueGear.Play("RightHandShotgunShoot");

                return;
            }
            Plugin.Log.LogInfo("----------------------------------------");
            foreach (var hand in __instance.triggerGrip.attachedHands)
            {
                Plugin.Log.LogInfo(hand.handedness);
                if (hand.handedness == Handedness.LEFT)
                {
                    Plugin.Log.LogInfo("LeftHandShotgunShoot");
                    _TrueGear.Play("LeftHandShotgunShoot");
                }
                else
                {
                    Plugin.Log.LogInfo("RightHandShotgunShoot");
                    _TrueGear.Play("RightHandShotgunShoot");
                }
            }
            Plugin.Log.LogInfo(__instance.gameObject.name);
        }

        /////////////////////   Hand     /////////////////////////////////////////////

        [HarmonyPostfix, HarmonyPatch(typeof(StressLevelZero.Interaction.Hand), "AttachObject")]
        private static void Hand_AttachObject_Postfix(StressLevelZero.Interaction.Hand __instance, GameObject objectToAttach)
        {

            if (__instance.handedness == Handedness.LEFT)
            {
                Plugin.Log.LogInfo("----------------------------------------");
                Plugin.Log.LogInfo("LeftHandPickupItem");
                _TrueGear.Play("LeftHandPickupItem");
                leftHandItem = objectToAttach.gameObject.transform.parent.name;
            }
            else
            {
                Plugin.Log.LogInfo("----------------------------------------");
                Plugin.Log.LogInfo("RightHandPickupItem");
                _TrueGear.Play("RightHandPickupItem");
                rightHandItem = objectToAttach.gameObject.transform.parent.name;
            }
            Plugin.Log.LogInfo(__instance.handedness);
            Plugin.Log.LogInfo(objectToAttach.gameObject.transform.parent.name);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(StressLevelZero.Interaction.Hand), "DetachObject")]
        private static void Hand_DetachObject_Postfix(StressLevelZero.Interaction.Hand __instance)
        {
            Plugin.Log.LogInfo("----------------------------------------");
            Plugin.Log.LogInfo("DetachObject");
            if (__instance.handedness == Handedness.LEFT)
            {
                leftHandItem = "";
            }
            else
            {
                rightHandItem = "";
            }
            Plugin.Log.LogInfo(__instance.handedness);
        }


        [HarmonyPostfix, HarmonyPatch(typeof(Haptor), "Haptic_Hit")]
        private static void Haptor_Haptic_Hit_Postfix(Haptor __instance, float amp)
        {
            if (((__instance != null) ? __instance.device_Controller : null) != null)
            {
                Plugin.Log.LogInfo("----------------------------------");
                Plugin.Log.LogInfo(amp);
                Plugin.Log.LogInfo(__instance.device_Controller.handedness);
                if (amp < 0.2f)
                {
                    return;
                }
                if (__instance.device_Controller.handedness == Handedness.LEFT)
                {

                    Plugin.Log.LogInfo("LeftHandMeleeHit");
                    _TrueGear.Play("LeftHandMeleeHit");
                }
                else if (__instance.device_Controller.handedness == Handedness.RIGHT)
                {
                    Plugin.Log.LogInfo("RightHandMeleeHit");
                    _TrueGear.Play("RightHandMeleeHit");
                }

            }

        }


        [HarmonyPostfix, HarmonyPatch(typeof(HandWeaponSlotReciever), "MakeDynamic")]
        private static void HandWeaponSlotReciever_MakeDynamic_Postfix(HandWeaponSlotReciever __instance)
        {
            Plugin.Log.LogInfo("----------------------------------------");
            if (__instance.isInUIMode)
            {
                Plugin.Log.LogInfo("ChestSlotOutputItem");
                _TrueGear.Play("ChestSlotOutputItem");
                return;
            }
            switch (__instance.saveBodySlotType)
            {
                case SaveState.BodySlot.LSHOULDER:
                    Plugin.Log.LogInfo("LeftBackSlotOutputItem");
                    _TrueGear.Play("LeftBackSlotOutputItem");
                    break;
                case SaveState.BodySlot.RSHOULDER:
                    Plugin.Log.LogInfo("RightBackSlotOutputItem");
                    _TrueGear.Play("RightBackSlotOutputItem");
                    break;
                case SaveState.BodySlot.LSIDEARM:
                    Plugin.Log.LogInfo("LeftChestSlotOutputItem");
                    _TrueGear.Play("LeftChestSlotOutputItem");
                    break;
                case SaveState.BodySlot.RSIDEARM:
                    Plugin.Log.LogInfo("RightChestSlotOutputItem");
                    _TrueGear.Play("RightChestSlotOutputItem");
                    break;
                case SaveState.BodySlot.BACK:
                    Plugin.Log.LogInfo("RightHipSlotOutputItem");
                    _TrueGear.Play("RightHipSlotOutputItem");
                    break;
            }
            Plugin.Log.LogInfo(__instance.saveBodySlotType);
            Plugin.Log.LogInfo(__instance.isInUIMode);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(HandWeaponSlotReciever), "MakeStatic")]
        private static void HandWeaponSlotReciever_MakeStatic_Postfix(HandWeaponSlotReciever __instance)
        {
            Plugin.Log.LogInfo("----------------------------------------");
            if (__instance.isInUIMode)
            {
                Plugin.Log.LogInfo("ChestSlotInputItem");
                _TrueGear.Play("ChestSlotInputItem");
                return;
            }
            switch (__instance.saveBodySlotType)
            {
                case SaveState.BodySlot.LSHOULDER:
                    Plugin.Log.LogInfo("LeftBackSlotInputItem");
                    _TrueGear.Play("LeftBackSlotInputItem");
                    break;
                case SaveState.BodySlot.RSHOULDER:
                    Plugin.Log.LogInfo("RightBackSlotInputItem");
                    _TrueGear.Play("RightBackSlotInputItem");
                    break;
                case SaveState.BodySlot.LSIDEARM:
                    Plugin.Log.LogInfo("LeftChestSlotInputItem");
                    _TrueGear.Play("LeftChestSlotInputItem");
                    break;
                case SaveState.BodySlot.RSIDEARM:
                    Plugin.Log.LogInfo("RightChestSlotInputItem");
                    _TrueGear.Play("RightChestSlotInputItem");
                    break;
                case SaveState.BodySlot.BACK:
                    Plugin.Log.LogInfo("RightHipSlotInputItem");
                    _TrueGear.Play("RightHipSlotInputItem");
                    break;
            }
            Plugin.Log.LogInfo(__instance.saveBodySlotType);
            Plugin.Log.LogInfo(__instance.isInUIMode);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(AmmoPouch), "OnSpawnGrab")]
        private static void AmmoPouch_OnSpawnGrab_Postfix(AmmoPouch __instance)
        {
            Plugin.Log.LogInfo("----------------------------------");
            Plugin.Log.LogInfo("LeftHipSlotOutputItem");
            _TrueGear.Play("LeftHipSlotOutputItem");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(MagazineReciever), "OnInteractableHostDrop")]
        private static void MagazineReciever_OnInteractableHostDrop_Postfix(MagazineReciever __instance)
        {
            Plugin.Log.LogInfo("----------------------------------");
            Plugin.Log.LogInfo("LeftHipSlotInputItem");
            _TrueGear.Play("LeftHipSlotInputItem");
        }



    }
}
