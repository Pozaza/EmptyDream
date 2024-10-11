using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class Extensions {
	public static T FindObj<T>(this GameObject gameObject, string text)
		=> gameObject.transform.Find(text).GetComponent<T>();
	public static GameObject FindObj(this GameObject gameObject, string text)
		=> gameObject.transform.Find(text).gameObject;
	public static GameObject FindObj(this Transform transform, string text)
		=> transform.Find(text).gameObject;
	public static T FindObj<T>(this Transform transform, string text)
		=> transform.Find(text).GetComponent<T>();
	public static T FindObjChild<T>(this Transform transform, string text)
		=> transform.Find(text).GetComponentInChildren<T>();
	public static GameObject FindTag(this Object _, string tag)
		=> GameObject.FindGameObjectWithTag(tag);
	public static T FindTag<T>(this Object _, string tag)
		=> GameObject.FindGameObjectWithTag(tag).GetComponent<T>();
	public static GameObject GetChildObj(this Transform transform, int index)
		=> transform.GetChild(index).gameObject;
	public static Transform GetChildObj(this GameObject gameObject, int index)
		=> gameObject.transform.GetChild(index);
	public static bool IsTag(this Collider2D collider, params string[] tags)
		=> tags.Any(tag => collider.CompareTag(tag));
	public static bool IsTag(this Collision2D collision, params string[] tags)
		=> tags.Any(tag => collision.collider.CompareTag(tag));
	public static bool IsTag(this Collider2D collider, List<string> tags)
		=> tags.Any(tag => collider.CompareTag(tag));
	public static bool IsTag(this Collision2D collision, List<string> tags)
		=> tags.Any(tag => collision.collider.CompareTag(tag));
	public static bool IsObj(this Collider2D collider, List<GameObject> objs)
		=> objs.Any(obj => collider.gameObject == obj);
	public static bool IsObj(this Collision2D collision, List<GameObject> objs)
		=> objs.Any(obj => collision.gameObject == obj);
	public static AudioClip Random(this AudioClip[] clips)
		=> clips[UnityEngine.Random.Range(0, clips.Length)];
	public static bool Some(this string text, params string[] values)
		=> values.Contains(text);
	public static Vector3 Add(ref this Vector3 vector3, float value) {
		vector3.x += value;
		vector3.y += value;
		vector3.z += value;
		return vector3;
	}
	public static Vector2 Add(ref this Vector2 vector2, float value) {
		vector2.x += value;
		vector2.y += value;
		return vector2;
	}
}