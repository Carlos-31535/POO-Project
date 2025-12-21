using POOProject.Models.Enums;
using System.Collections.Generic;

namespace POOProject.Models.Entities
{
    /// <summary>
    /// Representa um artigo individual entregue pelo cliente (ex: "Par nº 1 - Sapatos Pretos").
    /// </summary>
    public class Calcado
    {
        // --- PROPRIEDADES ---

        // Identificador simples para o talão (ex: "Par 1", "Par 2")
        public string NumPar { get; set; }

        // Enums para garantir que os dados são padronizados (evita erros de escrita como "Sapatilhas" vs "Ténis")
        public TipoCalcado Tipo { get; set; }
        public CorCalcado Cor { get; set; }

        // Detalhes extra opcionais (ex: "Sola partida ao meio")
        public string Descricao { get; set; }

        // Lista de tarefas a executar neste par específico.
        // É uma lista porque o mesmo sapato pode precisar de "Colar" E "Cosser".
        public List<Servicos> ServicosParaFazer { get; set; }

        // --- CONSTRUTOR ---

        public Calcado()
        {
            // Inicializamos a lista aqui para evitar "NullReferenceException"
            // quando tentarmos adicionar serviços na interface gráfica.
            ServicosParaFazer = new List<Servicos>();
        }

        // --- HELPERS VISUAIS ---

        // Propriedade de leitura apenas (read-only) que formata a lista de serviços numa string bonita.
        // Essencial para mostrar na DataGrid sem precisarmos de Conversores complexos no XAML.
        // Exemplo de output: "Colar, Mudar Sola, Limpeza"
        public string ResumoServicos => string.Join(", ", ServicosParaFazer);
    }
}