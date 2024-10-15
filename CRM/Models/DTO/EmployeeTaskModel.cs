namespace CRM.Models.DTO
{
    public class EmployeeTaskModel
    {
        public int Id { get; set; }
        public int? Emptaskid { get; set; }
        public string? Emptask { get; set; }
        public string? EmployeeId { get; set; }
        public string? TaskStatus { get; set; }
        public int? TaskStatusId { get; set; }
        public string? Taskname { get; set; }
        public List<TasksList> TasksLists { get; set; }
        public List<EmpTasknameDto> EmpTaskList { get; set; }
    }
    public class TasksList
    {
        public int EmpTaskId { get; set; }
        public string TaskName { get; set; }
        public int? TaskStatusId { get; set; }
    }
}
