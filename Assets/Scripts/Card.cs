using UnityEngine;
using System.Collections;

public class Card
{
    private string q;
    private string a;
    private string cat;
    private string catName;
    private string f;
    private int p;

    public Card(string category, string categoryName, string question, string answer, string fase, int points)
    {
        cat = category;
        catName = categoryName;
        q = question;
        a = answer;
        f = fase;
        p = points;
    }
    public string Category()
    {
        return cat;
    }
    public string CategoryName()
    {
        return catName;
    }
    public string Question()
    {
        return q;
    }
    public string Answer()
    {
        return a;
    }
    public string Fase()
    {
        return f;
    }
    public int Points()
    {
        return p;
    }
}
