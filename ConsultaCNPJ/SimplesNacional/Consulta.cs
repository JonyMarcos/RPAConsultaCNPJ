using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using SeleniumExtras.WaitHelpers;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Edge;
using System;
using System.Threading;

namespace ConsultaCNPJ.SimplesNacional
{
    class Consulta
    {

        private IWebDriver driver;

        public Consulta()
        {
            // Inicializar o driver do Chrome
            ChromeOptions options = new ChromeOptions();

            // Especifique o caminho para o perfil do usuário do Chrome
            string userProfilePath = @"C:\Users\a841074\AppData\Local\Google\Chrome\User Data\Default";
            options.AddArgument($"user-data-dir={userProfilePath}");

            // Adicionar opções para reduzir a detecção de automação
            options.AddArgument("disable-dev-shm-usage");
            options.AddArgument("disable-infobars");
            options.AddArgument("disable-notifications");
            options.AddArgument("disable-popup-blocking");
            options.AddArgument("disable-extensions");
            options.AddArgument("window-size=1920,1080"); // Definir a dimensão da janela do navegador

            // Inicializar o driver do Chrome com as opções configuradas
            driver = new ChromeDriver(options);
        }


        public void AcessarPaginaSimplesNacional()
        {
            // Navegar para a página inicial
            driver.Navigate().GoToUrl("https://www8.receita.fazenda.gov.br/simplesnacional/Default.aspx");

            // Localizar o link "Consulta Optantes" e clicar nele
            IWebElement linkConsultaOptantes = driver.FindElement(By.CssSelector("a[title='Consulta Optantes']"));
            linkConsultaOptantes.Click();
        }
        public void PesquisarEmpresa(string cnpj)
        {
            Random random = new Random();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10)); // Tempo máximo de espera

            // Aguardar um pouco antes de interagir
            Thread.Sleep(random.Next(2000, 4000));

            // Encontra o iframe que contém o elemento
            IWebElement iframeElement = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("frame")));

            // Muda o contexto do driver para o iframe
            driver.SwitchTo().Frame(iframeElement);

            // Aguardar um pouco antes de preencher o CNPJ
            Thread.Sleep(random.Next(1000, 3000));

            // Localiza e preenche o campo CNPJ
            IWebElement campoCNPJ = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Cnpj")));
            campoCNPJ.SendKeys(cnpj);

            // Localiza o elemento textarea pelo atributo 'name'
            IWebElement textareaElement = driver.FindElement(By.CssSelector("textarea[name='h-captcha-response']"));

            // Obtém o valor do atributo 'id'
            string textareaId = textareaElement.GetAttribute("id");

            // Extrai o valor da chave do site
            IWebElement siteKeyElement = driver.FindElement(By.CssSelector("div.h-captcha"));
            string siteKey = siteKeyElement.GetAttribute("data-sitekey");

            // Aguarda um pouco antes de resolver o captcha
            Thread.Sleep(random.Next(2000, 4000));

            // Obter código do captcha resolvido pelo 2captcha
            ORM.HCaptchaSolve hcaptchaSolver = new ORM.HCaptchaSolve();
            string captchaCode = hcaptchaSolver.HCaptcha(siteKey);

            // Executa o script JavaScript para inserir o código do captcha no elemento textarea
            ((IJavaScriptExecutor)driver).ExecuteScript($"document.querySelector('#{textareaId}').innerHTML = '{captchaCode}';");

            // Aguardar um pouco antes de clicar no botão de consulta
            Thread.Sleep(random.Next(2000, 4000));

            // Clicar no botão de consulta
            IWebElement botaoConsulta = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='consultarForm']/button")));
            botaoConsulta.Click();
        }


        public void SalvarDados()
        {
            // Implemente a lógica para salvar os dados obtidos no banco de dados
            Console.WriteLine("Salvando dados no banco de dados...");
            // Exemplo: Você pode extrair informações da página e salvar no banco de dados aqui
        }

        public void FecharNavegador()
        {
            // Fechar o navegador após a conclusão das operações
            driver.Quit();
        }
    }
}
