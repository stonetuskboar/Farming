using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolController : MonoBehaviour
{
    public GameManager gameManager;
    public UsableItem NowTool;
    public ToolButton NowSelectButton;
    public List<UsableItem> ToolList = new List<UsableItem>();
    public List<ToolButton> ButtonList = new();


    public void Start()
    {
        ToolList.Add(new Hoe());
        ToolList.Add(new WateringCan());
        ToolList.Add(new BasicSeed());
        ToolList.Add(new BasicTreeSeed());
        ToolList.Add(new Sickle());
        ToolList.Add(new Axe());
        for (int i = 0; i < ToolList.Count; i++)
        {
            ToolList[i].Init(gameManager);
        }
        InitButton();
        SelectNowTool(ButtonList[0]);

    }

    public void InitButton()
    {
        for(int i = 0; i < ButtonList.Count; i++)
        {
            ButtonList[i].Init(this, i);
        }
        for(int i = 0; i < ToolList.Count; i++)
        {
            ButtonList[i].gameObject.SetActive(true);
            Sprite sprite = ToolList[i].itemData.Icon;
            ButtonList[i].ToolIcon.sprite = sprite;
        }
        for (int i = ToolList.Count; i < ButtonList.Count; i++)
        {
            ButtonList[i].gameObject.SetActive(false);
        }
    }

    public void SelectNowTool(ToolButton button)
    {
        if(NowSelectButton != null)
        {
            NowSelectButton.ChangeStateToIdle();
        }
        NowSelectButton = button;
        NowSelectButton.ChangeStateToSelected();
        NowTool = ToolList[button.index];
        gameManager.GridCursorController.ChangeCursorSprite(NowTool.itemData.Icon);
    }
}
