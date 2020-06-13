using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject SelectMenuPrefab;
    [SerializeField]
    private GameObject FootMenuPrefab;
    [SerializeField]
    private GameObject InstructionMenuPrefab;
    [SerializeField]
    private GameObject EnterNamePrefab;
    [SerializeField]
    private GameObject ParentForInstructionHolograms;
    [SerializeField]
    private StabilizedTracking StabilizedTracking;
    [SerializeField]
    private GameObject OffsetHandler;

    private SelectInstructionMenuController _selectMenuController;
    private FootMenuController _footMenuController;
    private GameObject _instructionMenu;
    private GameObject _enterName;
    private MenuMode _mode = MenuMode.Replay;


    public void Start()
    {
        if (SelectMenuPrefab == null ||
            FootMenuPrefab == null ||
            InstructionMenuPrefab == null ||
            EnterNamePrefab == null)
        {
            throw new ArgumentException("Missing Prefab in GUIManager!");
        }

        if (ParentForInstructionHolograms == null)
        {
            ParentForInstructionHolograms = GameObject.Find("Offset");
        }

        InstructionManager.Instance.ImportCompleted += ImportOnCompleted;
        CreateFootMenu();
        CreateSelectMenu();
    }

    private void ImportOnCompleted(object sender, EventArgs e)
    {
        CreateSelectMenu();
    }

    private void CreateFootMenu()
    {
        _footMenuController = Instantiate(FootMenuPrefab).GetComponent<FootMenuController>();
        _footMenuController.HomeClicked.AddListener(FootMenu_OnHomeClick);
        _footMenuController.MarkerScanClicked.AddListener(FootMenu_OnMarkerScanClick);
        _footMenuController.OffsetClicked.AddListener(FootMenu_OnOffsetClick);
    }

    private void FootMenu_OnOffsetClick()
    {
        OffsetHandler.SetActive(true);
    }

    private void CreateSelectMenu()
    {
        _selectMenuController = Instantiate(SelectMenuPrefab).GetComponent<SelectInstructionMenuController>();
        _selectMenuController.CreateNewInstructionClicked.AddListener(SelectInstructionMenu_OnCreateNewInstructionClick);
        _selectMenuController.ImportInstructionClicked.AddListener(SelectInstructionMenu_OnImportInstructionClick);
        _selectMenuController.InstructionSelected += SelectInstructionMenu_OnSelect;
        _selectMenuController.ModeChanged += SelectInstructionMenu_ModeChanged;
    }

    private void SelectInstructionMenu_ModeChanged(object sender, ModeChangedEventArgs e)
    {
        _mode = e.Mode;
        _footMenuController.ChangeMode(_mode);
    }

    private void FootMenu_OnMarkerScanClick()
    {
        StabilizedTracking.Reset();
    }

    private void FootMenu_OnHomeClick()
    {
        ResetAllGuiElements();
    }

    private void ResetAllGuiElements()
    {
        InstructionManager.Instance.Reset();
        Destroy(_enterName);
        if (_instructionMenu != null)
        {
            Destroy(_instructionMenu);
        }
        DestroyAllSpawnedItems();
        DestroySelectMenu();
        CreateSelectMenu();
    }

    private void DestroyAllSpawnedItems()
    {
        while (ParentForInstructionHolograms.transform.childCount > 0)
        {
            DestroyImmediate(ParentForInstructionHolograms.transform.GetChild(0).gameObject);
        }
    }

    public void SelectInstructionMenu_OnCreateNewInstructionClick()
    {
        _enterName = Instantiate(EnterNamePrefab);
        _enterName.transform.position = transform.position;
        _enterName.GetComponent<EnterInstructionNameController>().Continue += EnterName_Continue;

        DestroySelectMenu();
    }

    private void EnterName_Continue(object sender, EventArgs e)
    {
        var tmp = _enterName.GetComponent<EnterInstructionNameController>().keyboardText;
        ShowInstructionMenu(MenuMode.Record);

        _enterName.GetComponent<EnterInstructionNameController>().Continue -= EnterName_Continue;
        Destroy(_enterName);
    }

    public void SelectInstructionMenu_OnImportInstructionClick()
    {
        DestroySelectMenu();
        InstructionManager.Instance.ImportInstruction();
    }
    private void SelectInstructionMenu_OnSelect(object sender, InstructionSelectionEventArgs e)
    {
        ShowInstructionMenu(_selectMenuController.Mode);
        ParentForInstructionHolograms.GetComponent<OffsetController>().SetTransform(InstructionManager.Instance.Instruction.OffsetForHolograms);
        
        DestroySelectMenu();
    }

    private void ShowInstructionMenu(MenuMode mode)
    {
        _instructionMenu = Instantiate(InstructionMenuPrefab);
        _instructionMenu.transform.position = transform.position;

        if (mode == MenuMode.Replay)
        {
            var menuController = _instructionMenu.GetComponent<MenuController>();
            if (menuController != null)
            {
                menuController.Init(MenuMode.Replay, ParentForInstructionHolograms);
            }
            else
            {
                Debug.Log("menuController is null");
            }

            var mainPanelController = _instructionMenu.GetComponentInChildren<MainPanelController>();
            if (mainPanelController != null)
            {
                mainPanelController.HomeButtonClicked.AddListener(ResetAllGuiElements);
            }
            else
            {
                Debug.Log("mainPanelController is null");
            }
        }
        else
        {
            _instructionMenu.GetComponent<MenuController>().Init(MenuMode.Record, ParentForInstructionHolograms);
        }
    }

    private void DestroySelectMenu()
    {
        if (_selectMenuController != null)
        {
            _selectMenuController.CreateNewInstructionClicked.RemoveAllListeners();
            _selectMenuController.ImportInstructionClicked.RemoveAllListeners();
            _selectMenuController.InstructionSelected -= SelectInstructionMenu_OnSelect;

            Destroy(_selectMenuController.gameObject);
            _selectMenuController = null;
        }
    }
}
