namespace UndirLaFlota;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

/// <summary>
/// Página de acceso. Valida las credenciales y habilita el menú lateral
/// solo si son correctas.
/// </summary>
public partial class LoginPage : ContentPage
{
    private const string UsuarioPredefinido   = "jugador";
    private const string ContrasenyaPredefinida = "1234";

    public LoginPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Valida las credenciales introducidas y, si son correctas,
    /// habilita el menú lateral y navega a la página principal del juego.
    /// </summary>
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string usuario    = UsernameEntry.Text    ?? string.Empty;
        string contrasenya = PasswordEntry.Text   ?? string.Empty;

        if (usuario == UsuarioPredefinido && contrasenya == ContrasenyaPredefinida)
        {
            Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
            await Shell.Current.GoToAsync("//MainPage");
        }
        else
        {
            await DisplayAlert("Acceso denegado", "Usuario o contraseña incorrectos.", "Cerrar");
        }
    }

    /// <summary>
	/// Solicita autenticación por huella dactilar y desbloquea la app si es correcta.
	/// </summary>
	private async void ClickedHuella(object sender, EventArgs e)
    {
        bool disponible = await CrossFingerprint.Current.IsAvailableAsync();
        if (!disponible)
        {
            await DisplayAlert("No disponible", "Este dispositivo no tiene huella dactilar configurada.", "Cerrar");
            return;
        }

        var request = new AuthenticationRequestConfiguration("Autenticación", "Autenticar con huella");
        var result = await CrossFingerprint.Current.AuthenticateAsync(request);
        if (result.Authenticated)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
        else
        {
            await DisplayAlert("Acceso", "Acceso denegado", "Cerrar");
        }
    }
}
