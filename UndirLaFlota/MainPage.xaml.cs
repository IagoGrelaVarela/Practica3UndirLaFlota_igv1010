using UndirLaFlota.Juego;

namespace UndirLaFlota
{
    /// <summary>
    /// Página principal del juego. Gestiona los dos tableros (jugador y enemigo),
    /// la alternancia de turnos, el marcador y el historial de partidas.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        private Tablero tabEnemigo;
        private Tablero tabJugador;
        private Button[,] botonesEnemigo = new Button[10, 10];
        private Button[,] botonesJugador  = new Button[10, 10];
        private Random random = new Random();
        private bool partidaTerminada = false;
        // Celdas pendientes de explorar tras un impacto (modo objetivo de la IA)
        private List<(int, int)> objetivosIA = new List<(int, int)>();

        // Contadores de victorias acumulados entre partidas
        private int victoriasJugador   = 0;
        private int victoriasOrdenador = 0;
        private int numeroPartida      = 0;

        // Disparos de la partida en curso (se resetean en cada nueva partida)
        private int disparosJugador   = 0;
        private int disparosOrdenador = 0;

        // Registro de resultados de partidas anteriores
        private List<string> historial = new List<string>();

        // Tamaños de los barcos hundidos en la partida en curso
        private List<int> barcosHundidosEnemigo = new List<int>(); // hundidos por el jugador
        private List<int> barcosHundidosJugador  = new List<int>(); // hundidos por el ordenador

        /// <summary>
        /// Inicializa la página, crea ambos tableros y genera los botones.
        /// </summary>
        public MainPage()
        {
            tabEnemigo = new Tablero();
            tabJugador = new Tablero();
            InitializeComponent();
            CrearTableroEnemigo();
            CrearTableroJugador();
        }

