using System;

namespace ConsultaCNPJ
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "RPA Consulta CNPJ";
            Console.WriteLine("=== RPA Consulta CNPJ ===");
            Console.WriteLine();

            // Solicitar confirmação para continuar
            Console.WriteLine("Deseja Iniciar o RPA? - (S/N)");
            var resposta = Console.ReadLine().ToUpper(); // Ler a resposta do usuário e converter para maiúsculas

            if (resposta == "N")
            {
                Console.WriteLine();
                Console.WriteLine("Operação cancelada pelo usuário.");
                return;
            }
            else if (resposta != "S")
            {
                Console.WriteLine();
                Console.WriteLine("Opção inválida. Operação cancelada.");
                return;
            }

            // Continuar com o processo de RPA
            BusinessProcess.ProcessFlow flow = new BusinessProcess.ProcessFlow();
            Console.WriteLine();
            Console.WriteLine("Iniciando o RPA...");
            flow.ExecuteRPA();

            Console.WriteLine();
            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();
        }
    }
}
