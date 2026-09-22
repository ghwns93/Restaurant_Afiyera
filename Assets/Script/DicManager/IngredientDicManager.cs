using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IngredientDicManager : BaseDicManager<IngredientDicManager, int, ItemIngredientData>
{
    protected override int GetKey(ItemIngredientData data)
    {
        return data.Id;
    }
}