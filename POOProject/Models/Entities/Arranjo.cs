using POOProject.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace POOProject.Models.Entities
{
    /// <summary>
    /// Representa um pedido de reparação (Talão). 
    /// Agrupa o cliente, o funcionário e a lista de sapatos a arranjar.
    /// </summary>
    public class Arranjo
    {
        // --- PROPRIEDADES ---

        // Gero um ID automático e curto (8 chars) logo na inicialização.
        // O ToUpper ajuda a ficar mais legível no talão (ex: "A1B2C3D4").
        public string Id { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

        // Dados de quem pediu e de quem atendeu.
        public Cliente Cliente { get; set; }
        public Funcionario FuncionarioResponsavel { get; set; }

        // Controlo de datas e estado do processo
        public DateTime DataEntrada { get; set; }
        public DateTime? DataConclusao { get; set; } // Nullable porque pode ainda não estar pronto
        public EstadoArranjo Estado { get; set; }

        // Lista de artigos deste pedido. 
        // Nota: Precisa de { get; set; } público para o serializer do JSON conseguir gravar e ler.
        public List<Calcado> ListaCalcado { get; set; } = new List<Calcado>();

        // --- CONSTRUTORES ---

        /// <summary>
        /// Construtor principal para criar novos arranjos na loja.
        /// Obriga a indicar logo o cliente e o funcionário.
        /// </summary>
        public Arranjo(Cliente cliente, Funcionario funcionario)
        {
            // Garante que não criamos arranjos "órfãos"
            Cliente = cliente ?? throw new ArgumentNullException(nameof(cliente));
            FuncionarioResponsavel = funcionario ?? throw new ArgumentNullException(nameof(funcionario));

            ListaCalcado = new List<Calcado>();
            DataEntrada = DateTime.Now;
            Estado = EstadoArranjo.Arranjar; // Estado inicial por defeito
        }

        // Construtor vazio: É obrigatório para o sistema de carregamento de ficheiros (JSON/XML).
        // Sem isto, a aplicação daria erro ao tentar ler os dados guardados.
        public Arranjo()
        {
            ListaCalcado = new List<Calcado>();
            // Inicializamos a null para evitar avisos do compilador, 
            // mas o JSON vai preencher isto logo a seguir.
            Cliente = null!;
            FuncionarioResponsavel = null!;
        }

        // --- MÉTODOS DE NEGÓCIO ---

        /// <summary>
        /// Adiciona um par de sapatos ao pedido, com validações.
        /// </summary>
        public void AdicionarCalcado(Calcado calcado)
        {
            if (calcado == null) throw new ArgumentNullException("Sapato inválido.");

            // Regra de negócio: Não faz sentido receber um sapato se não for para fazer nada.
            if (calcado.ServicosParaFazer.Count == 0)
                throw new InvalidOperationException("O sapato tem de ter pelo menos 1 serviço.");

            ListaCalcado.Add(calcado);
        }

        /// <summary>
        /// Finaliza o trabalho, registando a data de conclusão.
        /// </summary>
        public void MarcarComoPronto()
        {
            // Impede fechar talões vazios
            if (ListaCalcado.Count == 0)
                throw new InvalidOperationException("Não podes fechar um talão sem sapatos.");

            Estado = EstadoArranjo.Pronto;
            DataConclusao = DateTime.Now;
        }

        /// <summary>
        /// Regista a entrega ao cliente. Só possível se já estiver pronto.
        /// </summary>
        public void EntregarAoCliente()
        {
            if (Estado != EstadoArranjo.Pronto)
                throw new InvalidOperationException("O arranjo ainda não está pronto.");

            Estado = EstadoArranjo.Entregue;
        }

        // --- HELPERS VISUAIS (Binding) ---
        // Estas propriedades servem apenas para facilitar a exibição na DataGrid (WPF),
        // evitando ter lógica complexa no XAML.

        // Retorna 0 se a lista for nula (segurança contra crashes na UI)
        public int QuantidadePares => ListaCalcado?.Count ?? 0;

        // Mostra "N/A" se o cliente tiver sido apagado ou não carregado
        public string NomeClienteDisplay => Cliente?.NomeCompleto ?? "N/A";
    }
}