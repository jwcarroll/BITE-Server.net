using System;
using System.Web;

namespace BITE.Server.Plugins.Interfaces
{
    public interface IRequestFormatter
    {
        Object FormatRequest(HttpRequestBase request);
    }
}
