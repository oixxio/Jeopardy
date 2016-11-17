using UnityEngine;
using System.Collections;

public class CardDeck
{
    private Card[] cards;
    public CardDeck(TextAsset file)
    {
        /*[Start] Obtiene los datos de las cartas desde el csv, 
        antes de poder levantar los datos hay que agregar al csv el ';' 
        que falta entre cada fila de datos, yo lo hice desde el sublime,
        hice esto en vez de levantar por \n y ; porq hay opciones de respuesta
        que estaban listadas con \n*/
        string[] csvText = file.text.Split(';');
        cards = new Card[csvText.Length];
        int count = 0;
        for (int i = 5; i < csvText.Length; i += 6)
        {
            cards[count] = new Card(csvText[i - 5], csvText[i - 4], csvText[i - 3], csvText[i - 2], csvText[i - 1], int.Parse(csvText[i]));
            count++;
            //Debug.Log(csvText[i - 1]);
        }        
        //[End] 
    }
    public Card[] GetDeck()
    {
        return cards;
    }
}
