using Microsoft.AspNet.WebHooks;
using System.Threading.Tasks;

namespace WebHook.API.Handlers
{
    public class EshopHandler : WebHookHandler
    {
        public override Task ExecuteAsync(string receiver, WebHookHandlerContext context)
        {
            return Task.FromResult(true);
        }
    }
}
