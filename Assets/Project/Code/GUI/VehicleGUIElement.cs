using UnityEngine;

public class VehicleGUIElement
{
    [SerializeField] private string GUIPrefix = "Element Name";
    [SerializeField] private string GUISuffix = "Value Unit";
    [SerializeField] private bool showOnGUI = false;
    [SerializeField] private string value = "val";

    public VehicleGUIElement(string guiPrefix, string guiSuffix, string defualtVal, bool show)
    {
        this.GUIPrefix = guiPrefix;
        this.GUISuffix = guiSuffix;
        this.showOnGUI = show;
        this.value = defualtVal;

    }
    public void SetGUIPrefix(string val)
    {
        GUIPrefix = val;
    }
    public string GetGUIPrefix()
    {
        return GUIPrefix;
    }
    public void SetGUISuffix(string val)
    {
        GUISuffix = val;
    }
    public string GetGUISuffix()
    {
        return GUISuffix;
    }
    public void SetShowOnGUI(bool val)
    {
        showOnGUI = val;
    }
    public bool GetShowOnGUI()
    {
        return showOnGUI;
    }
    public void SetValue(string val)
    {
        value = val;
    }
    public string GetValue()
    {
        return value;
    }
}
