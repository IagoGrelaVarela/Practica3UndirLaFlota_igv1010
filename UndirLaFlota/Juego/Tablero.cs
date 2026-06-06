

namespace UndirLaFlota.Juego;

public class Tablero
{
    public List<List<int>> TableroList { get; set; }
    private List<int> Barcos;
    private int aciertos = 0;
    private int TotalPuntos;
    public int Dim;
    

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
        // ID único para cada barco, para detectar cuando se hunden
        int barcoID = 2;
        foreach (int i in Barcos)
        {
            GenerarBarco(i, barcoID);
            barcoID++;
        }
    }

    private void GenerarBarco(int tamano, int id)
    {
        Random random = new Random();
        int x=0; 
        int y=0; 
        int dir=0;
        int P_x=0;
        int P_y=0;
        bool entra = false;
        int pos;
        while (!entra)
        {
            entra = true;
            x = random.Next(0, Dim); 
            y = random.Next(0, Dim); 
            dir = random.Next(0, 3);
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
        if (entra)
        {
            pos = 0;
            
            while (pos<tamano)
            {
                TableroList[P_x][P_y] = id;
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
    }

    private void TableroLimpio()
    {
        aciertos = 0;
        for (int i = 0; i < Dim; i++)
        {
            TableroList.Add(new List<int>());
            for (int j = 0; j < Dim; j++)
            {
                TableroList[i].Add(0);
            }
        }
    }

    public String Jugada(int x, int y)
    {
        if (TableroList[x][y] == 0)
        {
            TableroList[x][y] = 1;
            return "Agua";
        }else if (TableroList[x][y] == 1 || TableroList[x][y] == 3)
        {
            return null;
        }
        else if (TableroList[x][y] == 2)
        {
            int barcoID = TableroList[x][y];
            aciertos ++;
            TableroList[x][y] = 3;
            if (BarcoHundido(barcoID))
            {
                if (aciertos >= TotalPuntos)
            {
                return "Partida finalizada";
            }
                else
                {
                    return "Hundido";
                }
            }
            return "Tocado";
        }

       

        return null;
    }

    private bool BarcoHundido(int id)
    {
        for (int i = 0; i < Dim; i++)
        {
            for (int j = 0; j < Dim; j++)
            {
                if (TableroList[i][j] == id)
                {
                    return false; // Aún no ha sido hundido
                }
            }
        }
        return true;
    }
}