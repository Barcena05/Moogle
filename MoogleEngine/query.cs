using System.Text.RegularExpressions;

namespace MoogleEngine
{
    
    // La clase query es la encargada de procesar la consulta introducia por el usuario con el objetivo de calcular el peso de dicha
    // consulta en cada uno de los documentos y ordenar esos valores para posteriormente devolver los documentos correspondientes en
    // orden de prioridad. Para otorgar un mayor control sobre la busqueda al usuario se implemento un mecanismo de Operadores de Busqueda
    // que modificara los documentos devueltos y el orden de prioridad que se les da.

    public class query
    {
    // La clase privada operators se encuentra dentro de la clase query y es la encargada de almacenar toda la informacion 
    // referente a la posible existencia enla consulta de dichos Operadores para luego, en funcion de ellos realizar las 
    // modificaciones pertinentes en el calculo de peso de la consulta en cada documento. La clase cuenta de cuatro campos
    // (cada una relacionada con un operador) descritos mas abajo.
    public class operators
    {
        Universe MyUniverse;
        // MustAppear es un diccionario en el cual se asocia a cada termino de la consulta que contenga el operador '^' (el cual
        // indica que el termino tiene que aparecer en todos los documentos devueltos) un valor verdadero para luego tener constancia
        // de dicha condicion especial al calcular el peso de la consulta en los documentos.
        Dictionary<string,bool> MustAppear;

        // MustNotAppear es un diccionario en el cual se asocia a cada termino de la consulta que contenga el operador '!' (el cual
        // indica que el termino no puede aparecer en ningun documento devuelto) un valor verdadero para luego tener constancia
        // de dicha condicion especial al calcular el peso de la consulta en los documentos.
        Dictionary<string,bool> MustNotAppear;

        // Importance es un diccionario en el cual se asocia a cada termino de la consulta un valor entero en correspondecia con
        // la cantiad de operadores '*' que aparezcan relacionados con el. Este operador es acumulativo, de manera que mientras mas 
        // simbolos '*' aparezcan delante del termino mayor sera su importancia.
        Dictionary<string,int> Importance;

        // Distance es un diccionario en el cual se relacionan dos terminos que en la consulta vengan "conectados" por el operador
        // de cercania: '~'. Basicamente eso sigifica que mientras mas cerca esten ambos terminos en un documento, mas relevancia tendra
        // dicho documento para la consulta.
        Dictionary<string,string> Distance;
        
        public operators(string query, Universe MyUniverse)
        {
            this.MyUniverse = MyUniverse;
            this.MustAppear = Appearences(query);
            this.MustNotAppear = MustNtAppear(query);
            this.Importance = HowImportant(query);
            this.Distance = HowClose(query);            
        }
        public Dictionary<string,bool>Aparitions
        {
            get {return MustAppear;}
        }
        public Dictionary<string,bool> NotAparitions
        {
            get{return MustNotAppear;}
        }
        public Dictionary<string, int> importance
        {
            get {return Importance;}
        }
        public Dictionary<string,string> EnclosedPairs
        {
            get {return Distance;}
        }
        
