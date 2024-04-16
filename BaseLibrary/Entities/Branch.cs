namespace BaseLibrary.Entities
{
    public class Branch : BaseEntity
    {
        //Many to one relationship with Department
        public Department? Department {get; set;}
        public int DepartmentId  {get; set;}
        
        //One to many relationship with Employee
        public List<Employee>? Employees {get; set;}
    }
}
