namespace CRM.Models.DTO
{
    public class EmpTasksassignDto
    {
        public int Id { get; set; }
        public string? Task { get; set; }
        public string? Tittle { get; set; }
        
        public DateTime? Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public int? TaskStatusId { get; set; }
        public string? EmployeeId { get; set; }
    }
    public class EmpTasknameDto
    {
        public int Id { get; set; }
        public string? Emptask { get; set; }
        public string? EmployeeId { get; set; }
        public int Emptaskid { get; set; }
        public string? TaskStatus { get; set; }
        public int? TaskStatusId { get; set; }
        public string? Taskname { get; set; }
        public int SubtaskId { get; set; }

    }
    public class EmpTasksListDto
    {
        public int? taskid { get; set; }
    }
    public class EmpSubTasksListDto
    {
        public int? subtaskid { get; set; }
        public string? Taskreason { get; set; }
    }
    public class EmpSubTasksDto
    {
        public int? subtaskid { get; set; }
    }
    public class TasksListDto
    {
        public int? subtaskid { get; set; }
        public string[]? taskid { get; set; }
        public string? Taskreason { get; set; }
        public string? Taskstatus { get; set; }
    }
    public class EmpApplyovertimeDto
    {
        public string? EmployeeId { get; set; }
        public DateTime? StartTime { get; set; }

    }
}
