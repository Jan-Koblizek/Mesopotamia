using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager
{
    private Selectable _selection = null;
    public Selectable selection { get { return _selection; } }

    private static SelectionManager instance;

    public static SelectionManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SelectionManager();
            }
            return instance;
        }
    }

    public void Select(Selectable newSelected)
    {
        if (_selection != null) selection.Deselect();
        _selection = newSelected;
        newSelected.Select();
    }
}
