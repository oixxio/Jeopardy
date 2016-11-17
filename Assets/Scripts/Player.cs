using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private string playerName = "";
    private int points = 0;
    GameObject playerObj;
    // El constructor esta incompleto, hay
    //que agregarle el avatar para cuando lo cree
    public Player(string n, GameObject player)
    {
        playerName = n;
        player.name = n;
        GameObject.Find(playerName + "/NameText").GetComponent<TextMesh>().text = playerName;
        playerObj = player;
    }

    public string Name()
    {
        return playerName;
    }
    public void SetPoints(int p)
    {
        points += p;
        GameObject.Find(playerName + "/PointsText").GetComponent<TextMesh>().text = points.ToString();
    }
    public int GetPoints()
    {
        return points;
    }
    public GameObject GetPlayerObj()
    {
        return playerObj;
    }
}
