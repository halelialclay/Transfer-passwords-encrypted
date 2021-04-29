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
    public class StartAppController : ApiController
    {


       // GET: api/StartApp/5
        public HttpResponseMessage Get(string id)
        {
            inDictionary valDictionary = (inDictionary)System.Web.HttpContext.Current.Cache[id];

            string val = (valDictionary == null || valDictionary.key == null) ? "" : valDictionary.key;

            return new HttpResponseMessage()
            {
                Content = new StringContent(val,
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }


        // POST: api/StartApp
        public void Post([FromBody] publicKey value)
        {
            inDictionary inDic = new inDictionary();
            inDic.key = value.key;
            inDic.symmetric = null;

            if (HttpContext.Current.Cache[value.id] == null)
                HttpContext.Current.Cache.Insert(value.id, inDic, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 5, 0));
            else
                HttpContext.Current.Cache[value.id] = inDic;
        }
    }

    public class publicKey
    {
        public string id { get; set;}
        public string key { get; set;}

    }

    public class inDictionary
    {
        public string key { get; set; }
        public string symmetric { get; set; }

    }

   
}