        /// <summary>
        /// Genera los botones del tablero enemigo con los barcos ocultos.
        /// Cada botón llama a <see cref="Seleccion"/> al ser pulsado.
        /// </summary>
        private void CrearTableroEnemigo()
        {
            for (int i = 0; i < tabEnemigo.Dim; i++)
            {
                for (int j = 0; j < tabEnemigo.Dim; j++)
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

                    botonesEnemigo[i, j] = button;
                    TableroGridEnemigo.Add(button, i + 1, j + 1);
                }
            }
        }

        /// <summary>
        /// Genera los botones del tablero del jugador mostrando los barcos propios.
        /// El grid tiene <c>InputTransparent="True"</c>, así que no son interactivos.
        /// </summary>
        private void CrearTableroJugador()
        {
            for (int i = 0; i < tabJugador.Dim; i++)
            {
                for (int j = 0; j < tabJugador.Dim; j++)
                {
                    // Las celdas con barco muestran "B", el agua muestra "0"
                    string txt = tabJugador.TableroList[i][j] >= 10 ? "B" : "0";

                    var button = new Button
                    {
                        Text = txt,
                        CommandParameter = new Tuple<int, int>(i, j)
                    };

                    botonesJugador[i, j] = button;
                    TableroGridJugador.Add(button, i + 1, j + 1);
                }
            }
        }

        /// <summary>
        /// Convierte coordenadas de tablero a notación legible tipo "A3".
        /// </summary>
        /// <param name="fila">Índice de fila (0–9).</param>
        /// <param name="col">Índice de columna (0–9).</param>
        /// <returns>Cadena con la letra de fila y el número de columna.</returns>
        private string FormatearCoordenada(int fila, int col)
            => $"{(char)('A' + fila)}{col + 1}";

        /// <summary>
        /// Devuelve una cadena con los tamaños de los barcos hundidos, por ejemplo "[6][3]".
        /// </summary>
        /// <param name="barcos">Lista de tamaños de barcos hundidos.</param>
        /// <returns>Cadena formateada, o "—" si no se ha hundido ninguno.</returns>
        private string FormatearBarcos(List<int> barcos)
        {
            if (barcos.Count == 0) return "—";
            var sb = new System.Text.StringBuilder();
            foreach (int tam in barcos)
                sb.Append($"[{tam}]");
            return sb.ToString();
        }

        /// <summary>
        /// Actualiza el label de barcos hundidos con el estado de la partida en curso.
        /// </summary>
        private void ActualizarContadorBarcos()
        {
            LabelBarcos.Text = $"Jugador: {FormatearBarcos(barcosHundidosEnemigo)}   " +
                               $"Enemigo: {FormatearBarcos(barcosHundidosJugador)}";
        }

        /// <summary>
        /// Registra el resultado de la partida terminada: actualiza contadores,
        /// añade una entrada al historial y refresca los elementos de la UI.
        /// </summary>
        /// <param name="ganador">Cadena que identifica al ganador: "Jugador" u "Ordenador".</param>
        private void RegistrarPartida(string ganador)
        {
            numeroPartida++;
            if (ganador == "Jugador")
                victoriasJugador++;
            else
                victoriasOrdenador++;

            string fecha   = DateTime.Now.ToString("dd/MM/yyyy");
            string entrada = $"Partida {numeroPartida} · {fecha} · {ganador} " +
                             $"({disparosJugador}j / {disparosOrdenador}o disparos)";
            historial.Add(entrada);

            LabelMarcador.Text = $"Tú: {victoriasJugador}  |  Ordenador: {victoriasOrdenador}";

            // Mostrar el historial con la partida más reciente primero
            var sb = new System.Text.StringBuilder();
            for (int i = historial.Count - 1; i >= 0; i--)
                sb.AppendLine(historial[i]);
            LabelHistorial.Text = sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Reinicia la partida en curso sin alterar el marcador ni el historial.
        /// Reutiliza los botones existentes actualizando solo su texto.
        /// </summary>
        private void NuevaPartida_Clicked(object sender, EventArgs e)
        {
            tabEnemigo = new Tablero();
            tabJugador = new Tablero();
            partidaTerminada  = false;
            disparosJugador   = 0;
            disparosOrdenador = 0;
            objetivosIA.Clear();
            barcosHundidosEnemigo.Clear();
            barcosHundidosJugador.Clear();
            ActualizarContadorBarcos();

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    botonesEnemigo[i, j].Text = "0";

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    botonesJugador[i, j].Text = tabJugador.TableroList[i][j] >= 10 ? "B" : "0";
        }

        /// <summary>
        /// Gestiona el disparo del jugador sobre el tablero enemigo.
        /// Si acierta, el jugador vuelve a disparar; si falla, cede el turno al ordenador.
        /// </summary>
        /// <param name="btn">Botón pulsado en el tablero enemigo.</param>
        /// <param name="row">Fila del disparo (0–9).</param>
        /// <param name="column">Columna del disparo (0–9).</param>
        public void Seleccion(Button btn, int row, int column)
        {
            if (partidaTerminada) return;

            string str = tabEnemigo.Jugada(row, column);
            if (str == null) return;

            disparosJugador++;

            if (str.Equals("Agua"))
            {
                btn.Text = "1";
                TurnoComputadora();
            }
            else if (str.Equals("Tocado"))
            {
                btn.Text = "3";
                // Acierto: el jugador conserva el turno
            }
            else if (str.Equals("Hundido"))
            {
                var coords = tabEnemigo.GetCoordsBarco(tabEnemigo.UltimoHundidoID);
                foreach (var (r, c) in coords)
                    botonesEnemigo[r, c].Text = "4";
                barcosHundidosEnemigo.Add(coords.Count);
                ActualizarContadorBarcos();
                DisplayAlert("Barco", "¡Barco enemigo hundido!", "OK");
                // Acierto: el jugador conserva el turno
            }
            else if (str.Equals("Partida finalizada"))
            {
                var coords = tabEnemigo.GetCoordsBarco(tabEnemigo.UltimoHundidoID);
                foreach (var (r, c) in coords)
                    botonesEnemigo[r, c].Text = "4";
                barcosHundidosEnemigo.Add(coords.Count);
                ActualizarContadorBarcos();
                partidaTerminada = true;
                RegistrarPartida("Jugador");
                DisplayAlert("Partida", "¡Has ganado!", "OK");
            }
        }

        /// <summary>
        /// Selecciona la celda que atacará el ordenador en este disparo.
        /// Si hay objetivos pendientes de un impacto anterior, los usa (modo objetivo);
        /// en caso contrario, elige una celda al azar (modo caza).
        /// </summary>
        /// <returns>Coordenadas (fila, columna) de la celda elegida.</returns>
        private (int, int) ElegirCeldaIA()
        {
            // Descartar objetivos que ya hayan sido atacados en turnos anteriores
            objetivosIA.RemoveAll(c =>
                tabJugador.TableroList[c.Item1][c.Item2] == 1 ||
                tabJugador.TableroList[c.Item1][c.Item2] == 3);

            if (objetivosIA.Count > 0)
            {
                // Modo objetivo: explorar adyacentes al último impacto
                int idx = random.Next(objetivosIA.Count);
                var celda = objetivosIA[idx];
                objetivosIA.RemoveAt(idx);
                return celda;
            }

            // Modo caza: disparo aleatorio entre las celdas sin atacar
            var disponibles = tabJugador.CeldasDisponibles();
            return disponibles[random.Next(disponibles.Count)];
        }

        /// <summary>
        /// Añade a la cola de objetivos las celdas adyacentes válidas al impacto indicado,
        /// evitando duplicados y celdas ya atacadas.
        /// </summary>
        /// <param name="x">Fila de la celda impactada.</param>
        /// <param name="y">Columna de la celda impactada.</param>
        private void AgregarObjetivosAdyacentes(int x, int y)
        {
            (int dx, int dy)[] direcciones = { (-1, 0), (1, 0), (0, -1), (0, 1) };
            foreach (var (dx, dy) in direcciones)
            {
                int nx = x + dx, ny = y + dy;
                if (nx < 0 || nx >= tabJugador.Dim || ny < 0 || ny >= tabJugador.Dim)
                    continue;

                int estado = tabJugador.TableroList[nx][ny];
                if (estado != 1 && estado != 3 && !objetivosIA.Contains((nx, ny)))
                    objetivosIA.Add((nx, ny));
            }
        }

        /// <summary>
        /// Ejecuta el turno del ordenador usando una IA de dos modos: caza y objetivo.
        /// Sigue disparando mientras acierte. Acumula todas las jugadas y las muestra
        /// en un único mensaje al ceder el turno.
        /// </summary>
        private void TurnoComputadora()
        {
            var log = new System.Text.StringBuilder();
            bool sigueJugando = true;

            while (sigueJugando && !partidaTerminada)
            {
                var (x, y) = ElegirCeldaIA();
                string coord = FormatearCoordenada(x, y);
                string str = tabJugador.Jugada(x, y);

                if (str == null) return;

                disparosOrdenador++;

                if (str.Equals("Agua"))
                {
                    botonesJugador[x, y].Text = "1";
                    log.AppendLine($"{coord} - Agua.");
                    sigueJugando = false;
                }
                else if (str.Equals("Tocado"))
                {
                    botonesJugador[x, y].Text = "3";
                    log.AppendLine($"{coord} - Tocado.");
                    AgregarObjetivosAdyacentes(x, y);
                }
                else if (str.Equals("Hundido"))
                {
                    var coords = tabJugador.GetCoordsBarco(tabJugador.UltimoHundidoID);
                    foreach (var (r, c) in coords)
                        botonesJugador[r, c].Text = "4";
                    barcosHundidosJugador.Add(coords.Count);
                    ActualizarContadorBarcos();
                    log.AppendLine($"{coord} - ¡Hundido!");
                    // Barco hundido: limpiar objetivos y volver al modo caza
                    objetivosIA.Clear();
                }
                else if (str.Equals("Partida finalizada"))
                {
                    var coords = tabJugador.GetCoordsBarco(tabJugador.UltimoHundidoID);
                    foreach (var (r, c) in coords)
                        botonesJugador[r, c].Text = "4";
                    barcosHundidosJugador.Add(coords.Count);
                    ActualizarContadorBarcos();
                    log.AppendLine($"{coord} - ¡Hundido!");
                    objetivosIA.Clear();
                    partidaTerminada = true;
                    RegistrarPartida("Ordenador");
                    sigueJugando = false;
                }
            }

            // Mostrar todas las jugadas del ordenador en un único mensaje
            string cierre = partidaTerminada ? "¡El ordenador ha ganado!" : "Tu turno.";
            DisplayAlert("Ordenador", log + cierre, "OK");
        }
    }
}
