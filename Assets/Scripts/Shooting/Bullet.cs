using UnityEngine;

public class Bullet : MonoBehaviour {
	public float speed = 5;
	[SerializeField] public float damage = 1f;
	public AudioClip[] enemySound;
	public AudioClip[] floorSound;
	public Rigidbody2D rb;
	public GameObject particlePrefab;

	void Start() => Destroy(gameObject, 1.5f);
	void FixedUpdate() {
		Vector3 v = transform.TransformDirection(Vector2.right);
		rb.velocity = v * speed;

		RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.left), 1f);

		if (!hit)
			hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.right), 1f);

		if (!hit)
			return;

		if (hit.collider.CompareTag("Enemy")) {
			GameManager.Instance?.sounds?.PlayOneShot(enemySound[Random.Range(0, enemySound.Length)]);
			hit.collider.GetComponent<Enemy>().Hit(damage);
			Instantiate(particlePrefab, hit.point, Quaternion.identity);
			Destroy(gameObject, .025f);
		}
		if (hit.collider.CompareTag("Collision")) {
			GameManager.Instance?.sounds?.PlayOneShot(floorSound[Random.Range(0, floorSound.Length)]);
			Instantiate(particlePrefab, hit.point, Quaternion.identity);
			Destroy(gameObject, .025f);
		}
	}
	void OnCollisionEnter2D(Collision2D other) {
		if (other.IsTag("Enemy")) {
			GameManager.Instance?.sounds?.PlayOneShot(enemySound[Random.Range(0, enemySound.Length)]);
			other.collider.GetComponent<Enemy>().Hit(damage);
			Instantiate(particlePrefab, transform.position, Quaternion.identity);
			Destroy(gameObject, .025f);
		}
		if (other.IsTag("Collision")) {
			GameManager.Instance?.sounds?.PlayOneShot(floorSound[Random.Range(0, floorSound.Length)]);
			Instantiate(particlePrefab, transform.position, Quaternion.identity);
			Destroy(gameObject, .025f);
		}
	}
}
