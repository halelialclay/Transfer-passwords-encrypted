using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Http;

namespace WebApplication2.Controllers
{
    public class SymmetricController : ApiController
    {
     

        // GET: api/Symmetric/5
        public HttpResponseMessage Get(string id)
        {
            inDictionary valDictionary = (inDictionary)System.Web.HttpContext.Current.Cache[id];

            string val = (valDictionary == null || valDictionary.symmetric==null) ? "" : valDictionary.symmetric;

            return new HttpResponseMessage()
            {
                Content = new StringContent(val,
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        // POST: api/Symmetric
        public void Post([FromBody]symmetricEncrypt value)
        {
            inDictionary inDic = new inDictionary();
            inDic.key = null;
            inDic.symmetric = value.Symmetric;

            if (HttpContext.Current.Cache[value.id] == null)
                HttpContext.Current.Cache.Insert(value.id, inDic, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 5, 0));
            else
                HttpContext.Current.Cache[value.id] = inDic;
        }
    }

    public class symmetricEncrypt
    {
        public string id { get; set; }
        public string Symmetric { get; set; }

    }

}
