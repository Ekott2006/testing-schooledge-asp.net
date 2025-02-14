using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Api.Middleware;

public static class OpenApiMiddleware
{
    public static void AddCustomOpenApiMiddleware(this IServiceCollection services)
    {
        services.AddOpenApi(option =>
        {
            option.AddDocumentTransformer(((document, context, cancellationToken) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "School Edge CBT API",
                    Description = "This is a wonderful API",
                };
                return Task.CompletedTask;
            }));

            option.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            option.CreateSchemaReferenceId = (type) =>
                type.Type.IsEnum ? null : OpenApiOptions.CreateDefaultSchemaReferenceId(type);
        });
    }

    internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
        : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            IEnumerable<AuthenticationScheme> authenticationSchemes =
                await authenticationSchemeProvider.GetAllSchemesAsync();
            if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
            {
                // Add the security scheme at the document level
                Dictionary<string, OpenApiSecurityScheme> requirements = new()
                {
                    ["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer", // "bearer" refers to the header name here
                        In = ParameterLocation.Header,
                        BearerFormat = "Json Web Token"
                    }
                };
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes = requirements;

                // Apply it as a requirement for all operations
                foreach (KeyValuePair<OperationType, OpenApiOperation> operation in document.Paths.Values.SelectMany(
                             path =>
                                 path.Operations))
                {
                    operation.Value.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecurityScheme { Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme } }] =
                            Array.Empty<string>()
                    });
                }
            }
        }
    }
}