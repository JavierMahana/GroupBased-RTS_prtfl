using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class ButtonDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameDisplay = null;
    [SerializeField]
    private Image iconDisplay = null;


    [SerializeField]
    private TextMeshProUGUI healthDisplay = null;
    [SerializeField]
    private TextMeshProUGUI capacityDisplay = null; 
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //acá debo hacer un sistema de eventos. Pero aun no se muy bien como hacerlo.----ver vien que cosa conectar a que
    public void Prime(BaseButtonData buttonData)
    {
        if (nameDisplay != null)
        {
            
        }
        if (iconDisplay != null)
        { }
        if (healthDisplay != null)
        { }
        if (capacityDisplay != null)
        { }
        //si reciven un valor null. se esconden?

        //acá revisa los componentes y/o valores,  para empezar a mostrar cosas
    }
}
