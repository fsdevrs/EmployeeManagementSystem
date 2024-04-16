namespace BaseLibrary.Entities
{
    public class SanctionType : BaseEntity
    {
       //Many To One relationship with SanctionType
       public List<Sanction>? Sanctions {get; set;} 
    }
}