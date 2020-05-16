using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SerializableCollections;
using System.Linq;

public class TileUI : MonoBehaviour
{
    private const int colorTexWidth = 10;
    private static readonly string colorTexProp = "_ColorTex";
    private static readonly string numColorsProp = "_NumActiveColors";
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
        ChargingAttackBoss,
    }

    public GameObject tileUIPrefab;
    public ColorDict tileColors;
    public TextureDict textures;
    private Dictionary<Pos, QuadMesh> tiles = new Dictionary<Pos, QuadMesh>();
    private Dictionary<Pos, List<Type>> tilesTypes = new Dictionary<Pos, List<Type>>();

    public bool HasActiveTileUI(Pos p)
    {
        if(tiles.ContainsKey(p))
        {
            var tile = tiles[p];
            if(tile == null)
            {
                ClearNullTile(p);
                return false;
            }
            return true;
        }
        return false;
    }

    public Entry AddType(Pos p, Type t)
    {
        var qMesh = tiles[p];
        // qMesh has been destroyed for some reason
        if (qMesh == null)
        {
            ClearNullTile(p);
            return new Entry();
        }
        var types = tilesTypes[p];
        types.Add(t);
        // Set property block values and return
        UpdatePropertyBlock(p);
        return new Entry { mesh = qMesh, pos = p, type = t };
    }

    public Entry SpawnTileUI(Pos p, Type t, Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
    {
        var obj = Instantiate(tileUIPrefab);
        var qMesh = obj.GetComponent<QuadMesh>();
        qMesh.SetMesh(v1, v2, v3, v4);
        // Log values
        tiles[p] = qMesh;
        tilesTypes[p] = new List<Type> { t };
        // Set property block values and return
        UpdatePropertyBlock(p);
        return new Entry { mesh = qMesh, pos = p, type = t };
    }

    private void UpdatePropertyBlock(Pos p)
    {
        var types = tilesTypes[p].Distinct();
        var count = types.Count();
        var colors = types.Select((t) => tileColors[t]).ToArray();
        var colorTex = new Texture2D(count, 1);
        colorTex.SetPixels(0, 0, count, 1, colors);
        colorTex.Apply();
        var qMesh = tiles[p];
        var pBlock = qMesh.PropertyBlock;
        pBlock.SetTexture(colorTexProp, colorTex);
        pBlock.SetFloat(numColorsProp, count);
        qMesh.PropertyBlock = pBlock;
    }

    public void RemoveType(Entry entry)
    {
        var p = entry.pos;
        var t = entry.type;
        if (!tiles.ContainsKey(p))
            return;
        var qMesh = tiles[p];
        // qMesh has been destroyed for some reason
        if (qMesh == null)
        {
            ClearNullTile(p);
            return;
        }
        var types = tilesTypes[p];
        // Tile actually has this type in it
        if (types.Contains(t))
        {
            types.Remove(t);
            if(types.Count == 0)
            {
                Destroy(tiles[p].gameObject);
                ClearNullTile(p);               
            }
            else
            {
                UpdatePropertyBlock(p);
            }
        }         
    }

    private void ClearNullTile(Pos p)
    {
        tiles.Remove(p);
        tilesTypes.Remove(p);
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
