using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ConsultaCNPJ.BusinessProcess
{
    class ProcessFlow
    {
        private string pastaConsulta = @"C:\ConsultaCNPJ";
        private string connectionString = "Data Source=DESKTOP-SI6G0B7;Initial Catalog=RPA_Consulta_CNPJ;User ID=RBLVOB01;Password=RPA123";

        public void ExecuteRPA()
        {
            List<string> arquivosEncontrados = null;

            DBConn dbConn = new DBConn(connectionString);

            try
            {
                arquivosEncontrados = VerificarArquivos();
                if (arquivosEncontrados.Count == 0)
                {
                    Console.WriteLine("Não há arquivos para processar.");
                    return;
                }

                foreach (var arquivo in arquivosEncontrados)
                {
                    int idExecucao = dbConn.RegistrarInicioExecucao(arquivo);
                    ImportarExcelParaBaseCNPJ(arquivo, idExecucao);

                    // Loop para acessar o site enquanto houver itens pendentes
                    while (dbConn.VerificarExecucaoPendente(idExecucao)) // Alteração aqui: Passar o idExecucao como parâmetro
                    {
                        // Obter CNPJ pendente
                        string cnpjPendente = dbConn.ObterCNPJPendente(idExecucao); // Alteração aqui: Passar o idExecucao como parâmetro

                        // Verificar se há CNPJ pendente
                        if (string.IsNullOrEmpty(cnpjPendente))
                            break;

                        // Realizar a consulta no site
                        RealizarConsultaNoSite(cnpjPendente);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro durante a execução do RPA: {ex.Message}");
                if (arquivosEncontrados != null)
                {
                    foreach (var arquivo in arquivosEncontrados)
                    {
                        string nomeArquivo = Path.GetFileName(arquivo);
                        dbConn.AtualizarStatusExecucao(0, nomeArquivo, "Falha", ex.Message); // idExecucao = 0 para arquivos não processados
                    }
                }
            }
        }

        private List<string> VerificarArquivos()
        {
            List<string> arquivosEncontrados = new List<string>();

            if (!Directory.Exists(pastaConsulta))
            {
                throw new DirectoryNotFoundException($"A pasta {pastaConsulta} não foi encontrada.");
            }

            string[] arquivos = Directory.GetFiles(pastaConsulta, "*.xlsx");

            foreach (var arquivo in arquivos)
            {
                arquivosEncontrados.Add(arquivo);
            }

            return arquivosEncontrados;
        }

        private void ImportarExcelParaBaseCNPJ(string caminhoArquivo, int idExecucao)
        {
            Excel excel = new Excel();
            excel.ImportarParaBaseCNPJ(caminhoArquivo, idExecucao);
        }

        private void RealizarConsultaNoSite(string cnpj)
        {
            try
            {
                SimplesNacional.Consulta consulta = new SimplesNacional.Consulta();
                consulta.AcessarPaginaSimplesNacional();
                consulta.PesquisarEmpresa(cnpj);
                consulta.SalvarDados();
                consulta.FecharNavegador();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro durante a consulta no site: {ex.Message}");
            }
        }
    }
}

