using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemPanelController : MonoBehaviour
{
    public ColorSelected ColorSelected;
    public TooltipSelected ToolTipSelected;
    public ItemSelected ItemSelcted;

    public void Start()
    {
        
    }


    public void onColorSelected(Material color)
    {
        ColorSelected?.Invoke(color);
    }
    public void onToolTipSelected(bool selected)
    {
        ToolTipSelected?.Invoke(selected);
    }

    public void onItemSelected(GameObject item)
    {
        ItemSelcted?.Invoke(item);
    }

    //public GameObject ContainerForSpawnedItems;
    //public GameObject Items;
    //public UnityEvent OnNewData;

    //private InstructionManager _instructionManager;

    //public void Start()
    //{
    //    foreach (Transform child in Items.transform)
    //    {
    //        var spawnNewItem = child.GetComponentInChildren<SpawnNewItem>();
    //        spawnNewItem.Parent = ContainerForSpawnedItems.transform;
    //        spawnNewItem.ObjectSpawned += SpawnNewItem_ObjectSpawned;
    //    }
    //    _instructionManager = InstructionManager.Instance;
    //}

    //private void SpawnNewItem_ObjectSpawned(object sender, ObjectSpawnedEventArgs e)
    //{
    //    _instructionManager.AddItem(new Item(e.SpawnedObject));

    //    var boundingBox = e.SpawnedObject.GetComponentInChildren<BoundingBox>();
    //    boundingBox.RotateStopped.AddListener(OnItemManipulated);
    //    boundingBox.ScaleStopped.AddListener(OnItemManipulated);

    //    var manipulationManagers = e.SpawnedObject.GetComponentsInChildren<ManipulationHandler>();
    //    foreach (var manager in manipulationManagers)
    //    {
    //        manager.OnManipulationEnded.AddListener(OnItemManipulated);
    //    }
    //}

    //private void OnItemManipulated(ManipulationEventData arg0)
    //{
    //    OnItemManipulated();
    //}

    //private void OnItemManipulated()
    //{
    //    OnNewData?.Invoke();
    //    InstructionManager.Instance.Save();
    //}
}



[System.Serializable]
public class TooltipSelected : UnityEvent<bool>
{
}

