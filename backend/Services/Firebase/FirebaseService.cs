using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace Shapper.Services.Firebase;

public class FirebaseService
{
    public FirebaseApp App { get; }

    public FirebaseService(string credentialPath)
    {
        if (FirebaseApp.DefaultInstance == null)
        {
            var credential = CredentialFactory
                .FromFile<ServiceAccountCredential>(credentialPath)
                .ToGoogleCredential();

            App = FirebaseApp.Create(new AppOptions { Credential = credential });
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
