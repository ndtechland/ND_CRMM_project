﻿using CRM.Models.APIDTO;
using CRM.Models.Crm;

namespace CRM.Repository
{
    public interface IHome
    {
        Task<List<Blog>> GetBlogs();
        public Task<aboutCompanyDto> Getaboutcompany(string userid);

    }
}
