using UndirLaFlota.Juego;

namespace UndirLaFlota
{
    public partial class MainPage : ContentPage
    {
        private Tablero tab;
        private Button[,] botones = new Button[10, 10];

        public MainPage()
        {
            tab = new Tablero();
            InitializeComponent();
            CrearTablero();
        }

        private void CrearTablero()
        {
            for (int i = 0; i < tab.Dim; i++)
            {
                for (int j = 0; j < tab.Dim; j++)
                {
                    var button = new Button
                    {
                        Text = "0",
                        CommandParameter = new Tuple<int, int>(i, j)
                    };

                    button.Clicked += (sender, e) =>
                    {
                        var btn = sender as Button;
                        var position = (Tuple<int, int>)btn.CommandParameter;
                        Seleccion(btn, position.Item1, position.Item2);
                    };

                    botones[i, j] = button;
                    TableroGrid.Add(button, i + 1, j + 1);
                }
            }
        }

        public void Seleccion(Button btn, int row, int column)
        {
            string str = tab.Jugada(row, column);
            if (str == null) return;

            if (str.Equals("Agua"))
            {
                btn.Text = "1";
            }
            else if (str.Equals("Tocado"))
            {
                btn.Text = "3";
            }
            else if (str.Equals("Hundido"))
            {
                foreach (var (r, c) in tab.GetCoordsBarco(tab.UltimoHundidoID))
                    botones[r, c].Text = "4";
                DisplayAlert("Barco", "Barco Hundido!!", "OK");
            }
            else if (str.Equals("Partida finalizada"))
            {
                foreach (var (r, c) in tab.GetCoordsBarco(tab.UltimoHundidoID))
                    botones[r, c].Text = "4";
                DisplayAlert("Partida", "Partida Finalizada!!", "OK");
            }
        }
    }
}