        private Dictionary<string,string> HowClose(string text)
        {
            Dictionary<string, string> result = new Dictionary<string,string>();
            for (int i = 0; i < text.Split(" ").Length; i++)
            {
                if (text.Split(" ")[i]=="~" && i != 0 && i !=text.Split(" ").Length-1)
                {
                    if (MyUniverse.GetIdf[Universe.Normalizer(text.Split(" ")[i-1])]!=0 && MyUniverse.GetIdf[Universe.Normalizer(text.Split(" ")[i+1])]!=0)
                    {
                        result.Add(Universe.Normalizer(text.Split(" ")[i-1]), Universe.Normalizer(text.Split(" ")[i+1]));
                    }                    
                }
            }
            return result;
        }
        // El metodo Appearences recibe el texo de la query previamente normalizado y tras separarlo en palabras usando el metodo
        // Split de la clase String realiza una busqueda caracter a caracter por cada palabra, de modo que cada vez que encuentre
        // el operador '^' incorporara el termino como llave en un diccionario y le hara corresponder un valor booleano verdadero.
        private Dictionary<string,bool> Appearences(string text)
        {            
            Dictionary<string,bool> result = new Dictionary<string, bool>();
            foreach (var item in text.Split(" "))
            {
                for (int i = 0; i < item.Length; i++)
                {
                    if (item[i]=='!' || item[i] =='^' || item[i]=='*')
                    {
                        if (item[i]=='^' && !result.Keys.Contains(item.Replace(" ","")))
                        {
                        result.Add(Universe.Normalizer(item).Replace(" ", ""),true);
                        }
                    }
                    else break;
                }
            }
            return result;
        }
        // El metodo MustNtAppear recibe el texo de la query previamente normalizado y tras separarlo en palabras usando el metodo
        // Split de la clase String realiza una busqueda caracter a caracter por cada palabra, de modo que cada vez que encuentre
        // el operador '!' incorporara el termino como llave en un diccionario y le hara corresponder un valor booleano verdadero.
        private Dictionary<string,bool> MustNtAppear(string text)
        {
            Dictionary<string,bool> result = new Dictionary<string, bool>();
            foreach (var item in text.Split(" "))
            {
                for (int i = 0; i < item.Length; i++)
                {
                    if (item[i]=='!' || item[i] =='^' || item[i]=='*')
                    {
                        if (item[i]=='!' && !result.Keys.Contains(item.Replace(" ","")))
                        {
                        result.Add(Universe.Normalizer(item).Replace(" ", ""),true);
                        }
                    }
                    else break;
                }
            }
            return result;
        }
         // El metodo HowImportant recibe el texo de la query previamente normalizado y tras separarlo en palabras usando el metodo
        // Split de la clase String realiza una busqueda caracter a caracter por cada palabra, de modo que  incorpore cada palabra como llave de un diccionario y le asocie un valor entero que represente su importancia
        // para la consulta. El valor que se le asociara por defecto a toda palabra sera 1 e ira aumentando en 1 a medida que se encuentrem
        // apariciones del operador '*'; de modo que si una palabra posee N veces el operador '*" se le asociara el valor entero N+1.
        private Dictionary<string,int> HowImportant(string text)
        {
            Dictionary<string,int> result = new Dictionary<string, int>();
            foreach (var item in text.Split(" ", StringSplitOptions.RemoveEmptyEntries))
            {
                
                    int count = 1;
                    for (int i = 0; i < item.Length; i++)
                    {
                        if (item[i]=='*' || item[i]=='!' || item[i]=='^')
                        {
                            if (item[i]=='*')
                            {
                                count++;
                            }
                        }
                        else break;
                    }
                    if (result.ContainsKey(Universe.Normalizer(item)) && result[Universe.Normalizer(item)]<count)
                    {
                        result[Universe.Normalizer(item)] = count;
                    }
                    else if (!result.ContainsKey(Universe.Normalizer(item).Replace(" ", "")))
                    {
                        result.Add(Universe.Normalizer(item).Replace(" ", ""), count);
                    }               
                
            }
            return result;
        }        
    }
    Universe MyUniverse;
    float[] tfidf;    
    int coincidences;
    List<string> myquery;
    int NumberOfDocuments;
    List<string> words;
    int[] rankedindexs;
    operators MyOperators;
    bool AllAppears;    
    
    public query(string text, Universe MyUniverse)
    {
        this.MyUniverse = MyUniverse;
        this.NumberOfDocuments =MyUniverse.CantDocumentos;
        this.words = MyUniverse.Words;
        this.myquery = Universe.Normalizer(text).Split(" ",StringSplitOptions.RemoveEmptyEntries).ToList();
        this.MyOperators = new operators(text, MyUniverse);
        this.tfidf = TfIdf(myquery, MyOperators);
        this.coincidences = match();   
        this.rankedindexs = rankedIndex(tfidf);  
        this.AllAppears = AppearsAll();            
    }
    
