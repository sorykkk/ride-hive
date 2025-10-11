using System.ComponentModel.DataAnnotations; // pentru [Key]
using System.ComponentModel.DataAnnotations.Schema; // pentru [Table]


namespace Counter.Model
{
    public class CounterClass // Exemplu de clasa pentru obiect
    {
        public int Id { get; set; }
        public required string Val { get; set; }
    }

    
    public class TestClass
    {
        public string Name { get; set; }
        public int Virtut { get; set; }
        public int Cost { get; set; }
    }
}

//pentru migrare ruleaza " dotnet ef migrations add InitialCreate " apoi " dotnet ef database update " 