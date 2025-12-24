using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class HexTerrainGenerator : MonoBehaviour
{
    public float hexRadius = 1f;
    public float heightUnit = 1f;
    [Range(0.05f, 0.3f)]
    public float rampBlendRatio = 0.15f;
    public TextAsset mapJson;

    [Header("Materials")]
    public Material topMaterial;
    public Material cliffMaterial;
    public Material floorMaterial;

    Transform topRoot, cliffRoot;
    Dictionary<HexCoord, TileData> tileMap;

    [System.Serializable]
    class TileJson
    {
        public int q, r, height;
        public List<int> ramps;
    }

    class TileData
    {
        public HexCoord coord;
        public int height;
        public HashSet<int> ramps = new();
    }

    class MapJson
    {
        public List<TileJson> tiles;
    }

    public struct HexCoord
    {
        public int q, r;
        public HexCoord(int q, int r) { this.q = q; this.r = r; }
        public static HexCoord operator +(HexCoord a, HexCoord b) => new HexCoord(a.q + b.q, a.r + b.r);
    }

    static readonly HexCoord[] Dir = { new(+1, 0), new(0, +1), new(-1, +1), new(-1, 0), new(0, -1), new(+1, -1) };
    static int Opp(int d) => (d + 3) % 6;

    public void GenerateMap()
    {
        ClearOld();
        LoadJson();
        SetupRoots();
        NormalizeRamps();
        Generate();
        CombineAllMeshesWithFloor();
    }

    void ClearOld()
    {
        List<GameObject> children = new();
        foreach (Transform t in transform)
            children.Add(t.gameObject);

        children.ForEach(go => DestroyImmediate(go));
    }

    void LoadJson()
    {
        tileMap = new();
        var data = JsonUtility.FromJson<MapJson>(mapJson.text);

        foreach (var t in data.tiles)
        {
            var tile = new TileData { coord = new HexCoord(t.q, t.r), height = t.height };

            if (t.ramps != null)
                foreach (var r in t.ramps)
                    tile.ramps.Add(r);

            tileMap[tile.coord] = tile;
        }
    }

    void SetupRoots()
    {
        topRoot = new GameObject("Top").transform;
        topRoot.SetParent(transform);

        cliffRoot = new GameObject("Cliff").transform;
        cliffRoot.SetParent(transform);
    }

    void NormalizeRamps()
    {
        foreach (var t in tileMap.Values)
            foreach (var d in t.ramps)
                if (tileMap.TryGetValue(t.coord + Dir[d], out var n))
                    n.ramps.Add(Opp(d));
    }

    void Generate()
    {
        foreach (var tile in tileMap.Values)
        {
            CreateTop(tile);

            Vector3 center = AxialToWorld(tile.coord, tile.height * heightUnit);

            for (int d = 0; d < 6; d++)
            {
                HexCoord nc = tile.coord + Dir[d];
                if (!tileMap.TryGetValue(nc, out var n))
                {
                    CreateFullCliff(center, d, tile.height * heightUnit, cliffRoot);
                    continue;
                }

                int dh = tile.height - n.height;
                if (tile.ramps.Contains(d))
                    CreateBlendCliff(tile, n, d);
                else if (dh > 0)
                    CreateFullCliff(center, d, dh * heightUnit, cliffRoot);
            }
        }
    }

    void CreateTop(TileData tile)
    {
        float baseY = tile.height * heightUnit;
        Vector3 center = AxialToWorld(tile.coord, baseY);

        Vector3[] v = new Vector3[1 + 6 + 6 * 3];
        v[0] = center;

        for (int i = 0; i < 6; i++)
            v[1 + i] = center + Corner(i);

        for (int i = 0; i < 6; i++)
        {
            Vector3 a = center + Corner(i);
            Vector3 b = center + Corner((i + 1) % 6);

            if (!tile.ramps.Contains(i))
            {
                v[7 + i * 3] = a;
                v[7 + i * 3 + 1] = Vector3.Lerp(a, b, 0.5f);
                v[7 + i * 3 + 2] = b;
                continue;
            }

            tileMap.TryGetValue(tile.coord + Dir[i], out var neighbor);
            float neighborY = neighbor != null ? neighbor.height * heightUnit : baseY;
            float rampY = (tile.height * heightUnit + neighborY) * 0.5f;
            float leftRatio = rampBlendRatio;
            float rightRatio = 1 - rampBlendRatio;

            Vector3 leftBlend = Vector3.Lerp(a, b, leftRatio);
            leftBlend.y = rampY;

            Vector3 mid1 = Vector3.Lerp(a, b, leftRatio);
            mid1.y = rampY;

            Vector3 mid2 = Vector3.Lerp(a, b, rightRatio);
            mid2.y = rampY;

            v[7 + i * 3] = leftBlend;
            v[7 + i * 3 + 1] = mid1;
            v[7 + i * 3 + 2] = mid2;
        }

        List<int> triangles = new();
        for (int i = 0; i < 6; i++)
        {
            int c = 0;
            int v1 = 1 + i;
            int v2 = 1 + (i + 1) % 6;
            int lb = 7 + i * 3;
            int m1 = lb + 1;
            int m2 = lb + 2;
            triangles.AddRange(new[] { c, lb, v1, c, m1, lb, c, m2, m1, c, v2, m2 });
        }

        Mesh m = new();
        m.vertices = v;
        m.triangles = triangles.ToArray();
        m.RecalculateNormals();
        CreateMesh(m, Color.green, topRoot);
    }

    void CreateFullCliff(Vector3 center, int d, float h, Transform parent)
    {
        Vector3 a = center + Corner(d);
        Vector3 b = center + Corner((d + 1) % 6);
        CreateQuad(a, b, h, parent);
    }

    void CreateBlendCliff(TileData high, TileData low, int d)
    {
        if (!high.ramps.Contains(d) || high.height <= low.height)
            return;

        float yHigh = high.height * heightUnit;
        float yLow = low.height * heightUnit;
        float rampY = (yHigh + yLow) * 0.5f;
        
        Vector3 center = AxialToWorld(high.coord, yHigh);
        Vector3 a = center + Corner(d);
        Vector3 b = center + Corner((d + 1) % 6);

        float leftRatio = rampBlendRatio, rightRatio = 1 - rampBlendRatio;

        Vector3 leftTop = Vector3.Lerp(a, b, leftRatio);
        leftTop.y = rampY;

        Vector3 rightTop = Vector3.Lerp(a, b, rightRatio);
        rightTop.y = rampY;

        // 사다리꼴 메쉬 생성: 상단은 경사, 하단은 yLow에서 수평
        CreateTrapezoid(a, leftTop, yLow, cliffRoot);
        CreateTrapezoid(rightTop, b, yLow, cliffRoot);
    }

    void CreateTrapezoid(Vector3 top1, Vector3 top2, float bottomY, Transform parent)
    {
        // 사다리꼴: top1(높음) → top2(낮음) → top2를 bottomY로 → top1을 bottomY로
        Vector3 bottom1 = new Vector3(top1.x, bottomY, top1.z);
        Vector3 bottom2 = new Vector3(top2.x, bottomY, top2.z);

        Mesh m = new();
        m.vertices = new[] { top1, top2, bottom2, bottom1 };
        m.triangles = new[] { 0, 1, 2, 0, 2, 3 };
        m.RecalculateNormals();
        CreateMesh(m, Color.gray, parent);
    }

    void CreateQuad(Vector3 a, Vector3 b, float h, Transform parent)
    {
        Mesh m = new();
        m.vertices = new[] { a, b, b - Vector3.up * h, a - Vector3.up * h };
        m.triangles = new[] { 0, 1, 2, 0, 2, 3 };
        m.RecalculateNormals();
        CreateMesh(m, Color.gray, parent);
    }

    void CombineAllMeshesWithFloor()
    {
        // Top 메쉬 결합
        List<CombineInstance> topCombine = new();
        foreach (Transform child in topRoot)
        {
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {
                CombineInstance ci = new() { mesh = mf.sharedMesh, transform = child.localToWorldMatrix };
                topCombine.Add(ci);
            }
        }

        // Cliff 메쉬 결합
        List<CombineInstance> cliffCombine = new();
        foreach (Transform child in cliffRoot)
        {
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {
                CombineInstance ci = new() { mesh = mf.sharedMesh, transform = child.localToWorldMatrix };
                cliffCombine.Add(ci);
            }
        }

        // 타일 영역 계산
        float minX = float.MaxValue, maxX = float.MinValue, minZ = float.MaxValue, maxZ = float.MinValue;
        foreach (var t in tileMap.Values)
        {
            Vector3 pos = AxialToWorld(t.coord, 0);
            minX = Mathf.Min(minX, pos.x - hexRadius);
            maxX = Mathf.Max(maxX, pos.x + hexRadius);
            minZ = Mathf.Min(minZ, pos.z - hexRadius);
            maxZ = Mathf.Max(maxZ, pos.z + hexRadius);
        }

        // 바닥 Mesh 생성
        Vector3 p0 = new(minX, 0, minZ);
        Vector3 p1 = new(maxX, 0, minZ);
        Vector3 p2 = new(maxX, 0, maxZ);
        Vector3 p3 = new(minX, 0, maxZ);

        Mesh floorMesh = new Mesh();
        floorMesh.vertices = new[] { p0, p1, p2, p3 };
        floorMesh.triangles = new[] { 0, 3, 2, 0, 2, 1 };
        floorMesh.RecalculateNormals();

        // 최종 오브젝트 생성 (서브메쉬 방식)
        GameObject map = new GameObject("HexMap");
        map.transform.SetParent(transform);
        map.transform.localPosition = Vector3.zero;

        Mesh combinedMesh = new Mesh();
        combinedMesh.subMeshCount = 3;

        // 각 서브메쉬 결합
        Mesh topMesh = new Mesh();
        topMesh.CombineMeshes(topCombine.ToArray(), true, true);

        Mesh cliffMesh = new Mesh();
        cliffMesh.CombineMeshes(cliffCombine.ToArray(), true, true);

        // 모든 버텍스 결합
        List<Vector3> vertices = new();
        List<Vector3> normals = new();
        List<int> topTris = new();
        List<int> cliffTris = new();
        List<int> floorTris = new();

        int offset = 0;

        // Top 메쉬 추가
        vertices.AddRange(topMesh.vertices);
        normals.AddRange(topMesh.normals);
        topTris.AddRange(topMesh.triangles);
        offset += topMesh.vertexCount;

        // Cliff 메쉬 추가
        vertices.AddRange(cliffMesh.vertices);
        normals.AddRange(cliffMesh.normals);
        foreach (int idx in cliffMesh.triangles)
            cliffTris.Add(idx + offset);
        offset += cliffMesh.vertexCount;

        // Floor 메쉬 추가
        vertices.AddRange(floorMesh.vertices);
        normals.AddRange(floorMesh.normals);
        foreach (int idx in floorMesh.triangles)
            floorTris.Add(idx + offset);

        combinedMesh.vertices = vertices.ToArray();
        combinedMesh.normals = normals.ToArray();
        combinedMesh.SetTriangles(topTris, 0);
        combinedMesh.SetTriangles(cliffTris, 1);
        combinedMesh.SetTriangles(floorTris, 2);

        MeshFilter mfCombined = map.AddComponent<MeshFilter>();
        mfCombined.sharedMesh = combinedMesh;

        MeshRenderer mr = map.AddComponent<MeshRenderer>();
        mr.materials = new Material[]
        {
            topMaterial != null ? topMaterial : new Material(Shader.Find("Standard")) { color = Color.green },
            cliffMaterial != null ? cliffMaterial : new Material(Shader.Find("Standard")) { color = Color.gray },
            floorMaterial != null ? floorMaterial : new Material(Shader.Find("Standard")) { color = new Color(0.3f, 0.2f, 0.1f) }
        };

        // 임시 오브젝트 제거
        DestroyImmediate(topRoot.gameObject);
        DestroyImmediate(cliffRoot.gameObject);
    }

    Vector3 AxialToWorld(HexCoord c, float y)
    {
        float x = Mathf.Sqrt(3f) * hexRadius * (c.q + c.r * 0.5f);
        float z = 1.5f * hexRadius * c.r;
        return new Vector3(x, y, z);
    }

    Vector3 Corner(int i)
    {
        float a = Mathf.Deg2Rad * (60 * i - 30);
        return new Vector3(hexRadius * Mathf.Cos(a), 0, hexRadius * Mathf.Sin(a));
    }

    void CreateMesh(Mesh m, Color c, Transform p)
    {
        var go = new GameObject("Mesh");
        go.transform.SetParent(p);
        go.AddComponent<MeshFilter>().mesh = m;
        var r = go.AddComponent<MeshRenderer>();
        r.material = new Material(Shader.Find("Standard")) { color = c };
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(HexTerrainGenerator))]
public class HexTerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        HexTerrainGenerator generator = (HexTerrainGenerator)target;
        if (GUILayout.Button("Generate Hex Map"))
        {
            generator.GenerateMap();
        }
    }
}

#endif