
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



//esto no hace nada aun.
//van a ser los botones que spawnean agentes.
// de este deben heredar dos variantes de botones.
//el boton que spawnea una unidad nueva y un agente.
// y el boton que spawnea un agnete par reforzar una unidad.
[RequireComponent(typeof(Image), (typeof(Button)))]
public abstract class SpawnAgentButton : MonoBehaviour
{
    private AIUnitData unitToSpawnData;
    private Team spawnerTeam;

    private Button button;
    private Image image;
    private RectTransform rectTransform;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    public abstract void SetUpButton(AIUnitData data, Team team);
   


}
