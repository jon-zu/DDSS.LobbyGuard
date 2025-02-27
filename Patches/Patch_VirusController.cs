﻿using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppProps.Scripts;
using Il2CppProps.WorkStation.InfectedUSB;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_VirusController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(VirusController), nameof(VirusController.PerformPotentialVirusActivity))]
        private static bool PerformPotentialVirusActivity_Prefix(VirusController __instance)
        {
            // Validate Server
            if (!__instance.isServer)
                return true;

            // Validate Workstation
            if ((__instance.computerController == null)
                || (__instance.computerController.user == null))
                return false;

            // Validate Role
            if ((__instance.computerController.user.playerRole == PlayerRole.Slacker)
                || !__instance.isFirewallActive)
            {
                // Get Game Rule
                float probability = GameManager.instance.virusProbability;

                // RandomGen
                if (UnityEngine.Random.Range(0f, 100f) < (probability * 100f))
                    __instance.CmdSetVirus(true);
            }

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(VirusController), nameof(VirusController.InvokeUserCode_CmdSetFireWall__Boolean))]
        private static bool InvokeUserCode_CmdSetFireWall__Boolean_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get VirusController
            VirusController controller = __0.TryCast<VirusController>();
            if ((controller == null)
                || (controller.computerController == null)
                || (controller.computerController._workStationController == null)
                || (controller.computerController._workStationController.usingPlayer == null))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, controller.transform.position))
                return false;

            // Check if Actually Sitting in Seat
            if (controller.computerController._workStationController.usingPlayer != sender)
                return false;

            // Get Value
            bool state = __1.ReadBool();
            if (controller.isFirewallActive == state)
                return false;

            // Run Game Command
            controller.UserCode_CmdSetFireWall__Boolean(state);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(VirusController), nameof(VirusController.InvokeUserCode_CmdSetFireWall__Boolean))]
        private static bool InvokeUserCode_CmdSetVirus__Boolean(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get VirusController
            VirusController controller = __0.TryCast<VirusController>();
            if ((controller == null)
                || (controller.computerController == null)
                || (controller.computerController._workStationController == null))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, controller.transform.position))
                return false;

            // Get Value
            bool state = __1.ReadBool();
            if (controller.isVirusActive == state)
                return false;

            // Check if should Enable
            if (state)
            {
                // Validate Slacker Role
                LobbyPlayer player = sender.GetComponent<LobbyPlayer>();
                if ((player == null)
                    || (player.playerRole != PlayerRole.Slacker))
                    return false;

                // Validate Placement
                Collectible collectible = sender.GetCurrentCollectible();
                if ((collectible == null)
                    || (collectible.GetIl2CppType() != Il2CppType.Of<InfectedUsb>()))
                    return false;
            }
            else
            {
                // Check if Actually Sitting in Seat
                if ((controller.computerController._workStationController.usingPlayer == null)
                    || (controller.computerController._workStationController.usingPlayer != sender))
                    return false;

                // Validate Manager Role
                LobbyPlayer player = sender.GetComponent<LobbyPlayer>();
                if ((player == null)
                    || ((player.playerRole != PlayerRole.Manager)
                        && (player.subRole != SubRole.Assistant)))
                    return false;
            }

            // Run Game Command
            controller.UserCode_CmdSetVirus__Boolean(state);

            // Prevent Original
            return false;
        }
    }
}
