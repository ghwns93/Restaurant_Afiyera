using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IngredientDicManager : BaseDicManager<IngredientDicManager, int, ItemData>
{
    protected override int GetKey(ItemData data)
    {
        return data.id;
    }
}