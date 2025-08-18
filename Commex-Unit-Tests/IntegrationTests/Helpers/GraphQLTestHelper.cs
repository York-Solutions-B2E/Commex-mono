using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Commex_Unit_Tests.IntegrationTests.Helpers
{
    public static class GraphQLTestHelper
    {
        public static StringContent CreateGraphQLRequest(string query, object? variables = null)
        {
            var request = new
            {
                query = query,
                variables = variables
            };

            var json = JsonSerializer.Serialize(request);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        public static async Task<T> DeserializeGraphQLResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var result = JsonSerializer.Deserialize<GraphQLResponse<T>>(content, options);
            return result!.Data;
        }

        public static string IntrospectionQuery => @"
            query IntrospectionQuery {
                __schema {
                    queryType { name }
                    mutationType { name }
                    subscriptionType { name }
                    types {
                        name
                        kind
                        description
                    }
                }
            }";

        public static string HealthQuery => @"
            query {
                health
            }";

        public static string CommunicationsQuery => @"
            query GetCommunications($first: Int, $after: String) {
                communications(first: $first, after: $after) {
                    edges {
                        cursor
                        node {
                            id
                            trackingNumber
                            recipientName
                            currentStatus
                            lastUpdatedUtc
                        }
                    }
                    pageInfo {
                        hasNextPage
                        hasPreviousPage
                        startCursor
                        endCursor
                    }
                    totalCount
                }
            }";
    }

    public class GraphQLResponse<T>
    {
        public T Data { get; set; } = default!;
        public GraphQLError[]? Errors { get; set; }
    }

    public class GraphQLError
    {
        public string Message { get; set; } = string.Empty;
        public string[]? Path { get; set; }
        public object? Extensions { get; set; }
    }
}