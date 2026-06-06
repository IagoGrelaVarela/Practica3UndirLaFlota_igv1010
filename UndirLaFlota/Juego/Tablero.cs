

namespace UndirLaFlota.Juego;

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
    public int UltimoHundidoID { get; private set; }

    public Tablero()
    {
        Dim = 10;
        TableroList = new List<List<int>>();
        Barcos= new List<int>();
        Barcos.Add(6);
        Barcos.Add(5);
        Barcos.Add(3);
        Barcos.Add(3);
        Barcos.Add(2);
        foreach (int i in Barcos)
        {
            TotalPuntos += i;
        }

        TableroLimpio();
        GenerarTablero();
    }

    public void GenerarTablero()
    {
        int barcoID = baseID;
        foreach (int i in Barcos)
        {
            GenerarBarco(i, barcoID);
            barcoID++;
        }
    }

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
        P_x = x;
        P_y = y;
        var coords = new List<(int, int)>();
        pos = 0;
            
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

    private void TableroLimpio()
    {
        aciertos = 0;
        posicionesBarcos = new Dictionary<int, List<(int, int)>>();
        TableroList = new List<List<int>>();
        for (int i = 0; i < Dim; i++)
        {
            TableroList.Add(new List<int>());
            for (int j = 0; j < Dim; j++)
            {
                TableroList[i].Add(0);
            }
        }
    }

    public List<(int, int)> GetCoordsBarco(int id) => posicionesBarcos[id];

    public String Jugada(int x, int y)
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

    private bool BarcoHundido(int id)
    {
        for (int i = 0; i < Dim; i++)
            for (int j = 0; j < Dim; j++)
                if (TableroList[i][j] == id)
                    return false; // Aún no ha sido hundido
        return true;
    }
}