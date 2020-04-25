using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class UnitUI : MonoBehaviour
{
    private static readonly string stunProp = "_Stunned";
    private static readonly string chargeProp = "_Charging";
    private static readonly string chargeColorProp = "_ChargeColor";

    public Color chargeColor;

    private SpriteRenderer sprRenderer;
    private MaterialPropertyBlock propertyBlock;
    public MaterialPropertyBlock PropertyBlock
    {
        get => propertyBlock;
        set
        {
            propertyBlock = value;
            sprRenderer.SetPropertyBlock(value);
        }
    }

    private void Awake()
    {
        sprRenderer = GetComponent<SpriteRenderer>();
        // Initialize property block cache
        propertyBlock = new MaterialPropertyBlock();
        sprRenderer.GetPropertyBlock(propertyBlock);
    }

    public void SetStun(bool value)
    {
        var pBlock = PropertyBlock;
        pBlock.SetFloat(stunProp, value ? 1 : 0);
        PropertyBlock = pBlock;
    }

    public void SetCharge(bool value)
    {
        var pBlock = PropertyBlock;
        pBlock.SetColor(chargeColorProp, chargeColor);
        pBlock.SetFloat(chargeProp, value ? 1 : 0);
        PropertyBlock = pBlock;
    }
}
