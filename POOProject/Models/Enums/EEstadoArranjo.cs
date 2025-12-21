using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POOProject.Models.Enums
{
    /// <summary>
    /// Define o ciclo de vida (Workflow) de um pedido de reparação.
    /// É fundamental para filtrar as listas no ecrã principal.
    /// </summary>
    public enum EstadoArranjo
    {
        Arranjar,       // O cliente entregou, mas o sapateiro ainda não terminou o serviço.
        Pronto,         // O trabalho está feito e o sapato está na prateleira à espera do cliente.
        Entregue        // Processo fechado: o cliente já pagou e levantou o artigo.
    }
}