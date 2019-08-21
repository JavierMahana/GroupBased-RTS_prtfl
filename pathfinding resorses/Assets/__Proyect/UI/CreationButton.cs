using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(RectTransform), typeof(Image), typeof(Button))]
public class CreationButton : MonoBehaviour
{
    private Image imgComp;
    private Button button;
    private RectTransform rectTransform;
    private AIUnitData currentData;
    private Spawner currSpawner;

    public static event Action<Spawner, AIUnitData> OnCreationButtonClick;
    public void UpdateButton(Spawner spawner, AIUnitData data, Sprite sprite)
    {
        currSpawner = spawner;
        currentData = data;        
        imgComp.sprite = sprite;
        

    }
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Debug.Assert(rectTransform != null, "creation button must be in a UI element");

        imgComp = GetComponent<Image>();
        button = GetComponent<Button>();
        
        button.onClick.AddListener(OnClickCallback); 
    }

    public void OnClickCallback()
    {
        OnCreationButtonClick(currSpawner, currentData);
    }

    
}
