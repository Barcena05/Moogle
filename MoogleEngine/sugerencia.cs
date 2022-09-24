namespace MoogleEngine
{
    public class sugerencia
    {
        Universe MyUniverse;
        string sugest;
        int lenght;
        List<string> words;
        int totalDePalabras;
        
        public sugerencia(string query, Universe MyUniverse)
        {
            this.MyUniverse = MyUniverse;
            this.words = MyUniverse.Words;
            this.totalDePalabras = MyUniverse.totalWords;
            this.sugest = QuerySugest(query);
            this.lenght = sugest.Length;                  
        }
        
        public string sugeren
        {
         get{return sugest;}
        }         
        // El metodo QuerySugest recibe como un string el contenido normalizado de la query y procede a separarlo para obtener las palabras
        // Su objetivo es sumamente simple: si la palabra analizada no aparece en el Universo de Palabras se busca una sugerencia para el mismo
        // y posteriormente se vuelve a conformar un string similar a la query original donde aquellas palabras que no aparecieran en el 
        // Universo de palabras serian sustituidas por sus respectivas sugerencias.
        private string QuerySugest(string query)
        {
            List<string> mylist = query.Split(" ").ToList();
            for (int i = 0; i < mylist.Count; i++)
            {
                if (!words.Contains(mylist[i]))
                {
                    mylist[i] = sug(mylist[i]);
                }
            }
            string result = "";
            foreach (var item in mylist)
            {
                result=result + item + " ";
            }
            return result;
        }
        // El metodo sug recibe un string en representacion de una palabra. Posteriormente se procede a apoyarse en el metodo 
        // LevenshteinDistance para obtener la palabra del Universo de palabras con menor distancia de edicion respecto al termino
        // recibido; si dicha distancia es menor a cuatro entonces se devuelve la palabra encontrada, de lo contrario se devuelve
        // un string vacio.
        private string sug(string word)
        {         
            int index = 0;
            for (int i = 0; i < totalDePalabras; i++)
            {
                if (!((LevenshteinDistance(word, words[index]))<=(LevenshteinDistance(word, words[i]))))
                {
                    index = i;
                }
                if (LevenshteinDistance(word, words[index])==1)
                {
                    break;
                }               
            }
            return LevenshteinDistance(word, words[index])<=3? words[index]: "";
        }
        // El metodo LevenshteinDistance recibe dos strings y compara la llamada distancia de edicion
        // entre ambos, que no es mas que el minimo numero de cambiosnecesarios para transformar un 
        // termino en otro atendiendo a los siguientes criterios de transformacion, cada uno con un 
        // coste operacional unitario:
        // 1- Insercion de una letra
        // 2- Eliminacion de una letra
        // 3- Sustitucion de una letra por otra
        private int LevenshteinDistance(string s, string t)
        {
            int costo = 0;
            int m = s.Length;
            int n = t.Length;
            int[,] d = new int[m+1, n+1];
            if(n==0) return m;
            if (m==0) return n;
            for (int i = 0; i <= m; d[i,0]=i++);
            for (int j = 0; j <= n; d[0,j]=j++);
            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    if (s[i-1]==t[j-1])
                    {
                        costo = 0;
                    }
                    else
                    {
                        costo = 1;
                    }
                    d[i,j]=Math.Min(Math.Min(d[i-1,j]+1, d[i,j-1]+1), d[i-1, j-1] + costo);
                }
            }
            return d[m,n];
        }
    }
}