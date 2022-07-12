namespace EzDomain.EventSourcing.EventStores.Sql.Data
{
    public interface ISqlStatementsLoader
    {
        void LoadScripts(string filesPath = "Data\\SqlScripts");
        
        string this[string key] { get; }
    }
}