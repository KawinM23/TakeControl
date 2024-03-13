using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentsManager : MonoBehaviour
{
    public EquipmentSlot[] equipmentSlots;

    int selectedSlot =-1;

    private void Start() 
    {
        ChangeSelectedSlot(0);    
    }

    private void Update() 
    {
        if (Input.inputString != null){
            bool isNumber = int.TryParse(Input.inputString,out int number);
            if( isNumber && number > 0 && number < 4){
                ChangeSelectedSlot(number-1);
            }
        }
    }
    void ChangeSelectedSlot(int newValue)
    {
        Debug.Log(selectedSlot);
        Debug.Log(newValue);
        if(selectedSlot > -1)
        {
            equipmentSlots[selectedSlot].Deselect();
        }
        equipmentSlots[newValue].Select();
        selectedSlot = newValue;
        Debug.Log(selectedSlot);
    }
}
