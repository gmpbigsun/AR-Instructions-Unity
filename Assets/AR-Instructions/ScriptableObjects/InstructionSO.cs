using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class InstructionSO : MonoBehaviour
{
    public String Name;
    public DateTime DateCreated;
    public SerializableTransform OffsetForHolograms;
    private ObservableCollection<Step> Steps;

    public int StepsCount
    {
        get
        {
            return Steps.Count;
        }
    }

    public InstructionSO(string name, DateTime dateCreated)
    {
        Name = name;
        DateCreated = dateCreated;
        Steps = new ObservableCollection<Step>();
        OffsetForHolograms = new SerializableTransform();
    }

    public InstructionSO()
    {
        Name = "default";
        DateCreated = DateTime.Now;
        Steps = new ObservableCollection<Step>();
        OffsetForHolograms = new SerializableTransform();
    }

    public void AddStep()
    {
        Steps.Add(new Step());
    }

    public Step GetCurrentStep()
    {
        throw new NotImplementedException();
    }
}

