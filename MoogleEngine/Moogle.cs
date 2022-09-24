namespace MoogleEngine;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public static class Moogle
{   
    public static Universe MyUniverse;
    
    public static string[] names;
    
    public static SearchResult Query(string query) 
    {
        //Modifique este método para responder a la búsqueda       
        
        query x = new query(query, MyUniverse);
        string sugerencia = new sugerencia(query, MyUniverse).sugeren;
        
            if (x.Coincidences == 0)
            {
                SearchItem[] items = new SearchItem[1]
                {
                new SearchItem("no hay coincidencias", "", 10)               
                };
                return new SearchResult(items, sugerencia);
            }
            else
            {
                SearchItem[] items;
                if (x.Coincidences<10)
                {
                    items = new SearchItem [x.Coincidences];
                }
                else
                {
                    items = new SearchItem [10];
                }
                for (int i = 0; i < items.Length; i++)
                {
                    items[i] = new SearchItem(names[x.indexes[i]], new snippet(x, MyUniverse, x.indexes[i]).Getsnippet, (float) x.Tfidf[x.indexes[i]]);
                }
                return x.appearsAll? new SearchResult(items):  new SearchResult(items, sugerencia); 
                
            }        
    }
}