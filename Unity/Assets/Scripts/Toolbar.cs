using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum Tool
    {
        Hand,
        Sponge,
        Knife,
    }

    public static Toolbar Instance { get; private set; }

    [SerializeField] private Toggle[] toggles;
    [SerializeField] private Texture2D hand;
    [SerializeField] private Texture2D handHold;
    [SerializeField] private Texture2D sponge;
    [SerializeField] private Texture2D knife;

    public Tool CurrentTool { get; private set; }
    public bool MouseOver { get; private set; }
    
    private Tool? lastTool;

    private void Awake() => Instance = this;

    private void Update()
    {
        CurrentTool = (Tool)toggles.ToList().FindIndex(t => t.isOn);
        if (lastTool == CurrentTool && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonUp(0)) 
            return;
        Cursor.SetCursor(CurrentTool switch
        {
            Tool.Hand => Input.GetMouseButton(0) ? handHold : hand,
            Tool.Sponge => sponge,
            Tool.Knife => knife,
            _ => null
        }, CurrentTool switch
        {
            Tool.Hand or Tool.Sponge => new Vector2(40, 40),
            Tool.Knife => new Vector2(8, 12),
            _ => new Vector2(0, 0)
        }, CursorMode.ForceSoftware);
        lastTool = CurrentTool;
    }
    
    public void SetTool(Tool tool)
    {
        toggles[(int)tool].isOn = true;
    }
    
    public void OnPointerEnter(PointerEventData eventData) => MouseOver = true;
    public void OnPointerExit(PointerEventData eventData) => MouseOver = false;
}