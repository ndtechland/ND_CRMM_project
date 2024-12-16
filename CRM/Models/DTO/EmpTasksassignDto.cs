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
        public string? EmployeeName { get; set; }
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
        public string? EmployeeName { get; set; }

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
        public string? StartTime { get; set; }

    }
    public class EmpApplyWfhDto
    {
        public DateTime? Startdate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Reason { get; set; }

    }
    public class TasksListDashDto
    {
        public string? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? TaskName { get; set; }
        public string? Taskstatus { get; set; }
    }
    public class TasksReplyListDashDto
    {
        public int? id { get; set; }
        public string? TaskName { get; set; }
        public string? SubTaskName { get; set; }
        public string? Replydate { get; set; }
        public string? Taskstatus { get; set; }
    }
    public class EmptaskReplyListDto
    {
        public int? id { get; set; }
        public string? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? TaskName { get; set; }
        public string? SubTaskName { get; set; }
        public string? Replydate { get; set; }
        public string? Taskstatus { get; set; }
        public string? TaskReason { get; set; }

    }
}
