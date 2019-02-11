using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChangelingArionumPool.Entities;
using ChangelingArionumPool.Services;
using ChangelingBizUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangelingArionumPool.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class PoolController : ControllerBase
    {
        // defined by Arionum protocol
        [HttpGet("mine.php")]
        public ActionResult<PoolResponse> Info(string q)
        {
            if (q == "info")
            {
                string wallet = Utils.Sanitize(HttpContext.Request.Query["address"]);
                string worker = HttpContext.Request.Query["worker"];
                string hashRateCblocks = HttpContext.Request.Query["hashrate"];
                string hashRateGblocks = (!String.IsNullOrEmpty(HttpContext.Request.Query["hrgpu"])) ? HttpContext.Request.Query["hrgpu"] : HttpContext.Request.Query["gpuhr"];
                int ip = BitConverter.ToInt32(HttpContext.Connection.RemoteIpAddress.GetAddressBytes(), 0);

                return Pool.Instance.Info(wallet, worker, hashRateCblocks, hashRateGblocks, ip);
            }
            else
                return new PoolErrorResponse("Invalid query.");
        }

        // defined by Arionum protocol
        [HttpPost("mine.php")]
        public ActionResult<PoolResponse> SubmitNonce(string q)
        {
            if (q == "submitNonce")
            {
                string wallet = Utils.Sanitize(HttpContext.Request.Form["address"]);
                string height = HttpContext.Request.Form["height"];
                string argon = HttpContext.Request.Form["argon"];
                string nonce = HttpContext.Request.Form["nonce"];
                if(nonce != null)
                {
                    if(nonce.Length > 120)
                    {
                        nonce = nonce.Substring(0, 120);
                    }
                    nonce = Utils.Sanitize(nonce);
                }
                string private_key = HttpContext.Request.Form["private_key"];
                string public_key = HttpContext.Request.Form["public_key"];
                int ip = BitConverter.ToInt32(HttpContext.Connection.RemoteIpAddress.GetAddressBytes(), 0);

                return Pool.Instance.SubmitNonce(wallet, height, argon, nonce, private_key, public_key, ip);
            }
            else
                return new PoolErrorResponse("Invalid query.");
        }

        // extension to Arionum protocol, supported by ariominer
        [HttpPost("mineExt.php")]
        public ActionResult<PoolResponse> SubmitData(string q)
        {
            string payload = String.Empty;
            using (StreamReader streamReader = new StreamReader(HttpContext.Request.Body))
            {
                payload = streamReader.ReadToEnd();
            }

            return Pool.Instance.SubmitData(payload);
        }
    }
}
