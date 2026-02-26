using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

public class FirebaseService
{
    public FirebaseApp App { get; }

    public FirebaseService(string credentialPath)
    {
        // Solo crea la app si no existe
        if (FirebaseApp.DefaultInstance == null)
        {
            App = FirebaseApp.Create(
                new AppOptions { Credential = GoogleCredential.FromFile(credentialPath) }
            );
        }
        else
        {
            App = FirebaseApp.DefaultInstance;
        }
    }

    public async Task<FirebaseToken> VerifyTokenAsync(string idToken)
    {
        return await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
    }
}
