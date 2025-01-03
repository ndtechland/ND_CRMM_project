﻿using CRM.Models.APIDTO;
using CRM.Models.Crm;
using CRM.Models.DTO;
using CRM.Repository;
using CRM.Utilities;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Services.WebApi.Jwt;

namespace CRM.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeApiController : ControllerBase
    {
        private readonly admin_NDCrMContext _context;
        private readonly IHome _home;
        public HomeApiController(admin_NDCrMContext context, IHome home)
        {
            _context = context;
            _home = home;
        }
        [HttpGet]
        [Route("Blogs")]
        public async Task<IActionResult> Blogs()
        {
            try
            {
                List<Blog> blogs = await _home.GetBlogs();
                if (blogs != null)
                {
                    return Ok(new { Status = 200, Message = "Blogs retrieved successfully.", data = blogs });
                }
                else
                {
                    return NotFound(new { Status = 404, Message = "No any blog available." });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("GetBlogById")]
        public async Task<IActionResult> GetBlogById(int Id)
        {
            try
            {
                var data = _context.Blogs.Where(b => b.IsPublished == true && b.Id == Id).FirstOrDefault();

                if (data != null)
                {
                    return Ok(new { Status = 200, Message = "Blog detail retrieved successfully.", data = data });
                }
                else
                {
                    return NotFound(new { Status = 404, Message = "Blog detail not found." });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet("Getaboutcompany")]
        public async Task<IActionResult> Getaboutcompany()
        {
            var response = new Response<aboutCompanyDto>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Claims.FirstOrDefault().Value;
                    var isLoginExists = await _home.Getaboutcompany(userid);
                    if (isLoginExists != null)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Status = "Success";
                        response.Message = "Aboutcompany retrieved successfully.";
                        response.Data = isLoginExists;
                        return Ok(response);
                    }
                    else
                    {
                        response.StatusCode = StatusCodes.Status401Unauthorized;
                        response.Message = "Data not found.";
                        return Ok(response);
                    }
                }
                else
                {
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.Message = "Token is expired.";
                    return BadRequest(response);
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetOurStory")]
        public async Task<IActionResult> GetOurStory()
        {
            try
            {
                List<OurStory> stories = _context.OurStories.Where(x => x.IsActive == true).ToList();
                if (stories != null)
                {
                    return Ok(new { Status = 200, Message = "Our Story retrieved successfully.", data = stories });
                }
                else
                {
                    return NotFound(new { Status = 404, Message = "No any OurStory available." });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("GetOurExpertise")]
        public async Task<IActionResult> GetOurExpertise()
        {
            try
            {
                List<OurExpertise> expertise = _context.OurExpertises.ToList();
                if (expertise != null)
                {
                    return Ok(new { Status = 200, Message = "Our Expertise retrieved successfully.", data = expertise });
                }
                else
                {
                    return NotFound(new { Status = 404, Message = "No any OurExpertise available." });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("GetRequestDemo")]
        public async Task<IActionResult> GetRequestDemo()
        {
            try
            {
                List<RequestDemo> RequestDemo = _context.RequestDemos.Where(x => x.IsActive == true).ToList();
                if (RequestDemo != null)
                {
                    return Ok(new { Status = 200, Message = "Request Demo retrieved successfully.", data = RequestDemo });
                }
                else
                {
                    return NotFound(new { Status = 404, Message = "No any RequestDemo available." });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("GetOurCoreValues")]
        public async Task<IActionResult> GetOurCoreValues()
        {
            try
            {
                List<OurCoreValue> OurCoreValues = _context.OurCoreValues.Where(x => x.IsActive == true).ToList();
                if (OurCoreValues != null)
                {
                    return Ok(new { Status = 200, Message = "OurCore Values retrieved successfully.", data = OurCoreValues });
                }
                else
                {
                    return NotFound(new { Status = 404, Message = "No any OurCoreValues available." });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("GetFeaturesandBenefits")]
        public async Task<IActionResult> GetFeaturesandBenefits()
        {
            try
            {
                List<Featurebenifit> FeaturesBenefits = _context.Featurebenifits.Where(x => x.IsActive == true).ToList();
                if (FeaturesBenefits != null)
                {
                    return Ok(new { Status = 200, Message = "Features & Benefits retrieved successfully.", data = FeaturesBenefits });
                }
                else
                {
                    return NotFound(new { Status = 404, Message = "No any Features & Benefits available." });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("GetOurTutorials")]
        public async Task<IActionResult> GetOurTutorials()
        {
            try
            {
                List<OurTutorial> tutorials = _context.OurTutorials.Where(x => x.IsActive == true).ToList();
                if (tutorials != null)
                {
                    return Ok(new { Status = 200, Message = "Our Tutorials retrieved successfully.", data = tutorials });
                }
                else
                {
                    return NotFound(new { Status = 404, Message = "No any tutorial available." });

                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        [Route("Contactus")]
        public async Task<IActionResult> Contactus(ContactU model)
        {
            try
            {
                var domainmodel = new ContactU()
                {
                    Name = model.Name,
                    Email = model.Email,
                    Message = model.Message
                };
                _context.Add(domainmodel);
                _context.SaveChanges();
                return Ok(new { Status = 200, Message = "Contact added successfully." });
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("GetCaseStudies")]
        public async Task<IActionResult> GetCaseStudies()
        {
            try
            {
                List<CaseStudy> cases = _context.CaseStudies.Where(x => x.IsActive == true).ToList();
                if (cases != null)
                {
                    return Ok(new { Status = 200, Message = "Case Studies retrieved successfully.", data = cases });
                }
                else
                {
                    return NotFound(new { Status = 404, Message = "No any Case Studies available." });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("GetPricingPlan")]
        //public async Task<IActionResult> GetPricingPlan()
        //{
        //    try
        //    {
        //        var result = _context.PricingPlans.Where(x => x.IsActive == true).ToList(); 

        //        List<PricingPlanDTO> plans = result.Select(x => new PricingPlanDTO
        //        {
        //            Id = x.Id,
        //            PlanName = x.PlanName,
        //            Title = x.Title,
        //            Price = x.Price,
        //            AnnulPrice = x.AnnulPrice,
        //            AnnulPriceInPercentage = x.AnnulPriceInPercentage,
        //            Description = x.Description,
        //            Image = x.Image,
        //            CreatedDate = x.CreatedDate,
        //            SavePrice = SavePrice(x.Price, (decimal)x.AnnulPriceInPercentage)
        //        }).ToList();
        //        if (plans != null)
        //        {
        //            var result1 = new
        //            {
        //                id = plans[0], // Assuming Id is a property in YourBusinessDetailsClass

        //                PricingPlanFeature = plans.Select(q => new
        //                {
        //                    Service = q.PlanFeatures
        //                }).ToList()
        //            };
        //            return Ok(new { Status = 200, Message = "Pricing Plan retrieved successfully.", data = plans });
        //        }
        //        else
        //        {
        //            return NotFound(new { Status = 404, Message = "No any Pricing Plan available." });
        //        }

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        public async Task<IActionResult> GetPricingPlan()
        {
            try
            {
                var result = await _context.PricingPlans
                     
                    .Where(x => x.IsActive==true)
                    .ToListAsync();

                if (result == null || !result.Any())
                {
                    return NotFound(new { Status = 404, Message = "No Pricing Plan available." });
                }

                List<PricingPlanDTO> plans = result.Select(x => new PricingPlanDTO
                {
                    Id = x.Id,
                    PlanName = x.PlanName,
                    Title = x.Title,
                    Price = x.Price,
                    AnnulPrice = x.AnnulPrice,
                    AnnulPriceInPercentage = x.AnnulPriceInPercentage,
                    Support = x.Support,
                    Image = x.Image,
                    CreatedDate = x.CreatedDate,
                    IsActive = x.IsActive,
                    SavePrice = SavePrice(x.Price, (decimal)x.AnnulPriceInPercentage),
                    PlanFeatures = _context.PricingPlanFeatures.Where(f=>f.PricingPlanId==x.Id).Select(f => new PlanFeature
                    {
                        Feature = f.Feature,
                        Id = f.Id
                    }).ToList() 
                }).ToList();

                return Ok(new { Status = 200, Message = "Pricing Plan retrieved successfully.", data = plans });
            }
            catch (Exception ex)
            {
                // Log exception here (consider using a logging framework)
                return StatusCode(500, new { Status = 500, Message = "An error occurred.", Error = ex.Message });
            }
        }


        public decimal SavePrice(decimal price, decimal per)
        {
            try
            {
                decimal annulprice = price * 12;
                decimal discountedprice = annulprice * per / 100;
                return discountedprice;
            }
            catch (Exception ex)
            {
                throw new Exception("Server Error : " + ex.Message);
            }
        }

        [HttpGet]
        [Route("GetOtherServices")]
        public async Task<IActionResult> GetOtherServices()
        {
            try
            {
                List<OtherService> plans = _context.OtherServices.Where(x => x.IsActive == true).ToList();
                if (plans != null)
                {
                    return Ok(new { Status = 200, Message = "Other Services retrieved successfully.", data = plans });
                }
                else
                {
                    return NotFound(new { Status = 404, Message = "No any Other Services available." });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        [Route("AddRequestDemo")]
        public async Task<IActionResult> AddRequestDemo(DemoRequest model)
        {
            try
            {
                var domainmodel = new DemoRequest()
                {
                    Name = model.Name,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    CompanyName = model.CompanyName,
                    Description = model.Description
                };
                _context.Add(domainmodel);
                _context.SaveChanges();
                return Ok(new { Status = 200, Message = "Request Demo added successfully." });
            }
            catch (Exception)
            {

                throw;
            }
        }

        [Route("GetCaseStudyById")]
        public async Task<IActionResult> GetCaseStudyById(int Id)
        {
            try
            {
                CaseStudy cases = await _context.CaseStudies.Where(x => x.IsActive == true && x.Id == Id).FirstOrDefaultAsync();
                if (cases != null)
                {
                    return Ok(new { Status = 200, Message = "Case Studies retrieved successfully.", data = cases });
                }
                else
                {
                    return NotFound(new { Status = 404, Message = "No any Case Studies available." });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Route("GetMissionVisions")]
        public async Task<IActionResult> GetMissionVisions()
        {
            try
            {
                List<MissionVision> cases = _context.MissionVisions.Where(x => x.IsActive == true).ToList();
                if (cases != null)
                {
                    return Ok(new { Status = 200, Message = "Mission & Vision retrieved successfully.", data = cases });
                }
                else
                {
                    return NotFound(new { Status = 404, Message = "No any Mission & Vision available." });

                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        [Route("AddHelpCenter")]
        public async Task<IActionResult> AddHelpCenter(HelpCenter model)
        {
            try
            {
                var domainmodel = new HelpCenter()
                {
                    Name = model.Name,
                    Email = model.Email,
                    Message = model.Message
                };
                _context.Add(domainmodel);
                _context.SaveChanges();
                return Ok(new { Status = 200, Message = "Help Center added successfully." });
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
