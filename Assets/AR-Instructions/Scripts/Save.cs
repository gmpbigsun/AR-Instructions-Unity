using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public class Instruction : INotifyPropertyChanged
{
    private ObservableCollection<Step> Steps;
    public string Name;
    public DateTime DateCreated;
    public SerializableTransform OffsetForHolograms;

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
    public int StepsCount {
        get
        {
            return Steps.Count;
        }
    }

    private void Steps_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        NotifyPropertyChanged(sender.ToString());
    }
    public Instruction(string name, DateTime dateCreated)
    {
        Name = name;
        DateCreated = dateCreated;
        Steps = new ObservableCollection<Step>();
        Steps.CollectionChanged += Steps_CollectionChanged;
        OffsetForHolograms = new SerializableTransform();
    }


    public Instruction()
    {
        Name = "default";
        DateCreated = DateTime.Now;
        Steps = new ObservableCollection<Step>();
        Steps.CollectionChanged += Steps_CollectionChanged;
        OffsetForHolograms = new SerializableTransform();
    }

    public Step AddStep()
    {
        var step = new Step(Steps.Count);
        step.PropertyChanged += Step_PropertyChanged;
        Steps.Add(step);
        return step;
    }

    private void Step_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        NotifyPropertyChanged(sender.ToString() + " " + e.PropertyName );
    }
}

[Serializable]
public class SerializableTransform
{
    public float[] Position = new float[3];
    public float[] Rotation = new float[4];
    public float[] Scale = new float[3];
    public bool WorldSpace = false;

    public SerializableTransform()
    {
        Position = new float[] { 0, 0, 0 };
        Rotation = new float[] { 0, 0, 0, 0 };
        Scale = new float[] { 1, 1, 1 };
        WorldSpace = false;
    }

    public SerializableTransform(Transform transform, bool worldSpace = true)
    {
        Position[0] = worldSpace ? transform.position.x : transform.localPosition.x;
        Position[1] = worldSpace ? transform.position.y : transform.localPosition.y;
        Position[2] = worldSpace ? transform.position.z : transform.localPosition.z;

        Rotation[0] = worldSpace ? transform.rotation.x : transform.localRotation.x;
        Rotation[1] = worldSpace ? transform.rotation.y : transform.localRotation.y;
        Rotation[2] = worldSpace ? transform.rotation.z : transform.localRotation.z;
        Rotation[3] = worldSpace ? transform.rotation.w : transform.localRotation.w;

        Scale[0] = transform.localScale.x;
        Scale[1] = transform.localScale.y;
        Scale[2] = transform.localScale.z;

        WorldSpace = worldSpace;
    }

    public void ToTransform(ref Transform transform)
    {
        if (WorldSpace)
        {
            transform.position = new Vector3(Position[0], Position[1], Position[2]);
            transform.rotation = new Quaternion(Rotation[0], Rotation[1], Rotation[2], Rotation[3]);
            transform.localScale = new Vector3(Scale[0], Scale[1], Scale[2]);
        }
        else
        {
            transform.localPosition = new Vector3(Position[0], Position[1], Position[2]);
            transform.localRotation = new Quaternion(Rotation[0], Rotation[1], Rotation[2], Rotation[3]);
            transform.localScale = new Vector3(Scale[0], Scale[1], Scale[2]);
        }
    }

    internal void AddNewOffset(Vector3 localPosition)
    {
        Position[0] += localPosition.x;
        Position[1] += localPosition.y;
        Position[2] += localPosition.z;
    }
}

[System.Serializable]
public class Step: INotifyPropertyChanged
{
    public int StepNumber;
    public String Text;
    public event PropertyChangedEventHandler PropertyChanged;

