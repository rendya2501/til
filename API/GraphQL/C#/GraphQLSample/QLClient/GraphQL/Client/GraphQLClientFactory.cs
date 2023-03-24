using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

namespace QLClient.GraphQL.Client
{
    public class GraphQLClientFactory
    {
        private readonly string _graphQLApiUrl;

        public GraphQLClientFactory(string graphQLApiUrl)
        {
            _graphQLApiUrl = graphQLApiUrl;
        }

        public IGraphQLClient Create()
        {
            return new GraphQLHttpClient(_graphQLApiUrl, new NewtonsoftJsonSerializer());
        }
    }
}
