using System.Text.RegularExpressions;
namespace MoogleEngine

{
    public class Universe
    {
        
    string MainDirection;
    string[] directions;
    string[] names;
    string[] contenido;
    
    Dictionary<string,float> idfs;
    Dictionary<string,float[]>TfIdfs;

    // La clase Universe constituye una fuente de almacenamiento de informacion en las cuales quedaran 
    // guardados los datos que se obtengan en pre-procesado (como las direcciones de los documentos, sus nombres,
    // los contenidos de dichos documentos despues de ser "normalizados", y los valores de TfIdf de cada palabra en 
    // cada documento); para luego ser utilizados en posteriores ocaciones.
    public Universe()
    {
        this.MainDirection = @"../Content";
        // esta es la direccion de la carpeta Content, donde se encuentran los archivos de texto.
        this.directions = Directory.GetFiles(@MainDirection, "*txt", SearchOption.AllDirectories);
        // en este array de string se almacenan las direcciones de todos los archivos '.txt'.
        this.names = Namess(directions);
        // en este array de string se almacenan los nombres de los archivos antes mencionados; respetando el orden en el q fueron almacenadas sus direcciones.
        this.contenido = contenidos(directions);
        // en este array de string se almacenan los contenidos YA NORMALIZADOS de los archivos de texto(completamente en minusculas y conteniendo solo letras Y/o numeros)
        this.idfs = getIdfs(contenido);  
        // en este diccionario se asocia a cada palabra existente en los archivos de texto su correspondiente peso con respecto al universo de documentos( inverse document frequency o IDF)
        this.TfIdfs = GetTfIdf(contenido, idfs);   
        // por ultimo en este campo se tiene un diccionario en el cual a cada palabra existente en los archivos de texto se le asocia un array de float en el cual se almacenan los valores del
        // peso de dicha palabra en ese documento multiplicado por su correspondiente valor de IDF; lo cual nos otorgaria el peso del termino en el documento con respecto a su peso en el 
        // universo de documentos (TF-Idf).
    }
    
    public string[] Directions
    {
        get{return directions;}
    }
    
    public int CantDocumentos
    {
        get{return directions.Length;}
    }

    public string[] Names
    {
        get{return names;}
    }
    public string[] Contenido
    {
        get{return contenido;}
    }
    public List<string> Words
    {
        get{return idfs.Keys.ToList();}
    }
    public int totalWords
    {
        get{return idfs.Keys.Count();}
    }
    public Dictionary<string, float> GetIdf
    {
        get{return idfs;}
    }
    
    public Dictionary<string, float[]> getTFIDF
    {
        get{return TfIdfs;}
    }
    // El metodo Namess retorna un array de string con los nombres de los documentos haciendo uso del metodo Substring
    // de la clase STring.
    private string[] Namess(string[] a) 
    {
        string[] b = new string[a.Length];
        for (int i = 0; i < a.Length; i++)
        {
            b[i] = Regex.Replace(a[i].Substring(MainDirection.Length + 1), ".txt", "");
        }
        return b;
    }   
    // El metodo Normalizer recibe una cadena de caracteres y reemplaza en ella todos los caracteres que no sean letras o numeros por un 
    // espacio en blanco; de igual manera que elimina las tildes en las vocales (reemplazandolas por vocales sin tilde) y convierte
    // todo el texto a minusculas.
    public static string Normalizer(string entry)
    {
        string result;        
        result = Regex.Replace(entry.ToLower(),"á", "a")
        .Replace("é", "e")
        .Replace("í", "i")
        .Replace("ó", "o")
        .Replace("ú", "u")        
        .Replace("ñ", "n")
        .Replace("\n", " ");
        return Regex
            .Replace(result,@"[^a-z0-9 ]+","");        
    }
    // EL metodo contenidos devuelve un array de string con el contenido de todos los documentos de texto NORMALIZADOS, para esto
    // se cargan dichos contenidos como una cadena de caracteres usando el metodo ReadAllText de la calase File y luego dicho texto
    // es transformado mediante el metodo Normalizer.
    private string[] contenidos(string[] directionArchivo)
    {   
        string[] content = new string[directionArchivo.Length];      
        for (int i = 0; i < content.Length; i++)
        {            
            string arch = Normalizer(File.ReadAllText(directionArchivo[i]));            
            content[i] = arch;
        }
        System.Console.WriteLine("Content Ok");
        return content;
    }
    // El metodo getIdfs se encarga de relacionar cada palabra con su peso en relacion con el universo de documentos mediante un 
    // diccionario. Para garantizar la obtencion de un valor que represente adecuadamente este peso se emplea el calculo del Idf
    // (inverse document frequency) medianle la formula siguiente: Log10(cantDocumentos/cantApariciones); de esta manera el termino
    // aumenta su peso a medida que aumenta la catidad de documentos en los que aparece pero se vuelve cero si aparece en todos los
    // documentos(preposiciones, conjunciones y otros terminos extremadamente comunes)
    private Dictionary<string,float> getIdfs(string[] cont)
    {
        Dictionary<string,float> MyIdfs = new Dictionary<string, float>();  
        Dictionary<string,int> Aux = new Dictionary<string, int>();      
        for (int i = 0; i < cont.Length; i++)
        {
            foreach (var item in cont[i].Split(" ").ToList())
            {                
                if (!MyIdfs.ContainsKey(item))
                {
                    MyIdfs.Add(item,1);
                    Aux.Add(item,i);
                }
                else if(Aux[item]!=i)
                {
                    MyIdfs[item]+=1;
                    Aux[item]=i;
                }

            }
        }
        foreach (var item in MyIdfs.Keys)
        {
            MyIdfs[item] = (float)Math.Log10(cont.Length/MyIdfs[item]);
        }
        System.Console.WriteLine("Idfs OK");
        return MyIdfs;
    }
    // El metodo GetIdf garantiza la obtencion de un diccionario en el cual se asociara cada palabra de los documentos con su
    // peso en cada documento en relacion con el universo de documentos. El calculo de dicho valor necesita previamente de la 
    // obtencio del Tf(term frequency) de los terminos en cada documento. A diferencia del valor del Idf, el cual es unico para
    // cada termico, el Tf representa el peso de un termino en un documento determinado; por lo cual sea N la cantidad de documentos
    // en nuestro universo, cada terino tendra N valores  de Tf. La formula para calcular el Tf es aun mas simple que la del Idf
    //  pues consiste en obtener la razon entre la cantidad de apariciones del termino en el documento y la cantidad total de palabras
    // de dicho documento(numApariciones/totalPalabras). Finalmente para obtener el Tf-Idf (el cual es propio del termino en cada documento)
    // se multiplican estos valores de Tf por el Idf correspondiente al termino.
    private Dictionary<string, float[]> GetTfIdf(string[] cont, Dictionary<string,float> idfs)
    {
        Dictionary<string, float[]> TfIdf = new Dictionary<string, float[]>();
        for (int i = 0; i < cont.Length; i++)
        {
            foreach (var item in cont[i].Split(" ").ToList())
            {
                if (!TfIdf.ContainsKey(item))
                {
                    TfIdf.Add(item, new float[cont.Length]);
                }
                TfIdf[item][i] ++;
            }            
        }
        foreach (var item in TfIdf.Keys)
        {
            for (var i = 0; i < contenido.Length; i++)
            {                
                TfIdf[item][i] = (TfIdf[item][i]/cont[i].Length) * idfs[item];            
            }
        }
        System.Console.WriteLine("TfIdf OK");
        return TfIdf;
    }
    }
}