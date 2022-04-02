using UnityEngine;

public class ProductData : MonoBehaviour {

	public Sprite icon;
	public float price;

	// Start is called before the first frame update
	void Start() {
		gameObject.layer = LayerMask.NameToLayer("Interactable");
	}
}
