using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

// ReSharper disable InconsistentNaming

namespace thmsn.InventoryTweaks.Patches
{
    /// <summary>
    ///     Fixes <see cref="PlayerInventory.ShiftMoveItem">PlayerInventory.ShiftMoveItem</see> behavior
    ///     when moving from/to hotbar
    /// </summary>
    [HarmonyPatch(typeof(PlayerInventory), nameof(PlayerInventory.ShiftMoveItem))]
    public static class PlayerInventory_ShiftMoveItem_Patch
    {
        private static readonly FieldInfo Inventory_secondInventory_FieldInfo =
            AccessTools.Field(typeof(Inventory), nameof(Inventory.secondInventory));

        private static readonly MethodInfo Slot_IsEmpty_Getter_MethodInfo =
            AccessTools.PropertyGetter(typeof(Slot), nameof(Slot.IsEmpty));

        private static readonly MethodInfo Replacement_MethodInfo =
            AccessTools.Method(typeof(PlayerInventory_ShiftMoveItem_Patch), "Replacement");

        internal static void Replacement(PlayerInventory inventory, Slot slot)
        {
            var allSlots = inventory.allSlots;
            int hotslotCount = inventory.hotslotCount;

            int index, end;
            if (!inventory.hotbar.ContainsSlot(slot))
            {
                index = 0;
                end = hotslotCount;
            }
            else
            {
                index = hotslotCount;
                end = allSlots.Count;
            }

            var item = slot.itemInstance;
            int uniqueIndex = item.UniqueIndex;

            Slot firstEmptySlot = null;
            for (; index < end; index++)
            {
                var iterSlot = allSlots[index];

                if (firstEmptySlot == null && iterSlot.IsEmpty)
                {
                    firstEmptySlot = iterSlot;
                    continue;
                }

                if (iterSlot.itemInstance?.UniqueIndex == uniqueIndex && !iterSlot.StackIsFull())
                    PatchHelpers.PlayerInventory_StackSlots(inventory, new object[]
                    {
                        slot, iterSlot, item.Amount
                    });

                item = slot.itemInstance;
                if (item == null)
                    break;
            }

            if (item != null && firstEmptySlot != null)
                PatchHelpers.PlayerInventory_MoveSlotToEmpty(inventory, new object[]
                {
                    slot, firstEmptySlot, item.Amount
                });
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            var list = instructions.ToList();
            int count = list.Count;

            var newLabel = generator.DefineLabel();
            var index = 0;
            for (; index < count; index++)
            {
                // if (this.secondInventory == null && !slot.IsEmpty)
                if (list[index + 0].Is(OpCodes.Ldfld, Inventory_secondInventory_FieldInfo) &&
                    list[index + 1].opcode == OpCodes.Ldnull &&
                    list[index + 2].opcode == OpCodes.Call && // "op_Equality"
                    list[index + 3].opcode == OpCodes.Brfalse &&
                    list[index + 4].opcode == OpCodes.Ldarg_1 &&
                    list[index + 5].Is(OpCodes.Callvirt, Slot_IsEmpty_Getter_MethodInfo) &&
                    list[index + 6].opcode == OpCodes.Brtrue)
                {
                    // goto REPLACEMENT;
                    list.Insert(index + 7, new CodeInstruction(OpCodes.Br, newLabel));
                    count += 1;
                    index += 8;
                    break;
                }
            }

            if (index != count)
            {
                var conditionalLabel = (Label)list[index - 2].operand;
                for (; index < count; index++)
                {
                    if (list[index].labels.Contains(conditionalLabel))
                        break;
                }

                // REPLACEMENT: /(label)/
                // Replacement(this, slot);
                // return;
                var instruction = new CodeInstruction(OpCodes.Ldarg_0);
                instruction.labels.Add(newLabel);
                list.InsertRange(index, new[]
                {
                    new CodeInstruction(OpCodes.Br, newLabel),
                    instruction,
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Call, Replacement_MethodInfo),
                    new CodeInstruction(OpCodes.Ret)
                });
            }

            return list;
        }
    }
}