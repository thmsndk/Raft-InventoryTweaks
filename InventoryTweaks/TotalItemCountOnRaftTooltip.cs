using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace thmsn.InventoryTweaks
{

    public class TotalItemCountOnRaftTooltip : MonoBehaviour
    {
        public GameObject container;
        public Text textComponent;

        protected virtual void Update()
        {
            // Move tooltip according to mouse
            this.textComponent.transform.position = Input.mousePosition;
            this.textComponent.rectTransform.localPosition += new Vector3(0.15f * Screen.height, -25f); // move it below durability tooltip from statistics mod
            this.textComponent.rectTransform.sizeDelta = new Vector2(0.2f * Screen.height, 50);
        }
    }

    /// <summary>
    /// Registers the tooltip on all inventories
    /// </summary>
    [HarmonyPatch(typeof(Inventory), "Start")]
    public class Patch_InventoryDisplayCreate
    {
        public static void Postfix(Inventory __instance)
        {
            var canvas = ComponentManager<CanvasHelper>.Value;

            var newObj = __instance.gameObject.AddComponent<TotalItemCountOnRaftTooltip>();
            // TODO: perhaps it would be better to make a single tooltip and attach it to the mouse
            newObj.container = UIHelper.CreateText(__instance.gameObject.transform,
                                                   0,
                                                   0,
                                                   "hi",
                                                   canvas.dropText.fontSize,
                                                   canvas.dropText.color,
                                                   0.2f * Screen.height,
                                                   50,
                                                   canvas.dropText.font,
                                                   "TotalItemCountOnRaftTooltipText");
            newObj.textComponent = newObj.container.GetComponent<Text>();
            newObj.textComponent.alignment = TextAnchor.MiddleLeft;
            UIHelper.CopyTextShadow(newObj.container, canvas.dropText.gameObject);
            newObj.textComponent.gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(Inventory), "HoverEnter")]
    public class Patch_InventoryStartHover
    {
        static void Postfix(ref Inventory __instance, ref Slot slot)
        {
            if (!slot.IsEmpty)
            {
                var obj = __instance.gameObject.GetComponent<TotalItemCountOnRaftTooltip>();
                if (obj != null)
                {
                    obj.textComponent.gameObject.SetActive(true);

                    var player = RAPI.GetLocalPlayer();
                    var totalItems = player.Inventory.GetItemCountWithoutDuplicates(slot.itemInstance.UniqueName); // using this method prevents CFAS from overriding the count
                    if (player.Inventory.secondInventory)
                    {
                        totalItems += player.Inventory.secondInventory.GetItemCountWithoutDuplicates(slot.itemInstance.UniqueName);
                    }

                    foreach (Storage_Small storage in StorageManager.allStorages)
                    {
                        Inventory container = storage.GetInventoryReference();
                        if (storage.IsOpen || container == null /*|| !Helper.LocalPlayerIsWithinDistance(storage.transform.position, player.StorageManager.maxDistanceToStorage)*/)
                            continue;

                        totalItems += container.GetItemCountWithoutDuplicates(slot.itemInstance.UniqueName);
                    }

                    obj.textComponent.text = "x" + totalItems.ToString();
                }
            }
        }
    }

    [HarmonyPatch(typeof(Inventory), "HoverExit")]
    public class Patch_InventoryEndHover
    {
        static void Postfix(ref Inventory __instance)
        {
            var obj = __instance.gameObject.GetComponent<TotalItemCountOnRaftTooltip>();
            //if (obj != null && InventoryTweaks.showTotalItemCountTooltip)
            //{
                obj.textComponent.gameObject.SetActive(false);
            //}
        }
    }
}
