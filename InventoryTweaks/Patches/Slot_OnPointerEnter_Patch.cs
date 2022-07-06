using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

// ReSharper disable InconsistentNaming

namespace thmsn.InventoryTweaks.Patches
{
    /// <summary>
    ///     "Shift-click" or "Drop" hovered <see cref="Slot" />s while holding LMB and corresponding button(s)
    /// </summary>
    [HarmonyPatch(typeof(Slot), "OnPointerEnter")]
    internal class Slot_OnPointerEnter_Patch
    {
        private static void Prefix(Slot __instance, PointerEventData eventData)
        {
            if (__instance.IsEmpty || !eventData.eligibleForClick)
                return;

            var inventory = PatchHelpers.Slot_inventory_Ref(__instance);

            if (MyInput.GetButton("Drop"))
            {
                var playerInventory = inventory as PlayerInventory ?? inventory.secondInventory as PlayerInventory;
                if (playerInventory != null)
                {
                    playerInventory.DropItem(__instance);
                    return;
                }
            }

            if (Input.GetKey(KeyCode.LeftShift))
                inventory.ShiftMoveItem(__instance, eventData);
        }
    }
}