    public List<string> QueryWords
    {
        get{return myquery;}
    }
    public int[] indexes
    {
        get{return rankedindexs;}
    }
    // El metodo rankedIndex recibe como arguento el array de double que contiene los valores del peso de la consulta en cada documento
    // y crea un array de igual longitud en el cual se colocaran los indices de los documentos. Posteriormente procede a ordenar de forma 
    // descendente el array con los valores del peso de la consulta mediante un algoritmo de ordenacion por burbuja y realiza los cambios
    // perinentes al array con los indices de los documentos ordenados con el objetivo de devolver dichos indices atendiendo a la evaluacion
    // del score de los documentos.
    private int[] rankedIndex(float[] array)
    {
        int[] indexes = new int[array.Length];
        for (int i = 0; i < indexes.Length; i++)
        {
            indexes[i] = i;
        }
               
        for (int i = 0; i < array.Length; i++)
        {
            for (int j = i; j < array.Length; j++)
            {
                if (array[i]<array[j])
                {
                    
                    int b = indexes[i];
                    indexes[i] = indexes[j];
                    indexes[j] = b;
                    float a = array[i];
                    array[i] = array[j];
                    array[j] = a;
                }
            }
        }
        return indexes;
    }  
    // El metodo match devuelve la cantidad de coincidencias que ha presentado la consulta en el universo de documentos.
    private int match()
    {
        int result = 0;
        foreach (var item in tfidf)
        {
            if (item != 0)
            {
                result++;
            }
        }
        return result;
    }
    public int Coincidences
    {
        get{return coincidences;}
    }
    // El metodo TfIdf recibe como argumentos una lista con las palabras normalizadas de la consulta realizada por el usuario y los operadores
    // detectados en dicha consulta y devuelve un array de double con la suma de los valores de TfIdf de dichas palabras de la consulta
    // realizando los calculos correspondientes a los operadores encontrados.
    private float[] TfIdf(List<string> list, operators MyOperators)
    {
        float[] tfidf = new float[NumberOfDocuments];        
        foreach (var item in list)
        {            
            if (words.Contains(item))
            {            
                for (int i = 0; i < NumberOfDocuments; i++)
                {
                    tfidf[i] += MyUniverse.getTFIDF[item][i]*MyOperators.importance[Universe.Normalizer(item)]*100;
                }
            }             
        }
        for (int i = 0; i < tfidf.Length; i++)
        {
            foreach (var item in MyOperators.Aparitions.Keys)
            {
                if (!MyUniverse.Contenido[i].Split(" ").Contains(Universe.Normalizer(item)))
                {
                    tfidf[i]*= 0;
                }
            }
            foreach (var item in MyOperators.NotAparitions.Keys)
            {
                if (MyUniverse.Contenido[i].Split(" ").Contains(Universe.Normalizer(item)))
                {
                    tfidf[i]*= 0;
                }
            }
        }
        foreach (var pair in MyOperators.EnclosedPairs.Keys)
            {                
                for (int i = 0; i < NumberOfDocuments; i++)
                {                    
                    if (MyUniverse.Contenido[i].Split(" ").Contains(pair) && MyUniverse.Contenido[i].Split(" ").Contains(MyOperators.EnclosedPairs[pair]))
                    {
                        string[] words = MyUniverse.Contenido[i].Split(" ",StringSplitOptions.RemoveEmptyEntries);
                        int distance = int.MaxValue;
                        for (int j = 0; j < words.Length; j++)
                        {
                            if (words[j]==pair)
                            {
                                for (int k = 0; k < words.Length; k++)
                                {
                                    if (words[k]==MyOperators.EnclosedPairs[pair] && distance>Math.Abs(k-j))
                                    {
                                        distance = Math.Abs(k-j);
                                    }
                                    if (k-j>distance) break;
                                }
                            }
                            if (distance ==1) break;
                        }
                        tfidf[i]*=100000/distance;
                    }
                }
            } 
        return tfidf;
    }
    // Este metodo devuelve un valor booleano que indica si todas las palabras de la query 
    // aparecen en el universo de palabras.
    private bool AppearsAll()
    {
        foreach (var item in myquery)
        {
            if (!words.Contains(item))
            {
               return false; 
            }
        }
        return true;
    }
    public operators GetOperators
    {
        get{return MyOperators;}
    }
    public bool appearsAll
    {
        get{return AllAppears;}
    }
    
    public float[] Tfidf
    {
        get{return tfidf;}
    }   
    
    }
}