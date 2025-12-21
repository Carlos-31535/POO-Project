namespace POOProject.Views.Enums
{
    /// <summary>
    /// Catálogo de todas as janelas disponíveis na aplicação.
    /// Usado pela Factory para saber o que criar.
    /// </summary>
    public enum ViewType
    {
        Login,
        Main,
        Registry,

        // Janelas de diálogo / formulários
        EditEmployee,
        AddArranjo,
        CreateFuncionario,
        DetalhesTalao
    }
}