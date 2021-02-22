# Welcome to Sinapse.Framework.Web.Http !

Sous ce namespace se cachent des classes utiles à la communication HTTP.

##Corrélation
Avec Sinapse il est possible d'intégrer des corrélation IDs aux APIs afin de mieux tracer les erreurs.

###Utilisation
Afin d'utiliser la corrélation d'IDs dans une API, il y a plusieurs choses à mettre en place.

####Prérequis projet
Dans votre projet, il faudra définir une classe de configuration dérivant de `IHttpClientConfiguration` pour pouvoir utiliser un client HTTP nommé.
Une fois la classe créée, assignez une valeur à `HttpClientName`. Celle-ci peut venir d'un fichier de configuration ou d'une constante, peu importe.

####Prérequis Startup
Dans le fichier Startup il faut ajouter le code suivant :

``` csharp
// Remplacer DefaultHttpClientConfiguration par la classe que vous avez créé dans l'étape 'Prérequis projet'.
var myConfig = new DefaultHttpClientConfiguration()
services.AddHttpCorrelationMiddleware<DefaultHttpClientConfiguration>(myConfig); // L'extension se trouve dans Sogetrel.Sinapse.Framework.Web.Http.Extensions
```