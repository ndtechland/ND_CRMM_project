using CRM.Models.Crm;
using System.Text.Json.Serialization;

namespace CRM.Models.DTO
{
    public class SelfassesstmentadminDTO
    {
        public int Id { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public bool? Ispoint { get; set; }
        public string? Pointname { get; set; }
        public bool? Isactive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? Isdelete { get; set; }
        public List<Selfassesstmentadmin> SelfAssessmentList { get; set; }
    }
    public class SelfassesstmentapiDTO
    {
        public string? EmployeeName { get; set; }
        public string? DepartmentName { get; set; }
        public string? financialstartYear  { get; set; }
        public string? financialEndYear { get; set; }
        public List<Selfassesstmentdetails> selfassesstmentdetails { get; set; }
        
    }
    public class Selfassesstmentdetails
    {
        public int Id { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public string? Pointname { get; set; }

    }
    public class SelfassesstmentdataDto
    {
        public int? Startyear { get; set; }
        public int? Endyear { get; set; }
        [JsonPropertyName("AssessmentAnswers")]
        public AllQuestion AssesstmentAns { get; set; } = new AllQuestion();
        public string? ManagerName { get; set; }

    }
    public class AllQuestion
    {
        public QuestionOne QuestionOne { get; set; } = new QuestionOne();
        public QuestionTwo QuestionTwo { get; set; } = new QuestionTwo();
        public QuestionThird QuestionThird { get; set; } = new QuestionThird();
        public QuestionFour QuestionFour { get; set; } = new QuestionFour();
        public QuestionFive QuestionFive { get; set; } = new QuestionFive();
        public QuestionSix QuestionSix { get; set; } = new QuestionSix();
        public QuestionSeven QuestionSeven { get; set; } = new QuestionSeven();
    }
    public class QuestionOne
    {
        public List<QuestionOne1> Question1 { get; set; } = new List<QuestionOne1>();
        public List<QuestionOne2> Question2 { get; set; } = new List<QuestionOne2>();
        public List<QuestionOne3> Question3 { get; set; } = new List<QuestionOne3>();
        public List<QuestionOne4> Question4 { get; set; } = new List<QuestionOne4>();
        public List<QuestionOne3> Question5 { get; set; } = new List<QuestionOne3>();
    }
    public class QuestionOne1
    {
        public int IndexId { get; set; }
        public string? ProjectName { get; set; }
        public string? TimeLine { get; set; }
        public string? OutCome { get; set; }
    }
    public class QuestionOne2
    {
        public int IndexId { get; set; }
        public string? Goal { get; set; }
        public string? Result { get; set; }
    }
    public class QuestionOne3
    {
        public int IndexId { get; set; }
        public string? Example { get; set; }
    }
    public class QuestionOne4
    {
        public int IndexId { get; set; }
        public string? Target { get; set; }
        public string? Deadline { get; set; }
        public string? Reason { get; set; }
    }
    public class QuestionTwo
    {
        public QuestionTwo1 Question1 { get; set; } = new QuestionTwo1();
        public QuestionTwo1 Question2 { get; set; } = new QuestionTwo1();
        public QuestionTwo1 Question3 { get; set; } = new QuestionTwo1();
        public QuestionTwo1 Question4 { get; set; } = new QuestionTwo1();
    }
    public class QuestionTwo1
    {
        public int IndexId { get; set; }
        public string? Rating { get; set; }
        public string? Timeline { get; set; }
        public string? Selfassestment { get; set; }
    }

    public class QuestionThird
    {
        public List<QuestionThird1> Question1 { get; set; } = new List<QuestionThird1>();
        public List<QuestionThird1> Question2 { get; set; } = new List<QuestionThird1>();
        public List<QuestionThird1> Question3 { get; set; } = new List<QuestionThird1>();
        public List<QuestionThird1> Question4 { get; set; } = new List<QuestionThird1>();
    }
    public class QuestionThird1
    {
        public int IndexId { get; set; }
        public string? Example { get; set; }
    }
    public class QuestionFour
    {
        public List<QuestionFour1> Question1 { get; set; } = new List<QuestionFour1>();
        public List<QuestionFour2> Question2 { get; set; } = new List<QuestionFour2>();
        public List<QuestionFour2> Question3 { get; set; } = new List<QuestionFour2>();
        public List<QuestionFour4> Question4 { get; set; } = new List<QuestionFour4>();
    }
    public class QuestionFour1
    {
        public int IndexId { get; set; }
        public string? feddback { get; set; }
    }
    public class QuestionFour2
    {
        public int IndexId { get; set; }
        public string? Improvement { get; set; }
    }
    public class QuestionFour4
    {
        public int IndexId { get; set; }
        public string? Action { get; set; }
    }
    public class QuestionFive
    {
        public List<QuestionFive1> Question1 { get; set; } = new List<QuestionFive1>();
        public List<QuestionFive2> Question2 { get; set; } = new List<QuestionFive2>();
        public List<QuestionFive2> Question3 { get; set; } = new List<QuestionFive2>();
        public List<QuestionFive2> Question4 { get; set; } = new List<QuestionFive2>();
        public List<QuestionFive2> Question5 { get; set; } = new List<QuestionFive2>();
    }
    public class QuestionFive1
    {
        public int IndexId { get; set; }
        public string? Priorites { get; set; }
    }
    public class QuestionFive2
    {
        public int IndexId { get; set; }
        public string? Example { get; set; }
    }
    public class QuestionSix
    {
        public List<QuestionSix1> Question1 { get; set; } = new List<QuestionSix1>();
        public List<QuestionSix1> Question2 { get; set; } = new List<QuestionSix1>();
        public List<QuestionSix3> Question3 { get; set; } = new List<QuestionSix3>();
    }
    public class QuestionSix1
    {
        public int IndexId { get; set; }
        public string? Example { get; set; }
    }
    public class QuestionSix3
    {
        public int IndexId { get; set; }
        public string? Resources { get; set; }
    }
    public class QuestionSeven
    {
        public List<QuestionSeven1> Question1 { get; set; } = new List<QuestionSeven1>();
        public List<QuestionSeven2> Question2 { get; set; } = new List<QuestionSeven2>();
        public List<QuestionSeven3> Question3 { get; set; } = new List<QuestionSeven3>();
        public List<QuestionSeven4> Question4 { get; set; } = new List<QuestionSeven4>();
    }
    public class QuestionSeven1
    {
        public int IndexId { get; set; }
        public string? PerformanceSummary { get; set; }
    }
    public class QuestionSeven2
    {
        public int IndexId { get; set; }
        public string? Achievement { get; set; }
    }
    public class QuestionSeven3
    {
        public int IndexId { get; set; }
        public string? FocusArea { get; set; }
    }
    public class QuestionSeven4
    {
        public int IndexId { get; set; }
        public string? Example { get; set; }
    }
}
