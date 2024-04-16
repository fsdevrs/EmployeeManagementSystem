
namespace BaseLibrary.Entities
{
    public class City : BaseEntity
    {
       //Many to one relationship with Country
       public Country? Country {get; set;}
       public int CountryId {get; set;} 

       //One to many relationship with Town
       public List<Town>? Towns {get; set;}
    }
}