﻿using Il2Cpp;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppPlayer.TaskManagement.Tasks;
using Il2CppPlayer.Tasks;
using Il2CppProps.Scripts;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Security
{
    internal static class InteractionSecurity
    {
        private static Dictionary<NetworkIdentity, Dictionary<string, int>> _itemSpawnCount = new();

        private const int MAX_ITEMS_HELD = 2;

        internal const float MAX_DISTANCE = 2f;
        internal const float MAX_SPANK_DISTANCE = 1f;

        internal static int MAX_CIGS { get; private set; }
        internal static int MAX_CIG_PACKS { get; private set; }
        internal static int MAX_INFECTED_USBS { get; private set; }
        internal static int MAX_DOCUMENTS_TRAY { get; private set; }

        internal static void OnSceneLoad()
        {
            // Validate Game Rules Manager
            if (GameRulesSettingsManager.instance == null)
                return;

            // Get Max Players
            int maxPlayers = Mathf.RoundToInt(GameRulesSettingsManager.instance.GetSetting("Max players")) + 1;

            // Adjust Limits
            MAX_CIG_PACKS = maxPlayers;
            MAX_INFECTED_USBS = maxPlayers;
            MAX_CIGS = maxPlayers * 2;
            MAX_DOCUMENTS_TRAY = maxPlayers * 2;
        }

        internal static bool IsWithinRange(Vector3 posA, Vector3 posB,
            float maxRange = MAX_DISTANCE)
        {
            float distance = Vector3.Distance(posA, posB);
            if (distance < 0f)
                distance *= -1f;
            return distance <= maxRange;
        }

        private static int GetTotalCountOfSpawnedItem(string interactableName)
        {
            if (GameManager.instance == null)
                return 0;

            return GameManager.instance.CountSpawnedItemsOfType(interactableName);
        }
        internal static bool CanSpawnItem(string interactableName, int maxCount)
            => GetTotalCountOfSpawnedItem(interactableName) < maxCount;

        internal static bool CanGrabCollectible(NetworkIdentity player, 
            Collectible collectible)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller == null)
                return false;

            int count = controller.currentUsables.Count;
            return ((count < MAX_ITEMS_HELD) 
                && (count < collectible.maxStack));
        }

        internal static bool IsHoldingCollectible(NetworkIdentity player)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller == null)
                return false;

            return controller.currentUsables.Count > 0;
        }

        internal static bool CanPickUpInfectedUsb(WorkStationController station, LobbyPlayer player)
        {
            LobbyPlayer stationOwner = station.NetworkownerLobbyPlayer.GetComponent<LobbyPlayer>();
            if (stationOwner == null)
                return false;
            return CanInfectStation(station, player)
                && (stationOwner == player);
        }

        internal static bool CanInfectStation(WorkStationController station, LobbyPlayer player)
        {
            TaskController component = player.GetComponent<TaskController>();
            if (component == null)
                return false;
            return component.GetActiveTask() is InfectComputerTask;
        }
    }
}
