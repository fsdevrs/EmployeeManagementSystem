namespace BaseLibrary.Entities
{
    public class GeneralDepartment : BaseEntity
    {
        //One to many relationship with Department
        public List<Department>? Departments {get; set;}
    }
}
