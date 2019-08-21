using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//ahora no hace nada. Pero luego lo que hace es referenciar a un agente y ir cambiado de color en referencia a la vida de ese agente.
[RequireComponent(typeof(RectTransform), typeof(Image))]
public class AgentPanel : MonoBehaviour
{
    private AIAgent agent;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }
    public void UpdatePanel(AIAgent agent, Sprite sprite)
    {
        this.agent = agent;
        image.sprite = sprite;
    }
}
