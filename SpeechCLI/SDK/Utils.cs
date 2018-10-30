using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace CRIS
{
    public static class Utils
    {
        public static string GetCreatedLocation(HttpResponseMessage response)
        {
            var datasetId = response.Headers.Location.AbsolutePath.Split('/').LastOrDefault();
            return datasetId;
        }
    }
}
