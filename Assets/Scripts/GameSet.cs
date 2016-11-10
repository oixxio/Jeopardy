using UnityEngine;
using System.Collections;

public class GameSet : MonoBehaviour {

    public GameObject card;
	// Use this for initialization
	void Start () {
        GameObject card1 = (GameObject)Instantiate(card, new Vector3(0,0,0),Quaternion.identity);
        card1.name = "card-1";
        Instantiate(card, new Vector3(-2, 0, 0), Quaternion.identity);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            Debug.Log(hit.transform.gameObject.name);
            StartCoroutine(rotateCard(hit.transform.parent.gameObject));
        }
    }
    private IEnumerator rotateCard(GameObject go)
    {
        float seconds = 0.005f;
        for (int i = 1; i <= 45; i++)
        {
            yield return new WaitForSeconds(seconds);
            go.transform.Rotate(Vector3.up * 4.0f);
        }
    }
}
