using System.Text.Json.Serialization;

namespace CRM.Models.DTO
{
    public class SelfassesstmentempdataDto
    {
        public int Id { get; set; }
        public int? Startyear { get; set; }
        public int? Endyear { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeId { get; set; }

        [JsonPropertyName("AssessmentAnswers")]
        public AllQuestiondata AssesstmentAns { get; set; } = new AllQuestiondata();
        public string? ManagerName { get; set; }
        public bool Status { get; set; }

    }
    public class AllQuestiondata
    {
        public QuestionOnedata QuestionOne { get; set; } = new QuestionOnedata();
        public QuestionTwodata QuestionTwo { get; set; } = new QuestionTwodata();
        public QuestionThirddata QuestionThird { get; set; } = new QuestionThirddata();
        public QuestionFourdata QuestionFour { get; set; } = new QuestionFourdata();
        public QuestionFivedata QuestionFive { get; set; } = new QuestionFivedata();
        public QuestionSixdata QuestionSix { get; set; } = new QuestionSixdata();
        public QuestionSevendata QuestionSeven { get; set; } = new QuestionSevendata();
    }
    public class QuestionOnedata
    {
        public List<QuestionOnedata1> Question1 { get; set; } = new List<QuestionOnedata1>();
        public List<QuestionOnedata2> Question2 { get; set; } = new List<QuestionOnedata2>();
        public List<QuestionOnedata3> Question3 { get; set; } = new List<QuestionOnedata3>();
        public List<QuestionOnedata4> Question4 { get; set; } = new List<QuestionOnedata4>();
        public List<QuestionOnedata3> Question5 { get; set; } = new List<QuestionOnedata3>();
    }
    public class QuestionOnedata1
    {
        public int Id { get; set; }
        public int IndexId { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public string? ProjectName { get; set; }
        public string? TimeLine { get; set; }
        public string? OutCome { get; set; }
    }
    public class QuestionOnedata2
    {
        public int Id { get; set; }
        public int IndexId { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public string? Goal { get; set; }
        public string? Result { get; set; }
    }
    public class QuestionOnedata3
    {
        public int Id { get; set; }
        public int IndexId { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public string? Example { get; set; }
    }
    public class QuestionOnedata4
    {
        public int Id { get; set; }

        public int IndexId { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public string? Target { get; set; }
        public string? Deadline { get; set; }
        public string? Reason { get; set; }
    }
    public class QuestionTwodata
    {
        public QuestionTwodata1 Question1 { get; set; } = new QuestionTwodata1();
        public QuestionTwodata1 Question2 { get; set; } = new QuestionTwodata1();
        public QuestionTwodata1 Question3 { get; set; } = new QuestionTwodata1();
        public QuestionTwodata1 Question4 { get; set; } = new QuestionTwodata1();
        public QuestionTwodata1 Question5 { get; set; } = new QuestionTwodata1();
    }
    public class QuestionTwodata1
    {
        public int Id { get; set; }
        public int IndexId { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public string? Rating { get; set; }
        public string? Timeline { get; set; }
        public string? Selfassestment { get; set; }
    }

    public class QuestionThirddata
    {
        public List<QuestionThirddata1> Question1 { get; set; } = new List<QuestionThirddata1>();
        public List<QuestionThirddata1> Question2 { get; set; } = new List<QuestionThirddata1>();
        public List<QuestionThirddata1> Question3 { get; set; } = new List<QuestionThirddata1>();
        public List<QuestionThirddata1> Question4 { get; set; } = new List<QuestionThirddata1>();
    }
    public class QuestionThirddata1
    {
        public int Id { get; set; }

        public int IndexId { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public string? Example { get; set; }
    }
    public class QuestionFourdata
    {
        public List<QuestionFourdata1> Question1 { get; set; } = new List<QuestionFourdata1>();
        public List<QuestionFourdata2> Question2 { get; set; } = new List<QuestionFourdata2>();
        public List<QuestionFourdata2> Question3 { get; set; } = new List<QuestionFourdata2>();
        public List<QuestionFourdata4> Question4 { get; set; } = new List<QuestionFourdata4>();
    }                                                                               
    public class QuestionFourdata1
    {
        public int Id { get; set; }
        public int IndexId { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public string? feddback { get; set; }
    }
    public class QuestionFourdata2
    {
        public int Id { get; set; }
        public int IndexId { get; set; }
        public string? Improvement { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
    }
    public class QuestionFourdata4
    {
        public int Id { get; set; }
        public int IndexId { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public string? Action { get; set; }
    }
    public class QuestionFivedata
    {
        public List<QuestionFivedata1> Question1 { get; set; } = new List<QuestionFivedata1>();
        public List<QuestionFivedata2> Question2 { get; set; } = new List<QuestionFivedata2>();
        public List<QuestionFivedata2> Question3 { get; set; } = new List<QuestionFivedata2>();
        public List<QuestionFivedata2> Question4 { get; set; } = new List<QuestionFivedata2>();
        public List<QuestionFivedata2> Question5 { get; set; } = new List<QuestionFivedata2>();
    }
    public class QuestionFivedata1
    {
        public int Id { get; set; }
        public int IndexId { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public string? Priorites { get; set; }
    }
    public class QuestionFivedata2
    {
        public int Id { get; set; }
        public int IndexId { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public string? Example { get; set; }
    }
    public class QuestionSixdata
    {
        public List<QuestionSixdata1> Question1 { get; set; } = new List<QuestionSixdata1>();
        public List<QuestionSixdata1> Question2 { get; set; } = new List<QuestionSixdata1>();
        public List<QuestionSixdata3> Question3 { get; set; } = new List<QuestionSixdata3>();
    }
    public class QuestionSixdata1
    {
        public int Id { get; set; }
        public int IndexId { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public string? Example { get; set; }
    }
    public class QuestionSixdata3
    {
        public int Id { get; set; }
        public int IndexId { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public string? Resources { get; set; }
    }
    public class QuestionSevendata
    {
        public List<QuestionSevendata1> Question1 { get; set; } = new List<QuestionSevendata1>();
        public List<QuestionSevendata2> Question2 { get; set; } = new List<QuestionSevendata2>();
        public List<QuestionSevendata3> Question3 { get; set; } = new List<QuestionSevendata3>();
        public List<QuestionSevendata4> Question4 { get; set; } = new List<QuestionSevendata4>();
    }
    public class QuestionSevendata1
    {
        public int Id { get; set; }
        public int IndexId { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public string? PerformanceSummary { get; set; }
    }
    public class QuestionSevendata2
    {
        public int Id { get; set; }
        public int IndexId { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public string? Achievement { get; set; }
    }
    public class QuestionSevendata3
    {
        public int Id { get; set; }
        public int IndexId { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public string? FocusArea { get; set; }
    }
    public class QuestionSevendata4
    {
        public int Id { get; set; }
        public int IndexId { get; set; }
        public string? Tittle { get; set; }
        public string? SubTittle { get; set; }
        public string? Example { get; set; }
    }
}
