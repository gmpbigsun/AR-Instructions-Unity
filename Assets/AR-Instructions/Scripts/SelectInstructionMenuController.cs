using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SelectInstructionMenuController : MonoBehaviour
{
    public MenuMode Mode { get; private set; }

    [SerializeField]
    private GameObject ItemParent;
    [SerializeField]
    private GameObject ItemPrefab;
    [SerializeField]
    private int NumberOfItemsToShow = 6;
    [SerializeField]
    private TextMeshPro PageCounterText;
    [SerializeField]
    private GameObject NextPageButton;
    [SerializeField]
    private GameObject PreviousPageButton;
    [SerializeField]
    private Interactable ImportButton;
    [SerializeField]
    private Interactable CreateButton;

    public UnityEvent CreateNewInstructionClicked;
    public UnityEvent ImportInstructionClicked;

    public event EventHandler<InstructionSelectionEventArgs> InstructionSelected;
    public event EventHandler<ModeChangedEventArgs> ModeChanged;

    private int _currentPage = 1;
    private int _maxPageNumber;

    // Start is called before the first frame update
    void Start()
    {
        Mode = MenuMode.Replay;
                
        var items = InstructionManager.Instance.GetInstructionNamesForPage(0, NumberOfItemsToShow);
        
        LoadItemsToMenu(items);

        if (PageCounterText != null)
        {
            _maxPageNumber = (InstructionManager.Instance.Count / NumberOfItemsToShow);
            if(InstructionManager.Instance.Count % NumberOfItemsToShow != 0)
            {
                _maxPageNumber++;
            }

            PageCounterText.text = _currentPage + "/" + _maxPageNumber;
        }

        NextPageButton.SetActive(InstructionManager.Instance.Count > NumberOfItemsToShow);
        PreviousPageButton.SetActive(false);

        CreateButton.OnClick.AddListener(() => { CreateNewInstructionClicked?.Invoke(); });
        ImportButton.OnClick.AddListener(() => { ImportInstructionClicked?.Invoke(); });
    }

    public void SetEditMode(bool mode)
    {
        Mode = mode ? MenuMode.Record : MenuMode.Replay;

        foreach (Transform item in ItemParent.transform)
        {
            item.gameObject.GetComponent<MenuItemController>().SetMode(mode);
        }
        ModeChanged?.Invoke(this, new ModeChangedEventArgs(Mode));
    }

    private void LoadItemsToMenu(IEnumerable<string> items, bool clearBeforeLoad = false)
    {
        if(clearBeforeLoad)
        {
            for (int i = 0; i < ItemParent.transform.childCount; i++)
            {
                ItemParent.transform.GetChild(i).GetComponent<MenuItemController>().InstructionSelected -= SelectInstructionMenuController_InstructionSelected;
                ItemParent.transform.GetChild(i).GetComponent<MenuItemController>().InstructionRemoved -= SelectInstructionMenuController_InstructionRemoved;
                ItemParent.transform.GetChild(i).gameObject.SetActive(false);
                Destroy(ItemParent.transform.GetChild(i).gameObject);
            }
        }

        foreach (var item in items)
        {
            var instruction = SaveLoadManager.Instance.Load(item);
            var itemGameObject = Instantiate(ItemPrefab, ItemParent.transform);
            itemGameObject.gameObject.GetComponent<MenuItemController>().SetMode(Mode == MenuMode.Replay ? false: true );
            itemGameObject.GetComponent<MenuItemController>().SetInstruction(instruction);
            itemGameObject.GetComponent<MenuItemController>().InstructionSelected += SelectInstructionMenuController_InstructionSelected;
            itemGameObject.GetComponent<MenuItemController>().InstructionRemoved += SelectInstructionMenuController_InstructionRemoved;

        }
        ItemParent.GetComponent<GridObjectCollection>()?.UpdateCollection();
    }

    private void SelectInstructionMenuController_InstructionRemoved(object sender, InstructionSelectionEventArgs e)
    {
        RefreshPage();
    }

    public void RefreshPage()
    {
        if (_currentPage == _maxPageNumber && InstructionManager.Instance.Count % NumberOfItemsToShow == 0)
        {
            NextPageButton.SetActive(false);
            _currentPage--;
            if(_currentPage == 1)
            {
                PreviousPageButton.SetActive(false);
            }
        }

        _maxPageNumber = InstructionManager.Instance.Count / NumberOfItemsToShow;
        _maxPageNumber = InstructionManager.Instance.Count % NumberOfItemsToShow == 0 ? _maxPageNumber : _maxPageNumber + 1;

        PageCounterText.text = _currentPage + "/" + _maxPageNumber;

        var items = InstructionManager.Instance.GetInstructionNamesForPage(_currentPage - 1, NumberOfItemsToShow);
        LoadItemsToMenu(items, true);
    }

    private void SelectInstructionMenuController_InstructionSelected(object sender, InstructionSelectionEventArgs e)
    {
        InstructionSelected?.Invoke(sender, e);
    }

    public void OnNextPage()
    {
        _currentPage++;
        //var items = InstructionManager.Instance.GetInstructionNames((_currentPage+1) * NumberOfItemsToShow, NumberOfItemsToShow);
        var items = InstructionManager.Instance.GetInstructionNamesForPage(_currentPage - 1,NumberOfItemsToShow);

        LoadItemsToMenu(items,true);

        PreviousPageButton.SetActive(true);
        if (_currentPage * NumberOfItemsToShow >= InstructionManager.Instance.Count)
        {
            NextPageButton.SetActive(false);
        }
        PageCounterText.text = _currentPage + "/" + _maxPageNumber;
    }

    public void OnPreviousPage()
    {
        _currentPage--;
        var items = InstructionManager.Instance.GetInstructionNamesForPage(_currentPage - 1, NumberOfItemsToShow);
        LoadItemsToMenu(items, true);

        NextPageButton.SetActive(true);

        if (_currentPage > 1)
        {
            PreviousPageButton.SetActive(true);
        }
        else
        {
            PreviousPageButton.SetActive(false);
        }
        PageCounterText.text = _currentPage + "/" + _maxPageNumber;
    }
    
}

public class ModeChangedEventArgs
{
    public MenuMode Mode;

    public ModeChangedEventArgs(MenuMode mode)
    {
        Mode = mode;
    }
}