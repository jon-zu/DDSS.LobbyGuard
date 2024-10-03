﻿using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_CatController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CatController), nameof(CatController.InvokeUserCode_CmdFeed__NetworkIdentity))]
        private static bool InvokeUserCode_CmdFeed__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get CatController
            CatController cat = __0.TryCast<CatController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, cat.transform.position))
                return false;

            // Run Game Command
            cat.UserCode_CmdFeed__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CatController), nameof(CatController.InvokeUserCode_CmdPet__NetworkIdentity))]
        private static bool InvokeUserCode_CmdPet__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get CatController
            CatController cat = __0.TryCast<CatController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, cat.transform.position))
                return false;

            // Run Game Command
            cat.UserCode_CmdPet__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }
    }
}