    private ObservableCollection<Item> Items;
    private ObservableCollection<MediaFile> MediaFiles;


    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        NotifyPropertyChanged(sender.ToString());
    }
    public Step()
    {
        StepNumber = -1;
        Items = new ObservableCollection<Item>();
        Items.CollectionChanged += CollectionChanged;
        MediaFiles = new ObservableCollection<MediaFile>();
        MediaFiles.CollectionChanged += CollectionChanged;
        Text = string.Empty;
    }

    public Step(int stepNumber, string text)
    {
        StepNumber = stepNumber;
        Text = text;
        Items = new ObservableCollection<Item>();
        Items.CollectionChanged += CollectionChanged;
        MediaFiles = new ObservableCollection<MediaFile>();
        MediaFiles.CollectionChanged += CollectionChanged;
    }
    public Step(int stepNumber, List<GameObject> items = null, List<MediaFile> mediaFiles = null, string text = null)
    {
        StepNumber = stepNumber;

        if (text != null)
        {
            Text = text;
        }
        else
        {
            Text = string.Empty;
        }

        if (items != null)
        {
            foreach (var item in items)
            {
                var tmp = new Item(item);
                tmp.PropertyChanged += Item_PropertyChanged;
                Items.Add(tmp);
            }
        }
        else
        {
            Items = new ObservableCollection<Item>();
            Items.CollectionChanged += CollectionChanged;
        }

        if(mediaFiles != null)
        {
            foreach (var mediaFile in mediaFiles)
            {
                MediaFiles.Add(mediaFile);
            }
        }
        else
        {
            MediaFiles = new ObservableCollection<MediaFile>();
            MediaFiles.CollectionChanged += CollectionChanged;
        }
    }

    public void AddItem(Item item)
    {
        Items.Add(item);
        item.PropertyChanged += Item_PropertyChanged;

    }
    public void AddMediaFile(MediaFile mediaFile)
    {
        MediaFiles.Add(mediaFile);
    }

    private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        NotifyPropertyChanged(sender.ToString() +" "+ e.PropertyName);
    }
}

[System.Serializable]

public class MediaFile
{
    public string FileName;
    public MediaType Type;

    public MediaFile()
    {
        FileName = string.Empty;
        Type = MediaType.Unkown;
    }

    public MediaFile(string fileName, MediaType type)
    {
        FileName = fileName;
        Type = type;
    }

}

public enum MediaType
{
    Image = 0,
    Video,
    Unkown
}

[System.Serializable]
public class Item: INotifyPropertyChanged
{
    private ItemType _type;
    public ItemType Type
    {
        get
        {
            return this._type;
        }
        set
        {
            if (value != this._type)
            {
                this._type = value;
                NotifyPropertyChanged();
            }
        }
    }
    private SerializableTransform _transform;
    public SerializableTransform Transform
    {
        get
        {
            return this._transform;
        }
        set
        {
            if (value != this._transform)
            {
                this._transform = value;
                NotifyPropertyChanged();
            }
        }
    }
    public float[] Color;
    public bool IsActive = false;
    public bool HasText = false;
    public string Text = string.Empty;
    public float[] TextPosition;

    [XmlIgnoreAttribute]
    public GameObject _gameObject;


    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public Item()
    {
        Type = ItemType.Unknown;
        _transform = new SerializableTransform();
        Color = new float[] { 0, 0, 0, 0 };
    }

    public Item(GameObject ItemAsGameObject)
    {
        Type = ItemAsGameObject.GetComponent<ItemTypeScript>().ItemType;
        _gameObject = ItemAsGameObject;
        var visual = ItemAsGameObject.transform.Find("Visual");
        _transform = new SerializableTransform(visual, false);
        var renderer = visual.gameObject.GetComponentInChildren<MeshRenderer>();
        Color = new float[] {   renderer.material.color.r,
                                renderer.material.color.g,
                                renderer.material.color.b,
                                renderer.material.color.a };

        HasText = visual.transform.Find("Simple Line ToolTip").gameObject.activeSelf;

        if(HasText)
        {
            Text = _gameObject.GetComponentInChildren<ToolTip>().ToolTipText;
        }
    }


    public void UpdateTransforms()
    {
        if (_gameObject != null)
        {
            var visual = _gameObject.transform.Find("Visual");
            Transform = new SerializableTransform(visual, false);
            if(HasText)
            {
                var tooltip = visual.transform.Find("Simple Line ToolTip");
                var tmp = tooltip.transform.Find("Pivot").localPosition;
                TextPosition = new float[] { tmp.x, tmp.y, tmp.z };
            }
        }
    }
}

[Serializable]
public enum ItemType
{
    Arrow = 0,
    Box,
    CircleArrow,
    ArrowWithToolTip,
    Unknown
}