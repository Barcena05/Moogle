using System.Text.RegularExpressions;
namespace MoogleEngine
{
    // La clase snippet es la encargada de devolver basicamente el fragmento de un documento determinado
    // que mas se ajuste a la consulta introducida por el usuario.
    public class snippet
    {
    Universe MyUniverse;
    query MyQuery;
    string[] contenido;
    float[] values;
    int indice;
    string Mysnippet;
    
    
    public snippet(query MyQuery, Universe MyUniverse, int index)
    {
        this.MyQuery = MyQuery;
        this.MyUniverse = MyUniverse;     
        // El campo contenido consiste en el texto indicado sin normalizar SIN NORMALIZAR, cargado mediante
        // el metodo myContent y representado mediante un array de string (cada palabra en una posicion de dicho array).
        this.contenido = myContent(index);
        this.indice = index;
        // El campo values consiste en un array de float donde cada posicion corresponde al TfIdf de cada palabra
        // del texto que se esta analizando. Dichos valores se incrementaran sustancialmente si la palabra
        // aparece en la consulta.
        this.values = valores(contenido, index);
        // El campo MySnippet consiste precisamente en el fragmento del texto que mas se ajusta a la consulta
        // realizada por el usuario.
        this.Mysnippet = GetSnippet(sumamax(values, 50));              
    }
    public string Getsnippet
    {
        get {return Mysnippet;}
    }
    // El metodo myContent hace uso de los metodos ReadAllLines de la clase File y Join de la clase String para concatenar 
    // esas lineas cargadas en una sola cadena de caracteres, representando asi el texto sin normalizar. Finalmente mediante el 
    // metodo Split de la clase String cambia dicha representacion a un array de string.
    private string[] myContent(int index)
    {
        string text = string.Join(" ", File.ReadAllLines(MyUniverse.Directions[index]));        
        return text.Split(" ");
    }
    // El metodo valores recibe como parametros un array de string con las palabras del texto sin normalizar y el indice
    // correspondiente a dicho contenido. Posteriormente procede a asignarle a cada una de esas palabras su valor de TfIdf en ese documento
    // representados en un array de float de longitud igual a la cantidad de palabras que posee el array recibido y devuelve dicho
    // array de valores.
    private float[] valores(string[] contenido, int index)
    {
        float[] result = new float[contenido.Length];
        string[] words = new string[contenido.Length];
        for (int i = 0; i < contenido.Length; i++)
        {
            words[i] = Universe.Normalizer(contenido[i]).Replace(" ", "");
        }
        for (int i = 0; i < words.Length; i++)
        {
            
                if (MyQuery.QueryWords.Contains(words[i]))
                {
                    result[i] = MyUniverse.getTFIDF[words[i]][index]*100000000;
                }
                else
                {
                    try
                    {
                        result[i] = MyUniverse.getTFIDF[words[i]][index];
                    }
                    catch (System.Collections.Generic.KeyNotFoundException)
                    {
                        result[i] = 0;                        
                    }
                
                }
            
        }
        return result;
    }
    // El metodo sumamax recibe como argumentos un array de float con los valores de TfIdf de las palabras del texto y un valor entero
    // "longi". El metodo basicamente se encarga de obtener el subarray de suma maxima de longitud "longi" dentro del antes mencionado
    // array de float, que se correspondera con el fragmento del texto que mejor se ajuste a la consulta realizada por el usuario.
    // Posteriormente creara un array de string de longitud "longi" y almacenara en el las palabras correspondientes a los valores
    // encontrados y almacenados en dicho subarray de suma maxima (las palabras del texto que mejor coincidan con la consulta) y 
    // devolvera dicho array de string.
    private string[] sumamax(float[] array, int longi)
    {
        if (array.Length<longi)
        {
            longi=array.Length;
        }
        int start = 0;
        int end = longi;
        double sum = 0;
        double sump = 0;
    
        for (int i = 0; i <= array.Length-longi; i++)
        {
            for (int j = i; j < i+longi; j++)
            {
                sump += array[j];
            }
            if (sump>sum)
            {
            sum = sump;
            start = i;
            end = i+longi;
            }
            sump = 0;
        }
        string[] result = new string[longi];
        for (int i = 0; i < longi; i++)
        {
            result[i] = contenido[start+i];
        }
        return result;
    }
    // El metodo GetSnippet recibe un array de string con las palabras correspondientes al fragmento de texto que mas se ajusta a
    // la consulta realizada por el usuario y se encarga de representar dichas palabras como una sola cadena de caracteres con el
    // objetivo de obtener dicho fragmento de texto.
    private string GetSnippet(string[] array)
    {
        string result = "";
        foreach (var item in array)
        {
            result += item + " ";
        }
        return result;
    }
    
    }
}