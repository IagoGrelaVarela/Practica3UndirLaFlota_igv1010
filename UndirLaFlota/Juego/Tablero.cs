

namespace UndirLaFlota.Juego;

/// <summary>
/// Representa un tablero de Hundir La Flota de 10×10 con lógica de colocación
/// aleatoria de barcos, detección de impactos y control de hundimientos.
/// </summary>
public class Tablero
{
    public List<List<int>> TableroList { get; set; }
    private List<int> Barcos;
    private int aciertos = 0;
    private int TotalPuntos;
    public int Dim;
    // IDs de barco empiezan en 10 para no colisionar con estados (0=agua,1=fallo,3=tocado)
    private const int baseID = 10;
    private Dictionary<int, List<(int, int)>> posicionesBarcos;

    /// <summary>ID del último barco hundido, para recuperar sus coordenadas.</summary>
    public int UltimoHundidoID { get; private set; }

    /// <summary>
    /// Inicializa un tablero nuevo con los barcos colocados aleatoriamente.
    /// </summary>
    public Tablero()
    {
        Dim = 10;
        TableroList = new List<List<int>>();
        Barcos = new List<int>();
        Barcos.Add(6);
        Barcos.Add(5);
        Barcos.Add(3);
        Barcos.Add(3);
        Barcos.Add(2);
        foreach (int i in Barcos)
            TotalPuntos += i;

        TableroLimpio();
        GenerarTablero();
    }

    /// <summary>
    /// Coloca todos los barcos en el tablero asignando un ID único a cada uno.
    /// </summary>
    public void GenerarTablero()
    {
        int barcoID = baseID;
        foreach (int i in Barcos)
        {
            GenerarBarco(i, barcoID);
            barcoID++;
        }
    }

    /// <summary>
    /// Coloca un barco de forma aleatoria sin solaparse con los ya existentes.
    /// Reintenta hasta encontrar una posición y dirección válidas.
    /// </summary>
    /// <param name="tamano">Número de celdas que ocupa el barco.</param>
    /// <param name="id">Identificador único del barco.</param>
    private void GenerarBarco(int tamano, int id)
    {
        Random random = new Random();
        int x = 0;
        int y = 0;
        int dir = 0;
        int P_x = 0;
        int P_y = 0;
        bool entra = false;
        int pos;

        while (!entra)
        {
            entra = true;
            x = random.Next(0, Dim);
            y = random.Next(0, Dim);
            dir = random.Next(0, 4);
            P_x = x;
            P_y = y;
            pos = 0;
            while (pos<tamano)
            {
                if (P_x<0 || P_x>=10 || P_y<0 || P_y>=10)
                {
                    entra = false;
                    break;
                }
                else if (TableroList[P_x][P_y] != 0)
                {
                    entra = false;
                    break;
                }

                switch (dir)
                {
                    case 0:
                        P_x++;
                        break;
                    case 1:
                        P_x--;
                        break;
                    case 2:
                        P_y++;
                        break;
                    case 3:
                        P_y--;
                        break;
                }
                pos++;
            }
        }

        // Posición válida encontrada: registrar las celdas del barco
        P_x = x;
        P_y = y;
        var coords = new List<(int, int)>();
        pos = 0;
        while (pos < tamano)
        {
            TableroList[P_x][P_y] = id;
            coords.Add((P_x, P_y));
            switch (dir)
            {
                    case 0:
                        P_x++;
                        break;
                    case 1:
                        P_x--;
                        break;
                    case 2:
                        P_y++;
                        break;
                    case 3:
                        P_y--;
                        break;
            }
            pos++;
        }
        posicionesBarcos[id] = coords;
    }

    /// <summary>
    /// Reinicia el tablero a su estado inicial con todas las celdas vacías.
    /// </summary>
    private void TableroLimpio()
    {
        aciertos = 0;
        posicionesBarcos = new Dictionary<int, List<(int, int)>>();
        TableroList = new List<List<int>>();
        for (int i = 0; i < Dim; i++)
        {
            TableroList.Add(new List<int>());
            for (int j = 0; j < Dim; j++)
                TableroList[i].Add(0);
        }
    }

    /// <summary>
    /// Devuelve las coordenadas de todas las celdas que pertenecen a un barco.
    /// </summary>
    /// <param name="id">Identificador del barco.</param>
    /// <returns>Lista de tuplas (fila, columna) que ocupa el barco.</returns>
    public List<(int, int)> GetCoordsBarco(int id) => posicionesBarcos[id];

    /// <summary>
    /// Devuelve todas las celdas que aún no han sido atacadas.
    /// </summary>
    /// <returns>Lista de tuplas (fila, columna) disponibles para disparar.</returns>
    public List<(int, int)> CeldasDisponibles()
    {
        var disponibles = new List<(int, int)>();
        for (int i = 0; i < Dim; i++)
            for (int j = 0; j < Dim; j++)
                if (TableroList[i][j] != 1 && TableroList[i][j] != 3)
                    disponibles.Add((i, j));
        return disponibles;
    }

    /// <summary>
    /// Procesa un disparo en la posición indicada y devuelve el resultado.
    /// </summary>
    /// <param name="x">Fila del disparo (0–9).</param>
    /// <param name="y">Columna del disparo (0–9).</param>
    /// <returns>
    /// "Agua" si se falla, "Tocado" si se impacta un barco,
    /// "Hundido" si el barco queda destruido, "Partida finalizada" si se gana,
    /// o <c>null</c> si la celda ya había sido atacada.
    /// </returns>
    public string Jugada(int x, int y)
    {
        int celda = TableroList[x][y];
        if (celda == 0)
        {
            TableroList[x][y] = 1;
            return "Agua";
        }
        else if (celda == 1 || celda == 3)
        {
            return null;
        }
        else if (celda >= baseID)
        {
            int barcoID = celda;
            aciertos++;
            TableroList[x][y] = 3;
            if (BarcoHundido(barcoID))
            {
                UltimoHundidoID = barcoID;
                if (aciertos >= TotalPuntos)
                    return "Partida finalizada";
                else
                    return "Hundido";
            }
            return "Tocado";
        }

        return null;
    }

    /// <summary>
    /// Comprueba si todas las celdas de un barco han sido impactadas.
    /// </summary>
    /// <param name="id">Identificador del barco.</param>
    /// <returns><c>true</c> si el barco está hundido; <c>false</c> en caso contrario.</returns>
    private bool BarcoHundido(int id)
    {
        for (int i = 0; i < Dim; i++)
            for (int j = 0; j < Dim; j++)
                if (TableroList[i][j] == id)
                    return false;
        return true;
    }
}