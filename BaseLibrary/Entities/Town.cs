namespace BaseLibrary.Entities
{
    public class Town : BaseEntity
    {
        //One to many rlationship with Employees
        public List<Employee>? Employees {get; set;}
        
        //Many to one relationship with City
        public City? City {get; set;}
        public int CityId {get; set;}
    }
}
