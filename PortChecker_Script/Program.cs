using System.Diagnostics;
using System.Runtime.InteropServices;
namespace PortChecker_Script
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Saber qual porta será verificada
            Console.Write("Digite a porta que deseja verificar: ");
            string port = Console.ReadLine() ?? "";

            if (!IsValidPort(port))
            {
                Console.WriteLine("A porta introduzida é inválida. Feche o script e volte a tentar");
                Console.ReadKey();
                return;
            }

            Console.Clear(); // Estética apenas

            // Iniciar script
            Console.WriteLine($"A verificar a porta {port}...");

            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c netstat -ano | findstr :{port}",
                // /c fecha a console após a execução do comando
                // -ano (a: Mostra todos os ativos; n: Mostra número da porta; o: Inclui o PID (Process ID))
                // findstr :{port} é o comando que permite filtrar a saída, mostrando apenas da porta escolhida anteriormente
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };

            // Cria processo
            using (var process = Process.Start(processInfo))
            {
                if(process is null)
                {
                    Console.Clear(); // Estética apenas
                    Console.WriteLine("Não foi possível realizar o processo");
                    return;
                }

                // Ler processo
                string? result = await process.StandardOutput.ReadToEndAsync();
                if(string.IsNullOrEmpty(result)) result = "Sem resultado";

                // Retornar o processo
                Console.Clear(); // Estética apenas
                Console.WriteLine(
                    $"ID DO PROCESSO: {process.Id}\r\n" +   
                    $"Resultado do processo:\r\n{result}");

                // Fechar
                await process.WaitForExitAsync();
            }

            // Fechar script
            Console.WriteLine("\r\nClique em qualquer botão para fechar o script");
            Console.ReadKey();

        }

        private static bool IsValidPort(string port)
        {
            bool validationResult = int.TryParse(port, out int portNumber);
            if (validationResult == true)
                return (portNumber >= 0 && portNumber <= 65535);
            return false;
        }

        // Primeiro teste do System.Diagnostics.Process
        private static  async Task OlaMundo()
        {
            Console.WriteLine("WORKING");
            // Instanciar o ProcessStartInfo
            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe", // Abrir a CMD
                Arguments = "/c echo Olá, Mundo", // No caso da CMD, os arguments funcionam como comandos a executar
                UseShellExecute = false,
                RedirectStandardOutput = true, // Permite ler o resultado do processo
            };

            // Iniciar o processo
            using (var process = Process.Start(processInfo)) // Inicia o processo com base nas informações
            {
                // Ler o output do processo
                string? result = await process.StandardOutput.ReadToEndAsync() ?? "Sem resultado"; // StandardOutput é o que nos permite ler a saída do processo em texto, já o ReadToEndAsync lê até ao fim da saída para nos retornar o valor
                // if (result is null) result = "Sem resultado";

                // Exibir o resultado do comando
                Console.WriteLine(
                    $"ID DO PROCESSO: {process.Id}\r\n" +
                    $"Resultado do processo: {result}");

                // Esperar o processo terminar
                await process.WaitForExitAsync();
            }
            Console.ReadKey();
        }
    }
}
