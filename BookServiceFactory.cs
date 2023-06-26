using Google.Apis.Auth.OAuth2;
using Google.Apis.Books.v1;
using Google.Apis.Services;

namespace MyBooks;

public static class BookServiceFactory
{
    public static BooksService Initialize()
    {
        // Set up the credentials
        var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            new ClientSecrets
            {
                ClientId = "32590108289-r2lfcelsb9e57h5ekkeri7mr62cqtqhi.apps.googleusercontent.com",
                ClientSecret = "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"
            },
            new[] { BooksService.Scope.Books },
            "user",
            CancellationToken.None).Result;

        // Create the Books API service
        var service = new BooksService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "MyBooks"
        });

        return service;
    }
}