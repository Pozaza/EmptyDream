using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public sealed class MeshGenerator : MonoBehaviour {
	[Range(0, 1)]
	public float alphaCutoff = 0.5f;
	PolygonCollider2D PGC2D;
	public void Regenerate() {
		alphaCutoff = Mathf.Clamp(alphaCutoff, 0, 1);
		TryGetComponent(out PGC2D);
		TryGetComponent(out SpriteRenderer SR);
		if (PGC2D == null)
			throw new Exception("Нужен компонент PolygonCollider2D");
		if (SR == null)
			throw new Exception("Нужен компонент SpriteRenderer");
		if (SR.sprite == null)
			throw new Exception("Нужна текстура в компоненте SpriteRenderer");
		if (SR.sprite.texture.isReadable == false)
			throw new Exception("Текстура не читабельна");
		PGC2D.useDelaunayMesh = true;
		List<List<Vector2Int>> Pixel_Paths = Get_Unit_Paths(SR.sprite.texture, alphaCutoff);
		Pixel_Paths = Simplify_Paths_Phase_1(Pixel_Paths);
		Pixel_Paths = Simplify_Paths_Phase_2(Pixel_Paths);
		Pixel_Paths = Simplify_Paths_Phase_3(Pixel_Paths);
		Simplify_Paths_Phase_5(ref Pixel_Paths);
		List<List<Vector2>> World_Paths = Finalize_Paths(Pixel_Paths, SR.sprite);
		PGC2D.pathCount = World_Paths.Count;
		for (int i = 0; i < World_Paths.Count; i++)
			PGC2D.SetPath(i, World_Paths[i].ToArray());

	}
	List<List<Vector2>> Finalize_Paths(List<List<Vector2Int>> Pixel_Paths, Sprite sprite) {
		Vector2 pivot = sprite.pivot;
		pivot.x *= Mathf.Abs(sprite.bounds.max.x - sprite.bounds.min.x);
		pivot.x /= sprite.texture.width;
		pivot.y *= Mathf.Abs(sprite.bounds.max.y - sprite.bounds.min.y);
		pivot.y /= sprite.texture.height;

		List<List<Vector2>> Output = new();
		for (int p = 0; p < Pixel_Paths.Count; p++) {
			List<Vector2> Current_List = new();
			for (int o = 0; o < Pixel_Paths[p].Count; o++) {
				Vector2 point = Pixel_Paths[p][o];
				point.x *= Mathf.Abs(sprite.bounds.max.x - sprite.bounds.min.x);
				point.x /= sprite.texture.width;
				point.y *= Mathf.Abs(sprite.bounds.max.y - sprite.bounds.min.y);
				point.y /= sprite.texture.height;
				point -= pivot;
				Current_List.Add(point);
			}
			Output.Add(Current_List);
		}
		return Output;
	}
	List<List<Vector2Int>> Simplify_Paths_Phase_1(List<List<Vector2Int>> Unit_Paths) {
		List<List<Vector2Int>> list = new();
		while (Unit_Paths.Count > 0) {
			List<Vector2Int> Current_Path = new(Unit_Paths[0]);
			Unit_Paths.RemoveAt(0);
			bool Keep_Looping = true;
			while (Keep_Looping) {
				Keep_Looping = false;
				for (int p = 0; p < Unit_Paths.Count; p++) {
					if (Current_Path[^1] == Unit_Paths[p][0]) {
						Keep_Looping = true;
						Current_Path.RemoveAt(Current_Path.Count - 1);
						Current_Path.AddRange(Unit_Paths[p]);
						Unit_Paths.RemoveAt(p--);
					} else if (Current_Path[0] == Unit_Paths[p][^1]) {
						Keep_Looping = true;
						Current_Path.RemoveAt(0);
						Current_Path.InsertRange(0, Unit_Paths[p]);
						Unit_Paths.RemoveAt(p--);
					} else {
						List<Vector2Int> Flipped_Path = new(Unit_Paths[p]);
						Flipped_Path.Reverse();
						if (Current_Path[^1] == Flipped_Path[0]) {
							Keep_Looping = true;
							Current_Path.RemoveAt(Current_Path.Count - 1);
							Current_Path.AddRange(Flipped_Path);
							Unit_Paths.RemoveAt(p--);
						} else if (Current_Path[0] == Flipped_Path[^1]) {
							Keep_Looping = true;
							Current_Path.RemoveAt(0);
							Current_Path.InsertRange(0, Flipped_Path);
							Unit_Paths.RemoveAt(p--);
						}
					}
				}
			}
			list.Add(Current_Path);
		}
		return list;
	}
	List<List<Vector2Int>> Simplify_Paths_Phase_2(List<List<Vector2Int>> Input_Paths) {
		for (int pa = 0; pa < Input_Paths.Count; pa++) {
			for (int po = 0; po < Input_Paths[pa].Count; po++) {
				Vector2 Start = po == 0 ? Input_Paths[pa][^1] : Input_Paths[pa][po - 1];
				Vector2Int End = po == Input_Paths[pa].Count - 1 ? Input_Paths[pa][0] : Input_Paths[pa][po + 1];

				Vector2Int Current_Point = Input_Paths[pa][po];
				Vector2 Direction1 = Current_Point - Start;
				Direction1 /= Direction1.magnitude;
				Vector2 Direction2 = End - Start;
				Direction2 /= Direction2.magnitude;
				if (Direction1 == Direction2)
					Input_Paths[pa].RemoveAt(po--);
			}
		}
		return Input_Paths;
	}
	List<List<Vector2Int>> Simplify_Paths_Phase_3(List<List<Vector2Int>> list) {
		list = list.Distinct().ToList();

		foreach (List<Vector2Int> points in list)
			for (int i = 1; i < points.Count - 1; i++) {
				if (points[i - 1].y != points[i].y && points[i].y == points[i + 1].y)
					points.Remove(points[i + 1]);
				else if (points[i - 1].y == points[i].y && points[i].y != points[i + 1].y)
					points.Remove(points[i]);
			}

		foreach (List<Vector2Int> points in list)
			for (int i = 1; i < points.Count - 1; i++) {
				if (points[i - 1] + ((points[i + 1] - points[i - 1]) / 2) == points[i]) {
					points.Remove(points[i]);
				}
			}

		return list;
	}
	void Simplify_Paths_Phase_5(ref List<List<Vector2Int>> list) {
		foreach (List<Vector2Int> points in list)
			for (int i = 1; i < points.Count - 1; i++) {
				Vector2 direction = new(points[i + 1].x - points[i - 1].x, points[i + 1].y - points[i - 1].y);
				if (points[i] == direction / 2)
					points.Remove(points[i]);
			}
	}
	List<List<Vector2Int>> Get_Unit_Paths(Texture2D texture, float alphaCutoff) {
		List<List<Vector2Int>> output = new();
		for (int x = 0; x < texture.width; x++) {
			for (int y = 0; y < texture.height; y++) {
				if (PixelSolid(texture, new Vector2Int(x, y), alphaCutoff)) {
					if (!PixelSolid(texture, new Vector2Int(x, y + 1), alphaCutoff))
						output.Add(new List<Vector2Int>() { new Vector2Int(x, y + 1), new Vector2Int(x + 1, y + 1) });
					if (!PixelSolid(texture, new Vector2Int(x, y - 1), alphaCutoff))
						output.Add(new List<Vector2Int>() { new Vector2Int(x, y), new Vector2Int(x + 1, y) });
					if (!PixelSolid(texture, new Vector2Int(x + 1, y), alphaCutoff))
						output.Add(new List<Vector2Int>() { new Vector2Int(x + 1, y), new Vector2Int(x + 1, y + 1) });
					if (!PixelSolid(texture, new Vector2Int(x - 1, y), alphaCutoff))
						output.Add(new List<Vector2Int>() { new Vector2Int(x, y), new Vector2Int(x, y + 1) });
				}
			}
		}
		return output;
	}
	bool PixelSolid(Texture2D texture, Vector2Int point, float alphaCutoff) {
		if (point.x < 0 || point.y < 0 || point.x >= texture.width || point.y >= texture.height)
			return false;

		float pixelAlpha = texture.GetPixel(point.x, point.y).a;
		return alphaCutoff == 0 ? pixelAlpha != 0 : alphaCutoff == 1 ? pixelAlpha == 1 : pixelAlpha >= alphaCutoff;
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(MeshGenerator))]
public class MeshGeneratorEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		MeshGenerator PC2D = (MeshGenerator)target;
		if (GUILayout.Button("Regenerate Collider"))
			PC2D.Regenerate();
	}
}
#endif