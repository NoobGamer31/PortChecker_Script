using System.Diagnostics;
using System.Runtime.InteropServices;
namespace PortChecker_Script
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                Intro();
                int.TryParse(Console.ReadLine(), out int menuOption);
                switch (menuOption)
                {
                    case 0: // Ver uma porta
                        // Saber qual porta será verificada
                        Console.Write("Digite a porta que deseja verificar: ");
                        string port = Console.ReadLine() ?? "";
                        await VerUmaPorta(port);

                        break;
                    case 1: // Ver x portas
                        ToBeImplemented();
                        break;
                    case 2: // Ver um intervalo de portas
                        ToBeImplemented();
                        break;
                    case 3: // Configurações
                        ToBeImplemented();
                        break;
                    case 4: // Sair
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Operação inválida");
                        break;
                }
                Console.WriteLine("\r\nPressione qualquer tecla para continuar");
                Console.ReadKey();
            }
        }

        private static void ToBeImplemented()
        {
            Console.WriteLine("Funcionalidade a implementar...");
        }

        private static void Intro()
        {
            Console.Clear();
            Console.WriteLine(@" ==================== ");
            Console.WriteLine(@"     PORT CHECKER    ");
            Console.WriteLine(@" ==================== ");
            Menu();

        
        }
        private static void Menu()
        {
            Console.WriteLine("Escolha a opção a efetuar:");
            Console.WriteLine("[0] - Conferir 1 porta");
            Console.WriteLine("[1] - Conferir x portas");
            Console.WriteLine("[2] - Conferir um intervalo de portas");
            Console.WriteLine("[3] - Configurações");
            Console.WriteLine("[4] - Sair");
        }

        private static void MandarInfosEcra(string output)
        {
            Console.WriteLine(output);
        }

        static void PrintBox(string titulo, Dictionary<string, string> campos)
        {
            int width = 60;
            Console.WriteLine("╔" + new string('═', width) + "╗");
            Console.WriteLine($"║  {titulo.PadRight(width - 2)}║");
            Console.WriteLine("╠" + new string('═', width) + "╣");
            foreach (var field in campos)
            {
                string line = $" {field.Key}: {field.Value}";
                Console.WriteLine($"║{line.PadRight(width)}║");
            }
            Console.WriteLine("╚" + new string('═', width) + "╝");
        }






        private static async Task VerUmaPorta(string port)
        {
            // Valida se a porta está dentro do intervalo
            if (!IsValidPort(port))
            {
                Console.WriteLine("A porta introduzida é inválida.");
                return;
            }

            Console.Clear();
            Console.WriteLine($"A verificar a porta {port}...");

            // Processo para executar o comando netstat e filtrar pela porta em questão
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

            using (var process = Process.Start(processInfo))
            {
                if (process is null) // Caso o processo não tenha sido iniciado corretamente
                {
                    Console.WriteLine("Não foi possível realizar o processo.");
                    return;
                }

                string? result = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();

                Console.Clear();

                if (string.IsNullOrWhiteSpace(result)) // Porta livre
                {
                    PrintBox($"PORTA {port} - LIVRE", new Dictionary<string, string>
                    {
                        { "Status", "Nenhum processo a usar esta porta" }
                    });
                    return;
                }

                // Parsing do conteúdo RAW que é retornado
                string firstLine = result.Trim().Replace("\r", "").Split('\n')[0]; // Pega só a primeira linha
                string[] parts = firstLine.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries); // Divide a linha em partes, ignorando espaços e tabulações extras

                // Extrair dados
                string protocol = parts[0];          // TCP ou UDP
                string status = parts.Length >= 4 ? parts[3] : "N/A"; // LISTENING, ESTABLISHED...
                string pid = parts.Length >= 5 ? parts[4] : "N/A"; // PID

                // Buscar nome do processo pelo PID
                string processName = "N/A";
                if (int.TryParse(pid, out int pidNumber))
                {
                    try { processName = Process.GetProcessById(pidNumber).ProcessName; }
                    catch { processName = "Sem acesso"; } // Alguns processos do sistema bloqueiam
                }

                PrintBox($"PORTA {port} - EM USO", new Dictionary<string, string>
                {
                    { "Protocolo", protocol },
                    { "Status",    status },
                    { "PID",       pid },
                    { "Processo",  processName }
                });
                    }
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
