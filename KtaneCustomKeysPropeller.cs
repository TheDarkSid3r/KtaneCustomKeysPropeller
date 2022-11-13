using System.Linq;
using RT.PropellerApi;
using RT.Servers;

namespace KtaneCustomKeys
{
    public sealed class KtaneCustomKeysPropeller : PropellerModuleBase<KtaneCustomKeysPropellerSettings>
    {
        public override string Name => "KTaNE Custom Keys Propeller";

        public override HttpResponse Handle(HttpRequest req)
        {
            switch (req.Method)
            {
                case HttpMethod.Post:
                    {
                        if (!req.Post.ContainsKey("data") || req.Post["data"].Count < 1)
                            return HttpResponse.PlainText("Missing data.", HttpStatusCode._400_BadRequest);

                        return HttpResponse.Json(KtaneCustomKeysHolster.Push(req.Post["data"].Value));
                    }
                case HttpMethod.Get:
                    {
                        string[] codes = req.Url.QueryValues("code").ToArray();
                        if (codes.Length < 1)
                            return HttpResponse.PlainText("Missing code.", HttpStatusCode._400_BadRequest);

                        string code = codes.First();
                        if (!KtaneCustomKeysHolster.Has(code))
                            return HttpResponse.PlainText("Invalid code.", HttpStatusCode._404_NotFound);

                        return HttpResponse.PlainText(KtaneCustomKeysHolster.Pull(code));
                    }
                case HttpMethod.Delete:
                    {
                        string[] codes = req.Url.QueryValues("code").ToArray();
                        string[] tokens = req.Url.QueryValues("token").ToArray();
                        if (codes.Length < 1 || tokens.Length < 1)
                            return HttpResponse.PlainText("Missing code or token.", HttpStatusCode._400_BadRequest);

                        string code = codes.First();
                        if (!KtaneCustomKeysHolster.Has(code))
                            return HttpResponse.PlainText("Invalid code.", HttpStatusCode._404_NotFound);

                        if (!KtaneCustomKeysHolster.IsAuthorized(code, tokens.First()))
                            return HttpResponse.PlainText("Invalid token.", HttpStatusCode._401_Unauthorized);

                        KtaneCustomKeysHolster.Remove(code);

                        return HttpResponse.Empty();
                    }
                default:
                    return HttpResponse.PlainText("Invalid request method.", HttpStatusCode._405_MethodNotAllowed);
            }
        }
    }

    public sealed class KtaneCustomKeysPropellerSettings { }
}
