using UndirLaFlota.Juego;

namespace UndirLaFlota
{
    public partial class MainPage : ContentPage
    {
        private Tablero tab;
        public MainPage()
        {
            tab = new Tablero();
            InitializeComponent();

            for (int i = 0; i < tab.Dim; i++)
            {
                for (int j = 0; j < tab.Dim; j++)
                {
                    String Txt = tab.TableroList[i][j].ToString();
                    if (Txt.Equals("2"))
                    {
                        Txt = "0";
                    }
                    var button = new Button
                    {
                        Text = Txt,
                        CommandParameter = new Tuple<int, int>(i, j)
                    };

                    button.Clicked += (sender, e) =>
                    {

                        var btn = sender as Button;
                        var position = (Tuple<int, int>)btn.CommandParameter;
                        int row = position.Item1;
                        int column = position.Item2;

                        // Función que deseas ejecutar
                        Seleccion(btn, row, column);
                    };

                    TableroGrid.Add(button, i + 1, j + 1);


                }
            }
        }

        public void Seleccion(Button btn, int row, int column)
        {
            String str = tab.Jugada(row, column);
            if (str == null)
            {

            }
            else if (str.Equals("Agua"))
            {
                btn.Text = "1";
            }
            else if (str.Equals("Tocado"))
            {
                btn.Text = "3";
            }
            
            else if (str.Equals("Partida finalizada"))
            {
                DisplayAlert("Partidad", "Partida Finalizada!!", "OK");
            }
        }


    }
}