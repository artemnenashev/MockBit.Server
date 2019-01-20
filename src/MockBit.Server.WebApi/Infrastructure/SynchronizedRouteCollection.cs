using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MockBit.Server.WebApi.Infrastructure
{
    public class SynchronizedRouteCollection : IRouteCollection
    {
        private readonly static char[] UrlQueryDelimiters = new char[] { '?', '#' };
        private readonly List<IRouter> _routes = new List<IRouter>();
        private readonly Dictionary<string, INamedRouter> _namedRoutes =
                                    new Dictionary<string, INamedRouter>(StringComparer.OrdinalIgnoreCase);
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

        private RouteOptions _options;

        public IRouter this[int index]
        {
            get
            {
                try
                {
                    _rwLock.EnterReadLock();
                    return _routes[index];
                }
                finally
                {
                    _rwLock.ExitReadLock();
                }
            }
        }

        public int Count
        {
            get
            {
                try
                {
                    _rwLock.EnterReadLock();
                    return _routes.Count;
                }
                finally
                {
                    _rwLock.ExitReadLock();
                }
            }
        }

        public void Add(IRouter router)
        {
            if (router == null)
            {
                throw new ArgumentNullException(nameof(router));
            }

            try
            {
                _rwLock.EnterWriteLock();

                var namedRouter = router as INamedRouter;
                if (namedRouter != null)
                {
                    if (!string.IsNullOrEmpty(namedRouter.Name))
                    {
                        if (_namedRoutes.ContainsKey(namedRouter.Name))
                        {
                            _routes[_routes.IndexOf(_namedRoutes[namedRouter.Name])] = namedRouter;
                            _namedRoutes[namedRouter.Name] = namedRouter;
                        }
                        else
                        {
                            _namedRoutes.Add(namedRouter.Name, namedRouter);
                            _routes.Add(router);
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException("Route must be named");
                }
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }
        
        public void Clear()
        {
            try
            {
                _rwLock.EnterWriteLock();
                _namedRoutes.Clear();
                _routes.Clear();
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        public async virtual Task RouteAsync(RouteContext context)
        {
            try
            {
                _rwLock.EnterReadLock();

                // Perf: We want to avoid allocating a new RouteData for each route we need to process.
                // We can do this by snapshotting the state at the beginning and then restoring it
                // for each router we execute.
                var snapshot = context.RouteData.PushState(null, values: null, dataTokens: null);

                for (var i = 0; i < _routes.Count; i++)
                {
                    var route = _routes[i];
                    context.RouteData.Routers.Add(route);
                    
                    try
                    {
                        await route.RouteAsync(context);
                    
                        if (context.Handler != null)
                        {
                            break;
                        }
                    }
                    finally
                    {
                        if (context.Handler == null)
                        {
                            snapshot.Restore();
                        }
                    }
                }
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        public virtual VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            EnsureOptions(context.HttpContext);

            try
            {
                _rwLock.EnterReadLock();
                if (!string.IsNullOrEmpty(context.RouteName))
                {
                    VirtualPathData namedRoutePathData = null;
                    INamedRouter matchedNamedRoute;
                    if (_namedRoutes.TryGetValue(context.RouteName, out matchedNamedRoute))
                    {
                        namedRoutePathData = matchedNamedRoute.GetVirtualPath(context);
                    }

                    // If the named route and one of the unnamed routes also matches, then we have an ambiguity.
                    if (namedRoutePathData != null)
                    {
                        var message = string.Format("The supplied route name '{0}' is ambiguous and matched more than one route.", (context.RouteName));
                        throw new InvalidOperationException(message);
                    }

                    return NormalizeVirtualPath(namedRoutePathData);
                }
                else
                {
                    return NormalizeVirtualPath(GetVirtualPath(context, _routes));
                }
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        private VirtualPathData GetVirtualPath(VirtualPathContext context, List<IRouter> routes)
        {
            for (var i = 0; i < routes.Count; i++)
            {
                var route = routes[i];

                var pathData = route.GetVirtualPath(context);
                if (pathData != null)
                {
                    return pathData;
                }
            }

            return null;
        }

        private VirtualPathData NormalizeVirtualPath(VirtualPathData pathData)
        {
            if (pathData == null)
            {
                return pathData;
            }

            var url = pathData.VirtualPath;

            if (!string.IsNullOrEmpty(url) && (_options.LowercaseUrls || _options.AppendTrailingSlash))
            {
                var indexOfSeparator = url.IndexOfAny(UrlQueryDelimiters);
                var urlWithoutQueryString = url;
                var queryString = string.Empty;

                if (indexOfSeparator != -1)
                {
                    urlWithoutQueryString = url.Substring(0, indexOfSeparator);
                    queryString = url.Substring(indexOfSeparator);
                }

                if (_options.LowercaseUrls)
                {
                    urlWithoutQueryString = urlWithoutQueryString.ToLowerInvariant();
                }

                if (_options.AppendTrailingSlash && !urlWithoutQueryString.EndsWith("/"))
                {
                    urlWithoutQueryString += "/";
                }

                // queryString will contain the delimiter ? or # as the first character, so it's safe to append.
                url = urlWithoutQueryString + queryString;

                return new VirtualPathData(pathData.Router, url, pathData.DataTokens);
            }

            return pathData;
        }

        private void EnsureOptions(HttpContext context)
        {
            try
            {
                _rwLock.EnterUpgradeableReadLock();
                if (_options == null)
                {
                    try
                    {
                        _rwLock.EnterWriteLock();
                        _options = context.RequestServices.GetRequiredService<IOptions<RouteOptions>>().Value;
                    }
                    finally
                    {
                        _rwLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _rwLock.ExitUpgradeableReadLock();
            }
        }
    }
}
