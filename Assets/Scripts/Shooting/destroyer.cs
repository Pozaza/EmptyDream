using UnityEngine;

public class destroyer : MonoBehaviour {
	public float lifeTime = 5f;

	void Update() {
		if (lifeTime > 0) {
			lifeTime -= Time.deltaTime;
			if (lifeTime <= 0)
				Destroy(gameObject);
		}
	}
}