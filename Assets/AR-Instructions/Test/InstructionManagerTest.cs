using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class InstructionManagerTest : MonoBehaviour
{
    public GameObject PhotoVideoPanelPrefab;

    public ObservableCollection<Step> steps;
    public void Start()
    {
        Instruction instruction = new Instruction();

        instruction.PropertyChanged += Instruction_PropertyChanged;

        var step = instruction.AddStep();

        step.AddMediaFile(new MediaFile());
        step.AddMediaFile(new MediaFile());
        step.AddMediaFile(new MediaFile());

        var item = new Item();

        step.AddItem(item);
        item.Transform = new SerializableTransform(this.transform);

        instruction.AddStep();
        instruction.AddStep();
        instruction.AddStep();

    }

    private void Instruction_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        Debug.Log("Property changed: " + e.PropertyName);
    }
}
