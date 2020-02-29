using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SerializableCollections;

public class TileUI : MonoBehaviour
{
    private static readonly string colorProp1 = "_Color1";
    private static readonly string colorProp2 = "_Color2";
    private static readonly string texProp1 = "_DetailTex";
    private static readonly string texProp2 = "_DetailTex2";
    public enum Type
    {
        Empty,
        MoveRangeParty,
        MoveRangeEnemy,
        TargetRangeParty,
        TargetRangeEnemy, // Unused
        TargetPreviewParty,
        TargetPreviewEnemy,
        ChargingAttackParty,
        ChargingAttackEnemy,
    }

    public GameObject tileUIPrefab;
    public ColorDict tileColors;
    public TextureDict textures;
    private Dictionary<Pos, QuadMesh> tiles = new Dictionary<Pos, QuadMesh>();
    private Dictionary<Pos, System.Tuple<Type,Type>> tilesTypes = new Dictionary<Pos, System.Tuple<Type, Type>>();

    public bool HasActiveTileUI(Pos p) => tiles.ContainsKey(p);

    public Entry SetPrimaryType(Pos p, Type t)
    {
        var qMesh = tiles[p];
        var pBlock = qMesh.PropertyBlock;
        var color2 = pBlock.GetColor(colorProp2);
        var color1 = tileColors[t];
        var tex2 = pBlock.GetTexture(texProp2);
        var tex1 = textures.ContainsKey(t) ? textures[t].texture : null;
        tilesTypes[p] = new System.Tuple<Type, Type>(t, tilesTypes[p].Item2);

        SetPropertyBlock(qMesh, color1, color2, tex1, tex2);

        return new Entry { mesh = qMesh, pos = p, type = t };
    }

    public Entry SetSecondaryType(Pos p, Type t)
    {
        var qMesh = tiles[p];
        var pBlock = qMesh.PropertyBlock;
        var color1 = pBlock.GetColor(colorProp1);
        var color2 = tileColors[t];
        var tex1 = pBlock.GetTexture(texProp1);
        var tex2 = textures.ContainsKey(t) ? textures[t].texture : null;
        tilesTypes[p] = new System.Tuple<Type, Type>(tilesTypes[p].Item1, t);

        SetPropertyBlock(qMesh, color1, color2, tex1, tex2);

        return new Entry { mesh = qMesh, pos = p, type = t };
    }

    public Entry SpawnTileUI(Pos p, Type t, Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
    {
        var obj = Instantiate(tileUIPrefab);
        var qMesh = obj.GetComponent<QuadMesh>();
        qMesh.SetMesh(v1, v2, v3, v4);
        var pBlock = qMesh.PropertyBlock;
        var color1 = tileColors[t];
        var color2 = tileColors[t];
        Texture tex1, tex2;
        if (textures.ContainsKey(t))
        {
            tex1 = textures[t].texture;
            tex2 = textures[t].texture;
        }
        else
        {
            tex1 = null;
            tex2 = null;
        }
        tiles[p] = qMesh;
        tilesTypes[p] = new System.Tuple<Type, Type>(t, t);

        SetPropertyBlock(qMesh, color1, color2, tex1, tex2);

        return new Entry { mesh = qMesh, pos = p, type = t };
    }

    private void SetPropertyBlock(QuadMesh qMesh, Color color1, Color color2, Texture tex1, Texture tex2)
    {
        var pBlock = qMesh.PropertyBlock;
        pBlock.SetColor(colorProp1, color1);
        pBlock.SetColor(colorProp2, color2);
        if(tex1 != null)
            pBlock.SetTexture(texProp1, tex1);
        if(tex2 != null)
            pBlock.SetTexture(texProp2, tex2);
        qMesh.PropertyBlock = pBlock;
    }

    public void ClearTileUI(Entry entry)
    {
        var key = entry.pos;
        if (!tiles.ContainsKey(key))
            return;
        if(tilesTypes[key].Item1 == entry.type)
        {
            if(tilesTypes[key].Item2 == entry.type)
            {
                Destroy(tiles[key].gameObject);
                tiles.Remove(key);
                tilesTypes.Remove(key);
            }
            else
            {
                SetPrimaryType(key, tilesTypes[key].Item2);
            }
        }
        else if(tilesTypes[key].Item2 == entry.type)
        {
            SetSecondaryType(key, tilesTypes[key].Item1);
        }
        // Else neither of the colors is actually that type. return.
    }

    public struct Entry
    {
        public QuadMesh mesh;
        public Pos pos;
        public Type type;
    }

    [System.Serializable] public class ColorDict : SDictionary<Type, Color> { };
    [System.Serializable] public class TextureDict : SDictionary<Type, Sprite> { };
}
