using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public GameObject Card;        
    public GameObject Booth;
    public TextAsset csvFile;
    public Material active;
    
    private string cardName;
    private string cardValue;
    private string[] bet = new string[5] {"","","","",""};    
    private int playerNumber = 0;
    private int level = 1;
    private bool timerFlag = false;
    private bool selectPlayer = false;
    private bool cardChosen = false;
    private bool activeTurn = false;
    private bool createInput = true;
    private Player[] players = new Player[5];
    private LinkedList<int> turns = new LinkedList<int>();
    private LinkedListNode<int> turn;
    private GameObject chosenCard;
   
    void Awake()
    {
        //Aca cargo manualmente 3 equipos esto se deberia hacer desde la pantalla de seleccion
        GameObject playerOixxio = (GameObject)Instantiate(Booth, new Vector3(4, -3, 6.6f), Quaternion.identity);
        Player player1 = new Player("Oixxio", playerOixxio);
        players[0] = player1;
        turns.AddLast(0);
        GameObject playerAvon = (GameObject)Instantiate(Booth, new Vector3(8, -3, 6.6f), Quaternion.identity);
        Player player2 = new Player("Avon", playerAvon);
        players[1] = player2;
        turns.AddLast(1);
        GameObject playerFacu = (GameObject)Instantiate(Booth, new Vector3(12 ,-3, 6.6f), Quaternion.identity);
        Player player3 = new Player("Facu", playerFacu);
        players[2] = player3;  
        turns.AddLast(2);
        GameObject playerGuille = (GameObject)Instantiate(Booth, new Vector3(16, -3, 6.6f), Quaternion.identity);
        Player player4 = new Player("Guille", playerGuille);
        players[3] = player4;
        turns.AddLast(3);
        GameObject playerFede = (GameObject)Instantiate(Booth, new Vector3(20, -3, 6.6f), Quaternion.identity);
        Player player5 = new Player("Fede", playerFede);
        players[4] = player5;
        turns.AddLast(4);

        turn = turns.First;       
        //genero el primer tablero 
        Card[] cardDeck = new CardDeck(csvFile).GetDeck();
        new Board(Card, cardDeck,level);
        
    }

    void Update()
    {        
        GameObject.Find("GUIPlayersTurn/TurnText").GetComponent<TextMesh>().text = turn.Value.ToString();
        if (level != 3)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Levanto el objeto sobre el cual clickea y actuo en base a eso        
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);

                if (hit.transform.parent.gameObject.name.Contains("card"))
                {
                    //Coordenadas para que la tarjeta se vea centrada en el frente
                    Vector3 frontPosition = new Vector3(5f, 2.3f, -6.1f);
                    chosenCard = hit.transform.parent.gameObject;


                    //Muestro la pista y seteo el timer y el valor a mostrar
                    if (hit.transform.gameObject.name.Contains("Points") && !activeTurn)
                    {
                        //evito que elija otra carta del tablero
                        activeTurn = true;
                        cardValue = hit.transform.FindChild("PointsText").gameObject.GetComponent<TextMesh>().text;
                        //Seteo el valor a mostrar de los puntos                    
                        GameObject.Find("GUIPoints/PointsText").GetComponent<TextMesh>().text = cardValue;
                        //muevo la pregunta adelante
                        hit.transform.parent.gameObject.transform.position = frontPosition;

                        Destroy(hit.transform.gameObject);
                                                
                        //Comienza el conteo de los 10 segundos             
                        StartCoroutine("cardTimer");
                        //Habilita las visualizaciones de los GUI
                        StartCoroutine(rotateCard(GameObject.Find("GUITimer")));
                        StartCoroutine(rotateCard(GameObject.Find("GUIPoints")));
                        StartCoroutine(rotateCard(GameObject.Find("GUIAnswerOk")));
                        StartCoroutine(rotateCard(GameObject.Find("GUIAnswerNotOk")));

                    }
                    /*Muestro la respuesta
                    if (hit.transform.gameObject.name.Contains("Front"))
                    {
                        StartCoroutine(rotateCard(hit.transform.parent.gameObject));                        
                        StopCoroutine("cardTimer");
                    }*/
                }



                //responde bien
                if (hit.transform.parent.gameObject.name.Contains("AnswerOk"))
                {                    
                    cardValue = GameObject.Find("GUIPoints/PointsText").GetComponent<TextMesh>().text;
                    //Se le suman los puntos al jugador
                    players[turn.Value].SetPoints(int.Parse(cardValue));

                    StartCoroutine(rotateCard(GameObject.Find("GUITimer")));
                    StartCoroutine(rotateCard(GameObject.Find("GUIPoints")));
                    StartCoroutine(rotateCard(GameObject.Find("GUIAnswerOk")));
                    StartCoroutine(rotateCard(GameObject.Find("GUIAnswerNotOk")));
                    StartCoroutine(rotateCard(chosenCard));
                    Destroy(chosenCard,5);
                    
                    StopCoroutine("cardTImer");
                    activeTurn = false;
                }

                //responde mal
                if (hit.transform.parent.gameObject.name.Contains("AnswerNotOk"))
                {

                    if (selectPlayer)
                    {
                        //Ningun equipo pudo responder
                        //StartCoroutine(rotateCard(GameObject.Find("GUIAnswerOk")));
                        StartCoroutine(rotateCard(GameObject.Find("GUIAnswerNotOk")));
                        StartCoroutine(rotateCard(GameObject.Find("GUITimer")));
                        StartCoroutine(rotateCard(GameObject.Find("GUIPoints")));
                        StartCoroutine(rotateCard(chosenCard));
                        selectPlayer = false;
                        activeTurn = false;
                        Destroy(chosenCard,5);
                        //pasa al siguiente turno
                        if (turn.Next != null)
                        {
                            turn = turn.Next;
                        }
                        else
                            turn = turns.First;
                    }
                    else
                    {                       
                        //Se le restan los puntos y se habilita a los otros jugadores a responder
                        selectPlayer = true;
                        cardValue = GameObject.Find("GUIPoints/PointsText").GetComponent<TextMesh>().text;
                        players[turn.Value].SetPoints(int.Parse(cardValue) * -1);
                        StartCoroutine(rotateCard(GameObject.Find("GUIAnswerOk")));
                        StopCoroutine("cardTimer");
                    }
                }
                //Algun equipo responde bien la pregunta
                if (selectPlayer)
                {
                    Debug.Log("Select Player");
                    //Elije el equipo que respondio bien        
                    if (hit.transform.gameObject.name.Contains("PlayerFront"))
                    {
                        //Asigna el turno siguiente al equipo elegido         
                        for (int i = 0; i < players.Length; i++)
                        {
                            if (players[i].Name() == hit.transform.parent.gameObject.name)
                            {
                                for (LinkedListNode<int> node = turns.First; node != null; node = node.Next)
                                {
                                    if (node.Value == i)
                                        turn = node;
                                }
                                break;
                            }
                        }
                        //suma los puntos al equipo elegido
                        players[turn.Value].SetPoints(int.Parse(cardValue));
                        //gira las tarjetas de GUI para que no se vean                    
                        StartCoroutine(rotateCard(GameObject.Find("GUIAnswerNotOk")));
                        StartCoroutine(rotateCard(GameObject.Find("GUITimer")));
                        StartCoroutine(rotateCard(GameObject.Find("GUIPoints")));
                        StartCoroutine(rotateCard(chosenCard));
                        //habilita las selecciones y saca la carta de la pregunta
                        selectPlayer = false;
                        activeTurn = false;
                        Destroy(chosenCard,5);
                    }
                }
            }
        }else
        {                        
            if (Input.GetMouseButtonDown(0))
            {
                //Levanto el objeto sobre el cual clickea y actuo en base a eso        
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);

                if (hit.transform.parent.gameObject.name.Contains("card"))
                {
                    //Coordenadas para que la tarjeta se vea centrada en el frente
                    Vector3 frontPosition = new Vector3(5f, 2.3f, -6.1f);
                    chosenCard = hit.transform.parent.gameObject;


                    //Muestro la pista y seteo el timer y el valor a mostrar
                    if (hit.transform.gameObject.name.Contains("Points") && !activeTurn)
                    {
                        //Muestro todos los botones de respuesta
                        foreach (Player player in players)
                        {
                            StartCoroutine(rotateCard(GameObject.Find(player.Name()).transform.FindChild("AnswerOk").gameObject));
                            StartCoroutine(rotateCard(GameObject.Find(player.Name()).transform.FindChild("AnswerNotOk").gameObject));
                        }
                        //evito que elija otra carta del tablero
                        activeTurn = true;                        
                        Destroy(hit.transform.gameObject);
                        hit.transform.parent.gameObject.transform.position = frontPosition;                                                
                        //Comienza el conteo de los 30 segundos             
                        StartCoroutine("cardTimer");
                        //Habilita las visualizaciones de los GUI
                        StartCoroutine(rotateCard(GameObject.Find("GUITimer")));                                               
                    }
                    //Muestro la respuesta
                    if (hit.transform.gameObject.name.Contains("Front"))
                    {
                        StartCoroutine(rotateCard(hit.transform.parent.gameObject));                                                
                    }
                }
                //responde bien
                if (hit.transform.parent.gameObject.name.Contains("AnswerOk"))
                {
                    string playerName = hit.transform.parent.parent.gameObject.name;
                    for(int i= 0; i< players.Length;i++)
                    {
                        if(players[i].Name() == playerName)
                        {
                            players[i].SetPoints(int.Parse(bet[i]));
                            StartCoroutine(rotateCard(GameObject.Find(players[i].Name()).transform.FindChild("AnswerOk").gameObject));
                            StartCoroutine(rotateCard(GameObject.Find(players[i].Name()).transform.FindChild("AnswerNotOk").gameObject));
                            break;
                        }
                    }
                    //Se le suman los puntos al jugador                    
                    activeTurn = false;
                }

                //responde mal
                if (hit.transform.parent.gameObject.name.Contains("AnswerNotOk"))
                {
                    string playerName = hit.transform.parent.parent.gameObject.name;
                    for (int i = 0; i < players.Length; i++)
                    {
                        if (players[i].Name() == playerName)
                        {
                            players[i].SetPoints(int.Parse("-"+bet[i]));
                            StartCoroutine(rotateCard(GameObject.Find(players[i].Name()).transform.FindChild("AnswerOk").gameObject));
                            StartCoroutine(rotateCard(GameObject.Find(players[i].Name()).transform.FindChild("AnswerNotOk").gameObject));
                            break;
                        }
                    }
                    //Se le suman los puntos al jugador                    
                    activeTurn = false;
                }                
            }
        }               
        //chequea que no haya mas cartas en la escena para pasar de nivel
        if (GameObject.FindGameObjectsWithTag("Card").Length == 0)
        {
            level += 1;
            //Asigna el turno siguiente al equipo elegido             
            if (level == 3)
            {
                //borra los equipos con puntaje negativo        
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].GetPoints() <= 0)
                    {
                        for (LinkedListNode<int> node = turns.First; node != null; node = node.Next)
                        {
                            if (node.Value == i)
                            {
                                turns.Remove(node.Value);
                                Destroy(GameObject.Find(players[i].Name()));
                            }
                        }
                        break;
                    }
                }
                if (GameObject.FindGameObjectsWithTag("Team").Length >= 2)
                {
                    Card[] cardDeck = new CardDeck(csvFile).GetDeck();
                    new Board(Card, cardDeck, level);
                }else
                {
                    //pantalla Final
                }
                
            }
            else
            {
                Card[] cardDeck = new CardDeck(csvFile).GetDeck();
                new Board(Card, cardDeck, level);
                int aux;
                int min = 0;
                aux = players[0].GetPoints();
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].GetPoints() < aux)
                    {
                        min = i;
                    }
                }
                for (LinkedListNode<int> node = turns.First; node != null; node = node.Next)
                {
                    if (players[node.Value].GetPoints() == players[min].GetPoints())
                        turn = node;
                }
            }                   
        }
        //Chequea la flag del timer para habilitar la pregunta a todos
        if (timerFlag)
        {
            Debug.Log("Timer flag");
            cardValue = GameObject.Find("GUIPoints/PointsText").GetComponent<TextMesh>().text;
            players[turn.Value].SetPoints(int.Parse(cardValue) * -1);
            //StartCoroutine(rotateCard(GameObject.Find("GUIAnswerOk")));
            timerFlag = false;
            selectPlayer = true;
        }        
    }
    
    void OnGUI()
    {
        int offset = 30;
        if (level == 3)
        {
            switch (GameObject.FindGameObjectsWithTag("Team").Length)
            {
                case 2:
                  bet[0] =  GUI.TextField(new Rect(10, 10, 200, 20), bet[0], 25);
                  bet[1] = GUI.TextField(new Rect(10, 10 + offset, 200, 20), bet[1], 25);
                    break;
                case 3:
                    bet[0] = GUI.TextField(new Rect(10, 10, 200, 20), bet[0], 25);
                    bet[1] = GUI.TextField(new Rect(10, 10 + offset, 200, 20), bet[1], 25);
                    bet[2] = GUI.TextField(new Rect(10, 10 + offset*2, 200, 20), bet[2], 25);
                    break;
                case 4:
                    bet[0] = GUI.TextField(new Rect(10, 10, 200, 20), bet[0], 25);
                    bet[1] = GUI.TextField(new Rect(10, 10 + offset, 200, 20), bet[1], 25);
                    bet[2] = GUI.TextField(new Rect(10, 10 + offset*2, 200, 20), bet[2], 25);
                    bet[3] = GUI.TextField(new Rect(10, 10 + offset*3, 200, 20), bet[3], 25);
                    break;
                case 5:
                    bet[0] = GUI.TextField(new Rect(10, 10, 200, 20), bet[0], 25);
                    bet[1] = GUI.TextField(new Rect(10, 10 + offset, 200, 20), bet[1], 25);
                    bet[2] = GUI.TextField(new Rect(10, 10 + offset*2, 200, 20), bet[2], 25);
                    bet[3] = GUI.TextField(new Rect(10, 10 + offset*3, 200, 20), bet[3], 25);
                    bet[4] = GUI.TextField(new Rect(10, 10 + offset*4, 200, 20), bet[4], 25);
                    break;
            }                    
            createInput = false;
        }               
    }
    //gira el GameObject 180 grados
    private IEnumerator rotateCard(GameObject go)
    {      
        float seconds = 0.001f;
        for (int i = 1; i <= 45; i++)
        {
            yield return new WaitForSeconds(seconds);
            go.transform.Rotate(Vector3.up * 4.0f);
        }
    }
    //funcion que cuenta los 10 segundos de cuando elijen la pregunta
    private IEnumerator cardTimer()
    {
        int levelTime = 10;

        if (level == 3)
            levelTime = 30;

        GameObject.Find("GUITimer/TimerText").GetComponent<TextMesh>().text = levelTime.ToString();
        for (int time = levelTime; time >= 0; time--)
        {
            yield return new WaitForSeconds(1);            
           GameObject.Find("GUITimer/TimerText").GetComponent<TextMesh>().text = time.ToString();            
        }
        timerFlag = true;               
    }
}
