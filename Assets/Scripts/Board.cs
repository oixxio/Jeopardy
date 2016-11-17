using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour
{

  public Board(GameObject card,Card[] cards,int level)
    {
        //Instantiate(Background);
        string category;
        string fase = "";
        //setea la fase segun el nivel
        switch (level)
        {
            case 1:
                fase = "Avonpardy";
                break;
            case 2:
                fase = "Avonpardy Doble";
                break;
            case 3:
                fase = "Avonpardy Final";
                break;
        }
        if(level != 3)
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject title = (GameObject)Instantiate(card, new Vector3(i, 5, 0), Quaternion.identity);
                //Levanto solo las categorias son las mismas para todos los niveles cambian las preguntas
                category = getCardInfo(cards, fase, 100 * level, i).Category();

                Transform children = title.GetComponentInChildren<Transform>();

                foreach (Transform child in children)
                {
                    Transform texts = child.GetComponentInChildren<Transform>();
                    foreach (Transform text in texts)
                    {
                        if (text.name == "PointsText")
                        {
                            text.GetComponent<TextMesh>().text = category;
                        }
                    }

                }

                title.name = "title-" + i.ToString();
                title.tag = "Untagged";
            }

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    GameObject cardNew = (GameObject)Instantiate(card, new Vector3(col, row, 0), new Quaternion(0, 0, 0, 0));
                    //((5 - row) * 100)*level calcula el valor de la tarjeta dependiendo la fila y el nivel seria bueno pasarlo a una funcion
                    setText(cardNew, getCardInfo(cards, fase, getCardValue(row, level), col).Question(),
                        getCardInfo(cards, fase, getCardValue(row, level), col).Answer(),
                        getCardInfo(cards, fase, getCardValue(row, level), col).Points().ToString());

                    cardNew.name = "card-" + row.ToString() + col.ToString();
                }
            }
        }else
        {
            GameObject cardNew = (GameObject)Instantiate(card, new Vector3(1.5f, 2.4f, 0), new Quaternion(0, 0, 0, 0));
            //((5 - row) * 100)*level calcula el valor de la tarjeta dependiendo la fila y el nivel seria bueno pasarlo a una funcion
            int catFinal = Random.Range(0, 3);
            setText(cardNew, getCardInfo(cards, "Avonpardy Final", 0, catFinal).Question(),
                getCardInfo(cards, "Avonpardy Final", 0, catFinal).Answer(),
                getCardInfo(cards, "Avonpardy Final", 0, catFinal).CategoryName());

            cardNew.name = "card-final";
        }
    }
    
        

    private void setText(GameObject obj, string front, string back, string points)
    {
        Transform children = obj.GetComponentInChildren<Transform>();

        foreach (Transform child in children)
        {
            Transform texts = child.GetComponentInChildren<Transform>();
            foreach (Transform text in texts)
            {
                switch (text.name)
                {
                    case "FrontText":
                        text.GetComponent<TextMesh>().text = front;
                        break;
                    case "BackText":
                        text.GetComponent<TextMesh>().text = back;
                        break;
                    case "PointsText":
                        text.GetComponent<TextMesh>().text = points;
                        break;
                }
                

            }

        }
    }

    private Card getCardInfo(Card[] cards, string fase, int points, int col)
    {
        string cat;
        switch (col)
        {
            case 0:
                cat = "ASLF";
                break;
            case 1:
                cat = "PATD";
                break;
            case 2:
                cat = "Producto";
                break;
            case 3:
                cat = "Varios";
                break;
            default:
                cat = "";
                break;
        }
        foreach (Card card in cards)
        {            
            if (card.Category() == cat && card.Fase() == fase && card.Points() == points)
                return card;

        }
        return new Card("", "", "", "", "", 0);
    }
    private int getCardValue(int row, int level)
    {
        return ((5 - row) * 100) * level;
    }
}
