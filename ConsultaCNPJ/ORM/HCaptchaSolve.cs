using System;
using System.Linq;
using TwoCaptcha;
using TwoCaptcha.Captcha;

namespace ConsultaCNPJ.ORM
{
    class HCaptchaSolve
    {
        public string HCaptcha(String siteKey)
        {
            TwoCaptcha.TwoCaptcha solver = new TwoCaptcha.TwoCaptcha("f5d0d08a5047e1fa83fe0248bb1ce046");
            HCaptcha captcha = new HCaptcha();
            captcha.SetSiteKey(siteKey);
            captcha.SetUrl("https://www8.receita.fazenda.gov.br/simplesnacional/aplicacoes.aspx?id=21");
            try
            {
                solver.Solve(captcha).Wait();
                Console.WriteLine("Captcha solved: " + captcha.Code);
                return captcha.Code; // Retorna o código do captcha após a resolução
            }
            catch (AggregateException e)
            {
                Console.WriteLine("Error occurred: " + e.InnerExceptions.First().Message);
                return null; // Em caso de erro, retorna null
            }
        }
    }
}