using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.Threading.Tasks;

namespace MockBit.Server.WebApi.Extensions
{
    internal static class RequestDelegateRouteCollectionExtensions
    {
        public static IRouteCollection MapVerb(
            this IRouteCollection collection,
            IInlineConstraintResolver inlineConstraintResolver,
            string verb,
            string template,
            Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            RequestDelegate requestDelegate = (httpContext) =>
            {
                return handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            };

            return collection.MapVerb(inlineConstraintResolver, verb, template, requestDelegate);
        }

        public static IRouteCollection MapVerb(
            this IRouteCollection collection,
            IInlineConstraintResolver inlineConstraintResolver,
            string verb,
            string template,
            RequestDelegate handler)
        {
            var route = new Route(
                new RouteHandler(handler),
                verb + template,
                template,
                defaults: null,
                constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint(verb) }),
                dataTokens: null,
                inlineConstraintResolver: inlineConstraintResolver);

            collection.Add(route);
            return collection;
        }
    }
}
