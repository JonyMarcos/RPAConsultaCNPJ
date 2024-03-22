using System;
using System.Data;
using System.Data.SqlClient;
using OfficeOpenXml;

namespace ConsultaCNPJ.BusinessProcess
{
    class Excel
    {
        static Excel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }
        public void ImportarParaBaseCNPJ(string caminhoArquivo, int idExecucao)
        {
            // Conexão com o banco de dados
            string connectionString = "Data Source=DESKTOP-SI6G0B7;Initial Catalog=RPA_Consulta_CNPJ;User ID=RBLVOB01;Password=RPA123";

            // Conexão com o arquivo Excel usando EPPlus
            using (ExcelPackage package = new ExcelPackage(new FileInfo(caminhoArquivo)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Obtém a primeira planilha do Excel

                int totalLinhas = worksheet.Dimension.End.Row;
                int totalColunas = worksheet.Dimension.End.Column;

                // Iterar sobre as linhas do Excel (começando da linha 4)
                for (int linha = 4; linha <= totalLinhas; linha++)
                {
                    // Ler dados do Excel
                    string nomeEmpresa = worksheet.Cells[linha, 1].Value.ToString();
                    string cnpj = worksheet.Cells[linha, 2].Value.ToString();

                    // Inserir dados no banco de dados
                    InserirNaBaseCNPJ(nomeEmpresa, cnpj, idExecucao, connectionString);
                }
            }
        }

        private void InserirNaBaseCNPJ(string nomeEmpresa, string cnpj, int idExecucao, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("INSERT INTO BaseCNPJ (NomeDaEmpresa, CNPJ, StatusRPA, IdExecucao) VALUES (@NomeEmpresa, @CNPJ, @Status, @IdExecucao)", connection);
                command.Parameters.AddWithValue("@NomeEmpresa", nomeEmpresa);
                command.Parameters.AddWithValue("@CNPJ", cnpj);
                command.Parameters.AddWithValue("@Status", "Pendente");
                command.Parameters.AddWithValue("@IdExecucao", idExecucao);

                command.ExecuteNonQuery();
            }
        }
    }
}