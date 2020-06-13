using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FootMenuController : MonoBehaviour
{
    public UnityEvent MarkerScanClicked;
    public UnityEvent HomeClicked;
    public UnityEvent OffsetClicked;

    [SerializeField]
    private Interactable MarkerScanInteractable;
    [SerializeField]
    private Interactable HomeInteractable;
    [SerializeField]
    private Interactable OffsetInteractable;
    [SerializeField]
    private GameObject BackplateQuad;

    public float CellWidth = 0.042f;
    public float Maring = 0.01f;

    public void Start()
    {
        if(MarkerScanInteractable == null || HomeInteractable == null || OffsetInteractable == null || BackplateQuad == null)
        {
            throw new NullReferenceException("Missing Reference in FootMenuController.");
        }

        MarkerScanInteractable.OnClick.AddListener(() => { MarkerScanClicked?.Invoke(); });
        HomeInteractable.OnClick.AddListener(() => { HomeClicked?.Invoke(); });
        OffsetInteractable.OnClick.AddListener(() => { OffsetClicked?.Invoke(); });
    }

    public void ChangeMode(MenuMode mode)
    {
        OffsetInteractable.gameObject.SetActive(mode == MenuMode.Record ? true : false);
        var gridObjectCollection = GetComponentInChildren<GridObjectCollection>();
        gridObjectCollection.UpdateCollection();

        int childCount = 0;
        foreach (Transform child in gridObjectCollection.transform)
        {
            if (child.gameObject.activeSelf)
                childCount++;
        }
        BackplateQuad.transform.localScale = new Vector3(childCount * CellWidth + Maring, BackplateQuad.transform.localScale.y, BackplateQuad.transform.localScale.z);
    }
}
