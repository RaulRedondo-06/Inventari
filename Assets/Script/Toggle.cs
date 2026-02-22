using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public void TogglePanel()
    {
        if (panel == null) return;

        panel.SetActive(!panel.activeSelf);
    }
}
