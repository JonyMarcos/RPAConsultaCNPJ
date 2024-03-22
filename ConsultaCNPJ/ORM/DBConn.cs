using System;
using System.Data;
using System.Data.SqlClient;

namespace ConsultaCNPJ.BusinessProcess
{
    public class DBConn
    {
        private string connectionString;

        public DBConn(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public int RegistrarInicioExecucao(string ApenasNomeArquivo)
        {
            string nomeArquivo = Path.GetFileName(ApenasNomeArquivo);

            int idExecucao = 0; // Inicializa com valor padrão

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SPRegistrarInicioExecucao", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Parâmetro de entrada para o nome do arquivo
                command.Parameters.AddWithValue("@NomeArquivo", nomeArquivo);

                // Parâmetro de saída para o ID
                SqlParameter idParam = new SqlParameter("@Id", SqlDbType.Int);
                idParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(idParam);

                command.ExecuteNonQuery();

                // Obtém o valor do ID retornado pela stored procedure
                idExecucao = Convert.ToInt32(command.Parameters["@Id"].Value);
            }

            return idExecucao;
        }

        public void AtualizarStatusExecucao(int idExecucao, string nomeArquivo, string novoStatus, string novaMensagem)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SPAtualizarStatusExecucao", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Id", idExecucao);
                command.Parameters.AddWithValue("@NomeArquivo", nomeArquivo);
                command.Parameters.AddWithValue("@NovoStatus", novoStatus);
                command.Parameters.AddWithValue("@NovaMensagem", novaMensagem);

                command.ExecuteNonQuery();
            }
        }
        public bool VerificarExecucaoPendente(int idExecucao)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM BaseCNPJ WHERE IdExecucao = @IdExecucao AND StatusRPA = 'Pendente'", connection);
                command.Parameters.AddWithValue("@IdExecucao", idExecucao);

                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }

        public ItemPendente ObterProximoItemPendente(int idExecucao)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT TOP 1 CNPJ, NomeDaEmpresa FROM BaseCNPJ WHERE IdExecucao = @IdExecucao AND StatusRPA = 'Pendente' ORDER BY Id ASC", connection);
                command.Parameters.AddWithValue("@IdExecucao", idExecucao);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string cnpj = reader["CNPJ"].ToString();
                    string nomeEmpresa = reader["NomeDaEmpresa"].ToString();
                    reader.Close();

                    AtualizarStatusParaEmAndamento(idExecucao, cnpj); // Atualiza o status para "Em andamento"

                    return new ItemPendente(cnpj, nomeEmpresa); // Retorna os detalhes do item pendente
                }
                else
                {
                    return null; // Não há itens pendentes
                }
            }
        }

        private void AtualizarStatusParaEmAndamento(int idExecucao, string cnpj)
        {
            // Atualiza o status do item pendente para "Em andamento"
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("UPDATE BaseCNPJ SET StatusRPA = 'Em andamento', IniciadoEm = GETDATE() WHERE IdExecucao = @IdExecucao AND CNPJ = @CNPJ", connection);
                command.Parameters.AddWithValue("@IdExecucao", idExecucao);
                command.Parameters.AddWithValue("@CNPJ", cnpj);

                command.ExecuteNonQuery();
            }
        }
        public string ObterCNPJPendente(int idExecucao)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT TOP 1 CNPJ FROM BaseCNPJ WHERE IdExecucao = @IdExecucao AND StatusRPA = 'Pendente' ORDER BY Id ASC", connection);
                command.Parameters.AddWithValue("@IdExecucao", idExecucao);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string cnpj = reader["CNPJ"].ToString();
                    reader.Close();

                    return cnpj;
                }
                else
                {
                    return null; // Não há CNPJ pendente
                }
            }
        }
        public class ItemPendente
        {
            public string CNPJ { get; set; }
            public string NomeDaEmpresa { get; set; }

            public ItemPendente(string cnpj, string nomeDaEmpresa)
            {
                CNPJ = cnpj;
                NomeDaEmpresa = nomeDaEmpresa;
            }
        }
    }
}
