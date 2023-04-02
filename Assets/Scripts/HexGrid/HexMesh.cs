using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
	public bool lowRes;
	public MeshCollider meshCollider;
	Mesh hexMesh;
	List<Vector3> vertices;
	List<int> triangles;
	public List<Color> colors;

	void Awake()
	{
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		hexMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		meshCollider = gameObject.AddComponent<MeshCollider>();
		hexMesh.name = "Hex Mesh";
		vertices = new List<Vector3>();
		triangles = new List<int>();
		colors = new List<Color>();
	}

	public void Triangulate(HexCell[] cells)
	{
		colors.Clear();
		hexMesh.Clear();
		vertices.Clear();
		triangles.Clear();
		for (int i = 0; i < cells.Length; i++)
		{
			Triangulate(cells[i]);
		}
		hexMesh.vertices = vertices.ToArray();
		hexMesh.triangles = triangles.ToArray();
		//hexMesh.colors = colors.ToArray();
		Simplify();
		hexMesh.RecalculateNormals();
		hexMesh.RecalculateTangents();
		meshCollider.sharedMesh = hexMesh;
	}

	private void Simplify()
    {
		var verts = new List<Vector3>(hexMesh.vertices);
		var colors = hexMesh.colors;
		var normals = hexMesh.normals;
		Dictionary<Vector3, int> duplicateHashTable = new Dictionary<Vector3, int>();
		List<int> newVerts = new List<int>();
		int[] map = new int[verts.Count];

		List<(Vector3, int)> vertsCopy = new List<(Vector3, int)>();
		for (int i = 0; i < verts.Count; i++)
		{
			vertsCopy.Add((verts[i], i));
		}
		vertsCopy.Sort((a, b) => a.Item1.x.CompareTo(b.Item1.x));
		float currentX = vertsCopy[0].Item1.x;
		List<(Vector3, int)> vertsSlice = new List<(Vector3, int)>();
		for (int i = 0; i < vertsCopy.Count; i++)
        {
			if (i < vertsCopy.Count - 1 && Mathf.Abs(vertsCopy[i].Item1.x - currentX) < 0.1)
            {
                vertsSlice.Add(vertsCopy[i]);
            }
			else
            {
				vertsSlice.Sort((a, b) => a.Item1.z.CompareTo(b.Item1.z));
				Vector3 currentVertex = new Vector3(-1000f, -1000f, -1000f);
				for (int j = 0; j < vertsSlice.Count; j++)
                {
					if (Vector3.Distance(currentVertex, vertsSlice[j].Item1) > 0.001f)
                    {
						map[vertsSlice[j].Item2] = newVerts.Count;
						newVerts.Add(vertsSlice[j].Item2);
					}
					else
                    {
						map[vertsSlice[j].Item2] = newVerts.Count - 1;
                    }
					currentVertex = vertsSlice[j].Item1;
                }
				if (i == vertsCopy.Count - 1)
				{
					if (Vector3.Distance(
							currentVertex, 
							vertsCopy[i].Item1
						) > 0.001f)
					{
						map[vertsCopy[i].Item2] = newVerts.Count;
						newVerts.Add(vertsCopy[i].Item2);
					}
					else
					{
						map[vertsCopy[i].Item2] = newVerts.Count - 1;
					}
				}
				else {
					vertsSlice = new List<(Vector3, int)>();
					vertsSlice.Add(vertsCopy[i]);
					currentX = vertsCopy[i].Item1.x;
				}
			}
        }
		// create new vertices
		var verts2 = new Vector3[newVerts.Count];
		//var colors2 = new Color[newVerts.Count];
		for (int i = 0; i < newVerts.Count; i++)
		{
			int a = newVerts[i];
			verts2[i] = verts[a];
			//colors2[i] = colors[a];
		}
		// map the triangle to the new vertices
		var tris = hexMesh.triangles;
		for (int i = 0; i < tris.Length; i++)
		{
			tris[i] = map[tris[i]];
		}
		hexMesh.triangles = tris;
		hexMesh.vertices = verts2;
		//hexMesh.colors = colors2;
	}

	public static void FixSeams(HexMesh a, HexMesh b, HexMesh c, HexMesh d)
    {
		var vertsA = new List<Vector3>(a.hexMesh.vertices);
		var vertsB = new List<Vector3>(b.hexMesh.vertices);
		var vertsC = new List<Vector3>(c.hexMesh.vertices);
		var vertsD = new List<Vector3>(d.hexMesh.vertices);

		var normalsA = new List<Vector3>(a.hexMesh.normals);
		var normalsB = new List<Vector3>(b.hexMesh.normals);
		var normalsC = new List<Vector3>(c.hexMesh.normals);
		var normalsD = new List<Vector3>(d.hexMesh.normals);

		List<(Vector3, int, int)> verts = new List<(Vector3, int, int)>();
		for (int i = 0; i < vertsA.Count; i++)
		{
			verts.Add((vertsA[i], i, 0));
		}
		for (int i = 0; i < vertsB.Count; i++)
		{
			verts.Add((vertsB[i], i, 1));
		}
		for (int i = 0; i < vertsC.Count; i++)
		{
			verts.Add((vertsC[i], i, 2));
		}
		for (int i = 0; i < vertsD.Count; i++)
		{
			verts.Add((vertsD[i], i, 3));
		}

		verts.Sort((a, b) => a.Item1.x.CompareTo(b.Item1.x));
		float currentX = verts[0].Item1.x;
		List<(Vector3, int, int)> vertsSlice = new List<(Vector3, int, int)>();
		for (int i = 0; i < verts.Count; i++)
		{
			if (i < verts.Count - 1 && Mathf.Abs(verts[i].Item1.x - currentX) < 0.1)
			{
				vertsSlice.Add(verts[i]);
			}
			else
			{
				vertsSlice.Sort((a, b) => a.Item1.z.CompareTo(b.Item1.z));
				Vector3 currentVertex = new Vector3(-1000f, -1000f, -1000f);
                int[] indices = new int[4];
				indices[0] = -1;
				indices[1] = -1;
				indices[2] = -1;
				indices[3] = -1;
				for (int j = 0; j < vertsSlice.Count; j++)
				{
					if (
						j == vertsSlice.Count - 1 || 
						Vector3.Distance(currentVertex, vertsSlice[j].Item1) > 0.001f
					)
					{
						Vector3 normalSum = new Vector3(0.0f, 0.0f, 0.0f);
						Vector3 vertexSum = new Vector3(0.0f, 0.0f, 0.0f);
						int vertexCount = 0;
						if (indices[0] != -1)
                        {
							vertexCount++;
							normalSum += normalsA[indices[0]];
							vertexSum += vertsA[indices[0]];
						}
						if (indices[1] != -1)
						{
							vertexCount++;
							normalSum += normalsB[indices[1]];
							vertexSum += vertsB[indices[1]];
						}
						if (indices[2] != -1)
						{
							vertexCount++;
							normalSum += normalsC[indices[2]];
							vertexSum += vertsC[indices[2]];
						}
						if (indices[3] != -1)
						{
							vertexCount++;
							normalSum += normalsD[indices[3]];
							vertexSum += vertsD[indices[3]];
						}
						if (vertexCount > 1)
						{
							Vector3 normal = normalSum / vertexCount;
							Vector3 vertex = vertexSum / vertexCount;
							if (indices[0] != -1)
							{
								normalsA[indices[0]] = normal;
								vertsA[indices[0]] = (vertexSum - vertsA[indices[0]]) / (vertexCount - 1);
								//vertsA[indices[0]] = vertex;// + new Vector3(0.0002f, 0.0f, 0.0002f);
							}
							if (indices[1] != -1)
							{
								normalsB[indices[1]] = normal;
								vertsB[indices[1]] = (vertexSum - vertsB[indices[1]]) / (vertexCount - 1);
								//vertsB[indices[1]] = vertex;// + new Vector3(-0.0002f, 0.0f, 0.0002f);
							}
							if (indices[2] != -1)
							{
								normalsC[indices[2]] = normal;
								vertsC[indices[2]] = (vertexSum - vertsC[indices[2]]) / (vertexCount - 1);
								//vertsC[indices[2]] = vertex;// + new Vector3(0.0002f, 0.0f, -0.0002f); ;
							}
							if (indices[3] != -1)
							{
								normalsD[indices[3]] = normal;
								vertsD[indices[3]] = (vertexSum - vertsD[indices[3]]) / (vertexCount - 1);
								//vertsD[indices[3]] = vertex;// + new Vector3(-0.0002f, 0.0f, -0.0002f); ;
							}
						}
						indices[0] = -1;
						indices[1] = -1;
						indices[2] = -1;
						indices[3] = -1;

						indices[vertsSlice[j].Item3] = vertsSlice[j].Item2;
					}
					else
					{
						indices[vertsSlice[j].Item3] = vertsSlice[j].Item2;
					}
					currentVertex = vertsSlice[j].Item1;
				}

				vertsSlice = new List<(Vector3, int, int)>();
				vertsSlice.Add(verts[i]);
				currentX = verts[i].Item1.x;
			}
		}

		a.hexMesh.vertices = vertsA.ToArray();
		b.hexMesh.vertices = vertsB.ToArray();
		c.hexMesh.vertices = vertsC.ToArray();
		d.hexMesh.vertices = vertsD.ToArray();

		a.hexMesh.normals = normalsA.ToArray();
		b.hexMesh.normals = normalsB.ToArray();
		c.hexMesh.normals = normalsC.ToArray();
		d.hexMesh.normals = normalsD.ToArray();
	}

	void Triangulate(HexCell cell)
	{
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
		{
			Triangulate(d, cell);
		}
	}

	void Triangulate(HexDirection direction, HexCell cell)
	{
		HexCell neighborBefore = cell.GetNeighbor(direction.Before()) ?? cell;
		HexCell neighborEdge = cell.GetNeighbor(direction) ?? cell;
		HexCell neighborAfter = cell.GetNeighbor(direction.After()) ?? cell;

		Vector3 center = cell.transform.localPosition;
		Vector3 middlePointSolid = HexMetrics.solidCorners[((int)direction + 1) % 6] / 2 + HexMetrics.solidCorners[(int)direction] / 2;
		Vector3 middlePoint = HexMetrics.corners[((int)direction + 1) % 6] / 2 + HexMetrics.corners[(int)direction] / 2;

		Vector3 solidCorner1 = center + HexMetrics.solidCorners[(int)direction];
		Vector3 solidCorner2 = center + HexMetrics.solidCorners[((int)direction + 1) % 6];
		Vector3 corner1 = center + HexMetrics.corners[(int)direction];
		Vector3 corner2 = center + HexMetrics.corners[((int)direction + 1) % 6];

		float thisY = cell.transform.localPosition.y;
		float neighborBeforeY = neighborBefore.transform.localPosition.y;
		float neighborEdgeY = neighborEdge.transform.localPosition.y;
		float neighborAfterY = neighborAfter.transform.localPosition.y;

		float vertex1MaxDifference = Mathf.Max(Mathf.Abs(thisY - neighborBeforeY), Mathf.Abs(thisY - neighborEdgeY), Mathf.Abs(neighborEdgeY - neighborBeforeY));
		float vertex2MaxDifference = Mathf.Max(Mathf.Abs(thisY - neighborAfterY), Mathf.Abs(thisY - neighborEdgeY), Mathf.Abs(neighborEdgeY - neighborAfterY));

		Vector3 quadCorner1 = solidCorner1 + middlePoint - middlePointSolid;
		Vector3 quadCorner2 = solidCorner2 + middlePoint - middlePointSolid;
		corner1.y = (thisY + neighborBeforeY + neighborEdgeY) / 3f;
		corner2.y = (thisY + neighborAfterY + neighborEdgeY) / 3f;

		bool cliffs = false;
		bool cliffsOpposite = false;


		if (vertex1MaxDifference < HexMetrics.cliffTreshold)
        {
            solidCorner1.y = corner1.y * 0.9f + 0.1f * thisY;
            //solidCorner1.y = (1.1f * thisY + 0.9f * neighborBeforeY + 0.9f * neighborEdgeY) / 2.9f;
			quadCorner1.y = corner1.y;
		}
		else
        {
			//Corner neighbor same level, edge neighbor different
			if (
				Mathf.Abs(thisY - neighborBeforeY) < Mathf.Abs(thisY - neighborEdgeY) && 
				Mathf.Abs(thisY - neighborBeforeY) < HexMetrics.cliffTreshold &&
				Mathf.Abs(thisY - neighborBeforeY) < Mathf.Abs(neighborBeforeY - neighborEdgeY)) 
			{
				solidCorner1.y = (1.1f * thisY + 0.9f * neighborBeforeY) / 2.0f;
				quadCorner1.y = (solidCorner1.y + neighborEdgeY) / 2;
				if (thisY > neighborEdgeY) cliffs = true;
				else cliffsOpposite |= true;
			}
			//Edge neighbor same level, corner neighbor different level
			else if (
				Mathf.Abs(thisY - neighborEdgeY) < HexMetrics.cliffTreshold && 
				Mathf.Abs(thisY - neighborEdgeY) < Mathf.Abs(neighborBeforeY - neighborEdgeY))
            {
				solidCorner1.y = (1.1f * thisY + 0.9f * neighborEdgeY) / 2.0f;
				quadCorner1.y = (thisY + neighborEdgeY) / 2;
			}
			//Both neighbors different level (the neighbors have the same level)
			else if (Mathf.Abs(neighborBeforeY - neighborEdgeY) < HexMetrics.cliffTreshold)
            {
				quadCorner1.y = (thisY + (1.1f * neighborEdgeY + 0.9f * neighborBeforeY) / 2) / 2;
				if (thisY > neighborEdgeY) cliffs = true;
                else cliffsOpposite |= true;
            }
			//All tiles different level
			else
            {
				quadCorner1.y = (thisY + neighborEdgeY) / 2;
				if (thisY > neighborEdgeY) cliffs = true;
                else cliffsOpposite |= true;
            }
        }

		if (vertex2MaxDifference < HexMetrics.cliffTreshold)
		{
			solidCorner2.y = corner2.y * 0.9f + 0.1f * thisY;
            //solidCorner2.y = (1.1f * thisY + 0.9f * neighborAfterY + 0.9f * neighborEdgeY) / 2.9f;
            quadCorner2.y = corner2.y;
		}
		else
		{
			//Corner neighbor same level, edge neighbor different
			if (
				Mathf.Abs(thisY - neighborAfterY) < Mathf.Abs(thisY - neighborEdgeY) &&
				Mathf.Abs(thisY - neighborAfterY) < HexMetrics.cliffTreshold &&
				Mathf.Abs(thisY - neighborAfterY) < Mathf.Abs(neighborAfterY - neighborEdgeY))
			{
				solidCorner2.y = (1.1f * thisY + 0.9f * neighborAfterY) / 2.0f;
				quadCorner2.y = (solidCorner2.y + neighborEdgeY) / 2;
				if (thisY > neighborEdgeY) cliffs = true;
			}
			//Corner neighbor different level
			else if (
				Mathf.Abs(thisY - neighborEdgeY) < HexMetrics.cliffTreshold &&
				Mathf.Abs(thisY - neighborEdgeY) < Mathf.Abs(neighborAfterY - neighborEdgeY))
			{
				solidCorner2.y = (1.1f * thisY + 0.9f * neighborEdgeY) / 2.0f;
				quadCorner2.y = (thisY + neighborEdgeY) / 2;
			}
			//Both neighbors different level (the neighbors have the same level)
			else if (Mathf.Abs(neighborAfterY - neighborEdgeY) < HexMetrics.cliffTreshold)
			{
				quadCorner2.y = (thisY + ((1.1f * neighborEdgeY + 0.9f * neighborAfterY) / 2)) / 2;
				if (thisY > neighborEdgeY) cliffs = true;
			}
			//All tiles different level
			else
			{
				quadCorner2.y = (thisY + neighborEdgeY) / 2;
			}
		}

		quadCorner1.y = Mathf.Clamp(quadCorner1.y, solidCorner1.y - 0.3f, solidCorner1.y + 0.3f);
		quadCorner2.y = Mathf.Clamp(quadCorner2.y, solidCorner2.y - 0.3f, solidCorner2.y + 0.3f);

		if (cliffs)
        {
			Vector3 realQuadCorner1 = Perturb(quadCorner1);
			Vector3 realQuadCorner2 = Perturb(quadCorner2);
			Vector3 realSolidCorner1 = Perturb(solidCorner1);
			Vector3 realSolidCorner2 = Perturb(solidCorner2);
			cell.AddCliffs(direction, new Vector3(0.7f * realQuadCorner1.x + 0.3f * realSolidCorner1.x, 0.5f * realQuadCorner1.y + 0.5f * realSolidCorner1.y, 0.7f * realQuadCorner1.z + 0.3f * realSolidCorner1.z), new Vector3(0.7f * realQuadCorner2.x + 0.3f * realSolidCorner2.x, 0.5f * realQuadCorner2.y + 0.5f * realSolidCorner2.y, 0.7f * realQuadCorner2.z + 0.3f * realSolidCorner2.z));
        }
		else
        {
			cell.RemoveCliffs(direction, cliffsOpposite);
        }

		if (lowRes)
		{
            AddTriangleSubdivided(
				center,
				solidCorner1,
				solidCorner2,
				1
			);
            AddTriangleSubdividedColor(cell.Color, 1);

            AddQuadStripe(
				solidCorner1,
				solidCorner2,
				quadCorner1,
				quadCorner2,
				1,
				0.3f
			);

            AddQuadStripeColors(
                cell.Color,
                cell.Color,
                (cell.Color + neighborEdge.Color) / 2f,
                (cell.Color + neighborEdge.Color) / 2f,
                1
            );


            AddTriangleSubdivided(
                solidCorner1,
                corner1,
                quadCorner1,
                1
            );
            AddTriangleSubdividedColor(cell.Color, (cell.Color + neighborBefore.Color + neighborEdge.Color) / 3, (cell.Color + neighborEdge.Color) / 2, 1);

            AddTriangleSubdivided(
                solidCorner2,
                quadCorner2,
                corner2,
                0
            );

            AddTriangleSubdividedColor(cell.Color, (cell.Color + neighborEdge.Color) / 2, (cell.Color + neighborAfter.Color + neighborEdge.Color) / 3, 0);

		}
		else {
			AddTriangleSubdivided(
				center,
				solidCorner1,
				solidCorner2,
				3
			);
            AddTriangleSubdividedColor(cell.Color, 3);

            AddQuadStripe(
				solidCorner1,
				solidCorner2,
				quadCorner1,
				quadCorner2,
				3,
				0.3f
			);

            AddQuadStripeColors(
                cell.Color,
                cell.Color,
                (cell.Color + neighborEdge.Color) / 2f,
                (cell.Color + neighborEdge.Color) / 2f,
                3
            );

            AddTriangleSubdivided(
                solidCorner1,
                corner1,
                quadCorner1,
                1
            );
            AddTriangleSubdividedColor(
				cell.Color, 
				(cell.Color + neighborBefore.Color + neighborEdge.Color) / 3, 
				(cell.Color + neighborEdge.Color) / 2, 
				1
			);

            AddTriangleSubdivided(
                solidCorner2,
                quadCorner2,
                corner2,
                1
            );

            AddTriangleSubdividedColor(
				cell.Color,
				(cell.Color + neighborEdge.Color) / 2,
				(cell.Color + neighborAfter.Color + neighborEdge.Color) / 3,
				1
			);
        }
	}

	void AddTriangleColor(Color c1, Color c2, Color c3)
	{
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c3);
	}

	void AddTriangleColor(Color color)
	{
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}

	void AddTriangleSubdividedColor(Color color, int level)
	{
		int count = 3;
		for (int i = 0; i < level; i++)
        {
			count *= 4;
        }

		for (int i = 0; i < count; i++) {
			colors.Add(color);
		}

	}

	void AddTriangleSubdividedColor(Color c1, Color c2, Color c3, int level)
	{
		if (level == 0) AddTriangleColor(c1, c2, c3);
		else
		{
			AddTriangleSubdividedColor(c1, level - 1);
			AddTriangleSubdividedColor(c2, level - 1);
			AddTriangleSubdividedColor(c1, c2, c3, level - 1);
			AddTriangleSubdividedColor(c3, level - 1);
		}

	}

	void AddTriangleColor(Color c1, Color c2, Color c3, int offset)
	{
		colors[offset] = c1;
		colors[offset+1] = c2;
		colors[offset+2] = c3;
	}

	void AddTriangleColor(Color color, int offset)
	{
		colors[offset] = color;
		colors[offset + 1] = color;
		colors[offset + 2] = color;
	}

	void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, bool perturb = true)
	{
		int vertexIndex = vertices.Count;
		if (perturb)
		{
			vertices.Add(Perturb(v1));
			vertices.Add(Perturb(v2));
			vertices.Add(Perturb(v3));
			vertices.Add(Perturb(v4));
		}
		else
        {
			vertices.Add(v1);
			vertices.Add(v2);
			vertices.Add(v3);
			vertices.Add(v4);
		}
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 3);
	}

	//Negative height limit means no limit
	void AddQuadStripe(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, int level, float heightLimit = -1.0f)
    {
		if (level == 0)
		{
			if (heightLimit > 0)
            {
				v3.y = Mathf.Clamp(v3.y, v1.y -heightLimit, v1.y + heightLimit);
				v4.y = Mathf.Clamp(v4.y, v2.y -heightLimit, v2.y + heightLimit);
            }
			AddQuad(v1, v2, (v1 + v3) / 2, (v2 + v4) / 2);
			AddQuad((v1 + v3) / 2, (v2 + v4) / 2, v3, v4);
		}
		else
		{
			Vector3 v5, v6;
			v5 = (v1 + v2) / 2;
			v6 = (v3 + v4) / 2;
			AddQuadStripe(v1, v5, v3, v6, level - 1);
			AddQuadStripe(v5, v2, v6, v4, level - 1);
		}
	}

	void AddQuadStripeColors(Color c1, Color c2, Color c3, Color c4, int level)
	{
		if (level == 0)  {
			AddQuadColor(c1, c2, c1, c2);
			AddQuadColor(c1, c2, c3, c4);
		}
		else
		{
			AddQuadStripeColors(c1, c2, c3, c4, level - 1);
			AddQuadStripeColors(c1, c2, c3, c4, level - 1);
		}
	}

	void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
	{
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c3);
		colors.Add(c4);
	}

	void AddQuadColor(Color c1, Color c2, Color c3, Color c4, int offset)
	{
		colors[offset] = c1;
		colors[offset + 1] = c2;
		colors[offset + 2] = c3;
		colors[offset + 3] = c4;
	}

	void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
	{
		int vertexIndex = vertices.Count;
		vertices.Add(Perturb(v1));
		vertices.Add(Perturb(v2));
		vertices.Add(Perturb(v3));
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}

	void AddTriangleSubdivided(Vector3 v1, Vector3 v2, Vector3 v3, int level)
	{
		if (level == 0) AddTriangle(v1, v2, v3);
		else if (level == 1) {
			Vector3 v4, v5, v6;
			v4 = (v1 + v2) / 2;
			v5 = (v1 + v3) / 2;
			v6 = (v3 + v2) / 2;
			AddTriangle(v1, v4, v5);
			AddTriangle(v4, v2, v6);
			AddTriangle(v4, v6, v5);
			AddTriangle(v5, v6, v3);
		}
		else
        {
			Vector3 v4, v5, v6;
			v4 = (v1 + v2) / 2;
			v5 = (v1 + v3) / 2;
			v6 = (v3 + v2) / 2;
			AddTriangleSubdivided(v1, v4, v5, level - 1);
			AddTriangleSubdivided(v4, v2, v6, level - 1);
			AddTriangleSubdivided(v4, v6, v5, level - 1);
			AddTriangleSubdivided(v5, v6, v3, level - 1);
		}
	}

	Vector3 Perturb(Vector3 position)
	{
		Vector4 sample = HexMetrics.SampleNoise(position);
		position.y += sample.y /** Mathf.Clamp(position.y, 1, 3)*/ * 0.5f;
		position.x += (2 * sample.x - 1) * 1;
		position.z += (2 * sample.z - 1) * 1;		
		return position;
	}
}
