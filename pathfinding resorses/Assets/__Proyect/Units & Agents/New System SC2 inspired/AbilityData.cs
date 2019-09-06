using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//la abilidad tiene datos especificos a la habilidad en sí.
//tiene toda la info para el, pero no sabe quien lo llama.
//costo
//efecto
//demora
//requerimientos para ejecutarse
//etc
public class AbilityData : ScriptableObject
{
    //cada habilidad debería tener comandos?
    //comandos que ejecuten algo¿?
    //public void Invoke
    public BaseCommand baseCommand = null;

}
