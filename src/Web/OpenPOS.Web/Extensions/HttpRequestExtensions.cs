using System;
using Microsoft.AspNetCore.Http;

namespace OpenPOS.Web.Extensions
{
    public static class HttpRequestExtensions
    {
        public static Guid GetStoreId(this HttpRequest request)
        {
            return Guid.Parse(request.Cookies[".o.p.s"] ?? Guid.Empty.ToString());
        }
    }
}